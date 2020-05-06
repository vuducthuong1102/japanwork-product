using MySite.Caching;
using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace MySite.Controllers
{
    public class ErrorController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var lang = UserCookieManager.GetCurrentLanguageOrDefault();
            var cultureInfo = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            return base.BeginExecuteCore(callback, state);
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

        public ActionResult RobotDetected()
        {
            return View();
        }

        public ActionResult Restricted()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}