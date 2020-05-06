using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity;

using Autofac;
using Manager.SharedLibs.Caching.Providers;
using System.Collections.Generic;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Resources;

namespace Manager.WebApp
{
    public class AccessRoleChecker : AuthorizeAttribute
    {
        private readonly ILog logger = LogProvider.For<AccessRoleChecker>();

        //Your Properties in AccessRoleChecker Filter
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        private readonly string ALL_PERMISSIONS_KEY = "PRIVILEGES_";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var rd = httpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentArea = rd.Values["area"] as string;
            string currentUser = HttpContext.Current.User.Identity.GetUserId();

            if (string.IsNullOrEmpty(this.ControllerName))
            {
                ControllerName += currentController;
            }
            if (string.IsNullOrEmpty(this.ActionName))
            {
                ActionName += currentAction;
            }

            // this is overriden for kendo menus to hide 
            /*
            var ctrl = filterContext.RequestContext.RouteData.GetRequiredString("controller");
            var action = filterContext.ActionDescriptor.ActionName;
            */

            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            bool hasPermission = false;
           

            try
            {
                var perList = PermissionHelper.GetAllPermission();
                //Has data from cache
                if (perList != null && perList.Count > 0)
                {
                    hasPermission = perList.Exists(m => string.Equals(m.Action, currentAction, StringComparison.CurrentCultureIgnoreCase)
                    && (string.Equals(m.Controller, currentController, StringComparison.CurrentCultureIgnoreCase)));
                }
                else
                {
                    hasPermission = false;
                }
                
                //var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                //List<IdentityPermission> perList;
                //var myKey = ALL_PERMISSIONS_KEY + currentUser;

                ////Check from cache first
                //cacheProvider.Get(myKey, out perList);

                ////Has data from cache
                //if(perList != null && perList.Count > 0 )
                //{
                //    hasPermission = perList.Exists(m => string.Equals(m.Action, currentAction, StringComparison.CurrentCultureIgnoreCase)
                //    && (string.Equals(m.Controller, currentController, StringComparison.CurrentCultureIgnoreCase)));
                //}
                //else
                //{
                //    var _rolesIdentityStore = GlobalContainer.IocContainer.Resolve<IAccessRolesStore>();

                //    perList = _rolesIdentityStore.GetPermissionsByUser(currentUser);
                //    if (perList != null && perList.Count > 0)
                //    {
                //        hasPermission = perList.Exists(m => string.Equals(m.Action, currentAction, StringComparison.CurrentCultureIgnoreCase)
                //    && (string.Equals(m.Controller, currentController, StringComparison.CurrentCultureIgnoreCase)));

                //        //Write to cache
                //        cacheProvider.Set(myKey, perList);
                //    }
                //    else
                //    {
                //        hasPermission = false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not check permission of user [{0}] because: {1}", currentUser, ex.ToString());
                logger.Error(strError);                
            }

            //var _rolesIdentityStore =  GlobalContainer.IocContainer.Resolve<IAccessRolesStore>();

            //hasPermission = _rolesIdentityStore.GetPermissionsByUser(currentUser,);
            /*
            string privilegeLevels = string.Join("", GetUserRights(httpContext.User.Identity.Name.ToString())); // Call another method to get rights of the user from DB
            
            if (privilegeLevels.Contains(this.AccessLevel))
            {
                return true;
            }
            else
            {
                return false;
            }
             */
            return hasPermission;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {           
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = ManagerResource.ERROR_ACTION_DENNIED },
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                //var viewResult = new ViewResult();
                //viewResult.ViewName = "~/Views/Errors/_Unauthorized.cshtml";
                //filterContext.Result = viewResult;

                filterContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary(
                           new
                           {
                               controller = "Error",
                               action = "Unauthorised"
                           })
                       );
            }

            //base.HandleUnauthorizedRequest(filterContext);
        }

    }
}
