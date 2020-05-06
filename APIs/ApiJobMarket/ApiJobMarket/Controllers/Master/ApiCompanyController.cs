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
using System.Web;
using System.Linq;
using ApiJobMarket.Services;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using System.Dynamic;
using ApiCompanyMarket.Helpers;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/companies")]
    public class ApiCompanyController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCompanyController>();

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Companies-GetDetail";
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

                var info = CompanyHelpers.GetBaseInfoCompany(id);
                if (info != null)
                {
                    info.address_info = new IdentityAddress();

                    var storePrefecture = GlobalContainer.IocContainer.Resolve<IStorePrefecture>();
                    var prefecture = storePrefecture.GetById(info.prefecture_id);
                    if (prefecture != null) info.address_info.prefecture_info = prefecture;

                    var storeRegion = GlobalContainer.IocContainer.Resolve<IStoreRegion>();
                    var region = storeRegion.GetById(info.region_id);
                    if (region != null) info.address_info.region_info = region;

                    var storeCity = GlobalContainer.IocContainer.Resolve<IStoreCity>();
                    var city = storeCity.GetById(info.city_id);
                    if (city != null) info.address_info.city_info = city;
                }
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
        [Route("{id:int}/count_job")]
        public async Task<IHttpActionResult> CountJobById(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Companies-CountJobById";
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

                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();
                var info = storeCompany.CountJobById(id);

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

        //[HttpDelete]
        //[Route("{id:int}")]
        //public async Task<IHttpActionResult> Delete(int id)
        //{
        //    CreateDocumentApi(id);
        //    var requestName = "Companies-Delete";
        //    var returnModel = new ApiResponseCommonModel();
        //    var jsonString = string.Empty;
        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin {0} request", requestName));

        //        var returnCode = EnumCommonCode.Success;
        //        var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

        //        var info = storeCompany.GetById(id);
        //        await Task.FromResult(info);

        //        if(info == null)
        //        {
        //            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);
        //        }
        //        else
        //        {
        //            storeCompany.Delete(id);

        //            //Clear cache
        //            CompanyHelpers.ClearCache(id);
        //        }

        //        jsonString = JsonConvert.SerializeObject(returnModel);

        //        //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);                
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
        //        logger.ErrorException(strError, ex);


        //        return CatchJsonExceptionResult(returnModel);
        //    }
        //    finally
        //    {
        //        //logger.Debug(string.Format("Ended {0} request", requestName));
        //    }

        //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //}

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(ApiCompanyDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Company-Delete";
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
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();


                var ressult = storeCompany.Delete(model.ids, model.staff_id);
                await Task.FromResult(ressult);
                returnModel.value = ressult;
                if (ressult)
                {
                    var listIds = model.ids.Split(',');
                    if (listIds.HasData())
                    {
                        foreach (var item in listIds)
                        {
                            CompanyHelpers.ClearCache(Utils.ConvertToInt32(item));
                        }
                    }
                }

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

            return SuccessDeletedResult(returnModel);
        }

        [HttpDelete]
        [Route("delete_all_jobs")]
        public async Task<IHttpActionResult> DeleteAllJobs(ApiCompanyDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Company-DeleteAllJobs";
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
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var info = CompanyHelpers.GetBaseInfoCompany(model.company_id);
                if (info != null)
                {
                    if (info.agency_id != model.agency_id)
                    {
                        returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                        return CachErrorResult(returnModel);
                    }
                }

                var items = storeCompany.DeleteAllJobs(model.company_id);
                await Task.FromResult(items);
                returnModel.value = items;

                if (items.HasData())
                {
                    foreach (var item in items)
                    {
                        var key = string.Format(EnumFormatInfoCacheKeys.Job, item) + "_";

                        CachingHelpers.ClearCacheByPrefix(key);
                    }
                }

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

            return SuccessDeletedResult(returnModel);
        }

        [HttpGet]
        [Route("{id:int}/jobs")]
        public async Task<IHttpActionResult> GetListJobByCompany(int id)
        {
            var requestName = "Companies-GetJobs";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var httpRequest = HttpContext.Current.Request;
                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.keyword = apiFilter.keyword;
                if (httpRequest["agency_id"] != null)
                    filter.agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                if (httpRequest["translate_status"] != null)
                    filter.translate_status = Utils.ConvertToInt32(httpRequest["translate_status"]);

                if (httpRequest["staff_id"] != null)
                    filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);

                if (httpRequest["sub_id"] != null)
                    filter.sub_field_id = Utils.ConvertToInt32(httpRequest["sub_id"]);

                if (httpRequest["prefecture_id"] != null)
                    filter.prefecture_id = Utils.ConvertToInt32(httpRequest["prefecture_id"]);

                if (httpRequest["japanese_level_number"] != null)
                    filter.japanese_level_number = Utils.ConvertToInt32(httpRequest["japanese_level_number"]);

                if (httpRequest["employment_type_id"] != null)
                    filter.employment_type_id = Utils.ConvertToInt32(httpRequest["employment_type_id"]);

                if (httpRequest["salary_min"] != null)
                    filter.salary_min = Utils.ConvertToInt32(httpRequest["salary_min"]);

                if (httpRequest["salary_max"] != null)
                    filter.salary_max = Utils.ConvertToInt32(httpRequest["salary_max"]);
                filter.company_id = id;
                filter.status = Utils.ConvertToIntFromQuest(apiFilter.status);

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                List<IdentityJob> myList = storeJob.GetListByCompany(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(myList);

                var returnData = new List<IdentityJob>();
                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        var returnItem = JobHelpers.Agency_GetBaseInfoJob(item.id, GetCurrentRequestLang());
                        if (returnItem != null)
                        {
                            returnItem.Extensions.application_count = item.Extensions.application_count;
                            returnItem.Extensions.candidate_count = item.Extensions.candidate_count;
                            returnItem.Extensions.introduce_count = item.Extensions.introduce_count;
                            returnItem.Extensions.invited_count = item.Extensions.invited_count;
                            returnItem.company_pic_id = item.company_pic_id;

                            returnData.Add(returnItem);
                        }
                    }

                    returnModel.total = myList[0].total_count;
                }

                returnModel.value = returnData;

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
        [Route("{id:int}/jobs_for_delete")]
        public async Task<IHttpActionResult> GetListJobForDelete(int id)
        {
            var requestName = "Companies-GetListJobForDelete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var httpRequest = HttpContext.Current.Request;
                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.keyword = apiFilter.keyword;
                if (httpRequest["agency_id"] != null)
                    filter.agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);
                filter.company_id = id;

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                List<IdentityJob> myList = storeJob.GetListForDelete(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(myList);

                var returnData = new List<IdentityJob>();
                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        var returnItem = JobHelpers.GetBaseInfoJob(item.id, GetCurrentRequestLang());
                        if (returnItem != null)
                        {
                            returnItem.Extensions.application_count = item.Extensions.application_count;
                            returnItem.Extensions.candidate_count = item.Extensions.candidate_count;
                            returnItem.Extensions.introduce_count = item.Extensions.introduce_count;
                            returnItem.Extensions.invited_count = item.Extensions.invited_count;
                            returnItem.company_pic_id = item.company_pic_id;

                            returnData.Add(returnItem);
                        }
                    }

                    returnModel.total = myList[0].total_count;
                }

                returnModel.value = returnData;

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
        [Route("get_by_page")]
        public async Task<IHttpActionResult> GetByPage()
        {
            var requestName = "Companies_GetBypage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCompany();
                filter.keyword = apiFilter.keyword;
                filter.language_code = GetCurrentRequestLang();

                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var myList = storeCompany.GetByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(myList);

                if (myList.HasData())
                {
                    returnModel.total = myList[0].total_count;

                    foreach (var item in myList)
                    {
                        item.logo_full_path = CdnHelper.SocialGetFullImgPath(item.logo_path);
                    }
                }

                returnModel.value = myList;
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

        [HttpPost]
        [Route("update_manager")]
        public async Task<IHttpActionResult> UpdateUserManager(ApiCompanyUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Companies-UpdateUserManager";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var fileCount = httpRequest.Files.Count;
                var returnId = 0;
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                if (id > 0)
                {
                    var info = storeCompany.GetById(id);
                    await Task.FromResult(info);

                    if (info == null)
                    {
                        returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                        return CachErrorResult(returnModel);
                    }
                    else
                    {
                        //Validate agency
                        if (model.agency_id != info.agency_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                var updateInfo = ExtractFormData(model);
                returnId = storeCompany.Update(updateInfo);
                returnModel.value = returnId;

                //Clear cache
                CompanyHelpers.ClearCache(id);

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

            return SuccessUpdatedResult(returnModel);
        }


        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> UpdateCompany(ApiCompanyUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Companies-UpdateCompany";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var fileCount = httpRequest.Files.Count;
                var returnId = 0;
                if (fileCount < 1)
                {
                    //return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                if (id > 0)
                {
                    var info = storeCompany.GetById(id);
                    await Task.FromResult(info);

                    if (info == null)
                    {
                        returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                        return CachErrorResult(returnModel);
                    }
                    else
                    {
                        //Validate agency
                        if (model.agency_id != info.agency_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                //if (fileCount > 0)
                //{
                //    //Begin upload
                //    var uploadReturn = UploadCompanyLogo(model.id, model.agency_id);
                //    if (uploadReturn != null && uploadReturn.HasData())
                //        logoPath = uploadReturn[0].Path;
                //}
                //else
                //    logoPath = info.logo_path;

                var updateInfo = ExtractFormData(model);
                returnId = storeCompany.Update(updateInfo);
                returnModel.value = returnId;

                //Clear cache
                CompanyHelpers.ClearCache(id);

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

            return SuccessUpdatedResult(returnModel);
        }

        [HttpPost]
        [Route("upload_logo")]
        public async Task<IHttpActionResult> UploadLogo()
        {
            var requestName = "Companies-UploadLogo";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var fileCount = httpRequest.Files.Count;
                var company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                var agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                if (fileCount < 1)
                {
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                if (company_id > 0)
                {
                    var info = storeCompany.GetById(company_id);
                    await Task.FromResult(info);

                    if (info == null)
                    {
                        returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                        return CachErrorResult(returnModel);
                    }
                    else
                    {
                        //Validate agency
                        if (agency_id != info.agency_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                if (fileCount > 0)
                {
                    returnModel.value = UploadCompanyLogo(company_id, agency_id);
                    CompanyHelpers.ClearCache(company_id);
                }

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

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        [HttpPost]
        [Route("get_list_by_ids")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Companies-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityCompany>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CompanyHelpers.GetBaseInfoCompany(currentId);
                        if (cachedData != null)
                        {
                            listData.Add(cachedData);
                        }
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
        [Route("get_list_active_by_ids")]
        public async Task<IHttpActionResult> GetListActiveByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Companies-GetListActiveByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityCompany>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CompanyHelpers.GetBaseInfoCompany(currentId);
                        if (cachedData != null)
                        {
                            if (cachedData.status != 9)
                            {
                                listData.Add(cachedData);
                            }
                        }
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

        private IdentityCompany ExtractFormData(ApiCompanyUpdateModel model)
        {
            var info = new IdentityCompany();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.company_name = model.company_name;
            info.description = model.description;
            info.email = model.email;
            info.phone = model.phone;
            info.website = model.website;
            info.fax = model.fax;
            info.logo_path = model.logo_path;
            info.company_size_id = Utils.ConvertToIntFromQuest(model.company_size_id);
            info.sub_industry_id = Utils.ConvertToIntFromQuest(model.sub_industry_id);
            info.establish_year = Utils.ConvertToIntFromQuest(model.establish_year);
            info.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);

            info.address_detail = model.address_detail;
            info.address_furigana = model.address_furigana;
            info.pic_id = model.pic_id;
            info.staff_id = model.staff_id;

            if (model.Address != null)
            {
                info.country_id = model.Address.country_id;
                info.region_id = model.Address.region_id;
                info.prefecture_id = model.Address.prefecture_id;
                info.city_id = model.Address.city_id;
            }

            info.language_code = GetCurrentRequestLang();

            return info;
        }

        private List<FileUploadResponseModel> UploadCompanyLogo(int company_id, int agency_id = 0)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "Companies";
            uploadModel.ObjectId = company_id.ToString();

            if (company_id <= 0)
            {
                uploadModel.SubDir = "Companies/age";
                uploadModel.ObjectId = agency_id.ToString();
            }

            uploadModel.InCludeDatePath = false;

            if (uploadModel.Files != null && uploadModel.Files[0] != null)
            {
                var apiResult = CdnServices.UploadImagesAsync(uploadModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.Code == EnumCommonCode.Success)
                    {
                        returnUploaded = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiResult.Data.ToString());
                        if (returnUploaded.HasData())
                        {
                            foreach (var item in returnUploaded)
                            {
                                item.FullPath = CdnHelper.SocialGetFullImgPath(item.Path);
                            }
                        }
                    }
                }
                else
                {
                    logger.Error("Failed to get Upload image because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload image because: The image is null");
            }

            return returnUploaded;
        }

        [HttpPost]
        [Route("counter_for_deletion")]
        public async Task<IHttpActionResult> GetCounterForDeletion(ApiCompanyDeleteModel model)
        {
            CreateDocumentApi(model);

            var requestName = "Companies-GetCounterForDeletion";
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
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var info = CompanyHelpers.GetBaseInfoCompany(model.company_id);
                IdentityCompanyCounter counter = null;
                if (info != null)
                {
                    if (info.agency_id != model.agency_id)
                    {
                        returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                        return CachErrorResult(returnModel);
                    }

                    counter = storeCompany.GetCounterForDeletion(model.company_id);
                }

                await Task.FromResult(counter);
                returnModel.value = counter;
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
