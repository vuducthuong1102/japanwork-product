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

using System.Linq;
using System.Globalization;
using System.Dynamic;

namespace Manager.WebApp.Controllers
{
    public class JobController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<JobController>();

        public JobController()
        {

        }

        public ActionResult Index(ManageJobModel model)
        {
            model = GetDefaultFilterModel(model);

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                model.translate_status = -1;
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }
            model.agency_id = GetCurrentAgencyId();

            var filter = new ApiJobModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = model.CurrentPage,
                page_size = model.PageSize,
                status = (model.Status == null) ? -1 : Utils.ConvertToIntFromQuest(model.Status),
                agency_id = GetCurrentAgencyId(),
                staff_id = model.staff_id,
                japanese_level_number = model.japanese_level_number,
                sub_id = model.sub_id,
                translate_status = model.translate_status ?? 0,
                employment_type_id = model.employment_type_id,
                salary_min = Utils.ConvertNumberCommaToInt32(model.salary_min),
                salary_max = Utils.ConvertNumberCommaToInt32(model.salary_max)
            };

            try
            {
                var country_id = 81;
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }

                var result = CompanyServices.GetJobsAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJob>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        model.TotalCount = Utils.ConvertToInt32(result.total);

                        List<int> listCompanyIds = model.SearchResults.Select(x => x.company_id).ToList();
                        if (listCompanyIds.HasData())
                        {
                            var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                            if (companyReturnApi != null && companyReturnApi.value != null)
                                model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                        }
                    }
                }

                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.Industries = CommonHelpers.GetListIndustries();
                model.SubFields = CommonHelpers.GetListSubFields();
                //model.Fields = CommonHelpers.GetListFields();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.SubFields = CommonHelpers.GetListSubFields();

                //if (model.employment_type_id == 0 && model.EmploymentTypes.HasData())
                //{
                //    model.employment_type_id = model.EmploymentTypes[0].id;
                //}
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(model.employment_type_id);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        public ActionResult SearchRender()
        {
            ManageJobModel model = new ManageJobModel();
            try
            {
                var country_id = 81;
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.Industries = CommonHelpers.GetListIndustries();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                //model.Fields = CommonHelpers.GetListFields();
                model.SubFields = CommonHelpers.GetListSubFields();

                //if (model.employment_type_id == 0 && model.EmploymentTypes.HasData())
                //{
                //    model.employment_type_id = model.EmploymentTypes[0].id;
                //}
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(model.employment_type_id);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get PopupSearch: " + ex.ToString());
            }

            return PartialView("Partials/_PopupSearch", model);
        }
        public ActionResult ChooseCompanyForNewJob()
        {
            try
            {
                var staffId = GetCurrentStaffId();
                var model = new ApiCompanyModel
                {
                    agency_id = staffId,
                    page_index = 1,
                    page_size = 1000
                };

                //var listDataCompany = AgencyServices.GetCompaniesAsync(model).Result;
                //var listCompany = JsonConvert.DeserializeObject<List<IdentityCompany>>(listDataCompany.value.ToString());

                var managingIdsResult = AgencyCompanyServices.GetListCompanysByIdsAsync(model.agency_id).Result;
                if (managingIdsResult.value != null)
                {
                    List<int> managingIds = JsonConvert.DeserializeObject<List<int>>(managingIdsResult.value.ToString());
                    if (managingIds.HasData())
                    {
                        var companiesResult = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = managingIds }).Result;

                        if (companiesResult.value != null)
                        {
                            List<IdentityCompany> companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companiesResult.value.ToString());

                            return PartialView("Partials/_ChooseCompanyForNewJob", companies);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to show ChooseCompanyForNewJob because: " + ex.ToString());
            }
            return PartialView("Partials/_ChooseCompanyForNewJob", null);
        }

        //[IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult Create()
        {
            var model = new JobEditModel();
            try
            {
                var agencyId = GetCurrentAgencyId();
                var managingIdsResult = AgencyCompanyServices.GetListCompanysByIdsAsync(agencyId).Result;
                if (managingIdsResult.value != null)
                {
                    List<int> managingIds = JsonConvert.DeserializeObject<List<int>>(managingIdsResult.value.ToString());
                    if (managingIds.HasData())
                    {
                        var companiesResult = CompanyServices.GetListActiveByIdsAsync(new ApiGetListByIdsModel { ListIds = managingIds }).Result;

                        if (companiesResult.value != null)
                        {
                            List<IdentityCompany> companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companiesResult.value.ToString());

                            model.Companies = companies;
                        }
                    }
                }
                if (model.Companies.HasData()) model.company_id = model.Companies[0].id;

                model.closed_time = DateTime.Now.AddDays(30).ToString("yyyy/MM/dd");
                var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = model.company_id }).Result;
                if (companyResult != null)
                {
                    if (companyResult.value != null)
                    {
                        var companyName = string.Empty;
                        IdentityCompany companyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());
                        if (companyInfo != null)
                        {
                            if (companyInfo.agency_id != GetCurrentAgencyId())
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
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.agency_id = GetCurrentStaffId();

                var employment_type = 0;
                if (model.EmploymentTypes.HasData())
                {
                    employment_type = model.EmploymentTypes[0].id;
                }
                model.Fields = CommonHelpers.GetListFieldsByEmploymentType(employment_type);

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
            int job_id = 0;
            try
            {
                //Begin create
                var apiJobModel = ExtractFormData(model);

                var apiReturned = JobServices.CreateAsync(apiJobModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        job_id = Utils.ConvertToInt32(apiReturned.value);
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
            string tk = SecurityHelper.GenerateUrlToken("Job", "Detail", new { id = job_id, company_id = model.company_id });

            return RedirectToAction("Detail", "Job", new { id = job_id, company_id = model.company_id, tk = tk });
        }

        [IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult Edit(int? id, string language_code)
        {
            ModelState.Clear();
            var jobId = Utils.ConvertToIntFromQuest(id);
            var model = new JobEditModel();
            try
            {
                if (jobId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var apiModel = new ApiJobModel();
                apiModel.id = jobId;
                apiModel.language_code = language_code;

                var result = JobServices.GetDetailAsync(apiModel).Result;
                IdentityJob info = null;
                if (result != null && result.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityJob>(result.value.ToString());
                    model = ParseDataToForm(info, language_code);
                }

                model.id = jobId;

                var agencyId = GetCurrentAgencyId();
                var managingIdsResult = AgencyCompanyServices.GetListCompanysByIdsAsync(agencyId).Result;
                if (managingIdsResult.value != null)
                {
                    List<int> managingIds = JsonConvert.DeserializeObject<List<int>>(managingIdsResult.value.ToString());
                    if (managingIds.HasData())
                    {
                        var companiesResult = CompanyServices.GetListActiveByIdsAsync(new ApiGetListByIdsModel { ListIds = managingIds }).Result;

                        if (companiesResult.value != null)
                        {
                            List<IdentityCompany> companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companiesResult.value.ToString());

                            model.Companies = companies;
                        }
                    }
                }

                if (info != null)
                {
                    model.company_id = info.company_id;
                    if (info.company_id != model.company_id)
                    {
                        return RedirectToErrorPage();
                    }

                    model.Countries = CommonHelpers.GetListCountries();
                    model.Industries = CommonHelpers.GetListIndustries();
                    model.Qualifications = CommonHelpers.GetListQualifications();

                    //model.TagIds = info.Tags;
                    model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                    model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();

                    var employment_type = 0;
                    if (model.sub_field_id != 0)
                    {
                        employment_type = model.employment_type_id;
                    }
                    else if (model.EmploymentTypes.HasData())
                    {
                        employment_type = model.EmploymentTypes[0].id;
                    }
                    model.Fields = CommonHelpers.GetListFieldsByEmploymentType(employment_type);

                    model.agency_id = agencyId;

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
                            var listResult = StationServices.GetListByCityIdAsync(add.city_id).Result;
                            if (listResult.value != null)
                            {
                                add.ListStations = JsonConvert.DeserializeObject<List<IdentityStation>>(listResult.value.ToString());
                            }
                            //var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                            //if (currentStationIds.HasData())
                            //{
                            //    stationIds.AddRange(currentStationIds);
                            //}
                        }

                        //model.Stations = CommonHelpers.GetListStations(stationIds);
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

        [IsValidURLRequest]
        public ActionResult Detail(int? id, int? company_id, string language_code = "", string tab = "")
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
                apiModel.language_code = language_code;

                var result = JobServices.GetDetailAsync(apiModel).Result;
                IdentityJob info = null;

                info = result.ConvertData<IdentityJob>();
                if (info != null)
                    model = ParseDataToForm(info, language_code);

                model.id = jobId;
                model.company_id = companyId;
                model.tab = tab;
                var staffId = GetCurrentStaffId();

                if (info.company_id > 0)
                {
                    var companyResult = CompanyServices.GetDetailAsync(new ApiCompanyModel { id = info.company_id }).Result;
                    model.CompanyInfo = companyResult.ConvertData<IdentityCompany>();
                    model.company_id = info.company_id;
                }
                //var managingIdsResult = AgencyCompanyServices.GetListCompanysByIdsAsync(staffId).Result;
                //if (managingIdsResult.value != null)
                //{
                //    List<int> managingIds = JsonConvert.DeserializeObject<List<int>>(managingIdsResult.value.ToString());
                //    if (managingIds.HasData())
                //    {
                //        var companiesResult = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = managingIds }).Result;

                //        if (companiesResult.value != null)
                //        {
                //            List<IdentityCompany> companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companiesResult.value.ToString());

                //            model.Companies = companies;
                //        }
                //    }
                //}

                if (info != null)
                {
                    //if (info.company_id != model.company_id)
                    //{
                    //    return RedirectToErrorPage();
                    //}

                    model.Countries = CommonHelpers.GetListCountries();
                    model.Industries = CommonHelpers.GetListIndustries();
                    model.Qualifications = CommonHelpers.GetListQualifications();

                    //model.TagIds = info.Tags;
                    model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                    model.Fields = CommonHelpers.GetListFields();
                    model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                    model.agency_id = GetCurrentAgencyId();

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
                            var apiInputModel = new ApiGetListByIdsModel();
                            apiInputModel.ListIds = add.Stations.Select(s => s.station_id).ToList();
                            var listResult = StationServices.GetListAsync(apiInputModel).Result;
                            if (listResult.value != null)
                            {
                                add.ListStations = JsonConvert.DeserializeObject<List<IdentityStation>>(listResult.value.ToString());
                            }
                            //var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                            //if (currentStationIds.HasData())
                            //{
                            //    stationIds.AddRange(currentStationIds);
                            //}
                        }

                        //model.Stations = CommonHelpers.GetListStations(stationIds);
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

                logger.Error(string.Format("Failed to display ViewJob [{0}] because: {1}", jobId, ex.ToString()));
            }
            model.IsViewed = true;

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateJob(JobEditModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

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
                            var status_return = Utils.ConvertToInt32(apiReturned.value);
                            if (status_return == (int)EnumJobStatus.Saved)
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
                            else
                            {
                                return Json(new { success = false, message = string.Format(ManagerResource.ERROR_JOB_UPDATE, GetNameStatus(status_return)), title = ManagerResource.LB_NOTIFICATION });
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

        private string GetNameStatus(int status)
        {
            if (status == (int)(EnumJobStatus.Draft))
            {
                return ManagerResource.LB_AWAITING_APPROVAL;
            }
            else if (status == (int)(EnumJobStatus.Closed))
            {
                return ManagerResource.LB_CLOSED;
            }
            if (status == (int)(EnumJobStatus.Published))
            {
                return ManagerResource.LB_ACTIVE;
            }
            return string.Empty;
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

        [IsValidURLRequest]
        public ActionResult DeleteAllProcesss(int id)
        {
            if (id <= 0)
            {
                return Content(ManagerResource.COMMON_ERROR_DATA_INVALID);
            }

            var model = new JobClearDataModel();

            try
            {
                model.id = id;
                model.tk = SecurityHelper.GenerateUrlToken("Job", "DeleteAllProcesss", new { id = id });
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get DeleteAllProcesss because: " + ex.ToString());

                return RedirectToErrorPage();
            }

            return PartialView("_PopupDeleteAllProcesss", model);
        }

        [HttpPost, ActionName("DeleteAllProcesss")]
        [ValidateAntiForgeryToken]
        [IsValidURLRequest]
        public ActionResult DeleteAllProcesss_Confirm(JobClearDataModel model)
        {
            var strError = string.Empty;

            if (model.id <= 0)
            {
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }

            if (string.IsNullOrEmpty(model.tk))
            {
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }
            else
            {
                var tk = SecurityHelper.GenerateUrlToken("Job", "DeleteAllProcesss", new { id = model.id });
                if (model.tk != tk)
                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }

            try
            {
                var agency_id = GetCurrentAgencyId();
                var result = CandidateServices.DeleteAllProcesssAsync(new ApiCandidateDeleteModel { job_id = model.id, agency_id = agency_id }).Result;

                if (result != null)
                {
                    if (result.status == (int)HttpStatusCode.OK)
                    {
                        if (result.error != null && !string.IsNullOrEmpty(result.error.error_code))
                        {
                            strError = result.error.message;

                            return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Candidate because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload();" });
        }

        //Show popup confirm delete        
        [AccessRoleChecker]
        //[IsValidURLRequest]
        public ActionResult Delete(int id, string tk)
        {
            var tkCheck = SecurityHelper.GenerateUrlToken("Job", "Delete", new { id = id });
            if (id <= 0 || tkCheck != tk)
            {
                return Content(ManagerResource.COMMON_ERROR_DATA_INVALID);
            }

            var model = new JobClearDataModel();

            try
            {
                model.id = Utils.ConvertToInt32(id);
                model.tk = SecurityHelper.GenerateUrlToken("Job", "Delete", new { id = id });

                var apiModel = new ApiJobModel();
                apiModel.id = id;

                var result = JobServices.GetDetailAsync(apiModel).Result;
                IdentityJob info = null;
                if (result != null && result.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityJob>(result.value.ToString());
                    model.JobInfo = info;
                }

                var jResult = JobServices.GetCounterForDeletionAsync(
                      new ApiJobDeleteModel { agency_id = GetCurrentAgencyId(), id = id }
                  ).Result;

                model.Counter = jResult.ConvertData<IdentityJobSeekerCounter>();
                if (model.Counter == null) model.Counter = new IdentityJobSeekerCounter();
            }
            catch
            {
            }

            return PartialView("Partials/_PopupDelete", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(JobClearDataModel model)
        {
            var strError = string.Empty;

            if (model.id <= 0)
            {
                return RedirectToErrorPage();
            }

            if (string.IsNullOrEmpty(model.tk))
            {
                return RedirectToErrorPage();
            }
            else
            {
                var tk = SecurityHelper.GenerateUrlToken("Job", "Delete", new { id = model.id });
                if (model.tk != tk)
                    return RedirectToErrorPage();
            }

            try
            {
                var result = JobServices.DeleteAsync(new ApiJobDeleteModel { id = model.id, agency_id = GetCurrentAgencyId() }).Result;
                if (result != null)
                {
                    if (result.status == (int)HttpStatusCode.OK)
                    {
                        if (result.error != null && !string.IsNullOrEmpty(result.error.error_code))
                        {
                            this.AddNotification(result.error.message, NotificationType.ERROR);
                            return View("ClearData", model);
                        }
                        //else
                        //{
                        //    if (!Utils.ConvertToBoolean(result.value))
                        //    {
                        //        this.AddNotification(ManagerResource.COMPANY_DELETE_ERROR, NotificationType.ERROR);
                        //        return View("ClearData", model);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;
                logger.Error("Failed to get Delete Job because: " + ex.ToString());
                this.AddNotification(ManagerResource.COMPANY_DELETE_ERROR, NotificationType.ERROR);
            }

            this.AddNotification(ManagerResource.LB_DELETE_SUCCESS, NotificationType.SUCCESS);

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterDelete()" });
        }

        //Show popup confirm delete        
        [AccessRoleChecker]
        //[IsValidURLRequest]
        public ActionResult Close(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = new JobCloseModel();

            try
            {
                model.id = id;
                model.tk = SecurityHelper.GenerateUrlToken("Job", "Close", new { id = id });
            }
            catch
            {
            }

            return PartialView("Partials/_PopupClose", model);
        }

        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        [IsValidURLRequest]
        public ActionResult Close_Confirm(JobCloseModel model)
        {
            var strError = string.Empty;
            int id = Utils.ConvertToInt32(model.id);

            if (string.IsNullOrEmpty(model.id))
            {
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }

            if (string.IsNullOrEmpty(model.tk))
            {
                return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }
            else
            {
                var tk = SecurityHelper.GenerateUrlToken("Job", "Close", new { id = model.id });
                if (model.tk != tk)
                    return Json(new { success = false, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
            }

            try
            {
                var result = JobServices.CloseAsync(id).Result;
                if (result != null)
                {

                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Close Jon because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_UPDATE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "AfterClosed()" });
        }

        [AccessRoleChecker]
        public ActionResult JobChoosenJobSeeker(JobSeekerChoosenModel model)
        {
            //int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;
            //if (model.status != (int)EnumJobStatus.Published)
            //{
            //    model.type_job_seeker = 1;
            //}
            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.gender = -1;
                model.major_id = -1;
                model.SearchExec = "Y";
                model.CurrentPage = 1;
                if (!ModelState.IsValid)
                    ModelState.Clear();
            }

            //if (Request["Page"] != null)
            //    currentPage = Utils.ConvertToInt32(Request["Page"], 1);

            //model.CurrentPage = currentPage;

            try
            {
                var filter = new ApiJobSeekerByPageModel
                {
                    keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    page_index = model.CurrentPage,
                    page_size = pageSize,
                    agency_id = GetCurrentAgencyId(),
                    type_job_seeker = model.type_job_seeker,
                    staff_id = 0,
                    gender = -1,
                    major_id = -1,
                    country_id = 0,
                    //gender = model.gender,
                    //major_id = model.major_id,
                    //country_id = model.country_id,
                    job_id = model.job_id
                };

                var result = A_JobSeekerServices.GetListAssignmenWorkByPageAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        var listPrefectureIds = new List<int>();
                        foreach (var item in model.SearchResults)
                        {
                            if (item.Addresses.HasData())
                            {
                                var currentPrefectId = item.Addresses[0].prefecture_id;
                                if (currentPrefectId > 0)
                                    listPrefectureIds.Add(currentPrefectId);
                            }
                        }
                        if (listPrefectureIds.HasData())
                        {
                            model.Prefectures = CommonHelpers.GetListPrefectures(listPrefectureIds);
                        }

                        model.ListStaffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        model.PageSize = pageSize;
                    }
                }
                model.Majors = CommonHelpers.GetListMajors();
                model.Countries = CommonHelpers.GetListCountries();
                model.JapanseLevels = CommonHelpers.GetListJapaneseLevels();
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.ListStaffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.pic_id = GetCurrentStaffId();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }
            return PartialView("~/Views/JobSeeker/Partials/ForIntroduce/_ChoosenJobSeeker.cshtml", model);
        }
        [HttpPost]
        public ActionResult JobChoosenJobSeekerSearch(JobSeekerChoosenModel model)
        {
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.gender = -1;
                model.major_id = -1;
                model.SearchExec = "Y";
                model.CurrentPage = 1;
                if (!ModelState.IsValid)
                    ModelState.Clear();
            }

            //if (Request["CurrentPage"] != null)
            //    currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);

            try
            {
                var filter = new ApiJobSeekerByPageModel
                {
                    keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    page_index = model.CurrentPage,
                    page_size = pageSize,
                    agency_id = GetCurrentAgencyId(),
                    type_job_seeker = model.type_job_seeker,
                    staff_id = 0,
                    gender = -1,
                    major_id = -1,
                    country_id = 0,
                    job_id = model.job_id
                };
                var result = A_JobSeekerServices.GetListAssignmenWorkByPageAsync(filter).Result;
                if (result != null && result.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(result.value.ToString());
                    if (model.SearchResults.HasData())
                    {
                        //var listPrefectureIds = new List<int>();
                        //foreach (var item in model.SearchResults)
                        //{
                        //    if (item.Addresses.HasData())
                        //    {
                        //        var currentPrefectId = item.Addresses[0].prefecture_id;
                        //        if (currentPrefectId > 0)
                        //            listPrefectureIds.Add(currentPrefectId);
                        //    }
                        //}
                        //if (listPrefectureIds.HasData())
                        //{
                        //    model.Prefectures = CommonHelpers.GetListPrefectures(listPrefectureIds);
                        //}

                        //model.ListStaffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                        model.TotalCount = Utils.ConvertToInt32(result.total);
                        model.PageSize = pageSize;
                    }
                }
                model.Majors = CommonHelpers.GetListMajors();
                model.Countries = CommonHelpers.GetListCountries();
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }
            return PartialView("~/Views/JobSeeker/Partials/ForIntroduce/_ChoosenJobSeekerList.cshtml", model);
        }
        public ActionResult JobSeekerInvite(int? job_id)
        {
            JobSeekerInviteModel model = new JobSeekerInviteModel();

            try
            {
                model.job_id = Utils.ConvertToInt32(job_id);
                var agency_id = GetCurrentAgencyId();
                var apiReturn = JobServices.GetDetailAsync(new ApiJobModel { id = model.job_id }).Result;
                if (apiReturn != null && apiReturn.value != null)
                {
                    IdentityJob info = JsonConvert.DeserializeObject<IdentityJob>(apiReturn.value.ToString());
                    if (info != null)
                    {
                        if (info.Job_translations.HasData())
                        {
                            model.job_name = info.Job_translations.Where(x => x.language_code == _currentLanguage).Select(x => x.title).FirstOrDefault();
                        }

                        var inviteApi = new ApiJobInvitationModel();
                        inviteApi.job_id = info.id;
                        inviteApi.agency_id = agency_id;

                        var checkingResult = JobServices.InvitationCheckingAsync(inviteApi).Result;
                        if (checkingResult != null && checkingResult.value != null)
                        {
                            var resultObj = JsonConvert.DeserializeObject<dynamic>(checkingResult.value.ToString());
                            if (resultObj != null)
                            {
                                if (resultObj.invitation_limit != null)
                                    model.invitation_limit = Utils.ConvertToInt32(resultObj.invitation_limit);
                                else
                                    model.invitation_limit = 20;

                                if (resultObj.total_count != null)
                                    model.invited_count = Utils.ConvertToInt32(resultObj.total_count);

                                if (model.invited_count >= model.invitation_limit)
                                {
                                    return PartialView("Partials/_JobSeekerInviteLimited", model);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)

            {
                logger.Error("Failed for Show JobSeekerInvite form: " + ex.ToString());
            }

            return PartialView("Partials/_JobSeekerInvite", model);
        }

        public ActionResult Language(int? id, int? company_id, int? status, int? translate_status)
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

                var result = JobServices.Admin_GetDetailAsync(apiModel).Result;
                IdentityJob info = null;
                if (result != null && result.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityJob>(result.value.ToString());
                    model = ParseDataToFormLanguage(info, "ja-JP", "vi-VN");
                }
                model.id = jobId;
                model.company_id = companyId;
                model.status = status ?? -1;
                model.translate_status = translate_status ?? 0;
                model.filter_translate_status = translate_status ?? -1;
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

                logger.Error("Failed to exec UpdateLanguage because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = ManagerResource.LB_NOTIFICATION });
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
            apiModel.language_code = "vi-VN";
            apiModel.job_id = model.id;
            apiModel.translate_status = model.translate_status;
            apiModel.staff_id = GetCurrentStaffId();
            return apiModel;
        }

        private JobEditLanguageModel ParseDataToFormLanguage(IdentityJob info, string from_language, string to_language)
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
                    if (checkLang == null)
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

        [HttpPost]
        [ActionName("JobSeekerInvite")]
        [ValidateAntiForgeryToken]
        public ActionResult JobSeekerInvite_Confirm(JobSeekerInviteModel model)
        {
            var strError = string.Empty;
            try
            {
                var apiModel = new ApiJobInvitationModel();
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.staff_id = GetCurrentStaffId();
                apiModel.job_id = model.job_id;
                if (!string.IsNullOrEmpty(model.applicant))
                {
                    apiModel.JobSeekers = model.applicant.Split(',').Select(Int32.Parse).ToList();
                    if (apiModel.JobSeekers.Count > model.invitation_limit)
                    {
                        return Json(new { success = false, message = ManagerResource.ERROR_INVITATION_OVER, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = ManagerResource.ERROR_ALEAST_ONE_APPLICANT_SELECT, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
                }
                apiModel.note = model.note;

                var apiResult = JobServices.ApplicationInviteAsync(apiModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.status == (int)HttpStatusCode.OK)
                    {
                        if (apiResult.error != null && !string.IsNullOrEmpty(apiResult.error.error_code))
                        {
                            strError = apiResult.error.message;

                            return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(apiResult.message))
                            {
                                return Json(new { success = true, message = apiResult.message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = ManagerResource.LB_APPLICATION_INVITATION_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to exec JobSeekerInvite because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_APPLICATION_INVITATION_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }

        [IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult CvInvite(int? job_id)
        {
            CvInviteModel model = new CvInviteModel();

            try
            {
                model.job_id = Utils.ConvertToInt32(job_id);
                var agency_id = GetCurrentAgencyId();
                var apiReturn = JobServices.GetDetailAsync(new ApiJobModel { id = model.job_id }).Result;
                if (apiReturn != null && apiReturn.value != null)
                {
                    IdentityJob info = JsonConvert.DeserializeObject<IdentityJob>(apiReturn.value.ToString());
                    if (info != null)
                    {
                        if (info.Job_translations.HasData())
                        {
                            model.job_name = info.Job_translations.Where(x => x.language_code == _currentLanguage).Select(x => x.title).FirstOrDefault();
                        }

                        var inviteApi = new ApiCvInvitationModel();
                        inviteApi.job_id = info.id;
                        inviteApi.agency_id = agency_id;

                        var checkingResult = JobServices.InvitationCheckingAsync(inviteApi).Result;
                        if (checkingResult != null && checkingResult.value != null)
                        {
                            var resultObj = JsonConvert.DeserializeObject<dynamic>(checkingResult.value.ToString());
                            if (resultObj != null)
                            {
                                if (resultObj.invitation_limit != null)
                                    model.invitation_limit = Utils.ConvertToInt32(resultObj.invitation_limit);
                                else
                                    model.invitation_limit = 20;

                                if (resultObj.total_count != null)
                                    model.invited_count = Utils.ConvertToInt32(resultObj.total_count);

                                if (model.invited_count >= model.invitation_limit)
                                {
                                    return PartialView("Partials/_CvInviteLimited", model);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed for Show CvInvite form: " + ex.ToString());
            }

            return PartialView("Partials/_CvInvite", model);
        }

        [HttpPost]
        [ActionName("CvInvite")]
        [ValidateAntiForgeryToken]
        public ActionResult CvInvite_Confirm(CvInviteModel model)
        {
            var strError = string.Empty;
            try
            {
                var apiModel = new ApiCvInvitationModel();
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.staff_id = GetCurrentStaffId();
                apiModel.job_id = model.job_id;
                if (!string.IsNullOrEmpty(model.applicant))
                {
                    apiModel.JobSeekers = new List<ApiCvInvitationItemModel>();
                    List<CvInviteInfoModel> selectedCvs = null;
                    try
                    {
                        selectedCvs = JsonConvert.DeserializeObject<List<CvInviteInfoModel>>(model.applicant);
                    }
                    catch
                    {

                    }

                    if (selectedCvs.HasData())
                    {
                        foreach (var item in selectedCvs)
                        {
                            var inviteInfo = new ApiCvInvitationItemModel();
                            inviteInfo.cv_id = item.id;
                            inviteInfo.job_seeker_id = item.job_seeker_id;

                            apiModel.JobSeekers.Add(inviteInfo);
                        }
                    }

                    if (apiModel.JobSeekers.Count > model.invitation_limit)
                    {
                        return Json(new { success = false, message = ManagerResource.ERROR_INVITATION_OVER, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = ManagerResource.ERROR_ALEAST_ONE_APPLICANT_SELECT, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
                }
                apiModel.note = model.note;

                var apiResult = JobServices.ApplicationInviteAsync(apiModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.status == (int)HttpStatusCode.OK)
                    {
                        if (apiResult.error != null && !string.IsNullOrEmpty(apiResult.error.error_code))
                        {
                            strError = apiResult.error.message;

                            return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(apiResult.message))
                            {
                                return Json(new { success = true, message = apiResult.message, title = ManagerResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = ManagerResource.LB_APPLICATION_INVITATION_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to exec CvInvite because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_APPLICATION_INVITATION_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
        }
        #region Helpers

        private JobEditModel ParseDataToForm(IdentityJob info, string language_code)
        {
            var model = new JobEditModel();
            model.id = info.id;
            model.company_id = info.company_id;
            model.quantity = info.quantity.ToString();
            model.age_min = Utils.ConvertZeroToEmpty(info.age_min);
            model.age_max = Utils.ConvertZeroToEmpty(info.age_max);
            model.salary_min = Utils.ConvertNumberComma(info.salary_min);
            model.salary_max = Utils.ConvertNumberComma(info.salary_max);
            model.salary_type_id = info.salary_type_id;
            model.work_start_time = info.work_start_time.TimeSpanQuestToString();
            model.work_end_time = info.work_end_time.TimeSpanQuestToString();
            model.probation_duration = Utils.ConvertZeroToEmpty(info.probation_duration);
            model.employment_type_id = info.employment_type_id;
            model.flexible_time = info.flexible_time;
            model.language_level = info.language_level;
            model.work_experience_doc_required = info.work_experience_doc_required;
            model.duration = info.duration;
            model.japanese_only = info.japanese_only;
            model.status = info.status;
            model.staff_id = info.staff_id;

            model.created_at = info.created_at;
            if (info.view_company == false)
            {
                model.hide_company_info = true;
            }
            else
            {
                model.hide_company_info = false;
            }

            model.qualification_id = info.qualification_id;
            model.japanese_level_number = info.japanese_level_number;
            model.sub_field_id = info.sub_field_id;
            model.sub_industry_id = info.sub_industry_id;
            model.pic_id = info.pic_id;

            if (info.Job_translations.HasData())
            {
                var myLang = info.Job_translations.Where(x => x.language_code == language_code).FirstOrDefault();

                if (string.IsNullOrEmpty(language_code))
                {
                    myLang = info.Job_translations.Where(x => x.language_code == GetCurrentLanguageOrDefault()).FirstOrDefault();
                    if (myLang == null)
                    {
                        myLang = info.Job_translations[0];
                    }
                }

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
                    model.translate_id = myLang.staff_id;
                    model.language_code = myLang.language_code;
                }
            }

            if (language_code != "")
            {
                model.language_code = language_code;
            }

            if (info.Tags.HasData())
            {
                model.TagIds = info.Tags.Select(x => x.id).ToList();
                model.SelectedTags = info.Tags;
            }
            if (info.closed_time != null)
            {
                model.closed_time = info.closed_time.Value.ToString("yyyy/MM/dd");
            }
            return model;
        }

        private ApiJobUpdateModel ExtractFormData(JobEditModel model)
        {
            var apiModel = new ApiJobUpdateModel();
            apiModel.job = new ApiJobUpdateInfoModel();

            apiModel.job.id = model.id;
            apiModel.job.staff_id = GetCurrentStaffId();
            apiModel.job.company_id = model.company_id;
            apiModel.job.quantity = Utils.ConvertToInt32(model.quantity);
            apiModel.job.age_min = Utils.ConvertToInt32(model.age_min);
            apiModel.job.age_max = Utils.ConvertToInt32(model.age_max);
            apiModel.job.salary_min = Utils.ConvertNumberCommaToInt32(model.salary_min);
            apiModel.job.salary_max = Utils.ConvertNumberCommaToInt32(model.salary_max);
            apiModel.job.salary_type_id = model.salary_type_id;
            apiModel.job.work_start_time = model.work_start_time;
            apiModel.job.work_end_time = model.work_end_time;
            apiModel.job.probation_duration = Utils.ConvertToInt32(model.probation_duration);
            apiModel.job.employment_type_id = model.employment_type_id;
            apiModel.job.flexible_time = model.flexible_time;
            apiModel.job.language_level = model.language_level;
            apiModel.job.work_experience_doc_required = model.work_experience_doc_required;
            apiModel.job.duration = model.duration;
            apiModel.job.japanese_only = model.japanese_only;
            apiModel.job.language_code = model.language_code;

            if (string.IsNullOrEmpty(model.language_code))
                apiModel.job.language_code = GetCurrentLanguageOrDefault();
            if (model.hide_company_info == false)
            {
                apiModel.job.view_company = true;
            }
            else
            {
                apiModel.job.view_company = false;
            }
            apiModel.job.qualification_id = model.qualification_id;
            apiModel.job.japanese_level_number = model.japanese_level_number;
            apiModel.job.sub_field_id = model.sub_field_id;
            apiModel.job.sub_industry_id = model.sub_industry_id;
            apiModel.job.duration = model.duration;
            apiModel.job.pic_id = model.pic_id;
            apiModel.job.status = model.status;
            if (!string.IsNullOrEmpty(model.closed_time))
            {
                apiModel.job.closed_time = DateTime.ParseExact(model.closed_time + " 14:59", "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);
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
                    addressInfo.train_line_id = item.train_line_id;

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