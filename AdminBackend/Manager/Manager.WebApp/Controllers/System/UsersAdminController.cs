using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using System.Configuration;
using System.Net;
using Manager.WebApp.Resources;
using Manager.SharedLibs.Logging;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class UsersAdminController : BaseAuthedController
    {
        private readonly IIdentityStore _identityStore;
        private readonly ILog logger = LogProvider.For<UsersAdminController>();
        public UsersAdminController(IIdentityStore identityStore)
        {
            _identityStore = identityStore;
            //Clear cache
            CachingHelpers.ClearUserCache();

        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, IIdentityStore identityStore)
        {
            UserManager = userManager;
            RoleManager = roleManager;

            _identityStore = identityStore;
            //Clear cache
            CachingHelpers.ClearUserCache();

        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /UsersAdmin/
        [AccessRoleChecker]
        public ActionResult Index(UserViewModel model)
        {
            int currentPage = 1;
            int pageSize = AdminSettings.PageSize;

            try
            {
                model.AllRoles = RoleManager.Roles.OrderBy(m => m.Name).ToList();
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 0);
            }

            try
            {
                var isLocked = Convert.ToBoolean(model.IsLocked);
                model.SearchResult = _identityStore.FilterUserList(model.Email, model.RoleId, isLocked, currentPage, pageSize);
                model.Total = _identityStore.CountAll(model.Email, model.RoleId, isLocked);
                model.CurrentPage = currentPage;
                model.PageNo = (int)(model.Total / pageSize);
                model.PageSize = pageSize;

                if (model.SearchResult != null && model.SearchResult.Count > 0)
                {
                    foreach (var record in model.SearchResult)
                    {
                        var _userRoles = UserManager.GetRoles(record.Id);
                        var _newModelUser = new IdentityUser();

                        _newModelUser = record;
                        if (_userRoles != null)
                        {
                            _newModelUser.Roles = _userRoles.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return View(model);
            }
            return View(model);
        }

        // GET: /Users/Details/5
        [AccessRoleChecker]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var detailsModel = new UserDetailsViewModel();
            try
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return RedirectToErrorPage();
                }

                ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
                detailsModel.User = user;
                detailsModel.Lockout = new LockoutViewModel();
                var isLocked = await UserManager.IsLockedOutAsync(user.Id);
                detailsModel.Lockout.Status = isLocked ? LockoutStatus.Locked : LockoutStatus.Unlocked;
                
                var userInfo = _identityStore.GetUserByID(user.Id);

                if (detailsModel.Lockout.Status == LockoutStatus.Locked)
                {
                    detailsModel.Lockout.LockoutEndDate = (await UserManager.GetLockoutEndDateAsync(user.Id)).DateTime;
                }

                var claims = await UserManager.GetClaimsAsync(user.Id);
                if (userInfo != null)
                {
                    detailsModel.FullName = userInfo.FullName;
                }

                detailsModel.Claims = claims.ToList();
                detailsModel.Email = Request["Email"];
                detailsModel.RoleId = Request["RoleId"];
                detailsModel.SearchExec = Request["SearchExec"];
                detailsModel.Page = Request["Page"];
                detailsModel.IsLocked = Convert.ToInt32(Request["IsLocked"]);
            }
            catch
            {
                return RedirectToErrorPage();
            }

            return View(detailsModel);
        }

        //
        // GET: /Users/Create
        [AccessRoleChecker]
        public ActionResult Create()
        {
            var model = new RegisterViewModel();

            try
            {
                //Get the list of Roles
                // ViewBag.RoleId = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");
                model.RolesList = RoleManager.Roles.OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name
                });
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(model);
        }


        //
        // POST: /Users/Create
        [HttpPost]
        [AccessRoleChecker]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                userViewModel.Password = Utility.Md5HashingData(userViewModel.Password);
                var user = new ApplicationUser { UserName = userViewModel.UserName, Email= userViewModel.UserName + "@email.com", FullName = userViewModel.FullName, EmailConfirmed = true };

                if (!string.IsNullOrEmpty(user.UserName))
                {
                    var existedUser = await UserManager.FindByNameAsync(user.UserName);
                    if (existedUser != null)
                    {
                        if (existedUser != null)
                        {
                            this.AddNotification(ManagerResource.ERROR_ACCOUNT_DUPLICATED, NotificationType.ERROR);
                            return View();
                        }
                    }
                }

                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Clear cache
                CachingHelpers.ClearUserCache();

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    _identityStore.UpdateProfile(new IdentityUser { FullName = user.FullName, Id = user.Id });

                    if (selectedRole != null)
                    {
                        var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRole);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                //return RedirectToAction("Edit/" + user.Id);
                this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);

                return RedirectToAction("Index");
            }
            //ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        public async Task<ActionResult> Lock(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            //var helper = new IdentityModelHelper(UserManager, RoleManager);
            //var model = await helper.GetUserDetailsViewModel(id);
            try
            {
                var result = await UserManager.LockUserAccount(id, 1000 * 1000);
                if (!result.Succeeded)
                {
                    AddErrors(result.Errors);
                }
                else
                {
                    //Clear cache
                    CachingHelpers.ClearUserCache();
                    CachingHelpers.ClearUserCache(id);

                    //model = await helper.GetUserDetailsViewModel(id);
                    return RedirectToAction("Details/" + id, "UsersAdmin");
                }
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            //ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            return View();
        }

        public async Task<ActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            //var helper = new IdentityModelHelper(UserManager, RoleManager);
            //var model = await helper.GetUserDetailsViewModel(id);
            try
            {
                var result = await UserManager.UnlockUserAccount(id);
                if (!result.Succeeded)
                {
                    AddErrors(result.Errors);
                }
                else
                {
                    //Clear cache
                    CachingHelpers.ClearUserCache();
                    CachingHelpers.ClearUserCache(id);

                    return RedirectToAction("Details/" + id, "UsersAdmin");
                }
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            //ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            return View();
        }

        //
        // GET: /Users/Edit/1
        [AccessRoleChecker]
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User id was not provided");
                }

                var user = _identityStore.GetUserByID(id);
                if (user == null)
                {
                    return RedirectToErrorPage();
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    //Email = user.Email,
                    //PhoneNumber = user.PhoneNumber,
                    RolesList = RoleManager.Roles.OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    }),
                    SEmail = Request["Email"],
                    SRoleId = Request["RoleId"],
                    SearchExec = Request["SearchExec"],
                    Page = Request["Page"],
                    SIsLocked = Convert.ToInt32(Request["IsLocked"]),
                    //ListProvider = _StoreProvider.GetList(),
                    //ProviderId=user.ProviderId
                });
            }
            catch
            {
                return RedirectToErrorPage();
            }
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Id,FullName")] EditUserViewModel editUser, params string[] selectedRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByIdAsync(editUser.Id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    user.UserName = editUser.UserName;
                    user.FullName = editUser.FullName;

                    if (!string.IsNullOrEmpty(user.UserName))
                    {
                        var existedUser = await UserManager.FindByNameAsync(user.UserName);
                        if (existedUser != null)
                        {
                            if (editUser.Id != existedUser.Id)
                            {
                                this.AddNotification(ManagerResource.ERROR_ACCOUNT_DUPLICATED, NotificationType.ERROR);
                                return RedirectToAction("Edit/" + editUser.Id);
                            }
                        }
                    }
                    //user.PhoneNumber = editUser.PhoneNumber;
                    //user.ProviderId = editUser.ProviderId;

                    var userRoles = await UserManager.GetRolesAsync(user.Id);
                    await UserManager.UpdateAsync(user);

                    _identityStore.UpdateProfile(new IdentityUser { FullName = user.FullName, Id = user.Id });

                    selectedRole = selectedRole ?? new string[] { };

                    var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRole.Except(userRoles).ToList<string>());

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    result = await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToList<string>());

                    if (!result.Succeeded)
                    {
                        this.AddNotification("Update user profiles faied: " + result.Errors.First(), NotificationType.ERROR);
                        return View();
                    }

                    MenuHelper.ClearUserMenuCache(editUser.Id);

                    //Clear cache
                    CachingHelpers.ClearUserCache();
                    CachingHelpers.ClearUserCache(editUser.Id);

                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                    //return RedirectToAction("Edit/" + editUser.Id);

                    return RedirectToAction("Index");
                }              
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not Update User info because: {0}", ex.ToString()));
            }

            this.AddNotificationModelStateErrors(ModelState);
            return RedirectToAction("Edit/" + editUser.Id);
        }

        //Show popup confirm delete        
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserViewModel record = new UserViewModel();
            record.UserInfoViewModel = new IdentityUser();
            record.UserInfoViewModel.Id = id;

            return PartialView("_DeleteUserInfo", record);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                //Clear cache
                CachingHelpers.ClearUserCache();
                CachingHelpers.ClearUserCache(id);

                return Json(new { success = true });
            }
            else
            {
                throw new Exception("Failed to delete the user");
            }
        }

        //Show popup confirm reset password        
        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserViewModel record = new UserViewModel();
            record.UserInfoViewModel = new IdentityUser();
            record.UserInfoViewModel.Id = id;

            return PartialView("_ConfirmResetPwd", record);
        }

        [HttpPost, ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> ResetPwd(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            try
            {
                var removePassword = UserManager.RemovePassword(id);
                string defaultPassword = ConfigurationManager.AppSettings["System:UserDefaultPassword"];
                if (removePassword.Succeeded)
                {
                    user.PasswordHash = Utility.Md5HashingData(UserManager.PasswordHasher.HashPassword(defaultPassword));
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true, message = ManagerResource.LB_PASSWORD_RESET_SUCCESS + ": " + defaultPassword });
                    }
                    else
                    {
                        throw new Exception("Reset password failed");
                    }
                }
                else
                {
                    throw new Exception("Reset password failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Reset password failed, reason: " + ex.Message);
            }
        }
    }
}