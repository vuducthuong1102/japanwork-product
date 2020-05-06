using System;
using System.Web.Mvc;
using MySite.Caching;
using System.Web;
using System.IO;
using System.Threading;
using System.Globalization;
using MySite.Settings;
using MySite.Logging;
using MySite.Models;
using MySite.ShareLibs;

namespace MySite.Controllers
{    
    public class BaseController : Controller
    {
        private readonly ILog logger = LogProvider.For<BaseController>();
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

        protected string PartialViewAsString(string partialviewName, object model)
        {
            if (string.IsNullOrEmpty(partialviewName))
            {
                partialviewName = ControllerContext.RouteData.GetRequiredString("action");
            }

            var viewData = ViewData;
            ViewData = new ViewDataDictionary(viewData) { Model = model };
            using (var writer = new StringWriter())
            {
                var viewResult = ViewEngineCollection.FindPartialView(ControllerContext, partialviewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, writer);
                viewResult.View.Render(viewContext, writer);
                ViewData = viewData;

               
                return writer.ToString();
            }
        }

        protected ActionResult RedirectToErrorPage(string pageName = "NotFound")
        {
            return View(string.Format("../Error/{0}", pageName));
        }

        protected void CloseRunTime()
        {
            try
            {
                HttpRuntime.Close();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to CloseRunTime due to: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        protected void ClearOutputCache(bool closeRuntime = false)
        {
            try
            {
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                //Response.Cache.SetNoStore();              

                //HttpResponse.RemoveOutputCacheItem(Request.UrlReferrer.AbsolutePath);
                //HttpResponse.RemoveOutputCacheItem("/home/index");
                //HttpResponse.RemoveOutputCacheItem("/");
                //HttpResponse.RemoveOutputCacheItem("/#_=_");
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to ClearOutputCache due to: {0}", ex.ToString());
                logger.Error(strError);
            }
            finally
            {
               //if(closeRuntime)
                    //CloseRunTime();
            }
        }

        public ApiCommonFilterModel GetFilterConfig()
        {
            var apiFilterModel = new ApiCommonFilterModel();
            var pageIndex = 1;
            var keyword = string.Empty;

            if (Request["keyword"] != null)
                keyword = Request["keyword"].ToString();

            if (!string.IsNullOrEmpty(keyword))
                keyword = keyword.Trim();

            if (Request["page_index"] != null)
                pageIndex = Utils.ConvertToInt32(Request["page_index"]);

            var pageSize = SystemSettings.DefaultPageSize;

            if (Request["page_size"] != null)
                pageSize = Utils.ConvertToInt32(Request["page_size"]);

            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize > SystemSettings.DefaultPageSize || pageSize <= 0)
                pageSize = SystemSettings.DefaultPageSize;

            apiFilterModel.page_index = pageIndex;
            apiFilterModel.page_size = pageSize;

            apiFilterModel.keyword = keyword;

            return apiFilterModel;
        }
    }
}
