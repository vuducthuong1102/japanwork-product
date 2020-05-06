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
using ApiJobMarket.ShareLibs;
using System.Web;
using ApiJobMarket.Services;
using ApiJobMarket.Resources;
using System.Dynamic;
using System.Text.RegularExpressions;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/job_seekers")]
    public class ApiJobSeekerController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiJobSeekerController>();

        [HttpGet]
        [Route("{id:int}/applications")]
        public async Task<IHttpActionResult> GetListApplication(int id)
        {
            var requestName = "Job_Seekers-GetListApplication";
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

                var apiFilter = GetFilterConfig();
                var filter = new IdentityApplication();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = id;

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var listData = storeApplication.GetListByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, GetCurrentRequestLang());

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }

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

        [HttpPost]
        [Route("get_list_by_ids")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "JobSeeker-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityJobSeeker>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = JobSeekerHelpers.GetBaseInfo(currentId);
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


        [HttpGet]
        [Route("{id:int}/saved_jobs")]
        public async Task<IHttpActionResult> GetListSavedJob(int id)
        {
            var requestName = "Job_Seekers-GetListSavedJob";
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

                var apiFilter = GetFilterConfig();
                var filter = new IdentityJob();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = id;

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var listData = storeJob.GetListSavedByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                    returnModel.total = listData[0].total_count;

                returnModel.value = listData;
                //if(listData.HasData())
                //{
                //    foreach (var item in listData)
                //    {
                //        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                //        {
                //            if (item.status == (int)enm)
                //                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                //        }
                //    }
                //}

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
        [Route("{id:int}/cvs")]
        public async Task<IHttpActionResult> GetListCV(int id)
        {
            var requestName = "Job_Seekers-GetListCV";
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

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCv();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = id;

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var listData = storeCv.GetListByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                var returnData = new List<IdentityCv>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = CvHelpers.GetBaseInfoCv(item.id);

                        returnData.Add(returnItem);
                    }
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
        [Route("{id:int}/cvs/sent_to_agency")]
        public async Task<IHttpActionResult> GetListCVSentToAgency(int id)
        {
            var requestName = "Job_Seekers-GetListCVSentToAgency";
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

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCv();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = id;
                filter.agency_id = apiFilter.agency_id ?? 0;

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var listData = storeCv.GetListCVSentToAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                var returnData = new List<IdentityCv>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = CvHelpers.GetBaseInfoCv(item.id);

                        returnData.Add(returnItem);
                    }
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

        [HttpPost]
        [Route("counter_for_deletion")]
        public async Task<IHttpActionResult> GetCounterForDeletion(ApiJobSeekerDeleteModel model)
        {
            CreateDocumentApi(model);

            var requestName = "JobSeeker-GetCounterForDeletion";
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
                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                IdentityJobSeekerCounter counter = storeJobSeeker.GetCounterForDeletion(model.id, model.agency_id ?? 0);

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

        [HttpGet]
        [Route("{id:int}/cs")]
        public async Task<IHttpActionResult> GetListCareerSheet(int id)
        {
            var requestName = "Job_Seekers-GetListCareerSheet";
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

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCs();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = id;

                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();

                var listData = storeCs.GetListByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                var returnData = new List<IdentityCs>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = CsHelpers.GetBaseInfoCs(item.id);

                        returnData.Add(returnItem);
                    }
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
        [Route("{id:int}/print_codes")]
        public async Task<IHttpActionResult> GetListPrintCode(int id)
        {
            var requestName = "Job_Seekers-GetListPrintCode";
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

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var listData = storeCv.GetListCodeByJobSeeker(id);
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
        [Route("{id:int}/notifications")]
        public async Task<IHttpActionResult> GetListNotification(int id)
        {
            var requestName = "Job_Seekers-GetListNotification";
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

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.job_seeker_id = id;

                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreNotification>();

                List<IdentityNotification> listData = storeNotif.GetListByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                }

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
        [Route("{agency_id:int}/get_list")]
        public async Task<IHttpActionResult> GetByAgency(int agency_id)
        {
            var requestName = "JobSeeker-GetByAgency";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var returnCode = EnumCommonCode.Success;

                var apiFilter = GetFilterConfig();
                var filter = new IdentityJobSeeker();
                filter.keyword = apiFilter.keyword;
                filter.agency_id = agency_id;

                var httpRequest = HttpContext.Current.Request;

                if (httpRequest["staff_id"] != null)
                    filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);

                if (httpRequest["status"] != null)
                    filter.status = Utils.ConvertToInt32(httpRequest["status"]);

                if (httpRequest["type_job_seeker"] != null)
                    filter.type_job_seeker = Utils.ConvertToInt32(httpRequest["type_job_seeker"]);

                if (httpRequest["japanese_level"] != null)
                    filter.japanese_level = Utils.ConvertToInt32(httpRequest["japanese_level"]);

                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var listData = storeJobSeeker.GetByAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);
                if (listData.HasData())
                {

                    returnModel.total = listData[0].total_count;
                }

                returnModel.value = listData;
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
        [Route("{id:int}/unread_notification_count")]
        public async Task<IHttpActionResult> CountUnreadNotification(int id)
        {
            var requestName = "Job_Seekers-CountUnreadNotification";
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

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.user_id = id;

                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreNotification>();

                var totalCount = storeNotif.CountUnread(filter);
                await Task.FromResult(totalCount);

                returnModel.total = totalCount;
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
        [Route("{id:int}/detail_for_update")]
        public async Task<IHttpActionResult> GetDetailForUpdate(int id)
        {
            var requestName = "Job_Seekers-GetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var language_code = GetCurrentRequestLang();
                var info = JobSeekerHelpers.GetBaseInfo(id);

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
        [Route("{id:int}/config")]
        public async Task<IHttpActionResult> GetConfig(int id)
        {
            var requestName = "Job_Seekers-GetConfig";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var info = JobSeekerHelpers.GetConfig(id);

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
        [Route("update_config")]
        public async Task<IHttpActionResult> UpdateConfig(ApiJobSeekerConfigModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-UpdateConfig";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var updateInfo = ExtractConfigData(model);

                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var result = storeJobSeeker.UpdateConfig(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerConfig, model.job_seeker_id));

                returnModel.value = result;

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
        [Route("{id:int}/upload_image")]
        public async Task<IHttpActionResult> UploadImage(int id)
        {
            var requestName = "Job_seekers-UploadImage";
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

                if (fileCount < 1)
                {
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var info = JobSeekerHelpers.GetBaseInfo(id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (fileCount > 0)
                        returnModel.value = UploadJobSeekerImage(info);
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
        [Route("update_profile")]
        public async Task<IHttpActionResult> UpdateProfile(ApiJobSeekerModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-UpdateProfile";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var updateInfo = ExtractFormData(model);

                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var result = storeJobSeeker.UpdateProfile(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.user_id));

                returnModel.value = result;

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

        [HttpGet]
        [Route("{id:int}/edu_histories")]
        public async Task<IHttpActionResult> GetListEduHistory(int id)
        {
            var requestName = "Job_Seekers-GetListEduHistory";
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

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var listData = storeEduHistory.GetListJobSeekerEduHistory(id);

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
        [Route("{id:int}/work_histories")]
        public async Task<IHttpActionResult> GetListWorkHistory(int id)
        {
            var requestName = "Job_Seekers-GetListWorkHistory";
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

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var listData = storeWorkHistory.GetListJobSeekerWorkHistory(id);

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
        [Route("{id:int}/certificates")]
        public async Task<IHttpActionResult> GetListCertificate(int id)
        {
            var requestName = "Job_Seekers-GetListCertificate";
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

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var listData = storeCertificate.GetListJobSeekerCertificate(id);

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

        [HttpPost]
        [Route("save_token_firebase")]
        public async Task<IHttpActionResult> SaveTokenFireBase(ApiSaveTokenFireBaseModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-SaveTokenFireBase";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                var user_id = Utils.ConvertToIntFromQuest(model.user_id);

                var info = new IdentityTokenFireBase();
                info.user_id = user_id;
                info.token = model.token_firebase;

                var status = storeJobSeeker.SaveTokenFireBase(info);
                await Task.FromResult(status);

                returnModel.value = status;
                returnModel.message = UserApiResource.SUCCESS_SAVED;
                //if (status == 0)
                //{
                //    returnModel.message = UserApiResource.SUCCESS_SAVED;
                //}
                //else
                //{
                //    //code already saved
                //    returnModel.error.error_code = EnumErrorCode.E030103.ToString();
                //    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030103);

                //    return CachErrorResult(returnModel);
                //}

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
        [Route("invitation_suggestions")]
        public async Task<IHttpActionResult> GetInvitationSuggestions()
        {
            var requestName = "Job_Seekers-GetInvitationSuggestions";
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
                var httpRequest = HttpContext.Current.Request;

                var agency_id = 0;
                var job_id = 0;
                if (httpRequest["agency_id"] != null)
                    agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                if (httpRequest["job_id"] != null)
                    job_id = Utils.ConvertToInt32(httpRequest["job_id"]);

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.agency_id = agency_id;
                filter.job_id = job_id;
                filter.keyword = apiFilter.keyword;

                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                List<IdentityJobSeeker> listData = storeJobSeeker.GetSuggestionsForInvitationByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                var returnData = new List<IdentityJobSeeker>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = JobSeekerHelpers.GetBaseInfo(item.user_id);
                        if (returnItem != null)
                        {
                            returnItem.Extensions.is_invited = item.Extensions.is_invited;
                            returnData.Add(returnItem);
                        }
                    }
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
        [Route("{id:int}/invitations")]
        public async Task<IHttpActionResult> GetInvitationsByPage(int id)
        {
            var requestName = "Job_Seekers-GetInvitationsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.job_seeker_id = id;

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                List<IdentityInvitation> listData = myStore.GetListByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, GetCurrentRequestLang());

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }

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

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register(ApiRegisterModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-Register";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                logger.Debug(string.Format("Begin {0} request", requestName));

                logger.Debug(string.Format("Raw data: {0}", JsonConvert.SerializeObject(model)));
                var userId = 0;
                var returnCode = EnumCommonCode.Success;
                //var updateInfo = ExtractFormData(model);
                var isPhoneNumber = IsPhoneNumber(model.UserName);
                var isEmail = IsEmail(model.UserName);

                var apiModel = ReFormatRegisterModel(model);

                apiModel.IsPhoneNumber = isPhoneNumber;
                apiModel.IsEmail = isEmail;
                //apiModel.Display_Name = apiModel.UserName;


                var apiResult = await AccountServices.RegisterAsync(apiModel);

                if (apiResult != null)
                {
                    userId = Utils.ConvertToInt32(apiResult.Data);
                    //if(userId == 0)
                    //{
                    //    returnModel.status = EnumCommonCode.Error;
                    //    returnModel.message = UserApiResource.COMMON_ERROR_SYSTEM;

                    //    jsonString = JsonConvert.SerializeObject(returnModel);

                    //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
                    //}

                    if (apiResult.Code == (int)HttpStatusCode.OK || apiResult.Code == (int)EnumCommonCode.Success)
                    {
                        if (string.IsNullOrEmpty(apiResult.Msg))
                        {
                            //Thong bao thanh cong
                            returnModel.status = returnCode;
                            returnModel.message = UserApiResource.REGISTER_SUCCESS;
                        }
                        else
                        {
                            returnModel.status = returnCode;
                            returnModel.message = apiResult.Msg;
                        }

                        var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();
                        var userResult = AccountServices.GetUserProfileAsync(new ApiUserModel { UserId = userId }).Result;

                        if (userResult != null)
                        {
                            IdentityUser userInfo = JsonConvert.DeserializeObject<IdentityUser>(userResult.Data.ToString());
                            var info = new IdentityJobSeeker();
                            info.user_id = userInfo.Id;
                            info.email = userInfo.Email;
                            info.phone = userInfo.PhoneNumber;
                            info.display_name = userInfo.DisplayName;
                            info.fullname = userInfo.FullName;
                            info.image = userInfo.Avatar;

                            if (userInfo.SocialProviderId > 0)
                            {
                                if (userInfo.SocialProviderId == 1)
                                    info.facebook_id = userInfo.UserName;
                                else if (userInfo.SocialProviderId == 2)
                                    info.google_id = userInfo.UserName;
                            }

                            var result = storeJobSeeker.UpdateProfile(info);
                            await Task.FromResult(result);

                            //Clear cache
                            CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, userId));
                        }
                    }
                    else
                    {
                        //Thong bao that bai
                        returnModel.status = apiResult.Code;
                        returnModel.message = apiResult.Msg;
                    }
                }
                else
                {
                    returnModel.status = EnumCommonCode.Error;
                    returnModel.message = UserApiResource.COMMON_ERROR_SYSTEM;
                }

                returnModel.value = userId;

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
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
        [Route("external_login")]
        public async Task<IHttpActionResult> ExternalLogin(ApiAuthLoginWithModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-ExternalLogin";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var apiModel = new ApiAuthLoginWithModel
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    DisplayName = model.DisplayName,
                    SocialProvider = model.SocialProvider,
                    AppCode = model.AppCode
                };


                var apiResult = await AccountServices.LoginWithAsync(apiModel);

                if (apiResult != null)
                {
                    if (apiResult.Code == (int)HttpStatusCode.OK || apiResult.Code == (int)EnumCommonCode.Success)
                    {
                        if (apiResult.Data != null)
                        {
                            var existedProfile = JobSeekerHelpers.GetBaseInfo(apiResult.Data.Id);
                            if (existedProfile == null)
                            {
                                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                                var info = new IdentityJobSeeker();
                                info.user_id = apiResult.Data.Id;
                                info.email = apiResult.Data.Email;
                                info.phone = apiResult.Data.PhoneNumber;
                                info.display_name = apiResult.Data.DisplayName;
                                info.fullname = apiResult.Data.FullName;
                                info.image = apiResult.Data.Avatar;

                                if (apiResult.Data.SocialProviderId > 0)
                                {
                                    if (apiResult.Data.SocialProviderId == 1)
                                        info.facebook_id = apiResult.Data.UserName;

                                    if (apiResult.Data.SocialProviderId == 2)
                                        info.google_id = apiResult.Data.UserName;
                                }

                                var result = storeJobSeeker.UpdateProfile(info);
                                await Task.FromResult(result);
                            }

                            if (string.IsNullOrEmpty(apiResult.Msg))
                            {
                                //Thong bao thanh cong
                                returnModel.status = returnCode;
                                returnModel.message = UserApiResource.LOGIN_SUCCESS;
                            }
                            else
                            {
                                returnModel.status = returnCode;
                                returnModel.message = apiResult.Msg;
                            }
                        }
                    }
                    else
                    {
                        //Thong bao that bai
                        returnModel.status = apiResult.Code;
                        returnModel.message = apiResult.Msg;
                    }

                    returnModel.value = apiResult.Data;
                }
                else
                {
                    returnModel.status = EnumCommonCode.Error;
                    returnModel.message = UserApiResource.COMMON_ERROR_SYSTEM;
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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("set_main_cv")]
        public async Task<IHttpActionResult> SetMainCv(ApiJobSeekerSetMainCvModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-SetMainCv";
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
                var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var info = new IdentityCv();
                info.id = Utils.ConvertToIntFromQuest(model.cv_id);
                info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var affectedIds = storeJobSeeker.SetMainCv(info);
                await Task.FromResult(affectedIds);

                if (affectedIds.HasData())
                {
                    foreach (var item in affectedIds)
                    {
                        //Clear cache
                        CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, item));
                    }
                }

                returnModel.message = UserApiResource.UPDATE_INFO_SUCCESS;

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
        [Route("update_device")]
        public async Task<IHttpActionResult> UpdateDevice(ApiJobSeekerDeviceModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-UpdateDevice";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var deviceInfo = new IdentityJobSeekerDevice();

                deviceInfo.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
                deviceInfo.device_name = model.device_name;
                deviceInfo.device_id = model.device_id;
                deviceInfo.registration_id = model.registration_id;
                deviceInfo.device_type = Utils.ConvertToIntFromQuest(model.device_type);
                deviceInfo.language_code = model.language_code;

                var storeDevice = GlobalContainer.IocContainer.Resolve<IStoreDevice>();

                var result = storeDevice.JobSeekerUpdate(deviceInfo);
                await Task.FromResult(result);

                //Clear cache
                //CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.user_id));

                returnModel.value = result;

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
        [Route("{id:int}/upload_video")]
        public async Task<IHttpActionResult> UploadVideo(int id)
        {
            var requestName = "Job_seekers-UploadVideo";
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
                if (fileCount < 1)
                {
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                var currentFile = httpRequest.Files.Get(0);
                if (currentFile != null)
                {
                    if (currentFile.ContentLength > (SystemSettings.MaximumVideoUploadLengthInMB * 1024 * 1024))
                    {
                        returnModel.error.error_code = EnumErrorCode.E000102.ToString();
                        returnModel.error.message = string.Format(UserApiResource.ERROR_VIDEO_UPLOAD_LIMIT_FORMAT, SystemSettings.MaximumVideoUploadLengthInMB);

                        return CachErrorResult(returnModel);
                    }
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var info = JobSeekerHelpers.GetBaseInfo(id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (fileCount > 0)
                    {
                        var storeJobSeeker = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                        var uploadResult = UploadJobSeekerVideo(info);
                        if (uploadResult.HasData())
                        {
                            var firstResult = uploadResult[0];
                            if (firstResult != null)
                            {
                                storeJobSeeker.UpdateVideoProfile(new IdentityJobSeeker { user_id = info.user_id, video_path = firstResult.Path });

                                //Clear cache
                                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, info.user_id));
                            }
                        }

                        returnModel.value = uploadResult;
                    }
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

        #region Helpers

        private ApiRegisterModel ReFormatRegisterModel(ApiRegisterModel model)
        {
            var apiModel = new ApiRegisterModel();
            if (model != null)
            {
                apiModel.UserName = !string.IsNullOrEmpty(model.UserName) ? model.UserName.Trim() : string.Empty;
                //apiModel.Password = Utility.Md5HashingData(model.Password);
                apiModel.Password = model.Password;
                apiModel.Display_Name = !string.IsNullOrEmpty(model.Display_Name) ? model.Display_Name.Trim() : string.Empty;
                apiModel.Full_Name = !string.IsNullOrEmpty(model.Full_Name) ? model.Full_Name.Trim() : string.Empty;
                apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);
                if (IsEmail(model.UserName))
                {
                    apiModel.Email = model.UserName;
                }

                if (IsPhoneNumber(model.UserName))
                {
                    apiModel.Phone = model.UserName;
                }

                apiModel.AppCode = model.AppCode;
            }

            return apiModel;
        }

        public static bool IsPhoneNumber(string input)
        {
            var regexPatern = "^([\\d() +-]+){10,}$";
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input, regexPatern);
            }

            return false;
        }

        public static bool IsEmail(string input)
        {
            var regexPatern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input, regexPatern);
            }

            return false;
        }

        private List<FileUploadResponseModel> UploadJobSeekerImage(IdentityJobSeeker info, int agency_id = 0)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "JobSeekers";
            uploadModel.ObjectId = info.user_id.ToString();
            uploadModel.InCludeDatePath = false;

            if (info.user_id <= 0)
            {
                uploadModel.SubDir = "JobSeeker_Agencies";
                uploadModel.ObjectId = agency_id.ToString();
            }

            if (!string.IsNullOrEmpty(info.image))
            {
                //Delete old file
                var apiDeleteModel = new FilesDeleteModel();
                apiDeleteModel.FilesPath = new List<string>();
                apiDeleteModel.FilesPath.Add(info.image);

                var deleteResult = CdnServices.DeleteImagesAsync(apiDeleteModel).Result;
            }

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

        private List<FileUploadResponseModel> UploadJobSeekerVideo(IdentityJobSeeker info)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;

            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "JobSeekers";
            uploadModel.ObjectId = info.user_id.ToString();
            uploadModel.InCludeDatePath = false;
            uploadModel.GenerateThumb = false;

            if (!string.IsNullOrEmpty(info.video_path))
            {
                //Delete old file
                var apiDeleteModel = new FilesDeleteModel();
                apiDeleteModel.FilesPath = new List<string>();
                apiDeleteModel.FilesPath.Add(info.video_path);

                var deleteResult = CdnServices.DeleteImagesAsync(apiDeleteModel).Result;
            }

            if (uploadModel.Files != null && uploadModel.Files[0] != null)
            {
                var apiResult = CdnServices.UploadVideoAsync(uploadModel).Result;
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
                                if (!string.IsNullOrEmpty(item.CoverPath))
                                    item.CoverFullPath = CdnHelper.SocialGetFullImgPath(item.CoverPath);
                            }
                        }
                    }
                }
                else
                {
                    logger.Error("Failed to get Upload video because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload video because: The video data is null");
            }

            return returnUploaded;
        }

        private IdentityJobSeeker ExtractFormData(ApiJobSeekerModel model)
        {
            var info = new IdentityJobSeeker();
            info.user_id = Utils.ConvertToIntFromQuest(model.user_id);
            info.email = model.email;
            info.phone = model.phone;
            info.marriage = Utils.ConvertToIntFromQuest(model.marriage);
            info.dependent_num = Utils.ConvertToIntFromQuest(model.dependent_num);
            info.fullname = model.fullname;
            info.fullname_furigana = model.fullname_furigana;
            info.display_name = model.display_name;
            info.image = model.image;
            info.birthday = Utils.ConvertStringToDateTimeByFormat(model.birthday, "dd-MM-yyyy");
            info.gender = Utils.ConvertToIntFromQuest(model.gender);
            info.id_card = model.id_card;
            info.note = model.note;
            info.video_path = model.video_path;
            info.expected_job_title = model.expected_job_title;
            info.expected_salary_min = Utils.ConvertToIntFromQuest(model.expected_salary_min);
            info.expected_salary_max = Utils.ConvertToIntFromQuest(model.expected_salary_max);
            info.work_status = Utils.ConvertToIntFromQuest(model.work_status);
            info.qualification_id = Utils.ConvertToIntFromQuest(model.qualification_id);
            info.job_seeking_status_id = Utils.ConvertToIntFromQuest(model.job_seeking_status_id);
            info.salary_type_id = Utils.ConvertToIntFromQuest(model.salary_type_id);
            info.japanese_level_number = Utils.ConvertToIntFromQuest(model.japanese_level_number);
            info.nationality_id = model.nationality_id;
            info.visa_id = model.visa_id;
            info.duration_visa = Utils.ConvertStringToDateTimeByFormat(model.duration_visa, "dd-MM-yyyy");
            if (model.religion == 1) info.religion = true;
            info.religion_detail = model.religion_detail;

            info.Addresses = model.Addresses;
            return info;
        }

        private IdentityJobSeekerConfig ExtractConfigData(ApiJobSeekerConfigModel model)
        {
            var info = new IdentityJobSeekerConfig();
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
            info.working_status = Utils.ConvertToIntFromQuest(model.working_status);
            info.working_detail = model.working_detail;
            info.salary_type = Utils.ConvertToIntFromQuest(model.salary_type);
            info.salary_min = Utils.ConvertToIntFromQuest(model.salary_min);
            info.salary_max = Utils.ConvertToIntFromQuest(model.salary_max);
            info.field_ids = model.field_ids;
            info.country_id = Utils.ConvertToIntFromQuest(model.country_id);
            info.region_id = Utils.ConvertToIntFromQuest(model.region_id);
            info.prefecture_id = Utils.ConvertToIntFromQuest(model.prefecture_id);
            info.city_id = Utils.ConvertToIntFromQuest(model.city_id);

            return info;
        }
        #endregion
    }
}
