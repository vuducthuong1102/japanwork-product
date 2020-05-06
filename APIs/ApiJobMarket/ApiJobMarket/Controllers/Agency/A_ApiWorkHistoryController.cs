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
    [RoutePrefix("api/A_work_histories")]
    public class A_ApiWorkHistoryController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<A_ApiWorkHistoryController>();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        [HttpPost]
        [Route("detail")]
        public async Task<IHttpActionResult> GetDetail(ApiJobSeekerWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-GetDetail";
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
                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = storeWorkHistory.A_JobSeekerGetDetail(id, job_seeker_id);
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
        public async Task<IHttpActionResult> UpdateWorkHistory(ApiJobSeekerWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-UpdateWorkHistory";
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
                var updateInfo = ExtractWorkHistoryFormData(model);

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var result = storeWorkHistory.A_JobSeekerUpdate(updateInfo);
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
        public async Task<IHttpActionResult> Delete(ApiJobSeekerWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-Delete";
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
                var info = new IdentityJobSeekerWorkHistory();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var result = storeWorkHistory.A_JobSeekerDelete(info);
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
        public async Task<IHttpActionResult> CvGetDetail(ApiCvWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-CvGetDetail";
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
                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var info = storeWorkHistory.CvGetDetail(id, cv_id);
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
        public async Task<IHttpActionResult> CvUpdateWorkHistory(ApiCvWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-CvUpdateWorkHistory";
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
                var updateInfo = ExtractCvWorkHistoryFormData(model);

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var result = storeWorkHistory.CvUpdate(updateInfo);
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
        public async Task<IHttpActionResult> CvDelete(ApiCvWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-CvDelete";
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
                var info = new IdentityCvWorkHistory();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var result = storeWorkHistory.CvDelete(info);
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
        public async Task<IHttpActionResult> CsGetDetail(ApiCsWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-CsGetDetail";
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
                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                var cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

                var info = storeWorkHistory.CsGetDetail(id, cs_id);
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
        public async Task<IHttpActionResult> CsUpdateWorkHistory(ApiCsWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-CsUpdateWorkHistory";
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
                var updateInfo = ExtractCsWorkHistoryFormData(model);

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var result = storeWorkHistory.CsUpdate(updateInfo);
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
        public async Task<IHttpActionResult> CsDelete(ApiCsWorkHistoryModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Work_Histories-CsDelete";
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
                var info = new IdentityCsWorkHistory();
                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var result = storeWorkHistory.CsDelete(info);
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

        private IdentityJobSeekerWorkHistory ExtractWorkHistoryFormData(ApiJobSeekerWorkHistoryModel model)
        {
            var info = new IdentityJobSeekerWorkHistory();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
            info.company = model.company;
            info.content_work = model.content_work;
            info.form = Utils.ConvertToIntFromQuest(model.form);
            info.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date, DATE_FORMAT);
            info.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date, DATE_FORMAT);
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.address = model.address;

            return info;
        }

        private IdentityCvWorkHistory ExtractCvWorkHistoryFormData(ApiCvWorkHistoryModel model)
        {
            var info = new IdentityCvWorkHistory();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
            info.company = model.company;
            info.content_work = model.content_work;
            info.form = Utils.ConvertToIntFromQuest(model.form);
            info.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date, DATE_FORMAT);
            info.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date, DATE_FORMAT);
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.address = model.address;

            return info;
        }

        private IdentityCsWorkHistory ExtractCsWorkHistoryFormData(ApiCsWorkHistoryModel model)
        {
            var info = new IdentityCsWorkHistory();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.cs_id = Utils.ConvertToIntFromQuest(model.cs_id);
            info.company = model.company;
            info.sub_field_id = Utils.ConvertToIntFromQuest(model.sub_field_id);
            info.sub_industry_id = Utils.ConvertToIntFromQuest(model.sub_industry_id);
            info.employment_type_id = Utils.ConvertToIntFromQuest(model.employment_type_id);
            info.employees_number = Utils.ConvertToIntFromQuest(model.employees_number);
            info.resign_reason = model.resign_reason;
            info.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date, DATE_FORMAT);
            info.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date, DATE_FORMAT);
            info.status = Utils.ConvertToIntFromQuest(model.status);
            info.address = model.address;

            if (model.Details.HasData())
            {
                info.Details = new List<IdentityCsWorkHistoryDetail>();
                foreach (var item in model.Details)
                {
                    var dt = new IdentityCsWorkHistoryDetail();
                    dt.id = Utils.ConvertToIntFromQuest(item.id);
                    dt.cs_work_history_id = Utils.ConvertToIntFromQuest(info.id);
                    dt.department = item.department;
                    dt.position = item.position;
                    dt.content_work = item.content_work;
                    dt.salary = Utils.ConvertToIntFromQuest(item.salary);
                    dt.start_date = Utils.ConvertStringToDateTimeQuestByFormat(item.start_date, DATE_FORMAT);
                    dt.end_date = Utils.ConvertStringToDateTimeQuestByFormat(item.end_date, DATE_FORMAT);

                    info.Details.Add(dt);
                }
            }

            return info;
        }

        #endregion
    }
}
