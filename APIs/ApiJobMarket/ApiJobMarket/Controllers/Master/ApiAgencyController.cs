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
using ApiJobMarket.Services;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/agencies")]
    public class ApiAgenyController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiAgenyController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Agencies-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var listAgency = CommonHelpers.GetListAgencies();

                await Task.FromResult(listAgency);

                returnModel.value = listAgency;

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
            var requestName = "Agencies-GetDetail";
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

                var info = AgencyHelpers.GetBaseInfoAgency(id);
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

        [HttpGet]
        [Route("{id:int}/companies")]
        public async Task<IHttpActionResult> GetListCompanyByAgency(int id)
        {
            var requestName = "Agencies-GetCompanies";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                var httpRequest = HttpContext.Current.Request;
                var filter = new IdentityCompany();
                filter.keyword = apiFilter.keyword;
                filter.prefecture_id = Utils.ConvertToInt32(httpRequest["prefecture_id"]);
                filter.sub_industry_id = Utils.ConvertToInt32(httpRequest["sub_industry_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.ishiring = Utils.ConvertToInt32(httpRequest["ishiring"]);
                filter.agency_id = id;
                filter.language_code = GetCurrentRequestLang();

                var storeCompany = GlobalContainer.IocContainer.Resolve<IStoreCompany>();

                var myList = storeCompany.GetListByAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);

                await Task.FromResult(myList);

                //List<ApiResponseCompanyItemModel> returnList = null;
                if (myList.HasData())
                {
                    returnModel.total = myList[0].total_count;
                    //returnList = new List<ApiResponseCompanyItemModel>();

                    foreach (var item in myList)
                    {
                        item.logo_full_path = CdnHelper.SocialGetFullImgPath(item.logo_path);

                        //ApiResponseCompanyItemModel record = new ApiResponseCompanyItemModel();
                        //record.id = item.id;
                        //record.company_name = item.company_name;
                        //record.description = item.description;
                        //record.company_size_id = item.company_size_id;
                        //record.logo_path = item.logo_path;
                        //record.sub_industry_id = item.sub_industry_id;
                        //record.establish_year = item.establish_year;
                        //record.website = item.website;
                        //record.phone = item.phone;
                        //record.fax = item.fax;
                        //record.branch = item.branch;
                        //record.agency_id = item.agency_id;
                        //record.desciption_translation_id = item.desciption_translation_id;
                        //record.headquater_id = item.headquater_id;
                        //record.region_id = item.region_id;
                        //record.prefecture_id = item.prefecture_id;
                        //record.city_id = item.city_id;
                        //record.lat = item.lat;
                        //record.lng = item.lng;
                        //record.map = item.map;
                        //record.job_count = item.job_count;
                        //record.application_count = item.application_count;

                        //returnList.Add(record);
                    }
                }

                returnModel.value = myList;
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
        [Route("{id:int}/applications")]
        public async Task<IHttpActionResult> GetApplicationsByAgency(int id)
        {
            var requestName = "Agencies-GetApplicationsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);
                filter.cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);

                filter.language_code = GetCurrentRequestLang();

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                List<IdentityApplication> listData = storeApplication.GetListByAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.Agency_GetBaseInfoJob(item.job_id, filter.language_code);
                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }

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


        [HttpPost]
        [Route("applications")]
        public async Task<IHttpActionResult> GetApplicationsByPage(ApiApplicationModel model)
        {
            var requestName = "Agencies-GetApplicationsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                model.language_code = GetCurrentRequestLang();

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                List<IdentityApplication> listData = storeApplication.GetListByPage(model, model.page_index.Value, model.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.Agency_GetBaseInfoJob(item.job_id, model.language_code);

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }

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
        [Route("{id:int}/application_invited")]
        public async Task<IHttpActionResult> GetApplicationInvitedByPage(int id)
        {
            var requestName = "GetApplicationInvitedByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);
                filter.cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);

                filter.language_code = GetCurrentRequestLang();

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                List<IdentityApplication> listData = storeApplication.GetListInvited(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                }

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
        [Route("{id:int}/application_offline")]
        public async Task<IHttpActionResult> GetApplicationOfflineByPage(int id)
        {
            var requestName = "GetApplicationOfflineByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);
                filter.cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);

                filter.language_code = GetCurrentRequestLang();

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                List<IdentityApplication> listData = storeApplication.GetListOffline(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                }

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
        [Route("{id:int}/application_recruited")]
        public async Task<IHttpActionResult> GetApplicationRecruitedByPage(int id)
        {
            var requestName = "GetApplicationRecruitedByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);
                filter.cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);

                filter.language_code = GetCurrentRequestLang();

                var storeApplication = GlobalContainer.IocContainer.Resolve<IStoreApplication>();

                List<IdentityApplication> listData = storeApplication.GetListRecruited(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    foreach (var item in listData)
                    {
                        if (item.type == 0)
                        {
                            var jobSeeker = JobSeekerHelpers.GetBaseInfo(item.job_seeker_id);
                            if (jobSeeker != null)
                            {
                                item.fullname = jobSeeker.fullname;
                                item.gender = jobSeeker.gender;
                                item.birthday = jobSeeker.birthday;
                            }
                        }
                        else
                        {
                            var jobSeeker = JobSeekerHelpers.A_GetBaseInfo(item.job_seeker_id);
                            if (jobSeeker != null)
                            {
                                item.fullname = jobSeeker.fullname;
                                item.gender = jobSeeker.gender;
                                item.birthday = jobSeeker.birthday;
                            }
                        }
                    }
                    returnModel.total = listData[0].total_count;
                }

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
        [Route("{id:int}/candidates")]
        public async Task<IHttpActionResult> GetCandidatesByPage(int id)
        {
            var requestName = "Agencies-GetCandidatesByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.type_job_seeker = Utils.ConvertToInt32(httpRequest["type_job_seeker"]);

                filter.language_code = GetCurrentRequestLang();

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                List<IdentityCandidate> listData = myStore.GetListByAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                       //item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, filter.language_code);
                        if (filter.type_job_seeker == 1)
                        {
                            item.job_seeker_info = JobSeekerHelpers.A_GetBaseInfo(item.job_seeker_id);
                        }

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }

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
        [Route("{id:int}/candidates/by_job_id")]
        public async Task<IHttpActionResult> GetCandidatesByJobId(int id)
        {
            var requestName = "Agencies-GetCandidatesByJobId";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);

                if (httpRequest["type_job_seeker"] != null)
                    filter.type_job_seeker = Utils.ConvertToInt32(httpRequest["type_job_seeker"]);

                if (httpRequest["staff_id"] != null)
                    filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);

                if (httpRequest["status"] != null)
                    filter.status = Utils.ConvertToInt32(httpRequest["status"]);

                filter.language_code = GetCurrentRequestLang();

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                List<IdentityCandidate> listData = myStore.GetListByJobId(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        //item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, filter.language_code);
                        if (item.type == 0)
                        {
                            var jobSeekerInfo = JobSeekerHelpers.GetBaseInfo(item.job_seeker_id, filter.agency_id);
                            if (jobSeekerInfo != null)
                            {
                                item.job_seeker_info = jobSeekerInfo;
                                item.job_seeker_info.code = item.code;
                            }
                        }
                        else
                        {
                            item.job_seeker_info = JobSeekerHelpers.A_GetBaseInfo(item.job_seeker_id, filter.agency_id);
                            item.job_seeker_info.code = item.code;
                        }

                        foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                        {
                            if (item.status == (int)enm)
                                item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                        }
                    }
                }

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
        [Route("{id:int}/invitations")]
        public async Task<IHttpActionResult> GetInvitationsByPage(int id)
        {
            var requestName = "Agencies-GetInvitationsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = id;
                filter.company_id = Utils.ConvertToInt32(httpRequest["company_id"]);
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);

                filter.language_code = GetCurrentRequestLang();

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreInvitation>();

                List<IdentityInvitation> listData = myStore.GetListByAgency(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.Agency_GetBaseInfoJob(item.job_id, filter.language_code);
                    }
                }

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

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateProfile(ApiAgencyCreateModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Agencies-CreateProfile";
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
                var agency = new IdentityAgency();
                agency.agency = model.agency;
                agency.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                agency.company_name = model.company_name;
                agency.email = model.email;
                agency.phone = model.phone;
                agency.website = model.website;
                agency.address = model.address;

                var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                var result = store.CreateProfile(agency);

                await Task.FromResult(result);

                returnModel.value = result;

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
            var requestName = "Job_seekers-UploadLogo";
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
                if (fileCount < 1)
                {
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                //logger.Debug(string.Format("Begin {0} request", requestName));
                var agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                var returnCode = EnumCommonCode.Success;

                var info = AgencyHelpers.GetBaseInfoAgency(agency_id);
                await Task.FromResult(info);

                if (info == null)
                {
                    returnModel.error.error_code = EnumErrorCode.E000112.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000112);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    if (fileCount > 0)
                    {
                        var storeAgency = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                        var uploadResult = UploadAgencyLogo(info);
                        if (uploadResult.HasData())
                        {
                            var firstResult = uploadResult[0];
                            if (firstResult != null)
                            {
                                storeAgency.UpdateLogo(new IdentityAgency { agency_id = info.agency_id, logo_path = firstResult.Path });

                                //Clear cache
                                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Agency, info.agency_id));
                            }
                        }

                        returnModel.value = uploadResult;
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

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateProfile(ApiAgencyModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Agencies-UpdateProfile";
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
                var agency = new IdentityAgency();
                agency.agency = model.agency;
                agency.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                agency.company_name = model.company_name;
                //agency.email = model.email;
                agency.phone = model.phone;
                agency.website = model.website;
                agency.address = model.address;
                agency.logo_path = model.logo_path;

                var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                var result = store.UpdateProfile(agency);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.Agency, model.agency_id));

                await Task.FromResult(result);

                returnModel.value = result;

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

        private IdentityJobSeeker ExtractCvToJobSeeker(IdentityCv record)
        {
            IdentityJobSeeker result = new IdentityJobSeeker();
            try
            {
                result.fullname = record.fullname;
                result.birthday = record.birthday;
                result.id = record.job_seeker_id;
                result.phone = record.phone;
                result.gender = record.gender;
                result.cv_id = record.id;
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to ExtractCvToJobSeeker :" + ex.ToString());
            }
            return result;
        }

        #region Helpers        

        private List<FileUploadResponseModel> UploadAgencyLogo(IdentityAgency info)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;

            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "Agencies";
            uploadModel.ObjectId = info.agency_id.ToString();
            uploadModel.InCludeDatePath = false;

            if (!string.IsNullOrEmpty(info.logo_path))
            {
                //Delete old file
                var apiDeleteModel = new FilesDeleteModel();
                apiDeleteModel.FilesPath = new List<string>();
                apiDeleteModel.FilesPath.Add(info.logo_path);

                var deleteResult = CdnServices.DeleteImagesAsync(apiDeleteModel).Result;
            }

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
                    logger.Error("Failed to get Upload Agency logo because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload Agency logo because: The image data is null");
            }

            return returnUploaded;
        }

        #endregion
    }
}
