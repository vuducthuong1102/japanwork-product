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
    [RoutePrefix("api/cv")]
    public class ApiCvController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCvController>();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "CVs-GetDetail";
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
                //var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                //var info = storeCv.GetDetail(id);
                //await Task.FromResult(info);

                //if (info != null)
                //{
                //    info.image_full_path = CdnHelper.SocialGetFullImgPath(info.image);
                //}
                var info = CvHelpers.GetBaseInfoCv(id);
                if (info.status == 9)
                {
                    info = null;
                }
                if (info != null)
                {
                    info.jobseeker = JobSeekerHelpers.GetBaseInfo(info.job_seeker_id);
                    if (info.jobseeker != null)
                    {
                        info.jobseeker.image = CdnHelper.SocialGetFullImgPath(info.jobseeker.image);
                    }
                }
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
        public async Task<IHttpActionResult> Create(ApiCvUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cv-Create";
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
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var updateInfo = ExtractFormData(model);

                var newId = storeCv.Insert(updateInfo);

                await Task.FromResult(newId);
                returnModel.value = newId;
                returnModel.message = UserApiResource.SUCCESS_CV_CREATED;

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
        public async Task<IHttpActionResult> Delete(ApiCvDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "CVs-Delete";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var cv_id = Utils.ConvertToIntFromQuest(model.cv_id);
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var info = storeCv.GetById(cv_id);
                await Task.FromResult(info);                

                if (info == null)
                {
                    //CV not found
                    returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.job_seeker_id != Utils.ConvertToIntFromQuest(model.job_seeker_id))
                    {
                        returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                        return CachErrorResult(returnModel);
                    }

                    storeCv.Delete(cv_id);

                    //Clear cache
                    CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, info.id));
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
        [Route("set_main_cv")]
        public async Task<IHttpActionResult> SetMainCv(ApiJobSeekerSetMainCvModel model)
        {
            CreateDocumentApi(model);
            var requestName = "CVs-SetMainCv";
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
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var info = new IdentityCv();
                info.id = Utils.ConvertToIntFromQuest(model.cv_id);
                info.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var affectedIds = storeCv.SetMainCv(info);
                await Task.FromResult(affectedIds);

                if (affectedIds.HasData())
                {
                    foreach (var item in affectedIds)
                    {
                        //Clear cache
                        CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, item));
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
        public async Task<IHttpActionResult> Update(ApiCvUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cv-Update";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var cv_id = Utils.ConvertToIntFromQuest(model.cv.id);
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var info = storeCv.GetById(cv_id);
                await Task.FromResult(info);

                if (info == null)
                {
                    //CV not found
                    returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (info.job_seeker_id != Utils.ConvertToIntFromQuest(model.cv.job_seeker_id))
                    {
                        returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                        return CachErrorResult(returnModel);
                    }
                }

                var updateInfo = ExtractFormData(model);

                var result = storeCv.Update(updateInfo);

                await Task.FromResult(result);
                returnModel.value = result;
                returnModel.message = UserApiResource.SUCCESS_CV_UPDATED;

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Cv, info.id));

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
        [Route("save_print_code")]
        public async Task<IHttpActionResult> SavePrintCode(ApiCvSavePrintCodeModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cv-SavePrintCode";
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
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                var cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var info = storeCv.GetById(cv_id);

                if (info == null)
                {
                    //CV not found
                    returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                    return CachErrorResult(returnModel);
                }

                var codeInfo = new IdentityCvPdfCode();
                codeInfo.cv_id = cv_id;
                codeInfo.user_id = info.job_seeker_id;
                codeInfo.code_id = model.cv_code_id;

                var status = storeCv.SavePrintCode(codeInfo);
                await Task.FromResult(status);

                returnModel.value = status;
                if (status == 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_SAVED;
                }
                else
                {
                    //code already saved
                    returnModel.error.error_code = EnumErrorCode.E030103.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030103);

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
        [Route("upload_image")]
        public async Task<IHttpActionResult> UploadCvImage()
        {
            var requestName = "Cv-UploadCvImage";
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
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                var cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);

                IdentityCv info = null;
                if (cv_id > 0)
                {
                    info = storeCv.GetById(cv_id);
                    await Task.FromResult(info);
                    if (info == null)
                    {
                        //CV not found
                        returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                        return CachErrorResult(returnModel);
                    }
                }

                var returnList = UploadImage(info);
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

        [HttpDelete]
        [Route("delete_print_code")]
        public async Task<IHttpActionResult> DeletePrintCode(ApiCvSavePrintCodeModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cv-DeletePrintCode";
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
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                var cv_id = Utils.ConvertToIntFromQuest(model.cv_id);

                var info = storeCv.GetById(cv_id);

                if (info == null)
                {
                    //CV not found
                    returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                    return CachErrorResult(returnModel);
                }

                var codeInfo = new IdentityCvPdfCode();
                codeInfo.cv_id = cv_id;
                codeInfo.user_id = info.job_seeker_id;
                codeInfo.code_id = model.cv_code_id;

                var status = storeCv.DeletePrintCode(codeInfo);
                await Task.FromResult(status);

                returnModel.value = status;
                if (status == 0)
                {
                    returnModel.message = UserApiResource.DELETE_SUCCESS;
                }
                else
                {
                    returnModel.error.error_code = EnumErrorCode.E030104.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030104);

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

        [HttpGet]
        [Route("{id:int}/edu_histories")]
        public async Task<IHttpActionResult> GetListEduHistory(int id)
        {
            var requestName = "Cv-GetListEduHistory";
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

                var storeEduHistory = GlobalContainer.IocContainer.Resolve<IStoreEduHistory>();

                var listData = storeEduHistory.GetListCvEduHistory(id);

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
            var requestName = "Cv-GetListWorkHistory";
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

                var storeWorkHistory = GlobalContainer.IocContainer.Resolve<IStoreWorkHistory>();

                var listData = storeWorkHistory.GetListCvWorkHistory(id);

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
        [Route("{agency_id:int}/get_list")]
        public async Task<IHttpActionResult> GetByAgency(int agency_id)
        {
            var requestName = "Cv-GetByAgency";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var returnCode = EnumCommonCode.Success;

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCv();
                filter.keyword = apiFilter.keyword;
                filter.agency_id = agency_id;

                var httpRequest = HttpContext.Current.Request;

                if (httpRequest["agency_parent_id"] != null)
                    filter.agency_parent_id = Utils.ConvertToInt32(httpRequest["agency_parent_id"]);

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                var listData = storeCv.GetByAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);
                if (listData.HasData())
                {

                    returnModel.total = listData[0].total_count;
                }

                returnModel.value = listData;
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

        [HttpGet]
        [Route("{id:int}/certificates")]
        public async Task<IHttpActionResult> GetListCertificate(int id)
        {
            var requestName = "Cv-GetListCertificate";
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

                var storeCertificate = GlobalContainer.IocContainer.Resolve<IStoreCertificate>();

                var listData = storeCertificate.GetListCvCertificate(id);

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
            var requestName = "Cv-GetSuggestionsByPage";
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

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                List<IdentityCv> listData = storeCv.GetSuggestionsByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

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
        public async Task<IHttpActionResult> CloneCv(ApiCvModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Cv-CloneCv";
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
                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                var cv_id = Utils.ConvertToIntFromQuest(model.id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var info = storeCv.GetById(cv_id);

                if (info == null)
                {
                    //CV not found
                    returnModel.error.error_code = EnumErrorCode.E030101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E030101);

                    return CachErrorResult(returnModel);
                }

                var newId = storeCv.Clone(new IdentityCv { id = cv_id, job_seeker_id = job_seeker_id });
                await Task.FromResult(newId);

                returnModel.value = newId;
                if (newId > 0)
                {
                    returnModel.message = UserApiResource.SUCCESS_CV_CREATED;
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
            var requestName = "Cv-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityCv>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CvHelpers.GetBaseInfoCv(currentId);
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
            var requestName = "Cv-GetInvitationSuggestions";
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

                var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();
                List<IdentityCv> listData = storeCv.GetSuggestionsForInvitationByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
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

        private IdentityCv ExtractFormData(ApiCvUpdateModel model)
        {
            var info = new IdentityCv();
            info.id = Utils.ConvertToIntFromQuest(model.cv.id);
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.cv.job_seeker_id);
            info.cv_title = model.cv.cv_title;
            try
            {
                info.date = DateTime.ParseExact(model.cv.date, "dd-MM-yyyy", null);
            }
            catch
            {
                info.date = DateTime.Now;
            }

            info.fullname = model.cv.fullname;
            info.fullname_furigana = model.cv.fullname_furigana;
            info.gender = Utils.ConvertToIntFromQuest(model.cv.gender);

            try
            {
                info.birthday = DateTime.ParseExact(model.cv.birthday, "dd-MM-yyyy", null);
            }
            catch
            {
                info.birthday = DateTime.Now;
            }

            info.email = model.cv.email;
            info.phone = model.cv.phone;
            info.japanese_level_number = model.cv.japanese_level_number;
            info.marriage = (model.cv.marriage == 0) ? false : true;
            info.dependent_num = Utils.ConvertToIntFromQuest(model.cv.dependent_num);
            info.highest_edu = Utils.ConvertToIntFromQuest(model.cv.highest_edu);
            info.pr = model.cv.pr;
            info.hobby_skills = model.cv.hobby_skills;
            info.reason = model.cv.reason;
            info.time_work = model.cv.time_work;
            info.aspiration = model.cv.aspiration;
            info.form = Utils.ConvertToIntFromQuest(model.cv.form);
            info.image = model.cv.image;
            info.reason_pr = model.cv.reason_pr;
            info.check_address = (model.cv.check_address == 0) ? false : true;
            info.check_work = (model.cv.check_work == 0) ? false : true;
            info.check_ceti = (model.cv.check_ceti == 0) ? false : true;
            info.check_timework = (model.cv.check_timework == 0) ? false : true;
            info.check_aspiration = (model.cv.check_aspiration == 0) ? false : true;
            info.station_id = Utils.ConvertToIntFromQuest(model.cv.station_id);
            info.train_line_id = Utils.ConvertToIntFromQuest(model.cv.train_line_id);

            if (model.work_history.HasData())
            {
                info.work_history = new List<IdentityJobSeekerWorkHistory>();
                foreach (var item in model.work_history)
                {
                    if (item.id > 0)
                        continue;

                    var work = new IdentityJobSeekerWorkHistory();

                    work.id = Utils.ConvertToInt32(item.id);
                    work.company = item.company;
                    work.content_work = item.content_work;
                    work.address = item.address;

                    try
                    {
                        if (!string.IsNullOrEmpty(item.start_date))
                            work.start_date = DateTime.ParseExact(item.start_date, DATE_FORMAT, null);

                        if (!string.IsNullOrEmpty(item.end_date))
                            work.end_date = DateTime.ParseExact(item.end_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                        //work.start_date = DateTime.Now;
                        //work.end_date = DateTime.Now;
                    }

                    work.form = Utils.ConvertToIntFromQuest(item.form);
                    work.status = Utils.ConvertToIntFromQuest(item.status);

                    info.work_history.Add(work);
                }
            }

            if (model.edu_history.HasData())
            {
                info.edu_history = new List<IdentityJobSeekerEduHistory>();
                foreach (var item in model.edu_history)
                {
                    if (item.id > 0)
                        continue;

                    var edu = new IdentityJobSeekerEduHistory();

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
                info.certification = new List<IdentityJobSeekerCertificate>();
                foreach (var item in model.certification)
                {
                    if (item.id > 0)
                        continue;

                    var cer = new IdentityJobSeekerCertificate();

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
                info.address = new IdentityJobSeekerAddress();
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

            if (model.address_contact != null)
            {
                info.address_contact = new IdentityJobSeekerAddress();
                info.address_contact.country_id = Utils.ConvertToIntFromQuest(model.address_contact.country_id);
                info.address_contact.region_id = Utils.ConvertToIntFromQuest(model.address_contact.region_id);
                info.address_contact.prefecture_id = Utils.ConvertToIntFromQuest(model.address_contact.prefecture_id);
                info.address_contact.city_id = Utils.ConvertToIntFromQuest(model.address_contact.city_id);
                info.address_contact.detail = model.address_contact.detail;
                info.address_contact.furigana = model.address_contact.furigana;
                info.address_contact.postal_code = model.address_contact.postal_code;
                info.address_contact.is_contact_address = true;
                info.address.train_line_id = Utils.ConvertToIntFromQuest(model.address.train_line_id);
                info.address.station_id = Utils.ConvertToIntFromQuest(model.address.station_id);
            }

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

        private List<FileUploadResponseModel> UploadImage(IdentityCv info = null)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            //uploadModel.FilesInString.Add(model.image);
            uploadModel.SubDir = "CVs/appStorage";

            var cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);
            var job_seeker_id = Utils.ConvertToInt32(httpRequest["job_seeker_id"]);
            if (cv_id > 0)
            {
                uploadModel.ObjectId = cv_id.ToString();

                if (!string.IsNullOrEmpty(info.image))
                {
                    //Delete old file
                    var apiDeleteModel = new FilesDeleteModel();
                    apiDeleteModel.FilesPath = new List<string>();
                    apiDeleteModel.FilesPath.Add(info.image);

                    var deleteResult = CdnServices.DeleteImagesAsync(apiDeleteModel).Result;
                }
            }
            else
            {
                uploadModel.SubDir = "CVs/svStorage";
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
