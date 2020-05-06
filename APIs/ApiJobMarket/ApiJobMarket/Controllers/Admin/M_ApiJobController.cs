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
    [RoutePrefix("api/admin/jobs")]
    public class M_ApiCompanyController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<M_ApiCompanyController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetListJobs()
        {
            var requestName = "Companies-GetJobs";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var agency_id = 0;
                var company_id = 0;
                var translate_status = 0;
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest["agency_id"] != null)
                    agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);
                if (httpRequest["company_id"] != null)
                    company_id = Utils.ConvertToInt32(httpRequest["company_id"]);

                if (httpRequest["translate_status"] != null)
                    translate_status = Utils.ConvertToInt32(httpRequest["translate_status"]);

                var apiFilter = GetFilterConfig();

                dynamic filter = new ExpandoObject();
                filter.keyword = apiFilter.keyword;
                filter.company_id = company_id;
                filter.agency_id = agency_id;
                filter.translate_status = translate_status;

                filter.status = Utils.ConvertToIntFromQuest(apiFilter.status);

                var storeJob = GlobalContainer.IocContainer.Resolve<IStoreJob>();

                List<IdentityJob> myList = storeJob.M_GetListByPage(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(myList);

                var returnData = new List<IdentityJob>();
                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        var returnItem = JobHelpers.Agency_GetBaseInfoJob(item.id, GetCurrentRequestLang());
                        if (returnItem != null)
                        {
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

        [HttpPost]
        [Route("status")]
        public async Task<IHttpActionResult> UpdateStatus(ApiJobUpdateStatusModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Jobs-Status";
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

                var info = storeJob.GetById(model.id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    storeJob.UpdateStatus(model.id, model.status);

                    //Clear cache
                    CachingHelpers.ClearCacheByPrefix(string.Format(EnumFormatInfoCacheKeys.Job, model.id));

                    if (model.status == (int)EnumJobStatus.Canceled)
                    {
                        NotificationHelper.Job_CanceledPublic(info.pic_id, info.staff_id, model.id);
                    }
                    else  if(model.status == (int)EnumJobStatus.Published)
                    {
                        NotificationHelper.Job_AcceptedPublic(info.pic_id, info.staff_id, model.id);
                    }
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

            return SuccessUpdatedResult(returnModel);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Jobs-GetDetail";
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

                var info = JobHelpers.Agency_GetBaseInfoJob(id, null);
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
        public async Task<IHttpActionResult> UpdateCompany(ApiCompanyUpdateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Companies-UpdateCompany";
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
                var fileCount = httpRequest.Files.Count;
                var returnId = 0;
                if (fileCount < 1)
                {
                    //return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var id = Utils.ConvertToIntFromQuest(model.id);
                if (id > 0)
                {
                    var info = storeCompany.GetById(id);
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
                        if (model.agency_id != info.agency_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                //if (fileCount > 0)
                //{
                //    //Begin upload
                //    var uploadReturn = UploadCompanyLogo(model.id, model.agency_id);
                //    if (uploadReturn != null && uploadReturn.HasData())
                //        logoPath = uploadReturn[0].Path;
                //}
                //else
                //    logoPath = info.logo_path;

                var updateInfo = ExtractFormData(model);
                returnId = storeCompany.Update(updateInfo);
                returnModel.value = returnId;

                //Clear cache
                CompanyHelpers.ClearCache(id);

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

            return SuccessUpdatedResult(returnModel);
        }

        [HttpPost]
        [Route("upload_logo")]
        public async Task<IHttpActionResult> UploadLogo()
        {
            var requestName = "Companies-UploadLogo";
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
                var fileCount = httpRequest.Files.Count;
                var company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                var agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                if (fileCount < 1)
                {
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                if (company_id > 0)
                {
                    var info = storeCompany.GetById(company_id);
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
                        if (agency_id != info.agency_id)
                        {
                            returnModel.error.error_code = EnumErrorCode.E060101.ToString();
                            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E060101);

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                if (fileCount > 0)
                {
                    returnModel.value = UploadCompanyLogo(company_id, agency_id);
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

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        [HttpPost]
        [Route("get_list_by_ids")]
        public async Task<IHttpActionResult> GetListByListIds(ApiGetListByIdsModel model)
        {
            var requestName = "Companies-GetListByListIds";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;

                var listData = new List<IdentityCompany>();
                if (model.ListIds.HasData())
                {
                    foreach (var currentId in model.ListIds)
                    {
                        var cachedData = CompanyHelpers.GetBaseInfoCompany(currentId);
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

        private IdentityCompany ExtractFormData(ApiCompanyUpdateModel model)
        {
            var info = new IdentityCompany();
            info.id = Utils.ConvertToIntFromQuest(model.id);
            info.company_name = model.company_name;
            info.description = model.description;
            info.email = model.email;
            info.phone = model.phone;
            info.website = model.website;
            info.fax = model.fax;
            info.logo_path = model.logo_path;
            info.company_size_id = Utils.ConvertToIntFromQuest(model.company_size_id);
            info.sub_industry_id = Utils.ConvertToIntFromQuest(model.sub_industry_id);
            info.establish_year = Utils.ConvertToIntFromQuest(model.establish_year);
            info.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);

            info.address_detail = model.address_detail;
            info.address_furigana = model.address_furigana;

            if (model.Address != null)
            {
                info.country_id = model.Address.country_id;
                info.region_id = model.Address.region_id;
                info.prefecture_id = model.Address.prefecture_id;
                info.city_id = model.Address.city_id;
            }

            info.language_code = GetCurrentRequestLang();

            return info;
        }

        private List<FileUploadResponseModel> UploadCompanyLogo(int company_id, int agency_id = 0)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "Companies";
            uploadModel.ObjectId = company_id.ToString();

            if (company_id <= 0)
            {
                uploadModel.SubDir = "Companies/age";
                uploadModel.ObjectId = agency_id.ToString();
            }

            uploadModel.InCludeDatePath = false;

            if (uploadModel.Files != null && uploadModel.Files[0] != null)
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
    }
}
