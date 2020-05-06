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
using ApiJobMarket.Resources;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/A_schedules")]
    public class A_ApiScheduleController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<A_ApiScheduleController>();

        [HttpPost]
        [Route("detail")]
        public async Task<IHttpActionResult> GetDetail(ApiScheduleModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Schedules-GetDetail";
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
                var storeSchedule = GlobalContainer.IocContainer.Resolve<IStoreSchedule>();

                var identity = new IdentitySchedule();
                identity.id = Utils.ConvertToIntFromQuest(model.id);
                identity.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                identity.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);

                var info = storeSchedule.GetDetail(identity);
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
        public async Task<IHttpActionResult> UpdateSchedule(ApiScheduleModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Schedules-UpdateSchedule";
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
                var updateInfo = ExtractScheduleFormData(model);

                var storeSchedule = GlobalContainer.IocContainer.Resolve<IStoreSchedule>();

                var result = storeSchedule.Update(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                //CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

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
        [Route("update_time")]
        public async Task<IHttpActionResult> UpdateScheduleTime(ApiScheduleModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Schedules-UpdateScheduleTime";
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
                var updateInfo = ExtractScheduleFormData(model);

                var storeSchedule = GlobalContainer.IocContainer.Resolve<IStoreSchedule>();

                var result = storeSchedule.UpdateTime(updateInfo);
                await Task.FromResult(result);

                //Clear cache
                //CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

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
        public async Task<IHttpActionResult> Delete(ApiScheduleModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Schedules-Delete";
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
                var info = new IdentitySchedule();

                info.id = Utils.ConvertToIntFromQuest(model.id);
                info.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                info.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);

                var storeSchedule = GlobalContainer.IocContainer.Resolve<IStoreSchedule>();

                var result = storeSchedule.Delete(info);
                await Task.FromResult(result);

                //Clear cache
                //CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.JobSeekerInfo, model.job_seeker_id));

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

        #region Helpers

        private IdentitySchedule ExtractScheduleFormData(ApiScheduleModel model)
        {
            var info = new IdentitySchedule();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
            info.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
            info.pic_id = Utils.ConvertToIntFromQuest(model.pic_id);
            info.schedule_cat = Utils.ConvertToIntFromQuest(model.schedule_cat);
            info.title = model.title;
            info.content = model.content;
            info.start_time = Utils.ConvertStringToDateTimeQuestByFormat(model.start_time, "yyyy-MM-dd HH:mm");
            info.end_time = Utils.ConvertStringToDateTimeQuestByFormat(model.end_time, "yyyy-MM-dd HH:mm");
           
            return info;
        }
        #endregion
    }
}
