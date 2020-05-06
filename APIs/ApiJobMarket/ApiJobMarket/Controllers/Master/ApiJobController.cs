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
using System.Linq;
using ApiJobMarket.Services;
using ApiJobMarket.DB.Sql.Entities;
using System.Web;
using ApiJobMarket.Settings;
using ApiJobMarket.ShareLibs;
using System.Dynamic;
using ApiCompanyMarket.Helpers;
using ApiJobMarket.Resources;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/jobs")]
    public class ApiJobController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiJobController>();

        [HttpPost]
        [Route("get_by_ids")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Jobs-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = JobHelpers.GetListJobs(model.ListIds, GetCurrentRequestLang());

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
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Jobs-GetDetail";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                //var info = storeJob.GetDetail(id);
                //await Task.FromResult(info);

                var info = JobHelpers.GetBaseInfoJob(id, GetCurrentRequestLang());
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
        [Route("check_invite")]
        public async Task<IHttpActionResult> CheckInvite(ApiJobCheckInviteModel model)
        {
            var requestName = "Jobs-CheckInvite";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var result = storeJob.CheckInvite(model);
                await Task.FromResult(result);
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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpGet]
        [Route("{id:int}/process")]
        public async Task<IHttpActionResult> GetListProcess(int id)
        {
            var requestName = "Jobs-GetListProcess";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var agency_id = 0;
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest["agency_id"] != null)
                    agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);
                var status = 0;
                if (httpRequest["status"] != null)
                    status = Utils.ConvertToInt32(httpRequest["status"]);

                var sub_id = 0;

                if (httpRequest["sub_id"] != null)
                    sub_id = Utils.ConvertToInt32(httpRequest["sub_id"]);

                var staff_id = 0;
                if (httpRequest["staff_id"] != null)
                    staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);

                var translate_status = 0;
                if (httpRequest["translate_status"] != null)
                    translate_status = Utils.ConvertToInt32(httpRequest["translate_status"]);

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.keyword = apiFilter.keyword;
                filter.company_id = id;
                filter.agency_id = agency_id;
                filter.sub_id = sub_id;
                filter.translate_status = translate_status;
                filter.staff_id = staff_id;

                filter.status = Utils.ConvertToIntFromQuest(apiFilter.status);

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                List<IdentityJob> myList = storeJob.GetListProcess(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

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
                            returnItem.Extensions.process_lastest = item.Extensions.process_lastest;
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
        [Route("get_list_hots")]
        public async Task<IHttpActionResult> GetListHots(int job_seeker_id, int page_size)
        {
            CreateDocumentApi(job_seeker_id);
            var requestName = "Jobs-GetListHots";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                //var info = storeJob.GetDetail(id);
                //await Task.FromResult(info);

                var listHots = storeJob.GetListHot(job_seeker_id, page_size);

                if (listHots.HasData())
                {
                    foreach (var item in listHots)
                    {
                        item.job_info = JobHelpers.GetBaseInfoJob(item.id, GetCurrentRequestLang());

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }
                await Task.FromResult(listHots);

                returnModel.value = listHots;
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
        [Route("get_list_assignment_work")]
        public async Task<IHttpActionResult> GetListAssignmentWork(ApiJobModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-GetListAssignmentWork";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var returnCode = EnumCommonCode.Success;
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                //var info = storeJob.GetDetail(id);
                //await Task.FromResult(info);

                var listJobs = storeJob.GetListAssignWorkByCompanyId(model);
                var listResults = new List<IdentityJob>();
                if (listJobs.HasData())
                {
                    returnModel.total = listJobs[0].total_count;
                    foreach (var item in listJobs)
                    {
                        var record = JobHelpers.Agency_GetBaseInfoJob(item.id, GetCurrentRequestLang());
                        record.Extensions.application_count = item.Extensions.application_count;
                        listResults.Add(record);
                    }

                }
                await Task.FromResult(listJobs);

                returnModel.value = listResults;
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
        [Route("detail")]
        public async Task<IHttpActionResult> GetDetailMetaData(ApiJobGetDetailModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-GetDetailMetaData";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var metaInfo = storeJob.GetMetaData(model);
                await Task.FromResult(metaInfo);

                if (metaInfo != null)
                {
                    var returnItem = JobHelpers.GetBaseInfoJob(model.id, GetCurrentRequestLang());
                    if (returnItem != null)
                    {
                        returnItem.view_count = metaInfo.view_count;
                        returnItem.Extensions.is_saved = metaInfo.Extensions.is_saved;
                        returnItem.Extensions.application_count = metaInfo.Extensions.application_count;
                        returnItem.Extensions.is_applied = metaInfo.Extensions.is_applied;

                        returnModel.value = returnItem;
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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpGet]
        [Route("{id:int}/applications")]
        public async Task<IHttpActionResult> GetListApplication(int id)
        {
            var requestName = "Jobs-GetListApplication";
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
                filter.job_id = id;

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var listData = storeApplication.GetListByJob(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    var returnData = new List<ApiResponseApplicationItemModel>();
                    List<int> listIds = new List<int>();
                    List<IdentityUser> listUsers = null;

                    listIds = listData.Select(x => x.job_seeker_id).ToList();
                    var apiUserInfoModel = new ApiListUserInfoModel();
                    apiUserInfoModel.ListUserId = listIds;

                    var listUserReturned = AccountServices.GetListUserProfileAsync(apiUserInfoModel).Result;
                    if (listUserReturned.Data != null)
                    {
                        listUsers = JsonConvert.DeserializeObject<List<IdentityUser>>(listUserReturned.Data.ToString());
                    }

                    foreach (var item in listData)
                    {
                        var candidateInfo = new ApiResponseApplicationItemModel();
                        candidateInfo.id = item.id;
                        candidateInfo.interview_accept_time = item.interview_accept_time;
                        candidateInfo.cancelled_time = item.cancelled_time;
                        candidateInfo.cv_id = item.cv_id;
                        candidateInfo.created_at = item.created_at;
                        candidateInfo.updated_at = item.updated_at;
                        candidateInfo.job_id = item.job_id;
                        candidateInfo.job_seeker_id = item.job_seeker_id;
                        candidateInfo.status = item.status;

                        if (listUsers.HasData())
                        {
                            foreach (var user in listUsers)
                            {
                                if (user.Id == item.job_seeker_id)
                                {
                                    candidateInfo.Cv = new ApiResponseApplicationCandidateModel();
                                    candidateInfo.Cv.fullname = user.FullName;
                                    candidateInfo.Cv.birthday = user.Birthday;
                                    candidateInfo.Cv.image = user.Avatar;
                                    break;
                                }
                            }
                        }

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (candidateInfo.status == (int)enm)
                                candidateInfo.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }

                        returnData.Add(candidateInfo);
                    }

                    returnModel.value = returnData;
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

        [HttpGet]
        [Route("{id:int}/candidates")]
        public async Task<IHttpActionResult> GetListCandidates(int id)
        {
            var requestName = "Jobs-GetListCandidates";
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
                var filter = new IdentityCandidate();
                filter.keyword = apiFilter.keyword;
                filter.job_id = id;

                var storeCandidate = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                var listData = storeCandidate.GetListByJob(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    var returnData = new List<ApiResponseCandidateItemModel>();
                    List<int> listIds = new List<int>();
                    List<IdentityUser> listUsers = null;

                    listIds = listData.Select(x => x.job_seeker_id).ToList();
                    var apiUserInfoModel = new ApiListUserInfoModel();
                    apiUserInfoModel.ListUserId = listIds;

                    var listUserReturned = AccountServices.GetListUserProfileAsync(apiUserInfoModel).Result;
                    if (listUserReturned.Data != null)
                    {
                        listUsers = JsonConvert.DeserializeObject<List<IdentityUser>>(listUserReturned.Data.ToString());
                    }

                    foreach (var item in listData)
                    {
                        var candidateInfo = new ApiResponseCandidateItemModel();
                        candidateInfo.id = item.id;
                        candidateInfo.request_time = item.request_time;
                        candidateInfo.applied_time = item.applied_time;
                        candidateInfo.cv_id = item.cv_id;
                        candidateInfo.created_at = item.created_at;
                        candidateInfo.updated_at = item.updated_at;
                        candidateInfo.job_id = item.job_id;
                        candidateInfo.job_seeker_id = item.job_seeker_id;
                        candidateInfo.status = item.status;

                        if (listUsers.HasData())
                        {
                            foreach (var user in listUsers)
                            {
                                if (user.Id == item.job_seeker_id)
                                {
                                    candidateInfo.Cv = new ApiResponseApplicationCandidateModel();
                                    candidateInfo.Cv.fullname = user.FullName;
                                    candidateInfo.Cv.birthday = user.Birthday;
                                    candidateInfo.Cv.image = user.Avatar;
                                    break;
                                }
                            }
                        }

                        //foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        //{
                        //    if (candidateInfo.status == (int)enm)
                        //        candidateInfo.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        //}

                        returnData.Add(candidateInfo);
                    }

                    returnModel.value = returnData;
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

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(ApiJobDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-Delete";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var info = JobHelpers.GetBaseInfoJob(model.id, GetCurrentRequestLang());
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.company_id > 0)
                    {
                        var companyInfo = CompanyHelpers.GetBaseInfoCompany(info.company_id);
                        if (companyInfo != null)
                        {
                            if (companyInfo.agency_id != model.agency_id)
                            {
                                returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                                returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                                return CachErrorResult(returnModel);
                            }
                        }
                    }
                }
                var result = storeJob.Delete(model.id, model.agency_id);
                await Task.FromResult(result);
                returnModel.value = result;
                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobsByCompany, info.company_id));

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

            return SuccessDeletedResult(returnModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateNewJob(ApiJobUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-CreateNewJob";
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

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                var newId = storeJob.Insert(updateInfo);
                await Task.FromResult(newId);

                returnModel.value = newId;

                //Clear cache
                //CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobsByCompany, model.job.company_id));

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
        [Route("update")]
        public async Task<IHttpActionResult> UpdateJob(ApiJobUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-UpdateJob";
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

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                var result = storeJob.Update(updateInfo);

                await Task.FromResult(result);

                returnModel.value = result;

                //Clear cache
                CachingHelpers.ClearCacheByPrefix(string.Format(EnumFormatInfoCacheKeys.Job, model.job.id));

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
        [Route("update_translation")]
        public async Task<IHttpActionResult> UpdateTranslation(ApiJobUpdateTranslationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-UpdateTranslate";
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
                var updateInfo = ExtractDataTranslation(model);

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                var result = storeJob.UpdateTranslation(updateInfo);

                await Task.FromResult(result);

                returnModel.value = result;

                //Clear cache
                CachingHelpers.ClearCacheByPrefix(string.Format(EnumFormatInfoCacheKeys.Job, model.job_id));

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


        [HttpDelete]
        [Route("{id:int}/close")]
        public async Task<IHttpActionResult> CloseJob(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Jobs-Close";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var info = storeJob.GetById(id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    storeJob.Close(id);

                    //Clear cache
                    CachingHelpers.ClearCacheByPrefix(string.Format(EnumFormatInfoCacheKeys.Job, id));
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

            return SuccessUpdatedResult(returnModel);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search(ApiJobSearchModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-Search";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var pageIndex = 1;
                var pageSize = SystemSettings.DefaultPageSize;
                if (model.page_index != null)
                    pageIndex = model.page_index.Value;
                if (pageIndex <= 0)
                    pageIndex = 1;

                if (model.page_size != null)
                    pageSize = model.page_size.Value;

                if (pageSize > SystemSettings.MaxPageSize)
                    pageSize = SystemSettings.MaxPageSize;

                if (string.IsNullOrEmpty(model.sorting_date))
                    model.sorting_date = "desc";

                model.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var listData = storeJob.SearchByPage(model, pageIndex, pageSize);

                await Task.FromResult(listData);
                var returnData = new List<IdentityJob>();
                if (listData.HasData())
                {
                    foreach (var item in listData)
                    {
                        var returnItem = JobHelpers.GetBaseInfoJob(item.id, GetCurrentRequestLang());
                        if (returnItem != null)
                        {
                            returnItem.Extensions.is_saved = item.Extensions.is_saved;
                            returnData.Add(returnItem);
                        }
                    }

                    returnModel.total = listData[0].total_count;
                }

                //if (listData.HasData())
                //{
                //    returnModel.total = listData[0].total_count;
                //    foreach (var item in listData)
                //    {
                //        if(item.company_info != null)
                //        {
                //            item.company_info.logo_path = CdnHelper.SocialGetFullImgPath(item.company_info.logo_path);
                //        }
                //    }
                //}                   

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
        [Route("recent_jobs")]
        public async Task<IHttpActionResult> GetRecentJobs(ApiJobGetRecentModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-GetRecentJobs";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                var filter = GetFilterConfig();
                filter.Extensions.company_id = model.company_id;
                filter.Extensions.ignore_ids = model.ignore_ids;
                filter.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var listData = storeJob.GetRecent(filter, filter.page_index.Value, filter.page_size.Value);

                await Task.FromResult(listData);
                var returnData = new List<IdentityJob>();
                if (listData.HasData())
                {
                    foreach (var item in listData)
                    {
                        var returnItem = JobHelpers.GetBaseInfoJob(item.id, GetCurrentRequestLang());
                        if (returnItem != null)
                        {
                            returnItem.Extensions.is_saved = item.Extensions.is_saved;
                            returnData.Add(returnItem);
                        }
                    }

                    returnModel.total = listData[0].total_count;
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
        [Route("application_invite")]
        public async Task<IHttpActionResult> ApplicationInvite(ApiCvInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-ApplicationInvite";
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
                var job_id = Utils.ConvertToIntFromQuest(model.job_id);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();


                var identity = new IdentityInvitation();
                identity.Invitations = new List<IdentityInvitationBase>();
                if (model.JobSeekers.HasData())
                {
                    foreach (var item in model.JobSeekers)
                    {
                        var inviteInfo = new IdentityInvitationBase();
                        inviteInfo.cv_id = item.cv_id;
                        inviteInfo.job_seeker_id = item.job_seeker_id;

                        identity.Invitations.Add(inviteInfo);
                    }
                }

                identity.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                identity.job_id = Utils.ConvertToIntFromQuest(model.job_id);
                identity.note = model.note;
                identity.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);

                var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                if (info == null)
                {
                    //Job not found
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    identity.company_id = info.company_id;

                    var checkingInfo = myStore.InvitationChecking(identity);
                    if (checkingInfo != null)
                    {
                        int invitationCount = (int)checkingInfo.total_count;
                        int invitationLimit = (int)checkingInfo.invitation_limit;
                        int jobSeekerCount = model.JobSeekers.Count;
                        if (jobSeekerCount > invitationLimit || ((jobSeekerCount + invitationCount) > invitationLimit))
                        {
                            //Limit exceeded
                            returnModel.error.error_code = EnumErrorCode.E000109.ToString();
                            returnModel.error.message = string.Format(UserApiResource.ERROR_INVITATION_LIMIT_FORMAT, invitationLimit);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                var result = myStore.Invite(identity);
                await Task.FromResult(result);

                //Send notification
                NotificationHelper.AgencyInviteApplication(identity, Utils.ConvertToIntFromQuest(model.agency_id));

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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }


        [HttpPost]
        [Route("application_invite_job_seeker")]
        public async Task<IHttpActionResult> ApplicationInviteJobSeeker(ApiCvInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-ApplicationInvite";
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
                var job_id = Utils.ConvertToIntFromQuest(model.job_id);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();


                var identity = new IdentityInvitation();
                identity.Invitations = new List<IdentityInvitationBase>();
                if (model.JobSeekers.HasData())
                {
                    foreach (var item in model.JobSeekers)
                    {
                        var inviteInfo = new IdentityInvitationBase();
                        inviteInfo.cv_id = item.cv_id;
                        inviteInfo.job_seeker_id = item.job_seeker_id;

                        identity.Invitations.Add(inviteInfo);
                    }
                }

                identity.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                identity.job_id = Utils.ConvertToIntFromQuest(model.job_id);
                identity.note = model.note;
                identity.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                if (info == null)
                {
                    //Job not found
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    identity.company_id = info.company_id;
                }

                var result = myStore.Invite(identity);
                await Task.FromResult(result);

                //Send notification
                NotificationHelper.AgencyInviteApplication(identity, Utils.ConvertToIntFromQuest(model.agency_id));

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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("counter_for_deletion")]
        public async Task<IHttpActionResult> GetCounterForDeletion(ApiJobDeleteModel model)
        {
            CreateDocumentApi(model);

            var requestName = "Jobs-GetCounterForDeletion";
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
                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var info = JobHelpers.GetBaseInfoJob(model.id, GetCurrentRequestLang());
                IdentityJobSeekerCounter counter = null;
                if (info != null)
                {
                    counter = storeJob.GetCounterForDeletion(model.id);
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

        [HttpPost]
        [Route("application_invite_jobs")]
        public async Task<IHttpActionResult> ApplicationInviteMultiJobs(ApiCvInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-ApplicationInviteMultiJobs";
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

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();


                var identity = new IdentityInvitation();
                identity.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                identity.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
                identity.note = model.note;
                identity.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                identity.job_ids = model.job_ids;

                var result = myStore.InviteMultiJobs(identity);
                await Task.FromResult(result);

                //Send notification
                NotificationHelper.AgencyInviteJobs(identity, Utils.ConvertToIntFromQuest(model.agency_id));

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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }


        [HttpPost]
        [Route("invitation_checking")]
        public async Task<IHttpActionResult> InvitationChecking(ApiCvInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-InvitationChecking";
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
                var job_id = Utils.ConvertToIntFromQuest(model.job_id);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                var identity = new IdentityInvitation();
                identity.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                identity.job_id = Utils.ConvertToIntFromQuest(model.job_id);

                var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                if (info == null)
                {
                    //Job not found
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    identity.company_id = info.company_id;
                }

                var result = myStore.InvitationChecking(identity);
                await Task.FromResult(result);

                if (result != null)
                {
                    int currentLimit = (int)result.invitation_limit;
                    if (currentLimit == 0)
                    {
                        result.invitation_limit = 20;
                    }
                }

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

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        #region Helpers

        private IdentityJob ExtractFormData(ApiJobUpdateModel model)
        {
            var info = new IdentityJob();
            info.language_code = GetCurrentRequestLang();
            info.id = Utils.ConvertToIntFromQuest(model.job.id);
            info.staff_id = model.job.staff_id;
            info.company_id = model.job.company_id;
            info.quantity = model.job.quantity;
            info.age_min = model.job.age_min;
            info.age_max = model.job.age_max;
            info.salary_min = model.job.salary_min;
            info.salary_max = model.job.salary_max;
            info.salary_type_id = model.job.salary_type_id;
            info.pic_id = model.job.pic_id;
            info.status = model.job.status;
            info.japanese_only = model.job.japanese_only;
            info.language_code = model.job.language_code;

            TimeSpan workStartTime;
            if (TimeSpan.TryParse(model.job.work_start_time, out workStartTime))
            {
                info.work_start_time = workStartTime;
            }

            TimeSpan workEndTime;
            if (TimeSpan.TryParse(model.job.work_end_time, out workEndTime))
            {
                info.work_end_time = workEndTime;
            }

            info.probation_duration = model.job.probation_duration;
            info.employment_type_id = model.job.employment_type_id;
            info.flexible_time = model.job.flexible_time;
            info.language_level = model.job.language_level;
            info.work_experience_doc_required = model.job.work_experience_doc_required;
            info.duration = model.job.duration;
            info.view_company = model.job.view_company;
            info.qualification_id = model.job.qualification_id;
            info.japanese_level_number = model.job.japanese_level_number;
            info.sub_field_id = model.job.sub_field_id;
            info.sub_industry_id = model.job.sub_industry_id;
            info.closed_time = model.job.closed_time;
            info.staff_id = model.job.staff_id;

            if (model.job.Addresses.HasData())
            {
                info.Addresses = new List<IdentityJobAddress>();
                foreach (var item in model.job.Addresses)
                {
                    var addressInfo = new IdentityJobAddress();
                    addressInfo.country_id = item.country_id;
                    addressInfo.region_id = item.region_id;
                    addressInfo.prefecture_id = item.prefecture_id;
                    addressInfo.city_id = item.city_id;
                    addressInfo.furigana = item.furigana;
                    addressInfo.detail = item.detail;
                    addressInfo.train_line_id = item.train_line_id;

                    if (item.Stations.HasData())
                    {
                        addressInfo.Stations = new List<IdentityJobAddressStation>();
                        foreach (var st in item.Stations)
                        {
                            var station = new IdentityJobAddressStation();
                            station.id = st.id;

                            addressInfo.Stations.Add(station);
                        }
                    }

                    info.Addresses.Add(addressInfo);
                }
            }

            if (model.job.Job_translations.HasData())
            {
                info.Job_translations = new List<IdentityJobTranslation>();
                foreach (var item in model.job.Job_translations)
                {
                    var tranInfo = new IdentityJobTranslation();

                    tranInfo.title = item.title;
                    tranInfo.subsidy = item.subsidy;
                    tranInfo.paid_holiday = item.paid_holiday;
                    tranInfo.bonus = item.bonus;
                    tranInfo.certificate = item.certificate;
                    tranInfo.work_content = item.work_content;
                    tranInfo.requirement = item.requirement;
                    tranInfo.plus = item.plus;
                    tranInfo.welfare = item.welfare;
                    tranInfo.training = item.training;
                    tranInfo.recruitment_procedure = item.recruitment_procedure;
                    tranInfo.remark = item.remark;
                    tranInfo.language_code = item.language_code;

                    info.Job_translations.Add(tranInfo);
                }
            }

            //if (model.job.Sub_fields.HasData())
            //{
            //    info.Sub_fields = new List<IdentityJobSubField>();                
            //    foreach (var item in model.job.Sub_fields)
            //    {
            //        var subFieldInfo = new IdentityJobSubField();
            //        subFieldInfo.id = item.id;

            //        info.Sub_fields.Add(subFieldInfo);
            //    }
            //}

            if (model.job.Tags.HasData())
            {
                info.Tags = new List<IdentityJobTag>();
                foreach (var item in model.job.Tags)
                {
                    var tagInfo = new IdentityJobTag();

                    tagInfo.id = item.id;
                    tagInfo.tag = item.tag;
                    info.Tags.Add(tagInfo);
                }
            }

            return info;
        }

        private IdentityJobTranslation ExtractDataTranslation(ApiJobUpdateTranslationModel model)
        {
            var tranInfo = new IdentityJobTranslation();

            tranInfo.title = model.title;
            tranInfo.subsidy = model.subsidy;
            tranInfo.paid_holiday = model.paid_holiday;
            tranInfo.bonus = model.bonus;
            tranInfo.certificate = model.certificate;
            tranInfo.work_content = model.work_content;
            tranInfo.requirement = model.requirement;
            tranInfo.plus = model.plus;
            tranInfo.welfare = model.welfare;
            tranInfo.training = model.training;
            tranInfo.recruitment_procedure = model.recruitment_procedure;
            tranInfo.remark = model.remark;
            tranInfo.language_code = model.language_code;
            tranInfo.job_id = model.job_id;
            tranInfo.translate_status = model.translate_status;
            tranInfo.staff_id = model.staff_id;
            return tranInfo;
        }

        #endregion
    }
}
