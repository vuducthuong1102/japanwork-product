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

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/A_edu_histories")]
    public class A_ApiEduHistoryController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<A_ApiEduHistoryController>();

        [HttpPost]
        [Route("detail")]
        public async Task<IHttpActionResult> GetDetail(ApiJobSeekerEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-GetDetail";
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
                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = storeEduHistory.A_JobSeekerGetDetail(id, job_seeker_id);
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
        public async Task<IHttpActionResult> UpdateEduHistory(ApiJobSeekerEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-UpdateEduHistory";
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
                var updateInfo = ExtractEduHistoryFormData(model);

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var result = storeEduHistory.A_JobSeekerUpdate(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

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

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(ApiJobSeekerEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-Delete";
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
                var info = new IdentityJobSeekerEduHistory();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var result = storeEduHistory.A_JobSeekerDelete(info);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

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

            return SuccessDeletedResult(returnModel);
        }

        #region Cv

        [HttpPost]
        [Route("cv_detail")]
        public async Task<IHttpActionResult> CvGetDetail(ApiCvEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-CvGetDetail";
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
                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var info = storeEduHistory.CvGetDetail(id, cv_id);
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
        [Route("cv_update")]
        public async Task<IHttpActionResult> CvUpdateEduHistory(ApiCvEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-CvUpdateEduHistory";
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
                var updateInfo = ExtractCvEduHistoryFormData(model);

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var result = storeEduHistory.CvUpdate(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, model.cv_id));

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

        [HttpDelete]
        [Route("cv_delete")]
        public async Task<IHttpActionResult> CvDelete(ApiCvEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-CvDelete";
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
                var info = new IdentityCvEduHistory();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var result = storeEduHistory.CvDelete(info);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, model.cv_id));

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

            return SuccessDeletedResult(returnModel);
        }

        #endregion

        #region Cs

        [HttpPost]
        [Route("cs_detail")]
        public async Task<IHttpActionResult> CsGetDetail(ApiCsEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-CsGetDetail";
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
                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

                var info = storeEduHistory.CsGetDetail(id, cs_id);
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
        [Route("cs_update")]
        public async Task<IHttpActionResult> CsUpdateEduHistory(ApiCsEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-CsUpdateEduHistory";
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
                var updateInfo = ExtractCsEduHistoryFormData(model);

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var result = storeEduHistory.CsUpdate(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, model.cs_id));

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

        [HttpDelete]
        [Route("cs_delete")]
        public async Task<IHttpActionResult> CsDelete(ApiCsEduHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Edu_Histories-CsDelete";
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
                var info = new IdentityCsEduHistory();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var result = storeEduHistory.CsDelete(info);
                await Task.FromResult(result);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, model.cs_id));

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

            return SuccessDeletedResult(returnModel);
        }

        #endregion

        #region Helpers

        private IdentityJobSeekerEduHistory ExtractEduHistoryFormData(ApiJobSeekerEduHistoryModel model)
        {
            var info = new IdentityJobSeekerEduHistory();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
            info.school = model.school;
            info.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date, "dd-MM-yyyy");
            info.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date, "dd-MM-yyyy");
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.address = model.address;
            info.qualification_id = Utils.ConvertToIntFromQuest(model.qualification_id);
            info.major_id = Utils.ConvertToIntFromQuest(model.major_id);
            info.major_custom = model.major_custom;

            return info;
        }

        private IdentityCvEduHistory ExtractCvEduHistoryFormData(ApiCvEduHistoryModel model)
        {
            var info = new IdentityCvEduHistory();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
            info.school = model.school;
            info.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date, "dd-MM-yyyy");
            info.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date, "dd-MM-yyyy");
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.address = model.address;
            info.qualification_id = Utils.ConvertToIntFromQuest(model.qualification_id);
            info.major_id = Utils.ConvertToIntFromQuest(model.major_id);
            info.major_custom = model.major_custom;

            return info;
        }

        private IdentityCsEduHistory ExtractCsEduHistoryFormData(ApiCsEduHistoryModel model)
        {
            var info = new IdentityCsEduHistory();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.cs_id = Utils.ConvertToIntFromQuest(model.cs_id);
            info.school = model.school;
            info.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date, "dd-MM-yyyy");
            info.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date, "dd-MM-yyyy");
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.address = model.address;
            info.qualification_id = Utils.ConvertToIntFromQuest(model.qualification_id);
            info.major_id = Utils.ConvertToIntFromQuest(model.major_id);
            info.major_custom = model.major_custom;

            return info;
        }

        #endregion
    }
}
