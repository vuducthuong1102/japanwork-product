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
using System.Linq;
using ApiJobMarket.Services;
using ApiJobMarket.DB.Sql.Entities;
using System.Web;
using ApiJobMarket.Settings;
using ApiJobMarket.ShareLibs;
using System.Dynamic;
using ApiCompanyMarket.Helpers;
using ApiJobMarket.Resources;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/candidates")]
    public class ApiCandidateController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCandidateController>();

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "Candidates-GetDetail";
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
                var storeCandidate = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                var candidateInfo = storeCandidate.GetById(id);
                await Task.FromResult(candidateInfo);
                if (candidateInfo != null)
                {
                    candidateInfo.job_info = JobHelpers.GetBaseInfoJob(candidateInfo.job_id, GetCurrentRequestLang());
                    if (candidateInfo.type == 1)
                    {
                        candidateInfo.job_seeker_info = JobSeekerHelpers.A_GetBaseInfo(candidateInfo.job_seeker_id);
                    }
                }
                returnModel.value = candidateInfo;
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
        public async Task<IHttpActionResult> Insert(ApiCandidateInsertModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Candidate-Insert";
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
                var storeCandidate = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                var insertInfo = ExtractCandidateData(model);
                returnId = storeCandidate.Insert(insertInfo);
                await Task.FromResult(returnId);
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
        [Route("multi_jobs")]
        public async Task<IHttpActionResult> InsertMultiJobs(ApiCandidateInsertModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Candidate-InsertMultiJobs";
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
                var storeCandidate = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                var insertInfo = ExtractCandidateData(model);
                returnId = storeCandidate.InsertMultiJobs(insertInfo);
                await Task.FromResult(returnId);
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

        [HttpDelete]
        [Route("delete_all_processs")]
        public async Task<IHttpActionResult> DeleteAllProcess(ApiCandidateDeleteModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Candidates-Delete";
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
                var storeCandidate = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                var ressult = storeCandidate.Delete(model.job_id, model.agency_id);
                await Task.FromResult(ressult);


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
        [Route("{job_seeker_id:int}/get_list")]
        public async Task<IHttpActionResult> GetListByJobSeeker(int job_seeker_id)
        {
            CreateDocumentApi(job_seeker_id);
            var requestName = "Candidates-GetListByJobSeeker";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                var returnCode = EnumCommonCode.Success;
                var storeCandidate = GlobalContainer.IocContainer.Resolve<IStoreCandidate>();

                var apiFilter = GetFilterConfig();
                var filter = new IdentityCandidate();
                filter.keyword = apiFilter.keyword;
                filter.job_seeker_id = job_seeker_id;

                var httpRequest = HttpContext.Current.Request;

                if (httpRequest["staff_id"] != null)
                    filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);

                if (httpRequest["agency_id"] != null)
                    filter.agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]);

                if (httpRequest["type_job_seeker"] != null)
                    filter.type_job_seeker = Utils.ConvertToInt32(httpRequest["type_job_seeker"]);

                if (httpRequest["status"] != null)
                    filter.status = Utils.ConvertToInt32(httpRequest["status"]);


                var ListResults = storeCandidate.GetListByJobSeekerId(filter, apiFilter.page_index ?? 0, apiFilter.page_size ?? 0);
                await Task.FromResult(ListResults);
                if (ListResults.HasData())
                {
                    foreach (var item in ListResults)
                    {
                        item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, GetCurrentRequestLang());
                    }
                    returnModel.total = ListResults[0].total_count;
                }
                returnModel.value = ListResults;

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
        #region Helpers

        private IdentityJob ExtractFormData(ApiJobUpdateModel model)
        {
            var info = new IdentityJob();
            info.language_code = GetCurrentRequestLang();
            info.id = Utils.ConvertToIntFromQuest(model.job.id);
            info.staff_id = model.job.staff_id;
            info.company_id = model.job.company_id;
            info.quantity = model.job.quantity;
            info.age_min = model.job.age_min;
            info.age_max = model.job.age_max;
            info.salary_min = model.job.salary_min;
            info.salary_max = model.job.salary_max;
            info.salary_type_id = model.job.salary_type_id;
            info.pic_id = model.job.pic_id;

            TimeSpan workStartTime;
            if (TimeSpan.TryParse(model.job.work_start_time, out workStartTime))
            {
                info.work_start_time = workStartTime;
            }

            TimeSpan workEndTime;
            if (TimeSpan.TryParse(model.job.work_end_time, out workEndTime))
            {
                info.work_end_time = workEndTime;
            }

            info.probation_duration = model.job.probation_duration;
            info.employment_type_id = model.job.employment_type_id;
            info.flexible_time = model.job.flexible_time;
            info.language_level = model.job.language_level;
            info.work_experience_doc_required = model.job.work_experience_doc_required;
            info.duration = model.job.duration;
            info.view_company = model.job.view_company;
            info.qualification_id = model.job.qualification_id;
            info.japanese_level_number = model.job.japanese_level_number;
            info.sub_field_id = model.job.sub_field_id;
            info.sub_industry_id = model.job.sub_industry_id;

            if (model.job.Addresses.HasData())
            {
                info.Addresses = new List<IdentityJobAddress>();
                foreach (var item in model.job.Addresses)
                {
                    var addressInfo = new IdentityJobAddress();
                    addressInfo.country_id = item.country_id;
                    addressInfo.region_id = item.region_id;
                    addressInfo.prefecture_id = item.prefecture_id;
                    addressInfo.city_id = item.city_id;
                    addressInfo.furigana = item.furigana;
                    addressInfo.detail = item.detail;

                    if (item.Stations.HasData())
                    {
                        addressInfo.Stations = new List<IdentityJobAddressStation>();
                        foreach (var st in item.Stations)
                        {
                            var station = new IdentityJobAddressStation();
                            station.id = st.id;

                            addressInfo.Stations.Add(station);
                        }
                    }

                    info.Addresses.Add(addressInfo);
                }
            }

            if (model.job.Job_translations.HasData())
            {
                info.Job_translations = new List<IdentityJobTranslation>();
                foreach (var item in model.job.Job_translations)
                {
                    var tranInfo = new IdentityJobTranslation();

                    tranInfo.title = item.title;
                    tranInfo.subsidy = item.subsidy;
                    tranInfo.paid_holiday = item.paid_holiday;
                    tranInfo.bonus = item.bonus;
                    tranInfo.certificate = item.certificate;
                    tranInfo.work_content = item.work_content;
                    tranInfo.requirement = item.requirement;
                    tranInfo.plus = item.plus;
                    tranInfo.welfare = item.welfare;
                    tranInfo.training = item.training;
                    tranInfo.recruitment_procedure = item.recruitment_procedure;
                    tranInfo.remark = item.remark;
                    tranInfo.language_code = item.language_code;

                    info.Job_translations.Add(tranInfo);
                }
            }

            //if (model.job.Sub_fields.HasData())
            //{
            //    info.Sub_fields = new List<IdentityJobSubField>();                
            //    foreach (var item in model.job.Sub_fields)
            //    {
            //        var subFieldInfo = new IdentityJobSubField();
            //        subFieldInfo.id = item.id;

            //        info.Sub_fields.Add(subFieldInfo);
            //    }
            //}

            if (model.job.Tags.HasData())
            {
                info.Tags = new List<IdentityJobTag>();
                foreach (var item in model.job.Tags)
                {
                    var tagInfo = new IdentityJobTag();

                    tagInfo.id = item.id;
                    tagInfo.tag = item.tag;
                    info.Tags.Add(tagInfo);
                }
            }

            return info;
        }
        private IdentityCandidate ExtractCandidateData(ApiCandidateInsertModel model)
        {
            IdentityCandidate item = new IdentityCandidate();
            item.job_id = model.job_id;
            item.job_seeker_id = model.job_seeker_id;
            item.type = model.type;
            item.cv_id = model.cv_id;
            item.company_id = model.company_id;
            item.agency_id = model.agency_id;
            item.staff_id = model.staff_id;
            item.list_job_seeker_ids = model.list_job_seeker_ids;
            item.list_job_ids = model.list_job_ids;
            item.pic_id = model.pic_id;
            return item;
        }
        private IdentityJobTranslation ExtractDataTranslation(ApiJobUpdateTranslationModel model)
        {
            var tranInfo = new IdentityJobTranslation();

            tranInfo.title = model.title;
            tranInfo.subsidy = model.subsidy;
            tranInfo.paid_holiday = model.paid_holiday;
            tranInfo.bonus = model.bonus;
            tranInfo.certificate = model.certificate;
            tranInfo.work_content = model.work_content;
            tranInfo.requirement = model.requirement;
            tranInfo.plus = model.plus;
            tranInfo.welfare = model.welfare;
            tranInfo.training = model.training;
            tranInfo.recruitment_procedure = model.recruitment_procedure;
            tranInfo.remark = model.remark;
            tranInfo.language_code = model.language_code;
            tranInfo.job_id = model.job_id;
            tranInfo.translate_status = model.translate_status;
            return tranInfo;
        }

        #endregion
    }
}
