using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity;
using Manager.WebApp.Settings;
using Manager.WebApp.Caching;
using System.Globalization;
using System.Threading;

namespace Manager.WebApp.Controllers
{
    public class BaseController : Controller
    {
        private List<ActionError> _actionErrors;
        protected readonly string _currentLanguage = GetCurrentLanguageOrDefault();

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var lang = UserCookieManager.GetCurrentLanguageOrDefault();
            var cultureInfo = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Preparing before executing action
            _actionErrors = new List<ActionError>();
            ViewBag.ActionErrors = _actionErrors;
            ViewBag.AdminNavMenu = MenuHelper.GetAdminNavigationMenuItems();


            base.OnActionExecuting(filterContext);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                _actionErrors.Add(new ActionError() { Reason = error });
            }
        }

        public void AddError(Exception ex)
        {
            _actionErrors.Add(new ActionError() { Reason = ex.Message, Details = ex.StackTrace });
        }


        protected string GetModelStateErrors(ModelStateDictionary stateDic)
        {
            var sb = new StringBuilder();
            foreach (var errorKey in stateDic.Keys)
            {
                foreach (var errorMsg in stateDic[errorKey].Errors)
                {
                    sb.AppendLine(errorMsg.ErrorMessage + "<br />");
                }
            }

            return sb.ToString();
        }


        public Dictionary<string, List<string>> GetModelStateErrorList(ModelStateDictionary stateDic)
        {
            var result = new Dictionary<string, List<string>>();

            foreach (var errorKey in stateDic.Keys)
            {
                var errors = new List<string>();
                foreach (var errorMsg in stateDic[errorKey].Errors)
                {
                    errors.Add(errorMsg.ErrorMessage);
                }

                result.Add(errorKey, errors);
            }

            return result;
        }


        public void AddNotificationModelStateErrors(ModelStateDictionary stateDic)
        {
            foreach (var errorKey in stateDic.Keys)
            {
                foreach (var errorMsg in stateDic[errorKey].Errors)
                {
                    this.AddNotification(errorMsg.ErrorMessage, NotificationType.ERROR);
                }
            }
        }

        protected IdentityUser GetCurrentUser()
        {
            var userId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userId))
                return AccountHelper.GetUserById(userId);

            return null;
        }
        protected IdentityUser GetByStaffId(int staffId)
        {
            if (staffId > 0)
                return AccountHelper.GetByStaffId(staffId);

            return null;
        }

        protected string GetCurrentUserId()
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
                return currentUser.Id;

            return string.Empty;
        }

        protected int GetCurrentAgencyId()
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                if (currentUser.ParentId == 0)
                    return currentUser.StaffId;
                else
                    return currentUser.ParentId;
            }

            return 0;
        }

        protected int GetCurrentStaffId()
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                return currentUser.StaffId;
            }

            return 0;
        }

        public string RemoveServerUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (HttpContext.Request.Url != null)
                {
                    var requestRootUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
                    url = url.Replace(requestRootUrl, "");
                }
            }

            return url;
        }

        public static string GetCurrentLanguageOrDefault()
        {
            try
            {
                var myObjStr = UserCookieManager.GetCookie(SystemSettings.CultureKey);
                if (myObjStr != null && !string.IsNullOrEmpty(myObjStr))
                {
                    if (LanguageMessageHandler.IsSupported(myObjStr))
                    {
                        return myObjStr;
                    }
                    else
                    {
                        return LanguageMessageHandler.GetDefaultLanguage();
                    }
                }
            }
            catch
            {
            }

            return LanguageMessageHandler.GetDefaultLanguage();
        }

        protected string GetPreviousPageQueryParams()
        {
            var queryParams = string.Empty;
            try
            {
                if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                {
                    queryParams = ControllerContext.HttpContext.Request.UrlReferrer.Query;
                }
            }
            catch
            {
            }

            return queryParams;
        }

        protected object GetPreviousPageQueryParamsObj()
        {
            var queryParams = string.Empty;
            try
            {
                if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                {
                    queryParams = ControllerContext.HttpContext.Request.UrlReferrer.Query;
                }

                if (!string.IsNullOrEmpty(queryParams))
                {
                    var myParams = HttpUtility.ParseQueryString(queryParams);

                    return myParams;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}