using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using MsSql.AspNet.Identity;

using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;
using System.Net;
using Manager.SharedLibs.Logging;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Controllers
{
    public class RolesAdminController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<RolesAdminController>();
        private readonly IActivityStore _activityStore;
        private IStoreRole _roleStore;

        public RolesAdminController(IActivityStore activityStore, IStoreRole roleStore)
        {
            _activityStore = activityStore;
            _roleStore = roleStore;
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager, IActivityStore activityStore, IStoreRole roleStore)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            _activityStore = activityStore;
            _roleStore = roleStore;
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
        // GET: /RolesAdmin/
        [AccessRoleChecker]
        public ActionResult Index()
        {
            var model = new IndexRoleViewModel(RoleManager.Roles);
            List<IdentityRole> roles = new List<IdentityRole>();
            model.UserId = GetCurrentUserId();
            try
            {
                roles = new RoleRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString).GetRoleByUserId(GetCurrentAgencyId()).OrderBy(m => m.Name).ToList();
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                return View(model);
            }

            model.RoleList = roles;
            return View(model);
        }

        //
        // POST: /RolesAdmin/Create
        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Create(IndexRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new IdentityRole()
                    {
                        Name = model.CurrentRole.Name,
                        UserId = GetCurrentUserId(),
                        Agency_Id = GetCurrentAgencyId()
                    };
                    var result = _roleStore.Insert(role);
                    if (result > 0)
                    {
                        this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                        MenuHelper.ClearAllMenuCache();
                    }
                    else
                    {
                        this.AddNotification(ManagerResource.ERROR_EXISTS_ROLE, NotificationType.ERROR);
                    }
                    //var roleresult =  await RoleManager.CreateAsync(role);
                    //if (!roleresult.Succeeded)
                    //{
                    //    ModelState.AddModelError("", roleresult.Errors.First());
                    //}
                    //else
                    //{

                    //this.AddNotification("The role [" + role.Name + "] is added succesffully", NotificationType.SUCCESS);

                    //Write log
                    var activityText = "Create new role device [Name: {0}]";
                    activityText = string.Format(activityText, model.CurrentRole.Name);
                    WriteActivityLog(activityText, ActivityLogType.CreateRole, model.CurrentRole.Id, TargetObjectType.RolesAdmin);

                    return RedirectToAction("Index", "RolesAdmin");
                    //}
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not Create Role because: {0}", ex.ToString()));
            }

            this.AddNotificationModelStateErrors(ModelState);
            return RedirectToAction("Index", "RolesAdmin");
        }

        //
        // POST: /RolesAdmin/Update
        [HttpPost]
        [AccessRoleChecker]
        public ActionResult Update(IndexRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new IdentityRole()
                    {
                        Name = model.CurrentRole.Name,
                        Id = model.CurrentRole.Id,
                        Agency_Id = GetCurrentAgencyId()
                    };
                    var result = _roleStore.Update(role);

                    if (result > 0)
                    {
                        this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                        MenuHelper.ClearAllMenuCache();
                    }
                    else
                    {
                        this.AddNotification(ManagerResource.ERROR_EXISTS_ROLE, NotificationType.ERROR);
                    }
                    //var roleresult =  await RoleManager.UpdateAsync(role);
                    //if (!roleresult.Succeeded)
                    //{
                    //    ModelState.AddModelError("", roleresult.Errors.First());
                    //}
                    //else
                    //{
                    //this.AddNotification("The role [" + role.Name + "] is updated succesffully", NotificationType.SUCCESS);

                    //Write log
                    var activityText = "Updated the role [Name: {0}]";
                    activityText = string.Format(activityText, model.CurrentRole.Name);
                    WriteActivityLog(activityText, ActivityLogType.UpdateRole, model.CurrentRole.Id, TargetObjectType.RolesAdmin);

                    return RedirectToAction("Index", "RolesAdmin");
                    //}
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not Create Role because: {0}", ex.ToString()));
            }
            this.AddNotificationModelStateErrors(ModelState);
            return RedirectToAction("Index", "RolesAdmin");
        }

        //Show popup confirm delete
        public ActionResult DeleteRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RoleViewModel record = new RoleViewModel();
            record.Id = id;

            return PartialView("_DeleteroleInfo", record);
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public async Task<ActionResult> AcceptDeleteRole(string id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { success = false, message = ManagerResource.LB_ERROR_OCCURED });
                }

                var role = new IdentityRole(string.Empty, id);
                var roleresult = await RoleManager.DeleteAsync(role);
                if (roleresult.Succeeded)
                {
                    MenuHelper.ClearAllMenuCache();

                    //Write log
                    var activityText = "Delete the role [Id: {0}]";
                    activityText = string.Format(activityText, id);
                    WriteActivityLog(activityText, ActivityLogType.DeleteRole, id, TargetObjectType.RolesAdmin);

                    //return Json(new { success = true });
                    return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, clientcallback = "location.reload();" });
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when Delete Role because: {0}", ex.ToString()));
            }

            return Json(new { success = false, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT });

        }

        #region Helpers

        private void WriteActivityLog(string activityText, ActivityLogType activityType, string targetId, TargetObjectType targetType)
        {
            var logData = new ActivityLog
            {
                UserId = User.Identity.GetUserId(),
                ActivityText = activityText,
                ActivityType = activityType.ToString(),
                TargetId = targetId,
                TargetType = targetType.ToString(),
                IPAddress = Request.UserHostAddress
            };

            _activityStore.WriteActivityLog(logData);
        }

        #endregion
    }
}