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
using ApiJobMarket.Models.Master;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/job_seeker_note")]
    public class ApiJobSeekerNoteController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiJobSeekerNoteController>();

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "JobSeekerNote-GetDetail";
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
                var storeJobSeekerNote = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerNote>();
                var info = storeJobSeekerNote.GetById(id);

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

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            CreateDocumentApi(id);
            var requestName = "JobSeekerNote-Delete";
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
                var storeJobSeekerNote = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerNote>();

                var info = storeJobSeekerNote.GetById(id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);
                }
                else
                {
                    storeJobSeekerNote.Delete(id);

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
        [HttpPost]
        [Route("getbypage")]
        public async Task<IHttpActionResult> GetByPage(ApiJobSeekerNoteModel model)
        {
            var requestName = "JobSeekerNote-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var storeJobSeekerNote = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerNote>();

                List<IdentityJobSeekerNote> myList = storeJobSeekerNote.GetByPage(model, model.page_index.Value,model.page_size.Value);

                await Task.FromResult(myList);

                var returnData = new List<IdentityJobSeekerNote>();
                if (myList.HasData())
                {
                    returnModel.total = myList[0].total_count;
                }

                returnModel.value = myList;

                jsonString = JsonConvert.SerializeObject(returnModel);
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
        public async Task<IHttpActionResult> Update(ApiJobSeekerNoteUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Companies-UpdateJobSeekerNote";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var returnId = 0;
              

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeJobSeekerNote = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerNote>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                if (id > 0)
                {
                    var info = storeJobSeekerNote.GetById(id);
                    await Task.FromResult(info);

                    if (info == null)
                    {
                        returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                        return CachErrorResult(returnModel);
                    }
                    else
                    {
                        //Validate agency
                        if (model.job_seeker_id != info.job_seeker_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                var updateInfo = ExtractFormData(model);
                returnId = storeJobSeekerNote.Update(updateInfo);
                returnModel.value = returnId;

                jsonString = JsonConvert.SerializeObject(returnModel);
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

        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> Insert(ApiJobSeekerNoteUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "JobSeekerNote-Insert";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var returnId = 0;

                var returnCode = EnumCommonCode.Success;
                var storeJobSeekerNote = GlobalContainer.IocContainer.Resolve<IStoreJobSeekerNote>();

                var insertInfo = ExtractFormData(model);
                await Task.FromResult(insertInfo);
                returnId = storeJobSeekerNote.Insert(insertInfo);
                returnModel.value = returnId;

                jsonString = JsonConvert.SerializeObject(returnModel);
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
        private IdentityJobSeekerNote ExtractFormData(ApiJobSeekerNoteUpdateModel model)
        {
            var info = new IdentityJobSeekerNote();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.job_seeker_id = model.job_seeker_id;
            info.note = model.note;
            info.staff_id = model.staff_id;
            info.type = model.type;
            info.type_job_seeker = model.type_job_seeker;
            info.agency_id = model.agency_id;
           
            info.language_code = GetCurrentRequestLang();

            return info;
        }
    }
}
