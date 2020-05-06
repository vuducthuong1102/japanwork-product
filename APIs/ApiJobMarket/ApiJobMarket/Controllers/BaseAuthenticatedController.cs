using System;
using System.Web.Mvc;
using ApiJobMarket.Caching;
using System.Web;

namespace ApiJobMarket.Controllers
{    
    public class BaseAuthenticatedController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = LanguageMessageHandler.GetDefaultLanguage();
                }
            }

            new LanguageMessageHandler().SetLanguage(lang);

            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!UserCookieManager.IsAuthenticated())
            {
                //RedirectToAction("WebAuth", "Login");
                var redirectUrl = Url.Encode(Request.Url.AbsolutePath);
                var urlReturn = string.Format("~/WebAuth/Login?UrlReturn={0}", redirectUrl);
                filterContext.Result = new RedirectResult(urlReturn);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
