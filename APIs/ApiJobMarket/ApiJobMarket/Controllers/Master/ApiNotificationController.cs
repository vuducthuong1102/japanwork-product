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
using ApiJobMarket.DB.Sql.Entities;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/notifications")]
    public class ApiNotificationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiNotificationController>();

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Notifications-GetDetail";
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
                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreNotification>();

                var info = storeNotif.GetById(id);

                await Task.FromResult(info);

                if (info == null)
                {
                    //Data not found
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }

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
        [Route("mark_is_read")]
        public async Task<IHttpActionResult> MarkIsRead(ApiMarkIsReadNotificationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Notifications-MarkIsReadNotification";
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

                var info = new IdentityNotification();
                info.user_id = Utils.ConvertToIntFromQuest(model.user_id);
                info.is_read = model.is_read;
                info.id = Utils.ConvertToIntFromQuest(model.id);

                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreNotification>();
                var detail = storeNotif.GetById(info.id);
                await Task.FromResult(info);
                if (detail == null)
                {
                    //Data not found
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }

                var status = storeJobSeeker.MarkIsReadNotification(info);
                await Task.FromResult(status);

                returnModel.value = detail;
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
    }
}
