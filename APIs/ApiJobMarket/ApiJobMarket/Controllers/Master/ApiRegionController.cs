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
using System.Linq;
using System.Web;
using ApiJobMarket.ShareLibs;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/regions")]
    public class ApiRegionController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiRegionController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Regions-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var country_id = 0;
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;
                if(httpRequest["country_id"] != null)
                {
                    country_id = Utils.ConvertToInt32(httpRequest["country_id"]);
                }

                var listRegion = CommonHelpers.GetListRegions(country_id);

                await Task.FromResult(listRegion);                

                returnModel.value = listRegion;

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
        [Route("get_suggestions")]
        public async Task<IHttpActionResult> GetSuggestionsByPage()
        {
            var requestName = "Region-GetSuggestionsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCity();
                filter.keyword = apiFilter.keyword;
                var storeSearchPlace = GlobalContainer.IocContainer.Resolve<IStoreSearchPlace>();

                var listPlace = storeSearchPlace.GetSuggestionsByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

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

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Regions-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityRegion>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CommonHelpers.GetBaseInfoRegion(currentId);
                        if (cachedData != null)
                            listData.Add(cachedData);
                    }
                }

                returnModel.value = listData;

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
            var requestName = "Regions-GetDetail";
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
                var storeRegion = GlobalContainer.IocContainer.Resolve<IStoreRegion>();

                var regionInfo = storeRegion.GetById(id);
                await Task.FromResult(regionInfo);
                
                returnModel.value = regionInfo;
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
        [Route("{id:int}/prefectures")]
        public async Task<IHttpActionResult> GetListPrefectureByRegion(int id)
        {
            var requestName = "Regions-GetPrefectures";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var myList = CommonHelpers.GetListPrefecturesByRegion(id);

                await Task.FromResult(myList);
                var returnList = new List<ApiResponsePrefectureItemModel>();

                if (myList.HasData())
                {
                    foreach (var record in myList)
                    {
                        ApiResponsePrefectureItemModel item = new ApiResponsePrefectureItemModel();
                        item.id = record.id;
                        item.prefecture = record.prefecture;
                        item.furigana = record.furigana;
                        item.region_id = record.region_id;

                        returnList.Add(item);
                    }
                }

                returnModel.value = returnList;

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
