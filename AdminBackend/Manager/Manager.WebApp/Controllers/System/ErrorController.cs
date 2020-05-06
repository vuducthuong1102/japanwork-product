using Manager.WebApp.Caching;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;

namespace Manager.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        private List<ActionError> _actionErrors;
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var lang = UserCookieManager.GetCurrentLanguageOrDefault();
            var cultureInfo = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            if (!User.Identity.IsAuthenticated && !Request.IsAjaxRequest())
                Response.Redirect("~/Account/Login");

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

        public ActionResult ChangeLanguage(string lang)
        {
            var currentLang = UserCookieManager.GetCurrentLanguageOrDefault();
            if (Request != null)
            {
                if (Request.UrlReferrer != null)
                {
                    var currentUrl = Request.UrlReferrer.ToString();
                    if (!string.IsNullOrEmpty(Request.UrlReferrer.ToString()))
                    {
                        if (currentLang != lang)
                            new LanguageMessageHandler().SetLanguage(lang);

                        //HttpResponse.RemoveOutputCacheItem(Request.UrlReferrer.AbsolutePath);
                        //HttpResponse.RemoveOutputCacheItem("/home/index");
                        //HttpResponse.RemoveOutputCacheItem("/");

                        return Redirect(currentUrl);
                    }
                }
            }

            new LanguageMessageHandler().SetLanguage(lang);

            return RedirectToAction("Index", "Home");
        }

        // GET: Error
        public ActionResult NotFound()
        {
            return View();
        }


        public ActionResult Unauthorised()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}