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
using ApiJobMarket.DB.Sql.Stores;
using Autofac;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using ApiJobMarket.ShareLibs;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/job_seeker_wish")]
    public class ApiJobSeekerWishController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiJobSeekerWishController>();

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "JobSeekerWish-GetDetail";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerWish>();

                var info = myStore.GetById(id);
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
        public async Task<IHttpActionResult> Update(ApiJobSeekerWishModel model)
        {
            CreateDocumentApi(model);
            var requestName = "JobSeekerWish-Update";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerWish>();

                var identity = new IdentityJobSeekerWish()
                {
                    job_seeker_id = model.job_seeker_id,
                    employment_type_ids = model.employment_type_ids,
                    prefecture_ids = model.prefecture_ids,
                    sub_field_ids = model.sub_field_ids,
                    salary_min = model.salary_min,
                    salary_max = model.salary_max,
                    start_date = Utils.ConvertStringToDateTimeByFormat(model.start_date, null, "dd-MM-yyyy")
                };
                var status = myStore.Update(identity);

                await Task.FromResult(status);
                if (status)
                {
                    returnModel.message = UserApiResource.UPDATE_INFO_SUCCESS;
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
