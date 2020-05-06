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
using ApiJobMarket.ShareLibs;
using ApiJobMarket.DB.Sql.Entities;
using System.Web;
using ApiJobMarket.Resources;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/example_all")]
    public class ApiSuggestController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiSuggestController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Suggests-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                var httpRequest = HttpContext.Current.Request;
                //logger.Debug(string.Format("Begin {0} request", requestName));                
                var returnCode = EnumCommonCode.Success;
                var form = 0;
                var type = 0;

                if (httpRequest["form"] != null)
                    form = Utils.ConvertToInt32(httpRequest["form"]);

                if (httpRequest["type"] != null)
                    type = Utils.ConvertToInt32(httpRequest["type"]);

                var listSuggest = CommonHelpers.GetListSuggests(new IdentitySuggest { form = form, type = type,language_code= GetCurrentRequestLang() });

                await Task.FromResult(listSuggest);                

                returnModel.value = listSuggest;

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
        [Route("get_by_page")]
        public async Task<IHttpActionResult> GetByPage()
        {
            var requestName = "Suggest-GetByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var apiFilter = GetFilterConfig();
                var filter = new IdentitySuggest();
                filter.keyword = apiFilter.keyword;
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSuggest>();

                var listResult = myStore.GetByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(listResult);

                returnModel.value = listResult;

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
            var requestName = "Suggest-GetDetail";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSuggest>();

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
        /// <summary>
        /// Delete salary filter saved by job seeker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var requestName = "Suggest-Delete";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSuggest>();

                var info = myStore.Delete(id);
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
        public async Task<IHttpActionResult> Update(ApiSuggestEditModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Suggest-Update";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSuggest>();

                var info = myStore.GetById(model.id);
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                var identity = new IdentitySuggest()
                {
                    id = model.id,
                    title = model.title,
                    content = model.content,
                    form = model.form,
                    type = model.type,
                    //language_code = model.language_code
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

        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> Insert(ApiSuggestInsertModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Suggest-Insert";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSuggest>();
                var identity = new IdentitySuggest()
                {
                    title = model.title,
                    content = model.content,
                    form = model.form,
                    type = model.type,
                    //language_code=model.language_code
                };
                var status = myStore.Insert(identity);

                await Task.FromResult(status);
                if (status > 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_SAVED;
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
