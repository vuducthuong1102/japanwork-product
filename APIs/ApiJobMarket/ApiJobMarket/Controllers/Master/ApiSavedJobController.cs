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

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/saved_jobs")]
    public class ApiSavedJobController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiSavedJobController>();

        /// <summary>
        /// Check job has has been saved or not
        /// </summary>
        /// <param name="job_id"></param>
        /// <param name="job_seeker_id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{job_id:int}/{job_seeker_id:int}")]
        public async Task<IHttpActionResult> CheckJobSaved(int job_id, int job_seeker_id)
        {
            var requestName = "Saved_Jobs-CheckJobSaved";
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

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                var info = storeJob.CheckJobSaved(job_id, job_seeker_id);
                await Task.FromResult(info);

                if(info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }

                returnModel.value = info;
                //if (info != null)
                //{
                //    foreach (var item in Enum.GetValues(typeof(EnumJobStatus)))
                //    {
                //        if (info.status == (int)item)
                //            info.status_label = EnumExtensions.GetEnumDescription((Enum)item);
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

        /// <summary>
        /// Delete job saved by job seeker
        /// </summary>
        /// <param name="job_id"></param>
        /// <param name="job_seeker_id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{job_id:int}/{job_seeker_id:int}")]
        public async Task<IHttpActionResult> Delete(int job_id, int job_seeker_id)
        {
            var requestName = "Saved_Jobs-Delete";
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

                var info = storeJob.DeleteJobSaved(job_id, job_seeker_id);
                await Task.FromResult(info);

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
        public async Task<IHttpActionResult> SaveJob(ApiJobActionSaveModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Applications-SaveJob";
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

                var info = storeJob.GetById(model.job_id);
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }

                var status = storeJob.Save(model);

                await Task.FromResult(status);
                if (status == 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_JOB_SAVE;
                }
                else
                {
                    returnModel.error.error_code = EnumErrorCode.E050103.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050103);

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
    }
}
