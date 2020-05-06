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
using ApiJobMarket.Resources;
using System.Dynamic;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/cs")]
    public class ApiCsController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCsController>();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "CS-GetDetail";
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
                //var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();

                //var info = storeCs.GetDetail(id);
                //await Task.FromResult(info);

                //if (info != null)
                //{
                //    info.image_full_path = CdnHelper.SocialGetFullImgPath(info.image);
                //}
                var info = CsHelpers.GetBaseInfoCs(id);
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
        public async Task<IHttpActionResult> Create(ApiCsUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cs-Create";
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
                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();

                var updateInfo = ExtractFormData(model);

                var newId = storeCs.Insert(updateInfo);

                await Task.FromResult(newId);
                returnModel.value = newId;
                returnModel.message = UserApiResource.SUCCESS_CS_CREATED;

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
        [Route("")]
        public async Task<IHttpActionResult> Delete(ApiCsDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "CS-Delete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var cs_id = Utils.ConvertToIntFromQuest(model.cs_id);
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();

                var info = storeCs.GetById(cs_id);
                await Task.FromResult(info);                

                if (info == null)
                {
                    //CS not found
                    returnModel.error.error_code = EnumErrorCode.E070101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.job_seeker_id != Utils.ConvertToIntFromQuest(model.job_seeker_id))
                    {
                        returnModel.error.error_code = EnumErrorCode.E070101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

                        return CachErrorResult(returnModel);
                    }

                    storeCs.Delete(cs_id);

                    //Clear cache
                    CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, info.id));
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

            return SuccessDeletedResult(returnModel);
        }

        [HttpPost]
        [Route("set_main_cs")]
        public async Task<IHttpActionResult> SetMainCs(ApiJobSeekerSetMainCsModel model)
        {
            CreateDocumentApi(model);
            var requestName = "CS-SetMainCs";
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
                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();

                var info = new IdentityCs();
                info.id = Utils.ConvertToIntFromQuest(model.cs_id);
                info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var affectedIds = storeCs.SetMainCs(info);
                await Task.FromResult(affectedIds);

                if (affectedIds.HasData())
                {
                    foreach (var item in affectedIds)
                    {
                        //Clear cache
                        CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, item));
                    }
                }

                returnModel.message = UserApiResource.UPDATE_INFO_SUCCESS;

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
        [Route("update")]
        public async Task<IHttpActionResult> Update(ApiCsUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cs-Update";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var cs_id = Utils.ConvertToIntFromQuest(model.cs.id);
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();

                var info = storeCs.GetById(cs_id);
                await Task.FromResult(info);

                if (info == null)
                {
                    //CS not found
                    returnModel.error.error_code = EnumErrorCode.E070101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.job_seeker_id != Utils.ConvertToIntFromQuest(model.cs.job_seeker_id))
                    {
                        returnModel.error.error_code = EnumErrorCode.E070101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

                        return CachErrorResult(returnModel);
                    }
                }

                var updateInfo = ExtractFormData(model);

                var result = storeCs.Update(updateInfo);

                await Task.FromResult(result);
                returnModel.value = result;
                returnModel.message = UserApiResource.SUCCESS_CS_UPDATED;

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cs, info.id));

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

        //[HttpPost]
        //[Route("save_print_code")]
        //public async Task<IHttpActionResult> SavePrintCode(ApiCsSavePrintCodeModel model)
        //{
        //    CreateDocumentApi(model);
        //    var requestName = "Cs-SavePrintCode";
        //    var returnModel = new ApiResponseCommonModel();
        //    var jsonString = string.Empty;
        //    var logoPath = string.Empty;

        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin {0} request", requestName));

        //        var returnCode = EnumCommonCode.Success;
        //        var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();
        //        var cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

        //        var info = storeCs.GetById(cs_id);

        //        if (info == null)
        //        {
        //            //CS not found
        //            returnModel.error.error_code = EnumErrorCode.E070101.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

        //            return CachErrorResult(returnModel);
        //        }

        //        var codeInfo = new IdentityCsPdfCode();
        //        codeInfo.cs_id = cs_id;
        //        codeInfo.user_id = info.job_seeker_id;
        //        codeInfo.code_id = model.cs_code_id;

        //        var status = storeCs.SavePrintCode(codeInfo);
        //        await Task.FromResult(status);

        //        returnModel.value = status;
        //        if (status == 0)
        //        {
        //            returnModel.message = UserApiResource.SUCCESS_SAVED;
        //        }
        //        else
        //        {
        //            //code already saved
        //            returnModel.error.error_code = EnumErrorCode.E070103.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070103);

        //            return CachErrorResult(returnModel);
        //        }

        //        jsonString = JsonConvert.SerializeObject(returnModel);
        //        //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
        //        logger.ErrorException(strError, ex);

        //        return CatchJsonExceptionResult(returnModel);
        //    }
        //    finally
        //    {
        //        //logger.Debug(string.Format("Ended {0} request", requestName));
        //    }

        //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //}

        [HttpPost]
        [Route("upload_image")]
        public async Task<IHttpActionResult> UploadCsImage()
        {
            var requestName = "Cs-UploadCsImage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var httpRequest = HttpContext.Current.Request;
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();
                var cs_id = Utils.ConvertToInt32(httpRequest["cs_id"]);

                if (cs_id > 0)
                {
                    var info = storeCs.GetById(cs_id);
                    await Task.FromResult(info);
                    if (info == null)
                    {
                        //CS not found
                        returnModel.error.error_code = EnumErrorCode.E070101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

                        return CachErrorResult(returnModel);
                    }
                }

                var returnList = UploadFromJson();
                returnModel.value = returnList;

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

        //[HttpDelete]
        //[Route("delete_print_code")]
        //public async Task<IHttpActionResult> DeletePrintCode(ApiCsSavePrintCodeModel model)
        //{
        //    CreateDocumentApi(model);
        //    var requestName = "Cs-DeletePrintCode";
        //    var returnModel = new ApiResponseCommonModel();
        //    var jsonString = string.Empty;
        //    var logoPath = string.Empty;

        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin {0} request", requestName));

        //        var returnCode = EnumCommonCode.Success;
        //        var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();
        //        var cs_id = Utils.ConvertToIntFromQuest(model.cs_id);

        //        var info = storeCs.GetById(cs_id);

        //        if (info == null)
        //        {
        //            //CS not found
        //            returnModel.error.error_code = EnumErrorCode.E070101.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

        //            return CachErrorResult(returnModel);
        //        }

        //        var codeInfo = new IdentityCsPdfCode();
        //        codeInfo.cs_id = cs_id;
        //        codeInfo.user_id = info.job_seeker_id;
        //        codeInfo.code_id = model.cs_code_id;

        //        var status = storeCs.DeletePrintCode(codeInfo);
        //        await Task.FromResult(status);

        //        returnModel.value = status;
        //        if (status == 0)
        //        {
        //            returnModel.message = UserApiResource.DELETE_SUCCESS;
        //        }
        //        else
        //        {
        //            returnModel.error.error_code = EnumErrorCode.E070104.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070104);

        //            return CachErrorResult(returnModel);
        //        }

        //        jsonString = JsonConvert.SerializeObject(returnModel);
        //        //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
        //        logger.ErrorException(strError, ex);

        //        return CatchJsonExceptionResult(returnModel);
        //    }
        //    finally
        //    {
        //        //logger.Debug(string.Format("Ended {0} request", requestName));
        //    }

        //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //}

        [HttpGet]
        [Route("{id:int}/edu_histories")]
        public async Task<IHttpActionResult> GetListEduHistory(int id)
        {
            var requestName = "Cs-GetListEduHistory";
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

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var listData = storeEduHistory.GetListCsEduHistory(id);

                await Task.FromResult(listData);

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

        [HttpGet]
        [Route("{id:int}/work_histories")]
        public async Task<IHttpActionResult> GetListWorkHistory(int id)
        {
            var requestName = "Cs-GetListWorkHistory";
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

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var listData = storeWorkHistory.GetListCsWorkHistory(id);

                await Task.FromResult(listData);

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

        [HttpGet]
        [Route("{id:int}/certificates")]
        public async Task<IHttpActionResult> GetListCertificate(int id)
        {
            var requestName = "Cs-GetListCertificate";
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

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var listData = storeCertificate.GetListCsCertificate(id);

                await Task.FromResult(listData);

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

        [Route("get_suggestions")]
        public async Task<IHttpActionResult> GetSuggestionsByPage()
        {
            var requestName = "Cs-GetSuggestionsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = 0;

                if (HttpContext.Current.Request["job_seeker_id"] != null)
                    filter.job_seeker_id = Utils.ConvertToInt32(HttpContext.Current.Request["job_seeker_id"]);

                if (HttpContext.Current.Request["job_id"] != null)
                    filter.job_id = Utils.ConvertToInt32(HttpContext.Current.Request["job_id"]);

                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();
                List<IdentityCs> listData = storeCs.GetSuggestionsByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(listData);

                var returnData = new List<IdentityCs>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = CsHelpers.GetBaseInfoCs(item.id);
                        if (returnItem != null)
                        {
                            returnItem.Extensions.is_invited = item.Extensions.is_invited;
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

        [HttpPost]
        [Route("clone")]
        public async Task<IHttpActionResult> CloneCs(ApiCsModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cs-CloneCs";
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
                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();
                var cs_id = Utils.ConvertToIntFromQuest(model.id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = storeCs.GetById(cs_id);

                if (info == null)
                {
                    //CS not found
                    returnModel.error.error_code = EnumErrorCode.E070101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E070101);

                    return CachErrorResult(returnModel);
                }

                var newId = storeCs.Clone(new IdentityCs { id = cs_id, job_seeker_id = job_seeker_id });
                await Task.FromResult(newId);

                returnModel.value = newId;
                if (newId > 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_CS_CREATED;
                }
                else
                {
                    //error
                    returnModel.error.error_code = EnumErrorCode.E000106.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000106);

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
        [Route("get_list_by_ids")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Cs-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityCs>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CsHelpers.GetBaseInfoCs(currentId);
                        if (cachedData != null)
                            listData.Add(cachedData);
                    }
                }

                await Task.FromResult(listData);

                returnModel.value = listData;

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
        [Route("invitation_suggestions")]
        public async Task<IHttpActionResult> GetInvitationSuggestions()
        {
            var requestName = "Cs-GetInvitationSuggestions";
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
                var httpRequest = HttpContext.Current.Request;

                var agency_id = 0;
                var job_id = 0;
                if (httpRequest["agency_id"] != null)
                    agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                if (httpRequest["job_id"] != null)
                    job_id = Utils.ConvertToInt32(httpRequest["job_id"]);

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();
                filter.agency_id = agency_id;
                filter.job_id = job_id;
                filter.keyword = apiFilter.keyword;

                var storeCs = GlobalContainer.IocContainer.Resolve<IStoreCs>();
                List<IdentityCs> listData = storeCs.GetSuggestionsForInvitationByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                var returnData = new List<IdentityCs>();
                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        var returnItem = CsHelpers.GetBaseInfoCs(item.id);
                        if (returnItem != null)
                        {
                            returnItem.Extensions.is_invited = item.Extensions.is_invited;
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

        #region Helpers

        private IdentityCs ExtractFormData(ApiCsUpdateModel model)
        {
            var info = new IdentityCs();
            info.id = Utils.ConvertToIntFromQuest(model.cs.id);
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.cs.job_seeker_id);
            info.cs_title = model.cs.cs_title;
            try
            {
                info.date = DateTime.ParseExact(model.cs.date, "dd-MM-yyyy", null);
            }
            catch
            {
                info.date = DateTime.Now;
            }

            info.fullname = model.cs.fullname;
            info.fullname_furigana = model.cs.fullname_furigana;
            info.gender = Utils.ConvertToIntFromQuest(model.cs.gender);

            try
            {
                info.birthday = DateTime.ParseExact(model.cs.birthday, "dd-MM-yyyy", null);
            }
            catch
            {
                info.birthday = DateTime.Now;
            }

            info.email = model.cs.email;
            info.phone = model.cs.phone;           
            info.highest_edu = Utils.ConvertToIntFromQuest(model.cs.highest_edu);           
            info.image = model.cs.image;            
            info.station_id = Utils.ConvertToIntFromQuest(model.cs.station_id);
            info.train_line_id = Utils.ConvertToIntFromQuest(model.cs.train_line_id);

            if (model.work_history.HasData())
            {
                info.work_history = new List<IdentityCsWorkHistory>();
                foreach (var item in model.work_history)
                {
                    if (item.id > 0)
                        continue;

                    var work = new IdentityCsWorkHistory();

                    work.id = Utils.ConvertToInt32(item.id);
                    work.company = item.company;
                    work.sub_field_id = Utils.ConvertToIntFromQuest(item.sub_field_id);
                    work.sub_industry_id = Utils.ConvertToIntFromQuest(item.sub_industry_id);
                    work.employment_type_id = Utils.ConvertToIntFromQuest(item.employment_type_id);
                    work.employees_number = Utils.ConvertToIntFromQuest(item.employees_number);
                    work.resign_reason = item.resign_reason;
                    work.address = item.address;

                    if(!string.IsNullOrEmpty(item.start_date))
                        work.start_date = Utils.ConvertStringToDateTimeQuestByFormat(item.start_date, DATE_FORMAT);

                    if (!string.IsNullOrEmpty(item.start_date))
                        work.end_date = Utils.ConvertStringToDateTimeQuestByFormat(item.end_date, DATE_FORMAT);

                    work.status = Utils.ConvertToIntFromQuest(item.status);

                    if (item.Details.HasData())
                    {
                        work.Details = new List<IdentityCsWorkHistoryDetail>();
                        foreach (var newDtItem in item.Details)
                        {
                            var dt = new IdentityCsWorkHistoryDetail();
                            dt.id = Utils.ConvertToIntFromQuest(newDtItem.id);
                            dt.cs_work_history_id = Utils.ConvertToIntFromQuest(newDtItem.id);
                            dt.department = newDtItem.department;
                            dt.position = newDtItem.position;
                            dt.content_work = newDtItem.content_work;
                            dt.salary = Utils.ConvertToIntFromQuest(newDtItem.salary);
                            dt.start_date = Utils.ConvertStringToDateTimeQuestByFormat(newDtItem.start_date, DATE_FORMAT);
                            dt.end_date = Utils.ConvertStringToDateTimeQuestByFormat(newDtItem.end_date, DATE_FORMAT);

                            work.Details.Add(dt);
                        }
                    }

                    info.work_history.Add(work);
                }
            }

            if (model.edu_history.HasData())
            {
                info.edu_history = new List<IdentityCsEduHistory>();
                foreach (var item in model.edu_history)
                {
                    if (item.id > 0)
                        continue;

                    var edu = new IdentityCsEduHistory();

                    edu.id = Utils.ConvertToInt32(item.id);
                    edu.school = item.school;
                    edu.address = item.address;
                    edu.qualification_id = Utils.ConvertToIntFromQuest(item.qualification_id);
                    edu.major_id = Utils.ConvertToIntFromQuest(item.major_id);

                    try
                    {
                        if (!string.IsNullOrEmpty(item.start_date))
                            edu.start_date = DateTime.ParseExact(item.start_date, DATE_FORMAT, null);

                        if (!string.IsNullOrEmpty(item.end_date))
                            edu.end_date = DateTime.ParseExact(item.end_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                        //edu.start_date = DateTime.Now;
                        //edu.end_date = DateTime.Now;
                    }

                    edu.status = Utils.ConvertToIntFromQuest(item.status);

                    info.edu_history.Add(edu);
                }
            }

            if (model.certification.HasData())
            {
                info.certification = new List<IdentityCsCertificate>();
                foreach (var item in model.certification)
                {
                    if (item.id > 0)
                        continue;

                    var cer = new IdentityCsCertificate();

                    cer.id = Utils.ConvertToInt32(item.id);
                    cer.name = item.name;
                    cer.point = item.point;
                    cer.pass = Utils.ConvertToIntFromQuest(item.pass);

                    try
                    {
                        if (!string.IsNullOrEmpty(item.start_date))
                            cer.start_date = DateTime.ParseExact(item.start_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                       
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(item.end_date))
                            cer.end_date = DateTime.ParseExact(item.end_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                       
                    }

                    info.certification.Add(cer);
                }
            }

            if (model.address != null)
            {
                info.address = new IdentityCsAddress();
                info.address.country_id = Utils.ConvertToIntFromQuest(model.address.country_id);
                info.address.region_id = Utils.ConvertToIntFromQuest(model.address.region_id);
                info.address.prefecture_id = Utils.ConvertToIntFromQuest(model.address.prefecture_id);
                info.address.city_id = Utils.ConvertToIntFromQuest(model.address.city_id);
                info.address.detail = model.address.detail;
                info.address.furigana = model.address.furigana;
                info.address.postal_code = model.address.postal_code;
                info.address.train_line_id = Utils.ConvertToIntFromQuest(model.address.train_line_id);
                info.address.station_id = Utils.ConvertToIntFromQuest(model.address.station_id);
            }

            //if (model.address_contact != null)
            //{
            //    info.address_contact = new IdentityCsAddress();
            //    info.address_contact.country_id = Utils.ConvertToIntFromQuest(model.address_contact.country_id);
            //    info.address_contact.region_id = Utils.ConvertToIntFromQuest(model.address_contact.region_id);
            //    info.address_contact.prefecture_id = Utils.ConvertToIntFromQuest(model.address_contact.prefecture_id);
            //    info.address_contact.city_id = Utils.ConvertToIntFromQuest(model.address_contact.city_id);
            //    info.address_contact.detail = model.address_contact.detail;
            //    info.address_contact.furigana = model.address_contact.furigana;
            //    info.address_contact.postal_code = model.address_contact.postal_code;
            //    info.address_contact.is_contact_address = true;
            //    info.address.train_line_id = Utils.ConvertToIntFromQuest(model.address.train_line_id);
            //    info.address.station_id = Utils.ConvertToIntFromQuest(model.address.station_id);
            //}

            return info;
        }

        private string UploadFromHttpRequest(int company_id)
        {
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "Companies";
            uploadModel.ObjectId = company_id.ToString();
            uploadModel.InCludeDatePath = false;

            if (uploadModel.Files != null && uploadModel.Files[0] != null)
            {
                var apiResult = CdnServices.UploadImagesAsync(uploadModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.Code == EnumCommonCode.Success)
                    {
                        var imagesList = JsonConvert.DeserializeObject<List<string>>(apiResult.Data.ToString());
                        if (imagesList != null && imagesList.Count > 0)
                        {
                            filePath = imagesList[0];
                        }
                    }

                    fileName = uploadModel.Files[0].FileName;
                }
                else
                {
                    logger.Error("Failed to get Upload image because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload image because: The image is null");
            }

            return filePath;
        }

        private List<FileUploadResponseModel> UploadFromJson()
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            //uploadModel.FilesInString.Add(model.image);
            uploadModel.SubDir = "CS/appStorage";

            var cs_id = Utils.ConvertToInt32(httpRequest["cs_id"]);
            var job_seeker_id = Utils.ConvertToInt32(httpRequest["job_seeker_id"]);
            if (cs_id > 0)
            {
                uploadModel.ObjectId = cs_id.ToString();
            }
            else
            {
                uploadModel.SubDir = "CS/svStorage";
                uploadModel.ObjectId = job_seeker_id.ToString();
            }

            uploadModel.InCludeDatePath = false;

            var files = httpRequest.Files;
            if (files != null && files.Count > 0)
            {
                uploadModel.Files.Add(files.Get(0));
            }

            if (uploadModel.Files.HasData())
            {
                var apiResult = CdnServices.UploadImagesAsync(uploadModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.Code == EnumCommonCode.Success)
                    {
                        returnUploaded = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiResult.Data.ToString());
                        if (returnUploaded.HasData())
                        {
                            foreach (var item in returnUploaded)
                            {
                                item.FullPath = CdnHelper.SocialGetFullImgPath(item.Path);
                            }
                        }
                    }
                }
                else
                {
                    logger.Error("Failed to get Upload image because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload image because: The image is null");
            }

            return returnUploaded;
        }

        #endregion
    }
}
