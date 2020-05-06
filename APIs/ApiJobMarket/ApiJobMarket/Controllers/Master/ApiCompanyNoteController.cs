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
    [RoutePrefix("api/company_note")]
    public class ApiCompanyNoteController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCompanyNoteController>();

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "CompanyNote-GetDetail";
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
                var storeCompanyNote = GlobalContainer.IocContainer.Resolve<IStoreCompanyNote>();
                var info = storeCompanyNote.GetById(id);

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
            var requestName = "CompanyNote-Delete";
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
                var storeCompanyNote = GlobalContainer.IocContainer.Resolve<IStoreCompanyNote>();

                var info = storeCompanyNote.GetById(id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);
                }
                else
                {
                    storeCompanyNote.Delete(id);

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
        public async Task<IHttpActionResult> GetByPage(ApiCompanyNoteModel model)
        {
            var requestName = "CompanyNote-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var storeCompanyNote = GlobalContainer.IocContainer.Resolve<IStoreCompanyNote>();

                List<IdentityCompanyNote> myList = storeCompanyNote.GetByPage(model, model.page_index.Value, model.page_size.Value);

                await Task.FromResult(myList);

                var returnData = new List<IdentityCompanyNote>();
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
        public async Task<IHttpActionResult> Update(ApiCompanyNoteUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Companies-UpdateCompanyNote";
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
                var storeCompanyNote = GlobalContainer.IocContainer.Resolve<IStoreCompanyNote>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                if (id > 0)
                {
                    var info = storeCompanyNote.GetById(id);
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
                        if (model.company_id != info.company_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                var updateInfo = ExtractFormData(model);
                returnId = storeCompanyNote.Update(updateInfo);
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
        public async Task<IHttpActionResult> Insert(ApiCompanyNoteUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "CompanyNote-Insert";
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
                var storeCompanyNote = GlobalContainer.IocContainer.Resolve<IStoreCompanyNote>();

                var insertInfo = ExtractFormData(model);
                await Task.FromResult(insertInfo);
                returnId = storeCompanyNote.Insert(insertInfo);
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
        private IdentityCompanyNote ExtractFormData(ApiCompanyNoteUpdateModel model)
        {
            var info = new IdentityCompanyNote();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.company_id = model.company_id;
            info.note = model.note;
            info.staff_id = model.staff_id;
            info.type = model.type;
            info.agency_id = model.agency_id;
           
            info.language_code = GetCurrentRequestLang();

            return info;
        }
    }
}
