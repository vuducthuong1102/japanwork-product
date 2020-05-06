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
using ApiJobMarket.Resources;
using ApiJobMarket.ShareLibs;
using System.Dynamic;
using ApiJobMarket.DB.Sql.Entities;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/applications")]
    public class ApiApplicationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiApplicationController>();

        [HttpGet]
        [Route("{job_id:int}/{job_seeker_id:int}")]
        public async Task<IHttpActionResult> CheckJobApplied(int job_id, int job_seeker_id)
        {
            var requestName = "Applications-CheckJobApplied";
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

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var info = storeApplication.CheckJobApplied(job_id, job_seeker_id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }

                returnModel.value = info;
                if (info != null)
                {
                    foreach (var item in Enum.GetValues(typeof(EnumApplicationStatus)))
                    {
                        if (info.status == (int)item)
                            info.status_label = EnumExtensions.GetEnumDescription((Enum)item);
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

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> ApplyJob(ApiJobActionApplyModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Applications-ApplyJob";
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

                var info = JobHelpers.GetBaseInfoJob(model.job_id, GetCurrentRequestLang());
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();
                var status = storeJob.Apply(model);

                await Task.FromResult(status);
                if (status == 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_JOB_APPLY;
                    var appInfo = new IdentityApplication();
                    appInfo.cv_id = model.cv_id;

                    //Send notification
                    AgencyNotificationHelper.JobSeekerApplyJob(info, appInfo);
                }
                else
                {
                    returnModel.error.error_code = EnumErrorCode.E050102.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050102);

                    return CachErrorResult(returnModel);
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
        [Route("{id:int}/cancels")]
        public async Task<IHttpActionResult> CancelApplication(int id, ApiJobActionApplyModel model)
        {
            CreateDocumentApi(id);
            var requestName = "Applications-CancelApplication";
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
                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var status = storeApplication.Cancel(model);

                await Task.FromResult(status);
                if (status == 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_JOB_CANCEL;

                    var jobInfo = JobHelpers.GetBaseInfoJob(model.job_id, GetCurrentRequestLang());

                    var appInfo = new IdentityApplication();
                    appInfo.cv_id = model.cv_id;

                    //Send notification
                    AgencyNotificationHelper.JobSeekerCancelJob(jobInfo, appInfo);
                }
                else
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
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
        [Route("ignorances")]
        public async Task<IHttpActionResult> IgnoreApplication(ApiApplicationIgnoreModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Applications-IgnoreApplication";
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
                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();
                var appId = Utils.ConvertToIntFromQuest(model.id);

                dynamic appData = new ExpandoObject();
                appData.id = model.id;
                appData.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                appData.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
                appData.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);

                var info = storeApplication.GetById(appId);
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.cv_id != model.cv_id)
                    {
                        returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                        return CachErrorResult(returnModel);
                    }
                }

                var status = storeApplication.Ignore(appData);

                await Task.FromResult(status);
                returnModel.message = UserApiResource.SUCCESS_APPLICATION_IGNORED;

                //Send notification
                NotificationHelper.AgencyIgnoreApplication(info, Utils.ConvertToIntFromQuest(model.agency_id));

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
        [Route("accepts")]
        public async Task<IHttpActionResult> AcceptApplication(ApiApplicationAcceptModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Applications-AcceptApplication";
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
                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();
                var appId = Utils.ConvertToIntFromQuest(model.id);

                dynamic appData = new ExpandoObject();
                appData.id = model.id;
                appData.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                appData.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
                appData.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                appData.pic_id = Utils.ConvertToIntFromQuest(model.pic_id);
                var info = storeApplication.GetById(appId);
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.cv_id != model.cv_id)
                    {
                        returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                        return CachErrorResult(returnModel);
                    }
                }


                var status = storeApplication.Accept(appData);

                await Task.FromResult(status);
                returnModel.message = UserApiResource.SUCCESS_APPLICATION_ACCEPT_INTERVIEW;

                //Send notification
                NotificationHelper.AgencyAcceptApplication(info, Utils.ConvertToIntFromQuest(model.agency_id));

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
        [Route("send_cv")]
        public async Task<IHttpActionResult> SendCV(ApiApplicationSendCvModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Applications-AcceptApplication";
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
                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var appId = Utils.ConvertToIntFromQuest(model.job_id);

                var info = JobHelpers.GetBaseInfoJob(appId, GetCurrentRequestLang());
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }

                dynamic appData = new ExpandoObject();
                appData.job_id = appId;
                appData.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
                appData.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var storeInvitation = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                int pic_id = storeApplication.SendCv(appData);
                if (pic_id > 0)
                {
                    NotificationHelper.Invitation_Accepted(info.pic_id, info.staff_id, appData.cv_id, appId);
                }
                await Task.FromResult(pic_id);
                returnModel.message = UserApiResource.SUCCESS_APPLICATION_SEND_CV;

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
        [Route("deletes")]
        public async Task<IHttpActionResult> Deletes(ApiJobSeekerDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_seekers-Delete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var returnCode = EnumCommonCode.Success;
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreApplication>();
                if (string.IsNullOrEmpty(model.ids))
                {
                    //Data not found
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    var result = myStore.Deletes(model.ids, model.agency_id ?? 0, 0);
                    await Task.FromResult(result);
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

            return SuccessDeletedResult(returnModel);
        }

        [HttpPost]
        [Route("{id:int}/interviews")]
        public async Task<IHttpActionResult> Interview(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Applications-Interview";
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
                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var info = storeApplication.GetById(id);
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }

                var status = storeApplication.Interview(id);

                await Task.FromResult(status);
                returnModel.message = UserApiResource.SUCCESS_APPLICATION_ACCEPT_INTERVIEW;

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
        [Route("update_pic")]
        public async Task<IHttpActionResult> UpdatePic(ApiApplicationPicModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Companies-UpdatePic";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                var listResult = myStore.UpdatePic(model.job_seeker_id, model.agency_id, model.pic_id);
                await Task.FromResult(listResult);
                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

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

            return SuccessUpdatedResult(returnModel);
        }
    }
}
