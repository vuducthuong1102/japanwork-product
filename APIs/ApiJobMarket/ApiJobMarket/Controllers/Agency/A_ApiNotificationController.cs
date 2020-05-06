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
using System.Dynamic;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/A_notifications")]
    public class A_ApiNotificationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<A_ApiNotificationController>();

        [HttpPost]
        [Route("unread_count")]
        public async Task<IHttpActionResult> CountUnreadNotification(ApiAgencyStaffModel model)
        {
            var requestName = "A_notifications-CountUnread";
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
                filter.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);

                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreAgencyNotification>();

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
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "A_Notifications-GetDetail";
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
                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreAgencyNotification>();

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
        [Route("get_list")]
        public async Task<IHttpActionResult> GetListNotification(ApiAgencyGetListNotificationModel model)
        {
            var requestName = "A_Notifications-GetList";
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

                dynamic filter = new ExpandoObject();
                filter.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                filter.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                filter.is_update_view = model.is_update_view;

                var pageSize = Utils.ConvertToIntFromQuest(model.page_size, SystemSettings.DefaultPageSize);
                var pageIndex = Utils.ConvertToIntFromQuest(model.page_index, 1);
                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreAgencyNotification>();

                List<IdentityAgencyNotification> listData = storeNotif.GetListByStaff(filter, pageIndex, pageSize);
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

        [HttpPost]
        [Route("mark_is_read")]
        public async Task<IHttpActionResult> MarkIsRead(ApiMarkIsReadAgencyNotificationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "A_Notifications-MarkIsRead";
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
                var info = new IdentityAgencyNotification();
                info.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                info.is_read = model.is_read;
                info.id = Utils.ConvertToIntFromQuest(model.id);

                var storeNotif = GlobalContainer.IocContainer.Resolve<IStoreAgencyNotification>();
                var detail = storeNotif.GetById(info.id);
                await Task.FromResult(info);
                if (detail == null)
                {
                    //Data not found
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }

                var status = storeNotif.MarkIsRead(info);
                await Task.FromResult(status);

                returnModel.value = detail;
                returnModel.message = UserApiResource.SUCCESS_SAVED;

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
