using Manager.WebApp.Caching;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    [Authorize]
    public class BaseAuthedController : BaseController
    {
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

        public List<string> UploadImages(string subDir, string objectId = "", bool isIncludeDatePath = false)
        {
            var uploadedList = new List<string>();

            if (Request.Files.Count < 1)
            {
                return null;
            }
            try
            {
                var subFolder = subDir;
                if (!string.IsNullOrEmpty(objectId) && objectId != "0")
                    subFolder = subDir + "/" + objectId;
                foreach (string file in Request.Files)
                {
                    var postedFile = Request.Files[file];
                    var fileDir = string.Empty;
                    var fileName = FileUploadHelper.UploadPostedFile(postedFile, subFolder, isIncludeDatePath);

                    uploadedList.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPLOAD IMAGES: " + ex.Message;
                
                return null;
            }

            return uploadedList;
        }

        protected dynamic GetFilterConfig()
        {
            var currentPage = 1;
            var status = -1;
            var keyword = string.Empty;
            var pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);

            if (Request["Keyword"] != null)
                keyword = Request["Keyword"].ToString();

            if (Request["Status"] != null)
                status = Utils.ConvertToInt32(Request["Status"], -1);

            if (!string.IsNullOrEmpty(keyword))
                keyword = keyword.Trim();

            dynamic filter = new System.Dynamic.ExpandoObject();

            filter.keyword = keyword;
            filter.status = status;
            filter.page_index = currentPage;
            filter.page_size = pageSize;
            filter.language_code = UserCookieManager.GetCurrentLanguageOrDefault();

            return filter;
        }
    }  
}