using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Manager.SharedLibs;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Newtonsoft.Json;

namespace Manager.WebApp.Controllers
{
    /// <summary>
    /// This class will act as a base class which other Web API controllers will inherit from, 
    /// for now it will contain three basic methods
    /// </summary>
    public class BaseApiController : ApiController
    {
        private readonly ILog logger = LogProvider.For<BaseApiController>();

        protected IHttpActionResult CreateBadRequest(string strErrorCode, string strErrorMessage)
        {
            var strError = strErrorCode + "-" + strErrorMessage;
            return CreateBadRequest(strError);
        }

        protected IHttpActionResult CreateBadRequest(string strErrorMessage)
        {
            logger.ErrorFormat("Return BadRequest: {0}", strErrorMessage);
            return BadRequest(strErrorMessage);
        }
        
        protected string GetCurrentRequestLang()
        {
            var lang = "vi-VN";
            if (HttpContext.Current.Request.Headers["Accept-Language"] != null)
            {
                lang = HttpContext.Current.Request.Headers["Accept-Language"].ToString();
            }

            try
            {
                if (!string.IsNullOrEmpty(lang))
                {
                    if(lang.Length > 5)
                    {
                        lang = lang.Substring(0, 5);
                    }
                }                
            }
            catch
            {

            }

            return lang;
        }

        public JsonActionResult CatchJsonExceptionResult(ApiResponseCommonModel returnModel)
        {
            returnModel.error.error_code = ((int)HttpStatusCode.InternalServerError).ToString();
            returnModel.error.message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

            var jsonString = JsonConvert.SerializeObject(returnModel);
            return new JsonActionResult(HttpStatusCode.InternalServerError, jsonString);
        }

        public JsonActionResult CachErrorResult(ApiResponseCommonModel returnModel)
        {
            var jsonString = JsonConvert.SerializeObject(returnModel);

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        public JsonActionResult SuccessUpdatedResult(ApiResponseCommonModel returnModel)
        {
            returnModel.message = ManagerResource.LB_UPDATE_SUCCESS;

            var jsonString = JsonConvert.SerializeObject(returnModel);

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        public JsonActionResult SuccessDeletedResult(ApiResponseCommonModel returnModel)
        {
            returnModel.message = ManagerResource.LB_DELETE_SUCCESS;

            var jsonString = JsonConvert.SerializeObject(returnModel);

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        public ApiFilterModel GetFilterConfig()
        {
            var apiFilterModel = new ApiFilterModel();

            var httpRequest = HttpContext.Current.Request;
            var pageIndex = 1;
            var keyword = string.Empty;
            var status = -1;

            if (httpRequest["keyword"] != null)
                keyword = httpRequest["keyword"].ToString();

            if (!string.IsNullOrEmpty(keyword))
                keyword = keyword.Trim();

            if (httpRequest["page_index"] != null)
                pageIndex = Utils.ConvertToInt32(httpRequest["page_index"]);

            var pageSize = SystemSettings.DefaultPageSize;

            if (httpRequest["page_size"] != null)
                pageSize = Utils.ConvertToInt32(httpRequest["page_size"]);

            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize > SystemSettings.DefaultPageSize || pageSize <= 0)
                pageSize = SystemSettings.DefaultPageSize;

            if (httpRequest["status"] != null)
                status = Utils.ConvertToInt32(httpRequest["status"]);

            apiFilterModel.language_code = GetCurrentRequestLang();

            apiFilterModel.page_index = pageIndex;
            apiFilterModel.page_size = pageSize;
            apiFilterModel.status = status;

            apiFilterModel.keyword = keyword;

            return apiFilterModel;
        }

        public dynamic GetFilterConfig(dynamic apiFilterModel)
        {
            var httpRequest = HttpContext.Current.Request;
            var pageIndex = 1;
            var keyword = string.Empty;

            if (httpRequest["keyword"] != null)
                keyword = httpRequest["keyword"].ToString();

            if (!string.IsNullOrEmpty(keyword))
                keyword = keyword.Trim();

            if (httpRequest["page_index"] != null)
                pageIndex = Utils.ConvertToInt32(httpRequest["page_index"]);

            var pageSize = SystemSettings.DefaultPageSize;

            if (httpRequest["page_size"] != null)
                pageSize = Utils.ConvertToInt32(httpRequest["page_size"]);

            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize > SystemSettings.DefaultPageSize || pageSize <= 0)
                pageSize = SystemSettings.DefaultPageSize;

            apiFilterModel.page_index = pageIndex;
            apiFilterModel.page_size = pageSize;

            apiFilterModel.keyword = keyword;

            return apiFilterModel;
        }

        public ApiFilterModel ValidateFilterConfig(int? page_index, int? page_size, string keyword = "")
        {
            var apiFilterModel = new ApiFilterModel();

            if (!string.IsNullOrEmpty(keyword))
                keyword = keyword.Trim();

            var pageIndex = 1;

            if (page_index != null)
                pageIndex = page_index.Value;

            var pageSize = SystemSettings.DefaultPageSize;

            if (page_size != null)
                pageSize = page_size.Value;

            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize > SystemSettings.DefaultPageSize || pageSize <= 0)
                pageSize = SystemSettings.DefaultPageSize;

            apiFilterModel.page_index = pageIndex;
            apiFilterModel.page_size = pageSize;

            apiFilterModel.keyword = keyword;

            return apiFilterModel;
        }

        /// <summary>
        /// Example: var result = RenderPartialViewAsString("../EmailTemplates/RecoverPassword", emailModel);
        /// </summary>
        /// <param name="partialName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderPartialViewAsString(string partialName, object model)
        {
            var sw = new StringWriter();
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            // point to an empty controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmptyController");

            var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new EmptyController());

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialName).View;

            view.Render(new ViewContext(controllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

            return sw.ToString();
        }
    }
    class EmptyController : ControllerBase
    {
        protected override void ExecuteCore() { }
    }
}