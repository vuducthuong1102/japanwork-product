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
using System.Dynamic;
using ApiCompanyMarket.Helpers;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/M_JobSeeker")]
    public class M_ApiJobSeekerController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<M_ApiJobSeekerController>();

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> GetListByPage(ApiJobSeekerByPageModel model)
        {
            var requestName = "M_JobSeeker-GetListByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                dynamic filter = new ExpandoObject();
                filter.keyword = model.keyword;
                filter.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                if (model.status != null)
                    filter.status = Utils.ConvertToIntFromQuest(model.status);
                else
                    filter.status = -1;

                var apiFilter = ValidateFilterConfig(model.page_index, model.page_size);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                List<IdentityJobSeeker> myList = myStore.M_GetByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(myList);

                var returnData = new List<IdentityJobSeeker>();
                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        var returnItem = JobSeekerHelpers.GetBaseInfo(item.user_id, filter.agency_id);
                        if (returnItem != null)
                        {
                            returnItem.Extensions.cv_count = item.Extensions.cv_count;
                            returnItem.Extensions.application_count = item.Extensions.application_count;
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
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "M_JobSeeker-GetDetail";
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

                var info = JobSeekerHelpers.GetBaseInfo(id);
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
        [Route("cvs")]
        public async Task<IHttpActionResult> GetListCVs(ApiCvByPageModel model)
        {
            var requestName = "M_JobSeeker-GetListCVs";
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

                dynamic filter = new ExpandoObject();
                filter.keyword = model.keyword;
                filter.page_index = model.page_index;
                filter.page_size = model.page_size;

                filter.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                List<IdentityCv> listData = storeCv.M_GetListByJobSeeker(filter);
                await Task.FromResult(listData);

                var returnData = new List<IdentityCv>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = CvHelpers.GetBaseInfoCv(item.id);

                        if (returnItem != null)
                        {
                            returnItem.Extensions.pdf_code_count = item.Extensions.pdf_code_count;
                            returnItem.Extensions.application_count = item.Extensions.application_count;
                            returnData.Add(returnItem);
                        }
                    }
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
    }
}
