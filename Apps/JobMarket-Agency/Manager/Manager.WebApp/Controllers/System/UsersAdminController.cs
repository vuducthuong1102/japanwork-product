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

using MsSql.AspNet.Identity.Repositories;
using Manager.SharedLibs;
using Manager.WebApp.Services;
using ApiJobMarket.DB.Sql.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;
using SelectItem = System.Web.Mvc.SelectListItem;

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
            model = GetDefaultFilterModel(model);

            try
            {
                model.RoleList = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id
                });
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


            try
            {
                var isLocked = Convert.ToBoolean(model.IsLocked);
                model.SearchResult = _identityStore.FilterUserList(model.Keyword, GetCurrentAgencyId(), model.RoleId, isLocked, model.CurrentPage, model.PageSize); ;
                model.Total = _identityStore.CountAll(model.Email, model.RoleId, isLocked, GetCurrentAgencyId());

                model.PageNo = (int)(model.Total / model.PageSize);

                if (model.SearchResult != null && model.SearchResult.Count > 0)
                {
                    foreach (var record in model.SearchResult)
                    {
                        var _userRoles = UserManager.GetRoles(record.Id);
                        var _newModelUser = new MsSql.AspNet.Identity.IdentityUser();

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
        [AccessRoleChecker(AgencyRequired = true)]
        public ActionResult Create()
        {
            var model = new RegisterViewModel();
            model.staff_id = GetCurrentStaffId();
            model.Companies = new List<IdentityCompany>();
            try
            {
                //Get the list of Roles
                // ViewBag.RoleId = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");
                model.RolesList = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
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
                var user = new ApplicationUser { UserName = userViewModel.UserName, Email = userViewModel.UserName, FullName = userViewModel.FullName, EmailConfirmed = true, ParentId = GetCurrentAgencyId() };

                if (!string.IsNullOrEmpty(user.UserName))
                {
                    var existedUser = await UserManager.FindByNameAsync(user.UserName);
                    if (existedUser != null)
                    {
                        if (existedUser != null)
                        {
                            this.AddNotification(ManagerResource.ERROR_ACCOUNT_DUPLICATED, NotificationType.ERROR);
                            userViewModel.RolesList = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                            {
                                Text = x.Name,
                                Value = x.Name
                            });
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
                    _identityStore.UpdateProfile(new MsSql.AspNet.Identity.IdentityUser { FullName = user.FullName, Id = user.Id });

                    var userDetail = _identityStore.GetUserByID(user.Id);
                    if (userDetail != null)
                    {
                        var apiModel = new ApiCompanyUpdateCompanyModel
                        {
                            agency_id = userDetail.StaffId,
                            ListCompanyId = userViewModel.selectCompany,
                            agency_parent_id = GetCurrentAgencyId()
                        };
                        var result = AgencyCompanyServices.UpdateCompanyAsync(apiModel).Result;
                    }

                    if (!string.IsNullOrEmpty(userViewModel.Role))
                    {
                        selectedRole = new string[] { userViewModel.Role };
                        var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRole);
                        if (!result.Succeeded)
                        {
                            //ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                            this.AddNotification(result.Errors.First(), NotificationType.ERROR);

                            userViewModel.RolesList = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                            {
                                Text = x.Name,
                                Value = x.Name
                            });
                            return View();
                        }
                    }
                }
                else
                {
                    //ModelState.AddModelError("", adminresult.Errors.First());

                    this.AddNotification(adminresult.Errors.First(), NotificationType.ERROR);
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    userViewModel.RolesList = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Name
                    });

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
                    return RedirectToAction("Edit/" + id, "UsersAdmin");
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

                    return RedirectToAction("Edit/" + id, "UsersAdmin");
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
            var model = new EditUserViewModel();
            model.allCompanies = new List<SelectItem>();
            var apiModel = new ApiCompanyModel
            {
                agency_id = GetCurrentAgencyId(),
                page_index = 1,
                page_size = 1000,
            };
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


                var applicationUser = await UserManager.FindByIdAsync(id);
                if (applicationUser == null)
                {
                    return RedirectToErrorPage();
                }

                model.User = applicationUser;
                model.Lockout = new LockoutViewModel();
                var isLocked = await UserManager.IsLockedOutAsync(applicationUser.Id);
                model.Lockout.Status = isLocked ? LockoutStatus.Locked : LockoutStatus.Unlocked;
                model.IsActived = !isLocked;
                if (model.Lockout.Status == LockoutStatus.Locked)
                {
                    model.Lockout.LockoutEndDate = (await UserManager.GetLockoutEndDateAsync(applicationUser.Id)).DateTime;
                }
                apiModel.staff_id = user.StaffId;
                var listDataCompany = AgencyServices.GetCompaniesAsync(apiModel).Result;

                List<IdentityCompany> listCompany = JsonConvert.DeserializeObject<List<IdentityCompany>>(listDataCompany.value.ToString());
                if (listCompany.HasData())
                {
                    foreach (var record in listCompany)
                    {
                        var company_name = string.Empty;
                        if (record.LangList.HasData())
                        {
                            var companyLang = record.LangList.FirstOrDefault(s => s.language_code == GetCurrentLanguageOrDefault());
                            if (companyLang != null) company_name = companyLang.company_name;
                        }
                        if (string.IsNullOrEmpty(company_name))
                        {
                            company_name = record.company_name;
                        }
                        record.company_name = company_name;
                    }
                }

                model.Companies = listCompany;
                model.Id = user.Id;
                model.UserName = user.UserName;
                model.FullName = user.FullName;
                model.RolesList = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                });
                if (model.RolesList.ToList().HasData())
                {
                    var selectRole = model.RolesList.ToList().FirstOrDefault(s => s.Selected == true);
                    if (selectRole != null)
                    {
                        model.Role = selectRole.Value;
                    }
                }
                model.SEmail = Request["Email"];
                model.SRoleId = Request["RoleId"];
                model.SearchExec = Request["SearchExec"];
                model.Page = Request["Page"];
                model.SIsLocked = Convert.ToInt32(Request["IsLocked"]);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToErrorPage();
            }
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Id,FullName,selectCompany,IsActived,Role")] EditUserViewModel editUser, params string[] selectedRole)
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
                    if (!editUser.IsActived)
                    {
                        var resultLock = await UserManager.LockUserAccount(editUser.Id, 1000 * 1000);
                    }
                    else
                    {
                        var unLock = await UserManager.UnlockUserAccount(editUser.Id);
                    }
                    //user.PhoneNumber = editUser.PhoneNumber;
                    //user.ProviderId = editUser.ProviderId;

                    var userRoles = await UserManager.GetRolesAsync(user.Id);
                    await UserManager.UpdateAsync(user);

                    _identityStore.UpdateProfile(new MsSql.AspNet.Identity.IdentityUser { FullName = user.FullName, Id = user.Id });

                    if (!string.IsNullOrEmpty(editUser.Role))
                    {
                        selectedRole = new string[] { editUser.Role };
                    }
                    var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRole.Except(userRoles).ToList<string>());
                    if (user != null)
                    {
                        var apiModel = new ApiCompanyUpdateCompanyModel
                        {
                            agency_id = user.StaffId,
                            ListCompanyId = editUser.selectCompany,
                            agency_parent_id = GetCurrentAgencyId()
                        };

                        var resultApi = AgencyCompanyServices.UpdateCompanyAsync(apiModel).Result;
                    }
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
            record.UserInfoViewModel = new MsSql.AspNet.Identity.IdentityUser();
            record.UserInfoViewModel.Id = id;

            return PartialView("_DeleteUserInfo", record);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { success = false, message = ManagerResource.LB_ERROR_OCCURED });
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = ManagerResource.LB_ERROR_OCCURED });
                }

                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    //Clear cache
                    CachingHelpers.ClearUserCache();
                    CachingHelpers.ClearUserCache(id);

                    //return Json(new { success = true });
                    return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, clientcallback = "location.reload();" });
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when delete user because: {0}", ex.ToString()));
            }

            return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
        }

        //Show popup confirm reset password        
        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserViewModel record = new UserViewModel();
            record.UserInfoViewModel = new MsSql.AspNet.Identity.IdentityUser();
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
                return Json(new { success = false, message = ManagerResource.LB_ERROR_OCCURED });
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = ManagerResource.LB_ERROR_OCCURED });
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
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Reset password failed, reason: {0}", ex.Message));
            }

            return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });
        }
    }
}