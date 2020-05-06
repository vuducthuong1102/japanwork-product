using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using ApiJobMarket.DB.Sql.Stores;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using Autofac;
using System.Collections.Generic;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.DB.Sql.Entities;
using System.Linq;
using System.Web;
using ApiJobMarket.ShareLibs;
using System.Dynamic;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/stations")]
    public class ApiStationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiStationController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Stations-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var listData = CommonHelpers.GetListStations();

                await Task.FromResult(listData);                

                returnModel.value = listData;

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

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Stations-GetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                //Check and register from database
                var returnCode = EnumCommonCode.Success;
                var info = CommonHelpers.GetBaseInfoStation(id);

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

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Stations-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;
                var listStations = CommonHelpers.GetListStations(model.ListIds);

                await Task.FromResult(listStations);

                returnModel.value = listStations;

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
        [Route("{id:int}/get_list_by_position")]
        public async Task<IHttpActionResult> GetListByPosition(int id)
        {
            var requestName = "Stations-GetListByPosition";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                var returnCode = EnumCommonCode.Success;
                var httpRequest = HttpContext.Current.Request;
                var storeStation = GlobalContainer.IocContainer.Resolve<IStoreStation>();
                var listResult = storeStation.GetListByPosition(id);

                await Task.FromResult(listResult);

                returnModel.value = listResult;

                jsonString = JsonConvert.SerializeObject(returnModel);
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
            var requestName = "Stations-GetListByCityId";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                var returnCode = EnumCommonCode.Success;
                var storeStation = GlobalContainer.IocContainer.Resolve<IStoreStation>();
                var listResult = storeStation.GetListByCityId(id);

                await Task.FromResult(listResult);

                returnModel.value = listResult;

                jsonString = JsonConvert.SerializeObject(returnModel);
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
            var requestName = "Stations-GetSuggestionsByPage";
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

                var storeStation = GlobalContainer.IocContainer.Resolve<IStoreStation>();
                var listPlace = storeStation.GetSuggestionsByPage(dynamicFilter, apiFilter.page_index.Value, apiFilter.page_size.Value);

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
