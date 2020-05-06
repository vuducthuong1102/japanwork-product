using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using ApiJobMarket.SharedLib.Extensions;
using System.Collections.Generic;
using ApiJobMarket.DB.Sql.Stores;
using Autofac;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using System.Web;
using System.Dynamic;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/train_lines")]
    public class ApiTrainLineController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiTrainLineController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "TrainLines-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;

                var listTrainLine = CommonHelpers.GetListTrainLines();

                await Task.FromResult(listTrainLine);

                returnModel.value = listTrainLine;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);

                //return CatchJsonExceptionResult(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }
        [HttpGet]
        [Route("{id:int}/get_list_by_city_id")]
        public async Task<IHttpActionResult> GetListByCityId(int id)
        {
            var requestName = "TrainLines-GetListByCityId";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;

                var storeTrainLine = GlobalContainer.IocContainer.Resolve<IStoreTrainLine>();
                string keyword = string.Empty;
                if (HttpContext.Current.Request["keyword"] != null)
                    keyword = HttpContext.Current.Request["keyword"].ToString();
                var listResult = storeTrainLine.GetListByCityId(id, keyword);
                await Task.FromResult(listResult);

                returnModel.value = listResult;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);

                //return CatchJsonExceptionResult(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "TrainLines-GetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeTrainLine = GlobalContainer.IocContainer.Resolve<IStoreTrainLine>();

                var info = storeTrainLine.GetById(id);
                await Task.FromResult(info);

                returnModel.value = info;
                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("get_suggestions")]
        public async Task<IHttpActionResult> GetSuggestionsByPage()
        {
            var requestName = "TrainLines-GetSuggestionsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var apiFilter = GetFilterConfig();
                dynamic dynamicFilter = new ExpandoObject();
                dynamicFilter.keyword = apiFilter.keyword;
                dynamicFilter.place_id = 0;

                if (HttpContext.Current.Request["place_id"] != null)
                    dynamicFilter.place_id = Utils.ConvertToInt32(HttpContext.Current.Request["place_id"]);

                var storeTrainLine = GlobalContainer.IocContainer.Resolve<IStoreTrainLine>();

                var listPlace = storeTrainLine.GetSuggestionsByPage(dynamicFilter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(listPlace);

                returnModel.value = listPlace;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }
    }
}
