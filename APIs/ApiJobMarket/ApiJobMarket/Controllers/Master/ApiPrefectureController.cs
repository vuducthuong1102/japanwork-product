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

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/prefectures")]
    public class ApiPrefectureController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiPrefectureController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Prefectures-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;

                var listPrefecture = CommonHelpers.GetListPrefectures();

                await Task.FromResult(listPrefecture);                

                returnModel.value = listPrefecture;

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
            var requestName = "Prefectures-GetDetail";
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
                var storePrefecture = GlobalContainer.IocContainer.Resolve<IStorePrefecture>();

                var info = storePrefecture.GetById(id);
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

        [HttpGet]
        [Route("{id:int}/cities")]
        public async Task<IHttpActionResult> GetListCityByPrefecture(int id)
        {
            var requestName = "Prefectures-GetCities";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var listCity = CommonHelpers.GetListCitiesByPrefecture(id);

                await Task.FromResult(listCity);
                var returnList = new List<ApiResponseCityItemModel>();

                if (listCity.HasData())
                {
                    foreach (var reg in listCity)
                    {
                        ApiResponseCityItemModel item = new ApiResponseCityItemModel();
                        item.id = reg.id;
                        item.city = reg.city;
                        item.furigana = reg.furigana;
                        item.prefecture_id = reg.prefecture_id;

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

        [HttpGet]
        [Route("{id:int}/train_lines")]
        public async Task<IHttpActionResult> GetListTrainLineByPrefecture(int id)
        {
            var requestName = "Prefectures-GetTrainLines";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var listTrainLines = CommonHelpers.GetListTrainLinesByPrefecture(id);

                await Task.FromResult(listTrainLines);
                //var returnList = new List<ApiResponseCityItemModel>();

                //if (listCity.HasData())
                //{
                //    foreach (var reg in listCity)
                //    {
                //        ApiResponseCityItemModel item = new ApiResponseCityItemModel();
                //        item.id = reg.id;
                //        item.city = reg.city;
                //        item.furigana = reg.furigana;
                //        item.prefecture_id = reg.prefecture_id;

                //        returnList.Add(item);
                //    }
                //}

                returnModel.value = listTrainLines;

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
            var requestName = "Prefectures-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;
                
                var listData = new List<IdentityPrefecture>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CommonHelpers.GetBaseInfoPrefecture(currentId);
                        if (cachedData != null)
                            listData.Add(cachedData);
                    }
                }
                await Task.FromResult(listData);

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

        [HttpPost]
        [Route("list_by_regions")]
        public async Task<IHttpActionResult> GetListByListRegionIds(ApiGetListByIdsModel model)
        {
            var requestName = "Prefectures-GetListByListRegionIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;                
                var returnList = new List<IdentityPrefecture>();

                if (model.ListIds.HasData())
                {
                    foreach (var regionId in model.ListIds)
                    {
                        var listPrefectures = CommonHelpers.GetListPrefecturesByRegion(regionId);
                        if (listPrefectures.HasData())
                            returnList.AddRange(listPrefectures);
                    }
                }

                await Task.FromResult(returnList);

                returnModel.value = returnList;

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
    }
}
