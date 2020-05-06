using System;
using System.Net;
using System.Web;
using System.Web.Http;
using ApiJobMarket.ActionResults;
using ApiJobMarket.DB.Sql.Stores;
using ApiJobMarket.Helpers;
using ApiJobMarket.Logging;
using ApiJobMarket.Models;
using ApiJobMarket.Resources;
using ApiJobMarket.Services;
using ApiJobMarket.Settings;
using ApiJobMarket.ShareLibs;
using Autofac;
using Newtonsoft.Json;

namespace ApiJobMarket.Controllers
{
    /// <summary>
    /// This class will act as a base class which other Web API controllers will inherit from, 
    /// for now it will contain three basic methods
    /// </summary>
    public class BaseApiAdminController : ApiController
    {
        private readonly ILog logger = LogProvider.For<BaseApiAdminController>();

        private IStoreDocumentApi _documentStore;
        public BaseApiAdminController()
        {
            _documentStore = GlobalContainer.IocContainer.Resolve<IStoreDocumentApi>();
        }

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

        protected ApiCheckUserTokenModel GetDataCheckToken(int userId, string tokenKey)
        {
            var returnModel = new ApiCheckUserTokenModel();
            returnModel.UserId = userId;
            returnModel.Token = tokenKey;
            returnModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

            return returnModel;
        }

        protected bool IsValidUserToken(int userId, string tokenKey)
        {
            var isValid = false;
            try
            {
                var checkModel = GetDataCheckToken(userId, tokenKey);
                //Check token 
                var dataValid = AccountServices.CheckUserTokenAsync(checkModel).Result;
                if (dataValid != null)
                {
                    if (dataValid.Code == EnumCommonCode.Success)
                    {
                        isValid = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                string strError = "Failed for CheckUserTokenAsync(IsValidUserToken) request: " + ex.Message;
                logger.ErrorException(strError, ex);
            }

            return isValid;
        }

        public void CreateDocumentApi(object ob)
        {
            if (SystemSettings.IsLogParamater)
            {
                string linkUrl = HttpContext.Current.Request.Url.AbsolutePath;

                string data = JsonConvert.SerializeObject(ob);
                _documentStore.Insert(linkUrl, data);
            }
        }

        protected string GetCurrentRequestLang()
        {
            var lang = "vi-VN";
            if (HttpContext.Current.Request.Headers["Accept-Language"] != null)
            {
                lang = HttpContext.Current.Request.Headers["Accept-Language"].ToString();
            }

            return lang;
        }

        public JsonActionResult CatchJsonExceptionResult(ApiResponseCommonModel returnModel)
        {
            returnModel.error.error_code = ((int)HttpStatusCode.InternalServerError).ToString();
            returnModel.error.message = Resources.UserApiResource.COMMON_ERROR_SYSTEM;

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
            returnModel.message = UserApiResource.UPDATE_INFO_SUCCESS;

            var jsonString = JsonConvert.SerializeObject(returnModel);

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        public JsonActionResult SuccessDeletedResult(ApiResponseCommonModel returnModel)
        {
            returnModel.message = UserApiResource.DELETE_SUCCESS;

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

            if (pageSize > SystemSettings.MaxPageSize || pageSize <= 0)
                pageSize = SystemSettings.MaxPageSize;

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

            if (pageSize > SystemSettings.MaxPageSize || pageSize <= 0)
                pageSize = SystemSettings.MaxPageSize;

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

            if (pageSize > SystemSettings.MaxPageSize || pageSize <= 0)
                pageSize = SystemSettings.MaxPageSize;

            apiFilterModel.page_index = pageIndex;
            apiFilterModel.page_size = pageSize;

            apiFilterModel.keyword = keyword;

            return apiFilterModel;
        }
    }
}