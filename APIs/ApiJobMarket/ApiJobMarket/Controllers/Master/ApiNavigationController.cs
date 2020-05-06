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
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using System.Web;
using System.Dynamic;
using ApiJobMarket.Resources;
using ApiJobMarket.SharedLibs.Extensions;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/navigations")]
    public class ApiNavigationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiNavigationController>();


        [HttpGet]
        [Route("manager")]
        public async Task<IHttpActionResult> M_GetList()
        {
            var requestName = "Navigation-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var listResult = myStore.GetList();

                await Task.FromResult(listResult);

                returnModel.value = listResult;

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
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Navigation-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var listResult = CommonHelpers.GetListNavigations();

                await Task.FromResult(listResult);

                returnModel.value = listResult;

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
        public async Task<IHttpActionResult> Update(ApiNavigationEditModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Navigation-Update";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var info = myStore.GetById(model.Id);
                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                var identity = new IdentityNavigation()
                {
                    Id = model.Id,
                    Action = model.Action,
                    Title = model.Title,
                    Visible = model.Visible,
                    Controller = model.Controller,
                    Active = model.Active,
                    CssClass = model.CssClass,
                    IconCss = model.IconCss,
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder,
                    Name = model.Name,
                    AbsoluteUri = model.AbsoluteUri,
                    Desc = model.Desc,
                    Area = model.Area,
                    language_code = GetCurrentRequestLang()
                };
                var status = myStore.Update(identity);
                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

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
        public async Task<IHttpActionResult> Insert(ApiNavigationInsertModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Navigation-Insert";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();
                var identity = new IdentityNavigation()
                {
                    Action = model.Action,
                    Title = model.Title,
                    Visible = model.Visible,
                    Controller = model.Controller,
                    Active = model.Active,
                    CssClass = model.CssClass,
                    IconCss = model.IconCss,
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder,
                    Name = model.Name,
                    AbsoluteUri = model.AbsoluteUri,
                    Desc = model.Desc,
                    Area = model.Area,
                    language_code = GetCurrentRequestLang()
                };
                var status = myStore.Insert(identity);
                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

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

        [HttpPost]
        [Route("sorting")]
        public async Task<IHttpActionResult> UpdateSorting(List<SortingElement> elements)
        {
            CreateDocumentApi(elements);
            var requestName = "Navigation-UpdateSorting";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();
                var status = myStore.UpdateSorting(elements);

                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

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

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var requestName = "Navigation-Delete";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var info = myStore.Delete(id);

                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

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

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Navigation-GetDetail";
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

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var info = myStore.GetById(id);
                await Task.FromResult(info);

                if (info != null)
                {
                    returnModel.value = info;
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
        [Route("lang")]
        public async Task<IHttpActionResult> UpdateLang(ApiNavigationLangEditModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Navigation-Update";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var item = new IdentityNavigationLang()
                {
                    AbsoluteUri = model.AbsoluteUri,
                    LangCode = GetCurrentRequestLang(),
                    NavigationId = model.NavigationId,
                    Title = model.Title,
                    Id = model.Id
                };
                var status = myStore.UpdateLang(item);
                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

                await Task.FromResult(status);
                if (status > 0)
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
        [Route("lang")]
        public async Task<IHttpActionResult> InsertLang(ApiNavigationLangInsertModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Navigation-Insert";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();
                var item = new IdentityNavigationLang()
                {
                    AbsoluteUri = model.AbsoluteUri,
                    LangCode = model.LangCode,
                    NavigationId = model.NavigationId,
                    Title = model.Title
                };

                var status = myStore.AddNewLang(item);
                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

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
        [HttpDelete]
        [Route("lang/{id:int}")]
        public async Task<IHttpActionResult> DeleteLang(int id)
        {
            var requestName = "Navigation-Delete";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var info = myStore.DeleteLang(id);

                CachingHelpers.ClearCacheByKey(EnumListCacheKeys.Navigations);

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

        [HttpGet]
        [Route("lang/{id:int}")]
        public async Task<IHttpActionResult> GetDetailLang(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Navigation-GetDetailLang";
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

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreNavigation>();

                var info = myStore.GetLangDetail(id);
                await Task.FromResult(info);

                if (info != null)
                {
                    returnModel.value = info;
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
