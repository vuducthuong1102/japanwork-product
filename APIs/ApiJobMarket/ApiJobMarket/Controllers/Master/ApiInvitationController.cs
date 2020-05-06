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
using System.Web;
using ApiJobMarket.DB.Sql.Entities;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/invitations")]
    public class ApiInvitationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiInvitationController>();

        [HttpPost]
        [Route("accepts")]
        public async Task<IHttpActionResult> AcceptInvitation(ApiInvitationActionModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Invitations-AcceptInvitation";
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
                var storeInvitation = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                var job_id = Utils.ConvertToIntFromQuest(model.job_id);
                dynamic appData = new ExpandoObject();
                appData.id = Utils.ConvertToIntFromQuest(model.id);
                appData.job_id = job_id;
                appData.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }

                var status = storeInvitation.Accept(appData);
                await Task.FromResult(status);

                returnModel.value = status;
                returnModel.message = UserApiResource.SUCCESS_INVITATION_ACCEPTED;
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
        public async Task<IHttpActionResult> IgnoreInvitation(ApiInvitationActionModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Invitations-IgnoreInvitation";
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
                var storeInvitation = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                var job_id = Utils.ConvertToIntFromQuest(model.job_id);
                dynamic appData = new ExpandoObject();
                appData.id = Utils.ConvertToIntFromQuest(model.id);
                appData.job_id = job_id;
                appData.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }

                var pic_id = storeInvitation.Ignore(appData);
                await Task.FromResult(pic_id);
                if (pic_id > 0)
                {
                    NotificationHelper.Invitation_Canceled(info.pic_id, info.staff_id, job_id, appData.job_seeker_id);
                }

                returnModel.value = pic_id;
                returnModel.message = UserApiResource.SUCCESS_INVITATION_IGNORED;

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
        [Route("{id:int}/receivers")]
        public async Task<IHttpActionResult> GetReceivers(int id)
        {
            var requestName = "Agencies-GetInvitationsByPage";
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
                filter.agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]); ;
                filter.invite_id = id;
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);

                filter.language_code = GetCurrentRequestLang();

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                List<IdentityInvitation> listData = myStore.GetReceivers(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, filter.language_code);
                        var info = JobSeekerHelpers.GetBaseInfo(item.job_seeker_id, filter.staff_id);
                        if (info != null)
                        {
                            item.JobSeeker = info;
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
    }
}
