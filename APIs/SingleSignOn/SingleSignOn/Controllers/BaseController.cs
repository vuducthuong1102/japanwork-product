using System;
using System.Web.Mvc;
using SingleSignOn.Caching;
using System.Web;

namespace SingleSignOn.Controllers
{    
    public class BaseController : Controller
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
                //var userLanguage = Request.UserLanguages;
                //var userLang = userLanguage != null ? userLanguage[0] : "";
                //if (userLang != "")
                //{
                //    lang = userLang;
                //}
                //else
                //{
                //    lang = LanguageMessageHandler.GetDefaultLanguage();
                //}
                lang = LanguageMessageHandler.GetDefaultLanguage();
            }

            new LanguageMessageHandler().SetLanguage(lang);

            return base.BeginExecuteCore(callback, state);
        }
    }
}
