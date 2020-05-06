using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using MySite.Helpers;
using MySite.Settings;
using System.Threading.Tasks;
using MySite.ShareLibs;
using MySite.Attributes;
using MySite.Services;
using MySite.Caching;
using System.Linq;
using System.Net;
using ApiJobMarket.DB.Sql.Entities;
using MySite.Resources;

namespace MySite.Controllers
{
    public class JobController : BaseController
    {
        private readonly ILog logger = LogProvider.For<JobController>();
        private readonly string currentLang = UserCookieManager.GetCurrentLanguageOrDefault();

        [HttpGet]
        public ActionResult Search(JobSearchModel model)
        {
            if (model.CurrentPage <= 0)
                model.CurrentPage = 1;

            if (string.IsNullOrEmpty(model.sorting_date))
                model.sorting_date = "desc";

            try
            {
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                model.FieldLists = CommonHelpers.GetListFields();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display JobSearch because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> ShowAllResults(JobSearchModel model)
        {
            if (!string.IsNullOrEmpty(model.keyword))
            {
                model.keyword = model.keyword.Trim();
            }
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new JobSearchResultModel();
            var total = 0;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Json(new { success = isSuccess, html = message, }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var apiFilterModel = new ApiJobSearchModel();

                apiFilterModel.page_index = model.CurrentPage;
                apiFilterModel.page_size = model.PageSize;

                if (apiFilterModel.page_size > SystemSettings.DefaultPageSize || apiFilterModel.page_size <= 0)
                    apiFilterModel.page_size = SystemSettings.DefaultPageSize;

                if (apiFilterModel.page_index <= 0)
                    apiFilterModel.page_index = 1;

                apiFilterModel.language_code = UserCookieManager.GetCurrentLanguageOrDefault();
                apiFilterModel.title = model.keyword;
                apiFilterModel.employment_type_id = model.employment_type_id;
                apiFilterModel.sorting_date = model.sorting_date;
                apiFilterModel.japanese_level_number = model.japanese_level_number;
                apiFilterModel.salary_min = model.salary_min;
                apiFilterModel.salary_max = model.salary_max;

                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                if (!string.IsNullOrEmpty(model.sub_industry_ids))
                    apiFilterModel.sub_industry_ids = model.sub_industry_ids.Split(',').Select(Int32.Parse).ToList();

                if (!string.IsNullOrEmpty(model.sub_field_ids))
                    apiFilterModel.sub_field_ids = model.sub_field_ids.Split(',').Select(Int32.Parse).ToList();

                if (!string.IsNullOrEmpty(model.city_ids))
                    apiFilterModel.city_ids = model.city_ids.Split(',').Select(Int32.Parse).ToList();

                if (!string.IsNullOrEmpty(model.station_ids))
                    apiFilterModel.station_ids = model.station_ids.Split(',').Select(Int32.Parse).ToList();

                var hasLoggedIn = User.Identity.IsAuthenticated;
                var apiReturned = await JobServices.SearchJobByPageAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.JobsList = JsonConvert.DeserializeObject<List<JobInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.JobsList.HasData())
                        {
                            returnModel.TotalCount = apiReturned.total;
                            returnModel.CurrentPage = apiFilterModel.page_index;
                            returnModel.PageSize = apiFilterModel.page_size;
                            total = apiReturned.total;

                            //var regionIds = new List<int>();
                            var prefectureIds = new List<int>();
                            var cityIds = new List<int>();
                            var stationIds = new List<int>();
                            var listEmployments = CommonHelpers.GetListEmploymentTypes();

                            foreach (var item in returnModel.JobsList)
                            {
                                item.HasLoggedIn = hasLoggedIn;
                                //var currentRegionIds = item.Addresses.Select(x => x.region_id).ToList();
                                //if (currentRegionIds.HasData())
                                //{
                                //    regionIds.AddRange(currentRegionIds);
                                //}                                

                                if (item.Addresses.HasData())
                                {
                                    var currentPrefectIds = item.Addresses.Select(x => x.prefecture_id).ToList();
                                    if (currentPrefectIds.HasData())
                                    {
                                        prefectureIds.AddRange(currentPrefectIds);
                                    }

                                    var currentCityIds = item.Addresses.Select(x => x.city_id).ToList();
                                    if (currentCityIds.HasData())
                                    {
                                        cityIds.AddRange(currentCityIds);
                                    }

                                    foreach (var add in item.Addresses)
                                    {
                                        var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                                        if (currentStationIds.HasData())
                                        {
                                            stationIds.AddRange(currentStationIds);
                                        }
                                    }
                                }
                            }

                            //var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            var allStations = CommonHelpers.GetListStations(stationIds);
                            var hasStations = allStations.HasData();
                            foreach (var item in returnModel.JobsList)
                            {
                                var employmentType = listEmployments.Where(x => x.id == item.employment_type_id).FirstOrDefault();
                                if (employmentType != null)
                                {
                                    var typeName = string.Empty;
                                    if (employmentType.LangList.HasData())
                                        typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

                                    if (string.IsNullOrEmpty(typeName))
                                        typeName = employmentType.employment_type;

                                    if (employmentType.calculate_by == (int)EnumEmploymentCalculateBy.Month)
                                    {
                                        item.employment_type_calculate_label = string.Format(UserWebResource.FILTER_SALARY_BY_MONTH_FORMAT, "");
                                    }
                                    else
                                    {
                                        item.employment_type_calculate_label = string.Format(UserWebResource.FILTER_SALARY_BY_HOUR_FORMAT, "");
                                    }

                                    item.employment_type_label = typeName;
                                    item.employment_type_show_trains = employmentType.show_trains;
                                }

                                if (item.Addresses.HasData())
                                {
                                    foreach (var add in item.Addresses)
                                    {
                                        //add.region_info = allRegions.Where(x => x.id == add.region_id).FirstOrDefault();
                                        add.prefecture_info = allPrefectures.Where(x => x.id == add.prefecture_id).FirstOrDefault();
                                        add.city_info = allCities.Where(x => x.id == add.city_id).FirstOrDefault();
                                        if (add.Stations.HasData() && item.employment_type_show_trains)
                                        {
                                            foreach (var stat in add.Stations)
                                            {
                                                var matchedStation = allStations.Where(x => x.id == stat.station_id).FirstOrDefault();
                                                if (matchedStation != null)
                                                {
                                                    stat.detail = matchedStation.address;
                                                    stat.furigana = matchedStation.furigana;
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            htmlReturn = PartialViewAsString("../Job/SearchResults", returnModel);
                        }

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying ShowAllResults because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOriginAttribute]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> GetRecent()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new JobSearchResultModel();
            var total = 0;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Json(new { success = isSuccess, html = message, }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var filter = GetFilterConfig();
                var apiFilterModel = new ApiJobGetRecentModel();
                apiFilterModel.page_index = filter.page_index;
                apiFilterModel.page_size = filter.page_size;

                apiFilterModel.company_id = Utils.ConvertToInt32(Request["company_id"]);
                apiFilterModel.ignore_ids = Utils.ConvertToInt32(Request["id"]).ToString();
                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var hasLoggedIn = User.Identity.IsAuthenticated;
                var apiReturned = await JobServices.GetRecentByPageAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.JobsList = JsonConvert.DeserializeObject<List<JobInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.JobsList.HasData())
                        {
                            returnModel.TotalCount = apiReturned.total;
                            returnModel.CurrentPage = apiFilterModel.page_index;
                            returnModel.PageSize = apiFilterModel.page_size;
                            total = apiReturned.total;

                            //var regionIds = new List<int>();
                            var prefectureIds = new List<int>();
                            var cityIds = new List<int>();
                            var stationIds = new List<int>();
                            var listEmployments = CommonHelpers.GetListEmploymentTypes();

                            //foreach (var item in returnModel.JobsList)
                            //{
                            //    var currentPrefectIds = item.Addresses.Select(x => x.prefecture_id).ToList();
                            //    if (currentPrefectIds.HasData())
                            //    {
                            //        prefectureIds.AddRange(currentPrefectIds);
                            //    }

                            //    item.HasLoggedIn = hasLoggedIn;

                            //    var currentCityIds = item.Addresses.Select(x => x.city_id).ToList();
                            //    if (currentCityIds.HasData())
                            //    {
                            //        cityIds.AddRange(currentCityIds);
                            //    }

                            //    if (item.Addresses.HasData())
                            //    {
                            //        foreach (var add in item.Addresses)
                            //        {
                            //            var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                            //            if (currentStationIds.HasData())
                            //            {
                            //                stationIds.AddRange(currentStationIds);
                            //            }
                            //        }
                            //    }
                            //}

                            foreach (var item in returnModel.JobsList)
                            {
                                if (item.Addresses.HasData())
                                {
                                    prefectureIds.AddRange(item.Addresses.Select(x => x.prefecture_id).ToList());
                                    cityIds.AddRange(item.Addresses.Select(x => x.city_id).ToList());

                                    foreach (var add in item.Addresses)
                                    {
                                        stationIds.AddRange(add.Stations.Select(x => x.station_id).ToList());
                                    }
                                }
                            }

                            //var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            var allStations = CommonHelpers.GetListStations(stationIds);
                            var hasStations = allStations.HasData();
                            foreach (var item in returnModel.JobsList)
                            {
                                var employmentType = listEmployments.Where(x => x.id == item.employment_type_id).FirstOrDefault();
                                if (employmentType != null)
                                {
                                    var typeName = string.Empty;
                                    if (employmentType.LangList.HasData())
                                        typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

                                    if (string.IsNullOrEmpty(typeName))
                                        typeName = employmentType.employment_type;

                                    if (employmentType.calculate_by == (int)EnumEmploymentCalculateBy.Month)
                                    {
                                        item.employment_type_calculate_label = string.Format(UserWebResource.FILTER_SALARY_BY_MONTH_FORMAT, "");
                                    }
                                    else
                                    {
                                        item.employment_type_calculate_label = string.Format(UserWebResource.FILTER_SALARY_BY_HOUR_FORMAT, "");
                                    }

                                    item.employment_type_label = typeName;
                                    item.employment_type_show_trains = employmentType.show_trains;
                                }

                                if (item.Addresses.HasData())
                                {
                                    foreach (var add in item.Addresses)
                                    {
                                        //add.region_info = allRegions.Where(x => x.id == add.region_id).FirstOrDefault();
                                        add.prefecture_info = allPrefectures.Where(x => x.id == add.prefecture_id).FirstOrDefault();
                                        add.city_info = allCities.Where(x => x.id == add.city_id).FirstOrDefault();
                                        if (add.Stations.HasData() && item.employment_type_show_trains)
                                        {
                                            foreach (var stat in add.Stations)
                                            {
                                                var matchedStation = allStations.Where(x => x.id == stat.station_id).FirstOrDefault();
                                                if (matchedStation != null)
                                                {
                                                    stat.detail = matchedStation.address;
                                                    stat.furigana = matchedStation.furigana;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            htmlReturn = PartialViewAsString("../Job/RecentJobs", returnModel);
                        }

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetRecent because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        [IsValidURLRequest]
        public async Task<ActionResult> Preview(int? id)
        {
            try
            {
                if (id <= 0)
                    return RedirectToErrorPage();

                var apiInputModel = new ApiJobGetDetailModel();
                apiInputModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiInputModel.id = Utils.ConvertToInt32(id);

                var apiReturned = await JobServices.GetDetailAsync(apiInputModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        JobInfoModel job_info = JsonConvert.DeserializeObject<JobInfoModel>(apiReturned.value.ToString());
                        if (job_info == null)
                            return RedirectToErrorPage();
                        else
                        {
                            var slug = string.Empty;
                            if (job_info.Job_translations.HasData())
                            {
                                slug = job_info.Job_translations[0].friendly_url;

                                if (string.IsNullOrEmpty(slug))
                                {
                                    slug = UrlFriendly.ConvertToUrlFriendly(job_info.Job_translations[0].title);
                                }
                            }

                            var detailLink = SecurityHelper.GenerateSecureLink("job", "detail", new { lr = slug, id = id });

                            return Redirect(detailLink);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Preview because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return RedirectToErrorPage();
        }

        [IsValidURLRequest]
        //[RestrictCopyRequest]
        public async Task<ActionResult> Detail(JobViewDetailModel model)
        {
            try
            {
                if (model.id <= 0)
                    return RedirectToErrorPage();

                var apiInputModel = new ApiJobGetDetailModel();
                apiInputModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiInputModel.id = model.id;

                var apiReturned = await JobServices.GetDetailAsync(apiInputModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.job_info = JsonConvert.DeserializeObject<JobInfoModel>(apiReturned.value.ToString());
                    }
                }

                var apiCheckInviteModel = new ApiJobCheckInviteModel();
                apiCheckInviteModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiCheckInviteModel.id = model.id;
                var apiCheckInvited = await JobServices.CheckInviteAsync(apiCheckInviteModel);
                if (apiCheckInvited != null)
                {
                    if (apiCheckInvited.value != null)
                    {
                        model.is_invitation = Utils.ConvertToInt32(apiCheckInvited.value.ToString());
                    }
                }

                if (model.job_info == null)
                    return RedirectToErrorPage();
                else
                {
                    var prefectureIds = new List<int>();
                    var cityIds = new List<int>();
                    var stationIds = new List<int>();

                    model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();

                    var listEmployments = CommonHelpers.GetListEmploymentTypes();
                    var employmentType = listEmployments.Where(x => x.id == model.job_info.employment_type_id).FirstOrDefault();
                    if (employmentType != null)
                    {
                        var typeName = string.Empty;
                        if (employmentType.LangList.HasData())
                            typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

                        if (string.IsNullOrEmpty(typeName))
                            typeName = employmentType.employment_type;

                        model.job_info.employment_type_label = typeName;
                        model.job_info.employment_type_show_trains = employmentType.show_trains;
                    }

                    var qualifications = CommonHelpers.GetListQualifications();
                    var qualify = qualifications.Where(x => x.id == model.job_info.qualification_id).FirstOrDefault();

                    if (qualify != null)
                    {
                        var qualifyName = string.Empty;
                        if (qualify.LangList.HasData())
                            qualifyName = qualify.LangList.Where(x => x.language_code == currentLang).Select(x => x.qualification).FirstOrDefault();

                        if (string.IsNullOrEmpty(qualifyName))
                            qualifyName = qualify.qualification;

                        model.job_info.qualification_label = qualifyName;
                    }

                    //Get company address info
                    if (model.job_info.company_id > 0)
                    {
                        var companyResult = CompanyServices.GetDetailAsync(model.job_info.company_id).Result;
                        if (companyResult != null && companyResult.value != null)
                            model.CompanyInfo = JsonConvert.DeserializeObject<IdentityCompany>(companyResult.value.ToString());

                        if (model.CompanyInfo != null)
                        {
                            if (model.CompanyInfo.prefecture_id > 0)
                                prefectureIds.Add(model.CompanyInfo.prefecture_id);

                            if (model.CompanyInfo.city_id > 0)
                                cityIds.Add(model.CompanyInfo.city_id);
                        }
                    }


                    var jobHasAddress = (model.job_info.Addresses.HasData());
                    if (jobHasAddress)
                    {
                        var currentPrefectIds = model.job_info.Addresses.Select(x => x.prefecture_id).ToList();
                        if (currentPrefectIds.HasData())
                        {
                            prefectureIds.AddRange(currentPrefectIds);
                        }

                        var currentCityIds = model.job_info.Addresses.Select(x => x.city_id).ToList();
                        if (currentCityIds.HasData())
                        {
                            cityIds.AddRange(currentCityIds);
                        }

                        if (model.job_info.Addresses.HasData())
                        {
                            foreach (var add in model.job_info.Addresses)
                            {
                                var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
                                if (currentStationIds.HasData())
                                {
                                    stationIds.AddRange(currentStationIds);
                                }
                            }
                        }
                    }

                    var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                    var allCities = CommonHelpers.GetListCities(cityIds);
                    var allStations = CommonHelpers.GetListStations(stationIds);
                    var hasStations = allStations.HasData();

                    if (jobHasAddress)
                    {
                        foreach (var add in model.job_info.Addresses)
                        {
                            //add.region_info = allRegions.Where(x => x.id == add.region_id).FirstOrDefault();
                            add.prefecture_info = allPrefectures.Where(x => x.id == add.prefecture_id).FirstOrDefault();
                            add.city_info = allCities.Where(x => x.id == add.city_id).FirstOrDefault();
                            if (add.Stations.HasData() && model.job_info.employment_type_show_trains)
                            {
                                foreach (var stat in add.Stations)
                                {
                                    var matchedStation = allStations.Where(x => x.id == stat.station_id).FirstOrDefault();
                                    if (matchedStation != null)
                                    {
                                        stat.detail = matchedStation.address;
                                        stat.furigana = matchedStation.furigana;
                                    }
                                }
                            }
                        }
                    }

                    if (model.CompanyInfo != null)
                    {
                        model.CompanyInfo.address_info = new IdentityAddress();
                        model.CompanyInfo.address_info.prefecture_info = allPrefectures.Where(x => x.id == model.CompanyInfo.prefecture_id).FirstOrDefault();
                        model.CompanyInfo.address_info.city_info = allCities.Where(x => x.id == model.CompanyInfo.city_id).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get Detail because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerifyLoggedInUser]
        public ActionResult SaveJob()
        {
            var strError = string.Empty;
            try
            {
                var job_id = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;

                var currentUser = AccountHelper.GetCurrentUser();
                var apiModel = new ApiSaveJobModel();
                apiModel.job_id = job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = JobServices.SaveJobAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.status != (int)HttpStatusCode.OK)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to SaveJob because: " + ex.ToString());
            }

            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerifyLoggedInUser]
        public async Task<ActionResult> UnSaveJob()
        {
            var strError = string.Empty;
            try
            {
                var job_id = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;

                var currentUser = AccountHelper.GetCurrentUser();
                var apiModel = new ApiSaveJobModel();
                apiModel.job_id = job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await JobServices.UnSaveJobAsync(apiModel);
                //if (apiReturned != null)
                //{
                //    if (apiReturned.status != (int)HttpStatusCode.OK)
                //    {

                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error("Failed to UnSaveJob because: " + ex.ToString());
            }

            return Json(new { success = true });
        }

        [IsValidURLRequest]
        public async Task<ActionResult> ApplyJob(int? id, int? is_invitation)
        {
            JobApplyModel model = new JobApplyModel();
            try
            {
                model.id = Utils.ConvertToIntFromQuest(id);
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                model.is_invitation = Utils.ConvertToIntFromQuest(is_invitation);

                model.token = SecurityHelper.GenerateUrlToken("Job", "ApplyJob", new { id = model.id, is_invitation = is_invitation });

                var apiReturned = await JobSeekerServices.GetAllCVsAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.value != null)
                        {
                            model.Cvs = JsonConvert.DeserializeObject<List<IdentityCv>>(apiReturned.value.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed for show ApplyJob form: " + ex.ToString());
            }

            return PartialView("Partials/_ApplyJob", model);
        }

        [HttpPost]
        [ActionName("ApplyJob")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApplyJob(JobApplyModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                if (model.cv_id <= 0)
                {
                    return Json(new { success = isSuccess, message = UserWebResource.ERROR_UNSELECTED_CV, title = UserWebResource.LB_NOTIFICATION, clientcallback="ShowMyModalAgain();" });
                }

                var currentToken = SecurityHelper.GenerateUrlToken("Job", "ApplyJob", new { id = model.id, is_invitation = model.is_invitation });
                if (model.token != currentToken)
                {
                    return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_DATA_INVALID, title = UserWebResource.LB_NOTIFICATION });
                }
                if (model.is_invitation == 0)
                {
                    var apiModel = new ApiJobActionApplyModel();
                    apiModel.job_id = model.id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                    apiModel.cv_id = model.cv_id;

                    var apiReturned = await ApplicationServices.ApplyJobAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = UserWebResource.LB_JOB_APPLIED_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
                            }
                        }
                    }
                }
                else
                {
                    var apiModel = new ApiApplicationSendCvModel();
                    apiModel.job_id = model.id;
                    apiModel.cv_id = model.cv_id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = await ApplicationServices.SendCvAsync(apiModel);

                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = UserWebResource.LB_JOB_APPLIED_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ApplyJob because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        [IsValidURLRequest]
        public async Task<ActionResult> InviteFriend(int? id)
        {
            JobInviteFriendApplyModel model = new JobInviteFriendApplyModel();
            try
            {
                model.job_id = Utils.ConvertToIntFromQuest(id);
                model.job_seeker_id = AccountHelper.GetCurrentUserId();

                model.token = SecurityHelper.GenerateUrlToken("Job", "InviteFriend", new { id = id });

                await Task.FromResult(model.job_seeker_id);
            }
            catch (Exception ex)
            {
                logger.Error("Failed for show InviteFriend form: " + ex.ToString());
            }

            return PartialView("Partials/_InviteFriend", model);
        }

        [HttpPost]
        [ActionName("InviteFriend")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteFriend(JobInviteFriendApplyModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiFriendInvitationModel();
                apiModel.job_id = model.job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.Emails = new List<string>();
                apiModel.Emails.Add(model.email);
                apiModel.note = model.note;

                var currentToken = SecurityHelper.GenerateUrlToken("Job", "InviteFriend", new { id = model.job_id });
                if (model.token != currentToken)
                {
                    return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_DATA_INVALID, title = UserWebResource.LB_NOTIFICATION });
                }

                var apiReturned = await JobSeekerServices.InviteFriendApplyJobAsync(apiModel);
                if (apiReturned != null)
                {
                    if (apiReturned.status == (int)HttpStatusCode.OK)
                    {
                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                        {
                            message = apiReturned.error.message;

                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
                        }
                        else
                        {
                            return Json(new { success = true, message = UserWebResource.LB_SENT_SUCCESSFULLY, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec InviteFriend because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #region Helpers


        #endregion
    }
}
