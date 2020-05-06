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

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/interviewprocess")]
    public class ApiInterviewProcessController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiInterviewProcessController>();

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Update(ApiInterviewProcessEditModel model)
        {
            var requestName = "InterviewProcess-Update";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;
                var storeInterviewProcess = GlobalContainer.IocContainer.Resolve<IStoreInterviewProcess>();
                var identity = new IdentityInterviewProcess()
                {
                    id = model.id,
                    agency_id = model.agency_id,
                    candidate_id = model.candidate_id,
                    created_at = model.created_at,
                    cv_id = model.cv_id,
                    job_id = model.job_id,
                    modified_at = model.modified_at,
                    note = model.note,
                    status_id = model.status_id,
                    staff_id = model.staff_id
                };
                var returnList = storeInterviewProcess.Update(identity);
                await Task.FromResult(returnList);

                returnModel.value = returnList;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);

                //return CatchJsonExceptionResult(returnModel);
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

        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> Insert(ApiInterviewProcessInsertModel model)
        {
            var requestName = "InterviewProcess-Insert";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;
                var storeInterviewProcess = GlobalContainer.IocContainer.Resolve<IStoreInterviewProcess>();
                var identity = new IdentityInterviewProcess()
                {
                    agency_id = model.agency_id,
                    candidate_id = model.candidate_id,
                    created_at = model.created_at,
                    cv_id = model.cv_id,
                    job_id = model.job_id,
                    modified_at = model.modified_at,
                    note = model.note,
                    status_id = model.status_id,
                    staff_id = model.staff_id
                };
                var returnList = storeInterviewProcess.Insert(identity);
                await Task.FromResult(returnList);

                returnModel.value = returnList;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);

                //return CatchJsonExceptionResult(returnModel);
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
        [Route("get_list_by_id/{id}")]
        public async Task<IHttpActionResult> GetListByCandidate_Id(int id)
        {
            var requestName = "Interview-GetListByCandidate_Id";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;

                var storeInterviewProcess = GlobalContainer.IocContainer.Resolve<IStoreInterviewProcess>();

                var returnList = storeInterviewProcess.GetListByCandidate_Id(id);

                await Task.FromResult(returnList);

                returnModel.value = returnList;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);

                //return CatchJsonExceptionResult(returnModel);
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
        [Route("get_list_candidate_ids/{ids}")]
        public async Task<IHttpActionResult> GetListByCandidate_Ids(string ids)
        {
            var requestName = "Interview-GetListByCandidate_Ids";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;

                var storeInterviewProcess = GlobalContainer.IocContainer.Resolve<IStoreInterviewProcess>();

                var returnList = storeInterviewProcess.GetListByCandidate_Ids(ids);

                await Task.FromResult(returnList);

                returnModel.value = returnList;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);

                //return CatchJsonExceptionResult(returnModel);
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
            var requestName = "InterviewProcess-GetDetail";
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
                var storeInterviewProcess = GlobalContainer.IocContainer.Resolve<IStoreInterviewProcess>();

                var info = storeInterviewProcess.GetById(id);
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
    }
}
