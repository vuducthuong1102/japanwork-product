using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;
using Manager.SharedLibs;
using System.Linq;
using System.Globalization;

namespace Manager.WebApp.Controllers
{
    public class JobController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<JobController>();

        public JobController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageJobModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = new ApiJobModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
                company_id = model.company_id,
                status = (model.Status == null) ? 0 : Utils.ConvertToIntFromQuest(model.Status),
                translate_status = (model.TranslateStatus == null) ? -1 : Utils.ConvertToIntFromQuest(model.TranslateStatus),
                agency_id = GetCurrentStaffId()
            };
            model.Status = filter.status;
            model.TranslateStatus = filter.translate_status;
            try
            {
                if (model.company_id > 0)
                {
                    var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
                    if (companyResult != null)
                    {
                        if (companyResult.value != null)
                        {
                            IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
                            if (companyInfo != null)
                            {
                                //if (companyInfo.agency_id != GetCurrentStaffId())
                                //{
                                //    return RedirectToErrorPage();
                                //}

                                model.CompanyInfo = companyInfo;
                                if (companyInfo.LangList.HasData())
                                {
                                    model.company_name = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
                                }

                                if (string.IsNullOrEmpty(model.company_name))
                                    model.company_name = companyInfo.company_name;
                            }
                            else
                            {
                                return RedirectToErrorPage();
                            }
                        }
                    }
                }

                var result = JobServices.M_GetJobsAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJob>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        model.CurrentPage = currentPage;
                        model.PageSize = pageSize;
                        model.Industries = CommonHelpers.GetListIndustries();

                        if (model.company_id <= 0)
                        {
                            var listCompanyIds = model.SearchResults.Select(x => x.company_id).ToList();

                            var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                            if (companyReturnApi != null && companyReturnApi.value != null)
                                model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                        }
                    }
                }

                //model.Industries = CommonHelpers.GetListIndustries();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        public ActionResult Create(int? company_id)
        {
            var companyId = Utils.ConvertToIntFromQuest(company_id);
            var model = new JobEditModel();
            try
            {
                if (company_id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                model.company_id = companyId;
                var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
                if (companyResult != null)
                {
                    if (companyResult.value != null)
                    {
                        var companyName = string.Empty;
                        IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
                        if (companyInfo != null)
                        {
                            if (companyInfo.agency_id != GetCurrentStaffId())
                            {
                                return RedirectToErrorPage();
                            }

                            model.CompanyInfo = companyInfo;
                            if (companyInfo.LangList.HasData())
                            {
                                companyName = companyInfo.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.company_name).FirstOrDefault();
                            }

                            if (string.IsNullOrEmpty(companyName))
                                companyName = companyInfo.company_name;

                            ViewBag.CompanyName = companyName;
                        }
                        else
                        {
                            return RedirectToErrorPage();
                        }
                    }
                }

                model.Countries = CommonHelpers.GetListCountries();
                model.Industries = CommonHelpers.GetListIndustries();
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.Fields = CommonHelpers.GetListFields();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();

                model.SelectedAddresses = new List<IdentityJobAddress>();
                model.SelectedAddresses.Add(new IdentityJobAddress());
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display CreateJob because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Job(JobEditModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            try
            {
                //Begin create
                var apiJobModel = ExtractFormData(model);

                var apiReturned = JobServices.CreateAsync(apiJobModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            this.AddNotification(apiReturned.error.message, NotificationType.ERROR);
                        }
                        else
                        {
                            this.AddNotification(ManagerResource.LB_INSERT_SUCCESS, NotificationType.SUCCESS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to CreateJob because: " + ex.ToString());
            }

            return RedirectToAction("Index", "Job", new { id = model.company_id });
        }
        [AccessRoleChecker]
        public ActionResult Edit(int? id, int? company_id)
        {
            ModelState.Clear();
            var jobId = Utils.ConvertToIntFromQuest(id);
            var companyId = Utils.ConvertToIntFromQuest(company_id);
            var model = new JobEditModel();
            try
            {
                if (jobId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var apiModel = new ApiJobModel();
                apiModel.id = jobId;

                var result = JobServices.GetDetailAsync(apiModel).Result;
                IdentityJob info = null;
                if (result != null && result.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityJob>(result.value.ToString());
                    model = ParseDataToForm(info);
                }

                model.id = jobId;
                model.company_id = companyId;

                if (info != null)
                {
                    if (info.company_id != model.company_id)
                    {
                        return RedirectToErrorPage();
                    }

                    model.Countries = CommonHelpers.GetListCountries();
                    model.Industries = CommonHelpers.GetListIndustries();
                    model.Qualifications = CommonHelpers.GetListQualifications();

                    //model.TagIds = info.Tags;
                    model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                    model.Fields = CommonHelpers.GetListFields();
                    model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();

                    var regionIds = new List<int>();
                    var prefectureIds = new List<int>();
                    var cityIds = new List<int>();
                    var stationIds = new List<int>();

                    if (info.Addresses.HasData())
                    {
                        model.SelectedAddresses = info.Addresses;

                        //var currentRegionIds = info.Addresses.Select(x => x.region_id).ToList();
                        //if (currentRegionIds.HasData())
                        //{
                        //    regionIds.AddRange(currentRegionIds);
                        //}

                        //var currentPrefectIds = info.Addresses.Select(x => x.prefecture_id).ToList();
                        //if (currentPrefectIds.HasData())
                        //{
                        //    prefectureIds.AddRange(currentPrefectIds);
                        //}

                        //var currentCityIds = info.Addresses.Select(x => x.city_id).ToList();
                        //if (currentCityIds.HasData())
                        //{
                        //    cityIds.AddRange(currentCityIds);
                        //}

                        foreach (var add in info.Addresses)
                        {
                            var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                            if (currentStationIds.HasData())
                            {
                                stationIds.AddRange(currentStationIds);
                            }
                        }

                        model.Stations = CommonHelpers.GetListStations(stationIds);
                    }
                    else
                    {
                        model.SelectedAddresses = new List<IdentityJobAddress>();
                        model.SelectedAddresses.Add(new IdentityJobAddress());
                    }
                }
                else
                {
                    return RedirectToErrorPage();
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditJob [{0}] because: {1}", jobId, ex.ToString()));
            }

            return View(model);
        }
       
        [HttpPost, ActionName("Language")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLanguage(JobEditLanguageModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage + x.Exception));
                return Json(new { success = isSuccess, message = messages, title = ManagerResource.LB_NOTIFICATION });
            }
            try
            {
                //Begin update
                var apiJobModel = ExtractFormTranslationData(model);

                var apiReturned = JobServices.UpdateTranslationAsync(apiJobModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(apiReturned.message))
                            {
                                return Json(new { success = true, message = apiReturned.message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateJob because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateJob(JobEditModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                //Begin update
                var apiJobModel = ExtractFormData(model);

                var apiReturned = JobServices.UpdateAsync(apiJobModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(apiReturned.message))
                            {
                                return Json(new { success = true, message = apiReturned.message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateJob because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
        }
        [AccessRoleChecker]
        public ActionResult Language(int? id, int? company_id,string from_language,string to_language,int? status,int ? translatedstatus)
        {
            ModelState.Clear();
            var jobId = Utils.ConvertToIntFromQuest(id);
            var companyId = Utils.ConvertToIntFromQuest(company_id);
            var model = new JobEditLanguageModel();
            try
            {
                if (jobId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var apiModel = new ApiJobModel();
                apiModel.id = jobId;

                var result = JobServices.GetDetailAsync(apiModel).Result;
                IdentityJob info = null;
                if (result != null && result.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityJob>(result.value.ToString());
                    model = ParseDataToFormLanguage(info,from_language,to_language);
                }
                model.id = jobId;
                model.company_id = companyId;
                model.status = status ?? -1;
                model.filter_translate_status = translatedstatus ?? -1;
                if (info != null)
                {
                    if (info.company_id != model.company_id)
                    {
                        return RedirectToErrorPage();
                    }

                    model.Countries = CommonHelpers.GetListCountries();
                    model.Industries = CommonHelpers.GetListIndustries();
                    model.Qualifications = CommonHelpers.GetListQualifications();

                    //model.TagIds = info.Tags;
                    model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                    model.Fields = CommonHelpers.GetListFields();
                    model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();

                    var regionIds = new List<int>();
                    var prefectureIds = new List<int>();
                    var cityIds = new List<int>();
                    var stationIds = new List<int>();

                    if (info.Addresses.HasData())
                    {
                        model.SelectedAddresses = info.Addresses;

                        //var currentRegionIds = info.Addresses.Select(x => x.region_id).ToList();
                        //if (currentRegionIds.HasData())
                        //{
                        //    regionIds.AddRange(currentRegionIds);
                        //}

                        //var currentPrefectIds = info.Addresses.Select(x => x.prefecture_id).ToList();
                        //if (currentPrefectIds.HasData())
                        //{
                        //    prefectureIds.AddRange(currentPrefectIds);
                        //}

                        //var currentCityIds = info.Addresses.Select(x => x.city_id).ToList();
                        //if (currentCityIds.HasData())
                        //{
                        //    cityIds.AddRange(currentCityIds);
                        //}

                        foreach (var add in info.Addresses)
                        {
                            var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                            if (currentStationIds.HasData())
                            {
                                stationIds.AddRange(currentStationIds);
                            }
                        }

                        model.Stations = CommonHelpers.GetListStations(stationIds);
                    }
                    else
                    {
                        model.SelectedAddresses = new List<IdentityJobAddress>();
                        model.SelectedAddresses.Add(new IdentityJobAddress());
                    }
                }
                else
                {
                    return RedirectToErrorPage();
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display EditJob [{0}] because: {1}", jobId, ex.ToString()));
            }

            return View(model);
        }
        public ActionResult UpdateAddress(int? id = 0)
        {
            var model = new JobUpdateAddressModel();
            try
            {
                var currentId = Utils.ConvertToIntFromQuest(id);
                model.Countries = CommonHelpers.GetListCountries();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateAddress form: " + ex.ToString());
            }

            return PartialView("Partial/_UpdateAddress", model);
        }

        //Show popup confirm delete        
        [AccessRoleChecker]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("Partials/_PopupDelete", id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var result = JobServices.DeleteAsync(new ApiJobDeleteModel { id = id, agency_id = GetCurrentStaffId() }).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Post because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }
        [AccessRoleChecker]
        public ActionResult UpdateStatus()
        {
            var jobid = 0;
            if (Request["jobid"] != null)
            {
                jobid = Utils.ConvertToInt32(Request["jobid"], 1);
            }
            if (jobid <= 0)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var status = 0;
            if (Request["status"] != null)
            {
                status = Utils.ConvertToInt32(Request["status"], 1);
            }
            var model = new JobUpdateStatusModel()
            {
                id = jobid,
                status = status
            };
            return PartialView("Partials/_UpdateStatus", model);
        }

        [HttpPost, ActionName("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus_Confirm(int id, int status)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new ApiJobUpdateStatusModel()
            {
                id = id,
                status = status
            };
            try
            {
                var result = JobServices.UpdateStatusAsync(model).Result;
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Update Job Status because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterUpdate()" });
        }

        #region Helpers

        private JobEditModel ParseDataToForm(IdentityJob info)
        {
            var model = new JobEditModel();
            model.id = info.id;
            model.company_id = info.company_id;
            model.quantity = info.quantity;
            model.age_min = info.age_min;
            model.age_max = info.age_max;
            model.salary_min = info.salary_min;
            model.salary_max = info.salary_max;
            model.salary_type_id = info.salary_type_id;
            model.work_start_time = info.work_start_time.TimeSpanQuestToString();
            model.work_end_time = info.work_end_time.TimeSpanQuestToString();
            model.probation_duration = info.probation_duration;
            model.employment_type_id = info.employment_type_id;
            model.flexible_time = info.flexible_time;
            model.language_level = info.language_level;
            model.work_experience_doc_required = info.work_experience_doc_required;
            model.duration = info.duration;
            model.view_company = info.view_company;
            model.qualification_id = info.qualification_id;
            model.japanese_level_number = info.japanese_level_number;
            model.sub_field_id = info.sub_field_id;
            model.sub_industry_id = info.sub_industry_id;
            if (info.closed_time != null)
            {
                model.closed_time = info.closed_time.Value.ToString("dd/MM/yyyy HH:mm");
            }

            if (info.Job_translations.HasData())
            {
                var myLang = info.Job_translations.Where(x => x.language_code == _currentLanguage).FirstOrDefault();
                if (myLang != null)
                {
                    model.title = myLang.title;
                    model.subsidy = myLang.subsidy;
                    model.paid_holiday = myLang.paid_holiday;
                    model.bonus = myLang.bonus;
                    model.certificate = myLang.certificate;
                    model.work_content = myLang.work_content;
                    model.requirement = myLang.requirement;
                    model.plus = myLang.plus;
                    model.welfare = myLang.welfare;
                    model.training = myLang.training;
                    model.recruitment_procedure = myLang.recruitment_procedure;
                    model.remark = myLang.remark;
                }
            }

            if (info.Tags.HasData())
            {
                model.TagIds = info.Tags.Select(x => x.id).ToList();
                model.SelectedTags = info.Tags;
            }

            return model;
        }

        private JobEditLanguageModel ParseDataToFormLanguage(IdentityJob info,string from_language, string to_language)
        {
            var model = new JobEditLanguageModel();
            model.id = info.id;
            model.company_id = info.company_id;
            model.quantity = info.quantity;
            model.age_min = info.age_min;
            model.age_max = info.age_max;
            model.salary_min = info.salary_min;
            model.salary_max = info.salary_max;
            model.salary_type_id = info.salary_type_id;
            model.work_start_time = info.work_start_time.TimeSpanQuestToString();
            model.work_end_time = info.work_end_time.TimeSpanQuestToString();
            model.probation_duration = info.probation_duration;
            model.employment_type_id = info.employment_type_id;
            model.flexible_time = info.flexible_time;
            model.language_level = info.language_level;
            model.work_experience_doc_required = info.work_experience_doc_required;
            model.duration = info.duration;
            model.view_company = info.view_company;
            model.qualification_id = info.qualification_id;
            model.japanese_level_number = info.japanese_level_number;
            model.sub_field_id = info.sub_field_id;
            model.sub_industry_id = info.sub_industry_id;

            if (info.Job_translations.HasData())
            {
                model.from_language = from_language;
                if (string.IsNullOrEmpty(model.from_language))
                {
                    model.from_language = "ja-JP";
                }
                var langList = LanguagesProvider.GetListLanguages();
                if (langList.HasData())
                {
                    var checkLang = langList.FirstOrDefault(s => s.LanguageCultureName == to_language);
                    if (checkLang==null)
                    {
                        checkLang = langList.Where(s => s.LanguageCultureName != model.from_language).FirstOrDefault();
                        if (checkLang != null)
                        {
                            model.to_language = checkLang.LanguageCultureName;
                            model.Language = langList[0];
                        }
                    }
                    else
                    {
                        model.to_language = to_language;
                        model.Language = checkLang;
                    }
                }

               
                var myLang = info.Job_translations.Where(x => x.language_code == model.to_language).FirstOrDefault();
                if (myLang != null)
                {
                    model.title = myLang.title;
                    model.subsidy = myLang.subsidy;
                    model.paid_holiday = myLang.paid_holiday;
                    model.bonus = myLang.bonus;
                    model.certificate = myLang.certificate;
                    model.work_content = myLang.work_content;
                    model.requirement = myLang.requirement;
                    model.plus = myLang.plus;
                    model.welfare = myLang.welfare;
                    model.training = myLang.training;
                    model.recruitment_procedure = myLang.recruitment_procedure;
                    model.remark = myLang.remark;
                }
                model.Job_translations = info.Job_translations;
            }

            if (info.Tags.HasData())
            {
                model.TagIds = info.Tags.Select(x => x.id).ToList();
                model.SelectedTags = info.Tags;
            }

            return model;
        }
        private ApiJobUpdateTranslationModel ExtractFormTranslationData(JobEditLanguageModel model)
        {
            var apiModel = new ApiJobUpdateTranslationModel();
            apiModel.title = model.title;
            apiModel.subsidy = HtmlHelpers.RemoveScriptTags(model.subsidy);
            apiModel.paid_holiday = HtmlHelpers.RemoveScriptTags(model.paid_holiday);
            apiModel.bonus = HtmlHelpers.RemoveScriptTags(model.bonus);
            apiModel.certificate = HtmlHelpers.RemoveScriptTags(model.certificate);
            apiModel.work_content = HtmlHelpers.RemoveScriptTags(model.work_content);
            apiModel.requirement = HtmlHelpers.RemoveScriptTags(model.requirement);
            apiModel.plus = HtmlHelpers.RemoveScriptTags(model.plus);
            apiModel.welfare = HtmlHelpers.RemoveScriptTags(model.welfare);
            apiModel.training = HtmlHelpers.RemoveScriptTags(model.training);
            apiModel.recruitment_procedure = HtmlHelpers.RemoveScriptTags(model.recruitment_procedure);
            apiModel.remark = HtmlHelpers.RemoveScriptTags(model.remark);
            apiModel.language_code = model.to_language;
            apiModel.job_id = model.id;
            apiModel.translate_status = model.translate_status;
            return apiModel;
        }
        private ApiJobUpdateModel ExtractFormData(JobEditModel model)
        {
            var apiModel = new ApiJobUpdateModel();
            apiModel.job = new ApiJobUpdateInfoModel();

            apiModel.job.id = model.id;
            apiModel.job.company_id = model.company_id;
            apiModel.job.quantity = model.quantity;
            apiModel.job.age_min = model.age_min;
            apiModel.job.age_max = model.age_max;
            apiModel.job.salary_min = model.salary_min;
            apiModel.job.salary_max = model.salary_max;
            apiModel.job.salary_type_id = model.salary_type_id;
            apiModel.job.work_start_time = model.work_start_time;
            apiModel.job.work_end_time = model.work_end_time;
            apiModel.job.probation_duration = model.probation_duration;
            apiModel.job.employment_type_id = model.employment_type_id;
            apiModel.job.flexible_time = model.flexible_time;
            apiModel.job.language_level = model.language_level;
            apiModel.job.work_experience_doc_required = model.work_experience_doc_required;
            apiModel.job.duration = model.duration;
            apiModel.job.view_company = model.view_company;
            apiModel.job.qualification_id = model.qualification_id;
            apiModel.job.japanese_level_number = model.japanese_level_number;
            apiModel.job.sub_field_id = model.sub_field_id;
            apiModel.job.sub_industry_id = model.sub_industry_id;
            if (!string.IsNullOrEmpty(model.closed_time))
            {
                apiModel.job.closed_time = DateTime.ParseExact(model.closed_time, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            if (model.Addresses.HasData())
            {
                apiModel.job.Addresses = new List<ApiJobUpdateAddressModel>();
                foreach (var item in model.Addresses)
                {
                    var addressInfo = new ApiJobUpdateAddressModel();
                    addressInfo.id = item.id;
                    addressInfo.country_id = item.country_id;
                    addressInfo.region_id = item.region_id;
                    addressInfo.prefecture_id = item.prefecture_id;
                    addressInfo.city_id = item.city_id;
                    addressInfo.furigana = item.furigana;
                    addressInfo.detail = item.detail;

                    if (item.StationsIds.HasData())
                    {
                        addressInfo.Stations = new List<ApiJobUpdateStationModel>();
                        foreach (var st in item.StationsIds)
                        {
                            var station = new ApiJobUpdateStationModel();
                            station.id = st;

                            addressInfo.Stations.Add(station);
                        }
                    }

                    apiModel.job.Addresses.Add(addressInfo);
                }
            }

            apiModel.job.Job_translations = new List<ApiJobUpdateTranslationModel>();
            var tranInfo = new ApiJobUpdateTranslationModel();

            tranInfo.title = model.title;
            tranInfo.subsidy = HtmlHelpers.RemoveScriptTags(model.subsidy);
            tranInfo.paid_holiday = HtmlHelpers.RemoveScriptTags(model.paid_holiday);
            tranInfo.bonus = HtmlHelpers.RemoveScriptTags(model.bonus);
            tranInfo.certificate = HtmlHelpers.RemoveScriptTags(model.certificate);
            tranInfo.work_content = HtmlHelpers.RemoveScriptTags(model.work_content);
            tranInfo.requirement = HtmlHelpers.RemoveScriptTags(model.requirement);
            tranInfo.plus = HtmlHelpers.RemoveScriptTags(model.plus);
            tranInfo.welfare = HtmlHelpers.RemoveScriptTags(model.welfare);
            tranInfo.training = HtmlHelpers.RemoveScriptTags(model.training);
            tranInfo.recruitment_procedure = HtmlHelpers.RemoveScriptTags(model.recruitment_procedure);
            tranInfo.remark = HtmlHelpers.RemoveScriptTags(model.remark);
            tranInfo.language_code = _currentLanguage;

            apiModel.job.Job_translations.Add(tranInfo);

            //if (model.Sub_fields.HasData())
            //{
            //    apiModel.job.Sub_fields = new List<ApiJobUpdateSubFieldModel>();
            //    foreach (var item in model.Sub_fields)
            //    {
            //        var subFieldInfo = new ApiJobUpdateSubFieldModel();
            //        subFieldInfo.id = item;

            //        apiModel.job.Sub_fields.Add(subFieldInfo);
            //    }
            //}

            if (model.TagIds.HasData())
            {
                apiModel.job.Tags = new List<ApiJobUpdateTagModel>();
                foreach (var item in model.TagIds)
                {
                    var tagInfo = new ApiJobUpdateTagModel();

                    tagInfo.id = item;
                    //tagInfo.tag = item.tag;

                    apiModel.job.Tags.Add(tagInfo);
                }
            }

            return apiModel;
        }

        #endregion

    }
}