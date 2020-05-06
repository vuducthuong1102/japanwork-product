using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using MySite.Helpers;
using System.Threading.Tasks;
using MySite.ShareLibs;
using MySite.Attributes;
using MySite.Services;
using MySite.Caching;
using System.Linq;
using System.Net;
using ApiJobMarket.DB.Sql.Entities;
using MySite.Resources;
using MySite.Models.Cv;
using Rotativa;

namespace MySite.Controllers
{
    [Authorize]
    public class MemberController : BaseAuthenticatedController
    {
        private readonly ILog logger = LogProvider.For<MemberController>();
        private readonly string currentLang = UserCookieManager.GetCurrentLanguageOrDefault();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        private JobSeekerUpdateProfileModel PreparingMyProfileViewModel()
        {
            var model = new JobSeekerUpdateProfileModel();
            try
            {
                var apiModel = new ApiJobSeekerGetDetailModel();
                apiModel.id = AccountHelper.GetCurrentUserId();

                var apiReturned = JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiReturned.value.ToString());
                        if (info != null)
                        {
                            model = ParseDataToForm(info);
                            model.Countries = CommonHelpers.GetListCountries();
                            model.Regions = CommonHelpers.GetListRegions();
                            model.Qualifications = CommonHelpers.GetListQualifications();
                            model.Visas = CommonHelpers.GetListVisas();

                            if (info.Addresses.HasData())
                            {
                                model.address = info.Addresses.Where(x => x.is_contact_address == true).FirstOrDefault();
                                model.address_contact = info.Addresses.Where(x => x.is_contact_address == false).FirstOrDefault();

                                if (model.address != null)
                                {
                                    if (model.address.country_id == (int)EnumCountry.Japan)
                                    {
                                        if (model.address.train_line_id > 0)
                                        {
                                            var trainLineApiReturn = TrainLineServices.GetDetailAsync(model.address.train_line_id).Result;
                                            if (trainLineApiReturn != null)
                                            {
                                                if (trainLineApiReturn.value != null)
                                                    model.train_line_info = JsonConvert.DeserializeObject<IdentityTrainLine>(trainLineApiReturn.value.ToString());
                                            }
                                        }

                                        if (model.address.station_id > 0)
                                        {
                                            var stationApiReturn = StationServices.GetDetailAsync(model.address.station_id).Result;
                                            if (stationApiReturn != null)
                                            {
                                                if (stationApiReturn.value != null)
                                                    model.station_info = JsonConvert.DeserializeObject<IdentityStation>(stationApiReturn.value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display PreparingMyProfileViewModel because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return model;
        }

        public async Task<ActionResult> MyProfile()
        {
            JobSeekerUpdateProfileModel model = null;
            try
            {
                model = PreparingMyProfileViewModel();

                await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyProfile Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }

        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> MyProfilePartial(int? exclude_ct_add)
        {
            JobSeekerUpdateProfileModel model = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                model = PreparingMyProfileViewModel();
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                if (Request["read_only"] != null)
                {
                    model.read_only = Utils.ConvertToBoolean(Request["read_only"]);
                }

                if (exclude_ct_add != null)
                    model.exclude_ct_add = Utils.ConvertToInt32(exclude_ct_add);

                await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyProfilePartial Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_Profile", model);
        }

        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> MyWish(int? exclude_ct_add)
        {
            JobSeekerWishModel model = new JobSeekerWishModel();

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var apiReturned = JobSeekerWishServices.GetDetailAsync(AccountHelper.GetCurrentUserId()).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        IdentityJobSeekerWish info = JsonConvert.DeserializeObject<IdentityJobSeekerWish>(apiReturned.value.ToString());
                        model = ExtractJobSeekerWishData(info);
                    }
                }

                if (exclude_ct_add != null)
                    model.exclude_ct_add = Utils.ConvertToInt32(exclude_ct_add);
                model.Fields = CommonHelpers.GetListFields();

                var country_id = 81;
                var apiRegion = RegionServices.GetListAsync(country_id).Result;
                if (apiRegion != null)
                {
                    if (apiRegion.value != null)
                    {
                        model.Regions = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiRegion.value.ToString());
                    }
                }

                await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyWish Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_WishItem", model);
        }
        private JobSeekerWishModel ExtractJobSeekerWishData(IdentityJobSeekerWish info)
        {
            JobSeekerWishModel record = new JobSeekerWishModel();
            record.employment_type_ids = FrontendHelpers.ConvertStringToList(info.employment_type_ids);
            record.prefecture_ids = FrontendHelpers.ConvertStringToList(info.prefecture_ids);
            record.sub_field_ids = FrontendHelpers.ConvertStringToList(info.sub_field_ids);
            record.salary_min = info.salary_min;
            record.salary_max = info.salary_max;
            record.start_date = info.start_date.DateTimeQuestToStringNow("dd-MM-yyyy");

            return record;
        }
        public ActionResult Resume()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Resume Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult Wish()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Resume Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult JobAlert()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Job Alert Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult MyCV()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyCV Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult MyCS()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyCS Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        //public ActionResult CreateCV()
        //{
        //    var model = new CvUpdateModel();
        //    try
        //    {
        //        model.created_date = DateTime.Now.ToString(DATE_FORMAT);
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when display CreateCV Page because: {0}", ex.ToString());
        //        logger.Error(strError);
        //    }

        //    return View("Sub/CreateCV", model);
        //}

        public ActionResult ChangePwd()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display ChangePwd Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult Applied()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Applied Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult Invitations()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Invitations Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        public ActionResult InvitedFriends()
        {
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display InvitedFriends Page because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> AppliedJobs()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new ApplicationSearchResultModel();
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
                var apiFilterModel = new ApiCommonFilterModel();
                apiFilterModel.page_index = filter.page_index;
                apiFilterModel.page_size = filter.page_size;
                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await JobSeekerServices.GetAllApplicationsAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.ApplicationList = JsonConvert.DeserializeObject<List<ApplicationInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.ApplicationList.HasData())
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

                            foreach (var item in returnModel.ApplicationList)
                            {
                                if (item.job_info != null)
                                {
                                    if (item.job_info.Addresses.HasData())
                                    {
                                        prefectureIds.AddRange(item.job_info.Addresses.Select(x => x.prefecture_id).ToList());
                                        cityIds.AddRange(item.job_info.Addresses.Select(x => x.city_id).ToList());

                                        foreach (var add in item.job_info.Addresses)
                                        {
                                            stationIds.AddRange(add.Stations.Select(x => x.station_id).ToList());
                                        }
                                    }
                                }
                            }

                            //var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            var allStations = CommonHelpers.GetListStations(stationIds);
                            var hasStations = allStations.HasData();

                            foreach (var item in returnModel.ApplicationList)
                            {
                                if (item.job_info != null)
                                {
                                    var employmentType = listEmployments.Where(x => x.id == item.job_info.employment_type_id).FirstOrDefault();
                                    if (employmentType != null)
                                    {
                                        var typeName = string.Empty;
                                        if (employmentType.LangList.HasData())
                                            typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

                                        if (string.IsNullOrEmpty(typeName))
                                            typeName = employmentType.employment_type;

                                        item.employment_type_label = typeName;
                                        item.employment_type_show_trains = employmentType.show_trains;
                                    }
                                }

                                if (item.job_info.Addresses.HasData())
                                {
                                    foreach (var add in item.job_info.Addresses)
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
                        }

                        htmlReturn = PartialViewAsString("../Member/Sub/_AppliedJobs", returnModel);

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get AppliedJobs because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> GetInvitations()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new InvitationSearchResultModel();
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
                var apiFilterModel = new ApiCommonFilterModel();
                apiFilterModel.page_index = filter.page_index;
                apiFilterModel.page_size = filter.page_size;
                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiFilterModel.status = -99;

                var apiReturned = await JobSeekerServices.GetAllInvitationsAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.InvitationList = JsonConvert.DeserializeObject<List<InvitationInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.InvitationList.HasData())
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

                            foreach (var item in returnModel.InvitationList)
                            {
                                if (item.job_info != null)
                                {
                                    if (item.job_info.Addresses.HasData())
                                    {
                                        prefectureIds.AddRange(item.job_info.Addresses.Select(x => x.prefecture_id).ToList());
                                        cityIds.AddRange(item.job_info.Addresses.Select(x => x.city_id).ToList());

                                        foreach (var add in item.job_info.Addresses)
                                        {
                                            stationIds.AddRange(add.Stations.Select(x => x.station_id).ToList());
                                        }
                                    }
                                }
                            }

                            //var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            var allStations = CommonHelpers.GetListStations(stationIds);
                            var hasStations = allStations.HasData();

                            foreach (var item in returnModel.InvitationList)
                            {
                                if (item.job_info != null)
                                {
                                    var employmentType = listEmployments.Where(x => x.id == item.job_info.employment_type_id).FirstOrDefault();
                                    if (employmentType != null)
                                    {
                                        var typeName = string.Empty;
                                        if (employmentType.LangList.HasData())
                                            typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

                                        if (string.IsNullOrEmpty(typeName))
                                            typeName = employmentType.employment_type;

                                        item.employment_type_label = typeName;
                                        item.employment_type_show_trains = employmentType.show_trains;
                                    }

                                    if (item.job_info.Addresses.HasData())
                                    {
                                        foreach (var add in item.job_info.Addresses)
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
                            }
                        }

                        htmlReturn = PartialViewAsString("../Member/Sub/_InvitationJobs", returnModel);

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get InvitationJobs because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> GetInvitedFriends()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new InvitedFriendSearchResultModel();
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
                var apiFilterModel = new ApiCommonFilterModel();
                apiFilterModel.page_index = filter.page_index;
                apiFilterModel.page_size = filter.page_size;
                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiFilterModel.status = -99;

                var apiReturned = await JobSeekerServices.GetInvitedFriendsAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.InvitedFriends = JsonConvert.DeserializeObject<List<InvitedFriendInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.InvitedFriends.HasData())
                        {
                            returnModel.TotalCount = apiReturned.total;
                            returnModel.CurrentPage = apiFilterModel.page_index;
                            returnModel.PageSize = apiFilterModel.page_size;
                            total = apiReturned.total;
                            var listEmployments = CommonHelpers.GetListEmploymentTypes();

                            foreach (var item in returnModel.InvitedFriends)
                            {
                                if (item.job_info != null)
                                {
                                    var employmentType = listEmployments.Where(x => x.id == item.job_info.employment_type_id).FirstOrDefault();
                                    if (employmentType != null)
                                    {
                                        var typeName = string.Empty;
                                        if (employmentType.LangList.HasData())
                                            typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

                                        if (string.IsNullOrEmpty(typeName))
                                            typeName = employmentType.employment_type;

                                        item.employment_type_label = typeName;
                                        item.employment_type_show_trains = employmentType.show_trains;
                                    }
                                }
                            }
                        }

                        htmlReturn = PartialViewAsString("../Member/Sub/_InvitedFriends", returnModel);

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetInvitedFriends because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        [IsValidURLRequest]
        //[RestrictCopyRequest]
        public async Task<ActionResult> Detail(int? id)
        {
            var model = new ManageCvPreviewModel();
            try
            {
                if (id <= 0)
                    RedirectToErrorPage();

                var cvId = Utils.ConvertToIntFromQuest(id);

                var apiResult = CvServices.GetDetailAsync(cvId).Result;

                await Task.FromResult(apiResult);

                if (apiResult != null)
                {
                    if (apiResult.value != null)
                    {
                        model.CvInfo = JsonConvert.DeserializeObject<IdentityCv>(apiResult.value.ToString());
                        if (model.CvInfo != null)
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            if (qualifications.HasData())
                            {
                                var matchedQualifi = qualifications.Where(x => x.id == model.CvInfo.highest_edu).FirstOrDefault();
                                if (matchedQualifi != null)
                                {
                                    if (matchedQualifi.LangList.HasData())
                                        model.qualification_label = matchedQualifi.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();

                                    if (string.IsNullOrEmpty(model.qualification_label))
                                        model.qualification_label = matchedQualifi.qualification;
                                }
                            }

                            var countryIds = new List<int>();
                            var regionIds = new List<int>();
                            var prefectureIds = new List<int>();
                            var cityIds = new List<int>();
                            var stationIds = new List<int>();

                            if (model.CvInfo.address != null)
                            {
                                if (model.CvInfo.address.country_id > 0)
                                    countryIds.Add(model.CvInfo.address.country_id);

                                if (model.CvInfo.address.region_id > 0)
                                    regionIds.Add(model.CvInfo.address.region_id);

                                if (model.CvInfo.address.prefecture_id > 0)
                                    prefectureIds.Add(model.CvInfo.address.prefecture_id);

                                if (model.CvInfo.address.city_id > 0)
                                    prefectureIds.Add(model.CvInfo.address.city_id);

                                if (model.CvInfo.address.station_id > 0)
                                    stationIds.Add(model.CvInfo.address.station_id);
                            }

                            if (model.CvInfo.address_contact != null)
                            {
                                if (model.CvInfo.address_contact.country_id > 0)
                                    countryIds.Add(model.CvInfo.address_contact.country_id);

                                if (model.CvInfo.address_contact.region_id > 0)
                                    regionIds.Add(model.CvInfo.address_contact.region_id);

                                if (model.CvInfo.address_contact.prefecture_id > 0)
                                    prefectureIds.Add(model.CvInfo.address_contact.prefecture_id);

                                if (model.CvInfo.address_contact.city_id > 0)
                                    prefectureIds.Add(model.CvInfo.address_contact.city_id);

                                if (model.CvInfo.address_contact.station_id > 0)
                                    stationIds.Add(model.CvInfo.address_contact.station_id);
                            }

                            //var allCountries = CommonHelpers.GetListCountries();
                            var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            //var allStations = CommonHelpers.GetListStations(stationIds);

                            if (model.CvInfo.address != null)
                            {
                                model.address = new ManageCvPreviewAddressModel();
                                model.address.AddressInfo = model.CvInfo.address;

                                //model.address.country_name = allCountries.Where(x => x.id == model.CvInfo.address.country_id).Select(x => x.country).FirstOrDefault();
                                if (model.CvInfo.address.region_id > 0)
                                    model.address.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address.region_id).FirstOrDefault();

                                if (model.CvInfo.address.prefecture_id > 0)
                                    model.address.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address.city_id > 0)
                                    model.address.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address.city_id).FirstOrDefault();
                            }

                            if (model.CvInfo.address_contact != null)
                            {
                                model.address_contact = new ManageCvPreviewAddressModel();
                                model.address_contact.AddressInfo = model.CvInfo.address_contact;

                                //model.address_contact.country_name = allCountries.Where(x => x.id == model.CvInfo.address_contact.country_id).Select(x => x.country).FirstOrDefault();
                                if (model.CvInfo.address_contact.region_id > 0)
                                    model.address_contact.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address_contact.region_id).FirstOrDefault();

                                if (model.CvInfo.address_contact.prefecture_id > 0)
                                    model.address_contact.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address_contact.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address_contact.city_id > 0)
                                    model.address_contact.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address_contact.city_id).FirstOrDefault();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to Preview because: " + ex.ToString());

                return View(model);
            }
            if (model.CvInfo.form == 0)
            {
                return PartialView("Partials/_FullTime", model);
            }
            else
            {
                return PartialView("Partials/_PartTime", model);
            }

        }

        [IsValidURLRequest]
        //[RestrictCopyRequest]
        public async Task<ActionResult> Career(int? id)
        {
            var model = new ManageCvPreviewModel();
            try
            {
                if (id <= 0)
                    RedirectToErrorPage();

                var cvId = Utils.ConvertToIntFromQuest(id);

                var apiResult = CvServices.GetDetailAsync(cvId).Result;

                await Task.FromResult(apiResult);

                if (apiResult != null)
                {
                    if (apiResult.value != null)
                    {
                        model.Qualifications = CommonHelpers.GetListQualifications();
                        model.Majors = CommonHelpers.GetListMajors();
                        model.CvInfo = JsonConvert.DeserializeObject<IdentityCv>(apiResult.value.ToString());

                        if (model.CvInfo != null)
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            if (qualifications.HasData())
                            {
                                var matchedQualifi = qualifications.Where(x => x.id == model.CvInfo.highest_edu).FirstOrDefault();
                                if (matchedQualifi != null)
                                {
                                    if (matchedQualifi.LangList.HasData())
                                        model.qualification_label = matchedQualifi.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();

                                    if (string.IsNullOrEmpty(model.qualification_label))
                                        model.qualification_label = matchedQualifi.qualification;
                                }
                            }

                            var countryIds = new List<int>();
                            var regionIds = new List<int>();
                            var prefectureIds = new List<int>();
                            var cityIds = new List<int>();
                            var stationIds = new List<int>();

                            if (model.CvInfo.address != null)
                            {
                                if (model.CvInfo.address.country_id > 0)
                                    countryIds.Add(model.CvInfo.address.country_id);

                                if (model.CvInfo.address.region_id > 0)
                                    regionIds.Add(model.CvInfo.address.region_id);

                                if (model.CvInfo.address.prefecture_id > 0)
                                    prefectureIds.Add(model.CvInfo.address.prefecture_id);

                                if (model.CvInfo.address.city_id > 0)
                                    prefectureIds.Add(model.CvInfo.address.city_id);

                                if (model.CvInfo.address.station_id > 0)
                                    stationIds.Add(model.CvInfo.address.station_id);
                            }

                            if (model.CvInfo.address_contact != null)
                            {
                                if (model.CvInfo.address_contact.country_id > 0)
                                    countryIds.Add(model.CvInfo.address_contact.country_id);

                                if (model.CvInfo.address_contact.region_id > 0)
                                    regionIds.Add(model.CvInfo.address_contact.region_id);

                                if (model.CvInfo.address_contact.prefecture_id > 0)
                                    prefectureIds.Add(model.CvInfo.address_contact.prefecture_id);

                                if (model.CvInfo.address_contact.city_id > 0)
                                    prefectureIds.Add(model.CvInfo.address_contact.city_id);

                                if (model.CvInfo.address_contact.station_id > 0)
                                    stationIds.Add(model.CvInfo.address_contact.station_id);
                            }

                            //var allCountries = CommonHelpers.GetListCountries();
                            var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            //var allStations = CommonHelpers.GetListStations(stationIds);

                            if (model.CvInfo.address != null)
                            {
                                model.address = new ManageCvPreviewAddressModel();
                                model.address.AddressInfo = model.CvInfo.address;

                                //model.address.country_name = allCountries.Where(x => x.id == model.CvInfo.address.country_id).Select(x => x.country).FirstOrDefault();
                                if (model.CvInfo.address.region_id > 0)
                                    model.address.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address.region_id).FirstOrDefault();

                                if (model.CvInfo.address.prefecture_id > 0)
                                    model.address.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address.city_id > 0)
                                    model.address.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address.city_id).FirstOrDefault();
                            }

                            if (model.CvInfo.address_contact != null)
                            {
                                model.address_contact = new ManageCvPreviewAddressModel();
                                model.address_contact.AddressInfo = model.CvInfo.address_contact;

                                //model.address_contact.country_name = allCountries.Where(x => x.id == model.CvInfo.address_contact.country_id).Select(x => x.country).FirstOrDefault();
                                if (model.CvInfo.address_contact.region_id > 0)
                                    model.address_contact.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address_contact.region_id).FirstOrDefault();

                                if (model.CvInfo.address_contact.prefecture_id > 0)
                                    model.address_contact.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address_contact.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address_contact.city_id > 0)
                                    model.address_contact.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address_contact.city_id).FirstOrDefault();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to Preview because: " + ex.ToString());

                return View(model);
            }

            if (model.CvInfo.form == 0)
            {
                return PartialView("Career/_FullTime", model);
            }
            else
            {
                return PartialView("Career/_PartTime", model);
            }

        }

        public ActionResult CancelApplication(JobApplicationModel model)
        {
            return PartialView("Partials/_CancelApplication", model);
        }

        [HttpPost]
        [ActionName("CancelApplication")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmCancelApplication(JobApplicationModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiApplicationCancelModel();
                apiModel.id = model.id;
                apiModel.job_id = model.job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await ApplicationServices.CancelAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_APPLICATION_CANCELED_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec CancelApplication because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(JobSeekerUpdateProfileModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = ExtractFormData(model);
                if (model.image_file_upload != null)
                {
                    var apiUploadReturned = await JobSeekerServices.UploadImageAsync(apiModel, model.image_file_upload);
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiModel.image = images[0].Path;
                            }
                        }
                    }
                }

                var apiReturned = await JobSeekerServices.UpdateProfileAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.MS_UPDATE_PROFILE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateProfile because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateWish(JobSeekerWishModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            try
            {
                var apiModel = ExtractFormData(model);

                var apiReturned = await JobSeekerWishServices.UpdateAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.MS_UPDATE_PROFILE_SUCCESS, title = UserWebResource.LB_NOTIFICATION });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateWish because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #region CV

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> MyCVs()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new CvSearchResultModel();
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
                var apiFilterModel = new ApiGetListByPageModel();
                apiFilterModel.page_index = filter.page_index;
                apiFilterModel.page_size = filter.page_size;
                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await JobSeekerServices.GetAllCVsAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.CvList = JsonConvert.DeserializeObject<List<CvInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.CvList.HasData())
                        {
                            returnModel.TotalCount = apiReturned.total;
                            returnModel.CurrentPage = apiFilterModel.page_index;
                            returnModel.PageSize = apiFilterModel.page_size;
                            total = apiReturned.total;
                        }

                        htmlReturn = PartialViewAsString("../Member/Sub/_CVs", returnModel);

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get MyCVs because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCv(ApiCvModel model)
        {
            return PartialView("Partials/_DeleteCv", model);
        }

        [HttpPost]
        [ActionName("DeleteCv")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCv(ApiCvModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiCvDeleteModel();
                apiModel.cv_id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await CvServices.DeleteAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetMyCVs();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteCv because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult SetMainCv(ApiCvModel model)
        {
            return PartialView("Partials/_SetMainCv", model);
        }

        [HttpPost]
        [ActionName("SetMainCv")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmSetMainCv(ApiCvModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiSetMainCvModel();
                apiModel.cv_id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await CvServices.SetMainCvAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetMyCVs();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmSetMainCv because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult AddCvEduHistory()
        {
            var model = new JobSeekerEduHistoryModel();
            try
            {
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Majors = CommonHelpers.GetListMajors();

                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerEduHistoryModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = EduHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerEduHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerEduHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.school = info.school;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;
                                model.qualification_id = info.qualification_id;
                                model.major_id = info.major_id;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display AddCvEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/CV/_AddCvEduHistory", model);
        }

        [HttpPost]
        [ActionName("AddCvEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmAddCvEduHistory(JobSeekerEduHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiJobSeekerEduHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.school = model.school;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;
                apiModel.qualification_id = model.qualification_id;
                apiModel.major_id = model.major_id;

                if (apiModel.id > 0)
                {
                    var apiReturned = await EduHistoryServices.JobSeekerUpdateAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = false, html = htmlReturn, message = message, title = UserWebResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
                            }
                        }
                    }
                }
                else
                {
                    model.Qualifications = CommonHelpers.GetListQualifications();
                    model.Majors = CommonHelpers.GetListMajors();

                    model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                    model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                    var qualification = model.Qualifications.Where(x => x.id == model.qualification_id).FirstOrDefault();
                    if (qualification != null)
                    {
                        model.qualification_label = qualification.LangList.Where(x => x.language_code == currentLang).Select(x => x.qualification).FirstOrDefault();
                        if (string.IsNullOrEmpty(model.qualification_label))
                            model.qualification_label = qualification.qualification;
                    }

                    var major = model.Majors.Where(x => x.id == model.major_id).FirstOrDefault();
                    if (major != null)
                    {
                        model.major_label = major.LangList.Where(x => x.language_code == currentLang).Select(x => x.major).FirstOrDefault();
                        if (string.IsNullOrEmpty(model.major_label))
                            model.major_label = major.major;
                    }

                    htmlReturn = PartialViewAsString("../Member/Partials/CV/_CvEduHistoryItem", model);
                    return Json(new { success = true, html = htmlReturn });
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmAddCvEduHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult AddCvWorkHistory()
        {
            var model = new JobSeekerWorkHistoryModel();
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display AddCvWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/CV/_AddCvWorkHistory", model);
        }

        [HttpPost]
        [ActionName("AddCvWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmAddCvWorkHistory(JobSeekerWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiJobSeekerWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.company = model.company;
                apiModel.content_work = model.content_work;
                apiModel.form = model.form;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;

                if (apiModel.id > 0)
                {
                    var apiReturned = await WorkHistoryServices.JobSeekerUpdateAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = false, message = message, title = UserWebResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
                            }
                        }
                    }
                }
                else
                {
                    model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                    model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                    htmlReturn = PartialViewAsString("../Member/Partials/CV/_CvWorkHistoryItem", model);
                    return Json(new { success = true, html = htmlReturn });
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmAddCvWorkHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #region CS

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> MyCSs()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var returnModel = new CsSearchResultModel();
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
                var apiFilterModel = new ApiGetListByPageModel();
                apiFilterModel.page_index = filter.page_index;
                apiFilterModel.page_size = filter.page_size;
                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await JobSeekerServices.GetAllCSsAsync(apiFilterModel);
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnModel.CsList = JsonConvert.DeserializeObject<List<CsInfoModel>>(apiReturned.value.ToString());

                        if (returnModel.CsList.HasData())
                        {
                            returnModel.TotalCount = apiReturned.total;
                            returnModel.CurrentPage = apiFilterModel.page_index;
                            returnModel.PageSize = apiFilterModel.page_size;
                            total = apiReturned.total;
                        }

                        htmlReturn = PartialViewAsString("../Member/Sub/_CSs", returnModel);

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get MyCSs because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCs(ApiCsModel model)
        {
            return PartialView("Partials/_DeleteCs", model);
        }

        [HttpPost]
        [ActionName("DeleteCs")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCs(ApiCsModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiCsDeleteModel();
                apiModel.cs_id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await CsServices.DeleteAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetMyCSs();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteCs because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult SetMainCs(ApiCsModel model)
        {
            return PartialView("Partials/_SetMainCs", model);
        }

        [HttpPost]
        [ActionName("SetMainCs")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmSetMainCs(ApiCsModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiSetMainCsModel();
                apiModel.cs_id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await CsServices.SetMainCsAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetMyCSs();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmSetMainCs because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult AddCsEduHistory()
        {
            var model = new JobSeekerEduHistoryModel();
            try
            {
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Majors = CommonHelpers.GetListMajors();

                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerEduHistoryModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = EduHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerEduHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerEduHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.school = info.school;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;
                                model.qualification_id = info.qualification_id;
                                model.major_id = info.major_id;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display AddCsEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/CS/_AddCsEduHistory", model);
        }

        [HttpPost]
        [ActionName("AddCsEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmAddCsEduHistory(JobSeekerEduHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiJobSeekerEduHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.school = model.school;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;
                apiModel.qualification_id = model.qualification_id;
                apiModel.major_id = model.major_id;

                if (apiModel.id > 0)
                {
                    var apiReturned = await EduHistoryServices.JobSeekerUpdateAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = false, html = htmlReturn, message = message, title = UserWebResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
                            }
                        }
                    }
                }
                else
                {
                    model.Qualifications = CommonHelpers.GetListQualifications();
                    model.Majors = CommonHelpers.GetListMajors();

                    model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                    model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                    var qualification = model.Qualifications.Where(x => x.id == model.qualification_id).FirstOrDefault();
                    if (qualification != null)
                    {
                        model.qualification_label = qualification.LangList.Where(x => x.language_code == currentLang).Select(x => x.qualification).FirstOrDefault();
                        if (string.IsNullOrEmpty(model.qualification_label))
                            model.qualification_label = qualification.qualification;
                    }

                    var major = model.Majors.Where(x => x.id == model.major_id).FirstOrDefault();
                    if (major != null)
                    {
                        model.major_label = major.LangList.Where(x => x.language_code == currentLang).Select(x => x.major).FirstOrDefault();
                        if (string.IsNullOrEmpty(model.major_label))
                            model.major_label = major.major;
                    }

                    htmlReturn = PartialViewAsString("../Member/Partials/CS/_CsEduHistoryItem", model);
                    return Json(new { success = true, html = htmlReturn });
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmAddCsEduHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult AddCsWorkHistory()
        {
            var model = new JobSeekerWorkHistoryModel();
            try
            {

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display AddCsWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/CS/_AddCsWorkHistory", model);
        }

        [HttpPost]
        [ActionName("AddCsWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmAddCsWorkHistory(JobSeekerWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiJobSeekerWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.company = model.company;
                apiModel.content_work = model.content_work;
                apiModel.form = model.form;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;

                if (apiModel.id > 0)
                {
                    var apiReturned = await WorkHistoryServices.JobSeekerUpdateAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned.status == (int)HttpStatusCode.OK)
                        {
                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                            {
                                message = apiReturned.error.message;

                                return Json(new { success = false, message = message, title = UserWebResource.LB_NOTIFICATION });
                            }
                            else
                            {
                                return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
                            }
                        }
                    }
                }
                else
                {
                    model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                    model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                    htmlReturn = PartialViewAsString("../Member/Partials/CS/_CsWorkHistoryItem", model);
                    return Json(new { success = true, html = htmlReturn });
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmAddCsWorkHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #region Edu history

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> EduHistories()
        {
            List<JobSeekerEduHistoryModel> myList = null;
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturn = await JobSeekerServices.GetEduHistoryAsync(apiModel);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<JobSeekerEduHistoryModel>>(apiReturn.value.ToString());

                        if (myList.HasData())
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            var majors = CommonHelpers.GetListMajors();
                            foreach (var item in myList)
                            {
                                var qualification = qualifications.Where(x => x.id == item.qualification_id).FirstOrDefault();
                                if (qualification != null)
                                {
                                    item.qualification_label = qualification.LangList.Where(x => x.language_code == currentLang).Select(x => x.qualification).FirstOrDefault();
                                    if (string.IsNullOrEmpty(item.qualification_label))
                                        item.qualification_label = qualification.qualification;
                                }

                                var major = majors.Where(x => x.id == item.major_id).FirstOrDefault();
                                if (major != null)
                                {
                                    item.major_label = major.LangList.Where(x => x.language_code == currentLang).Select(x => x.major).FirstOrDefault();
                                    if (string.IsNullOrEmpty(item.major_label))
                                        item.major_label = major.major;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display EduHistories Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_EduHistories", myList);
        }

        public ActionResult UpdateEduHistory(int id = 0)
        {
            var model = new JobSeekerEduHistoryModel();
            try
            {
                model.id = id;

                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Majors = CommonHelpers.GetListMajors();

                if (model.id > 0)
                {
                    var apiModel = new ApiCvEduHistoryModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = EduHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerEduHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerEduHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.school = info.school;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;
                                model.qualification_id = info.qualification_id;
                                model.major_id = info.major_id;
                                model.major_custom = info.major_custom;
                            }
                        }
                    }
                }
                else
                {
                    model.major_id = -1;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return PartialView("Partials/_UpdateEduHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateEduHistory(JobSeekerEduHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerEduHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.school = model.school;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;
                apiModel.qualification_id = model.qualification_id;
                apiModel.major_id = model.major_id;
                apiModel.major_custom = model.major_custom;

                var apiReturned = await EduHistoryServices.JobSeekerUpdateAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateEduHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult DeleteEduHistory(JobSeekerEduHistoryModel model)
        {
            return PartialView("Partials/_DeleteEduHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteEduHistory(JobSeekerEduHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerEduHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await EduHistoryServices.JobSeekerDeleteAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteEduHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #region Work history

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> WorkHistories()
        {
            List<JobSeekerWorkHistoryModel> myList = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }
            try
            {
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturn = await JobSeekerServices.GetWorkHistoryAsync(apiModel);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<JobSeekerWorkHistoryModel>>(apiReturn.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display WorkHistories Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_WorkHistories", myList);
        }

        public ActionResult UpdateWorkHistory(int id = 0)
        {
            var model = new JobSeekerWorkHistoryModel();
            try
            {
                model.id = id;
                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerWorkHistoryModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = WorkHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerWorkHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerWorkHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.company = info.company;
                                model.content_work = info.content_work;
                                model.form = info.form;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return PartialView("Partials/_UpdateWorkHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateWorkHistory(JobSeekerWorkHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.company = model.company;
                apiModel.content_work = model.content_work;
                apiModel.form = model.form;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;

                var apiReturned = await WorkHistoryServices.JobSeekerUpdateAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateWorkHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult DeleteWorkHistory(JobSeekerWorkHistoryModel model)
        {
            return PartialView("Partials/_DeleteWorkHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteWorkHistory(JobSeekerWorkHistoryModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await WorkHistoryServices.JobSeekerDeleteAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteWorkHistory because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #region Certificate

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> Certificates()
        {
            List<JobSeekerCertificateModel> myList = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }
            try
            {
                var apiModel = new ApiGetListByPageModel();
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturn = await JobSeekerServices.GetCertificateAsync(apiModel);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<JobSeekerCertificateModel>>(apiReturn.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Certificates Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_Certificates", myList);
        }

        public ActionResult UpdateCertificate(int id = 0)
        {
            var model = new JobSeekerCertificateModel();
            try
            {
                model.id = id;
                if (model.id > 0)
                {
                    var apiModel = new ApiJobSeekerCertificateModel();
                    apiModel.id = model.id;
                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = CertificateServices.JobSeekerGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeekerCertificate info = JsonConvert.DeserializeObject<IdentityJobSeekerCertificate>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.name = info.name;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                //model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.point = info.point;
                                model.pass = Utils.ConvertToIntFromQuest(info.pass);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateCertificate because: {0}", ex.ToString());
                logger.Error(strError);
            }
            return PartialView("Partials/_UpdateCertificate", model);
        }

        [HttpPost]
        [ActionName("UpdateCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCertificate(JobSeekerCertificateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerCertificateModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.name = model.name;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                //apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.point = model.point;
                apiModel.pass = Utils.ConvertToIntFromQuest(model.pass);

                var apiReturned = await CertificateServices.JobSeekerUpdateAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetCertificates();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCertificate because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult DeleteCertificate(JobSeekerCertificateModel model)
        {
            return PartialView("Partials/_DeleteCertificate", model);
        }

        [HttpPost]
        [ActionName("DeleteCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCertificate(JobSeekerCertificateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiJobSeekerCertificateModel();
                apiModel.id = model.id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await CertificateServices.JobSeekerDeleteAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetCertificates();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmDeleteCertificate because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        public ActionResult AcceptInvitation(InvitationActionModel model)
        {
            try
            {
                //To do here              
            }
            catch (Exception ex)
            {
                logger.Error("Failed for show AcceptInvitation form: " + ex.ToString());
            }

            return PartialView("Partials/_AcceptInvitation", model);
        }

        [HttpPost]
        [ActionName("AcceptInvitation")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcceptInvitation_Confirm(InvitationActionModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiInvitationActionModel();
                apiModel.id = model.id;
                apiModel.job_id = model.job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await InvitationServices.AcceptAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_INVITATION_ACCEPT_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetInvitations();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec AcceptInvitation because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult IgnoreInvitation(InvitationActionModel model)
        {
            try
            {
                //To do here              
            }
            catch (Exception ex)
            {
                logger.Error("Failed for show IgnoreInvitation form: " + ex.ToString());
            }

            return PartialView("Partials/_IgnoreInvitation", model);
        }

        [HttpPost]
        [ActionName("IgnoreInvitation")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IgnoreInvitation_Confirm(InvitationActionModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiInvitationActionModel();
                apiModel.id = model.id;
                apiModel.job_id = model.job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = await InvitationServices.IgnoreAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_INVITATION_IGNORE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " reload();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec AcceptInvitation because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }
        public ActionResult PrintPDF(int? id)
        {
            var model = new ManageCvPreviewModel();
            try
            {
                if (id <= 0)
                    RedirectToErrorPage();

                var cvId = Utils.ConvertToIntFromQuest(id);

                var apiResult = CvServices.GetDetailAsync(cvId).Result;

                if (apiResult != null)
                {
                    if (apiResult.value != null)
                    {
                        model.CvInfo = JsonConvert.DeserializeObject<IdentityCv>(apiResult.value.ToString());
                        if (model.CvInfo != null)
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            if (qualifications.HasData())
                            {
                                var matchedQualifi = qualifications.Where(x => x.id == model.CvInfo.highest_edu).FirstOrDefault();
                                if (matchedQualifi != null)
                                {
                                    if (matchedQualifi.LangList.HasData())
                                        model.qualification_label = matchedQualifi.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();

                                    if (string.IsNullOrEmpty(model.qualification_label))
                                        model.qualification_label = matchedQualifi.qualification;
                                }
                            }

                            var countryIds = new List<int>();
                            var regionIds = new List<int>();
                            var prefectureIds = new List<int>();
                            var cityIds = new List<int>();
                            var stationIds = new List<int>();

                            if (model.CvInfo.address != null)
                            {
                                if (model.CvInfo.address.country_id > 0)
                                    countryIds.Add(model.CvInfo.address.country_id);

                                if (model.CvInfo.address.region_id > 0)
                                    regionIds.Add(model.CvInfo.address.region_id);

                                if (model.CvInfo.address.prefecture_id > 0)
                                    prefectureIds.Add(model.CvInfo.address.prefecture_id);

                                if (model.CvInfo.address.city_id > 0)
                                    prefectureIds.Add(model.CvInfo.address.city_id);

                                if (model.CvInfo.address.station_id > 0)
                                    stationIds.Add(model.CvInfo.address.station_id);
                            }

                            if (model.CvInfo.address_contact != null)
                            {
                                if (model.CvInfo.address_contact.country_id > 0)
                                    countryIds.Add(model.CvInfo.address_contact.country_id);

                                if (model.CvInfo.address_contact.region_id > 0)
                                    regionIds.Add(model.CvInfo.address_contact.region_id);

                                if (model.CvInfo.address_contact.prefecture_id > 0)
                                    prefectureIds.Add(model.CvInfo.address_contact.prefecture_id);

                                if (model.CvInfo.address_contact.city_id > 0)
                                    prefectureIds.Add(model.CvInfo.address_contact.city_id);

                                if (model.CvInfo.address_contact.station_id > 0)
                                    stationIds.Add(model.CvInfo.address_contact.station_id);
                            }

                            //var allCountries = CommonHelpers.GetListCountries();
                            var allRegions = CommonHelpers.GetListRegions(regionIds);
                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            var allCities = CommonHelpers.GetListCities(cityIds);
                            //var allStations = CommonHelpers.GetListStations(stationIds);

                            if (model.CvInfo.address != null)
                            {
                                model.address = new ManageCvPreviewAddressModel();
                                model.address.AddressInfo = model.CvInfo.address;

                                //model.address.country_name = allCountries.Where(x => x.id == model.CvInfo.address.country_id).Select(x => x.country).FirstOrDefault();
                                if (model.CvInfo.address.region_id > 0)
                                    model.address.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address.region_id).FirstOrDefault();

                                if (model.CvInfo.address.prefecture_id > 0)
                                    model.address.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address.city_id > 0)
                                    model.address.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address.city_id).FirstOrDefault();
                            }

                            if (model.CvInfo.address_contact != null)
                            {
                                model.address_contact = new ManageCvPreviewAddressModel();
                                model.address_contact.AddressInfo = model.CvInfo.address_contact;

                                //model.address_contact.country_name = allCountries.Where(x => x.id == model.CvInfo.address_contact.country_id).Select(x => x.country).FirstOrDefault();
                                if (model.CvInfo.address_contact.region_id > 0)
                                    model.address_contact.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address_contact.region_id).FirstOrDefault();

                                if (model.CvInfo.address_contact.prefecture_id > 0)
                                    model.address_contact.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address_contact.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address_contact.city_id > 0)
                                    model.address_contact.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address_contact.city_id).FirstOrDefault();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to Preview because: " + ex.ToString());
            }
            logger.Debug(model.CvInfo.form.ToString());
            if (model.CvInfo.form == 0)
            {
                return new PartialViewAsPdf("Partials/_FullTime", model)
                {
                    FileName = "CvFullTime.pdf",
                    PageSize = Rotativa.Options.Size.A4,
                };
            }
            else
            {
                return new PartialViewAsPdf("Partials/_PartTime", model)
                {
                    FileName = "CvPartTime.pdf",
                    PageSize = Rotativa.Options.Size.A4,
                };
            }

        }

        [PreventCrossOrigin]
        public async Task<ActionResult> GetNewNotification(int? id = 0)
        {
            var model = new JobSeekerNotificationModel();
            try
            {
                var notifId = Utils.ConvertToIntFromQuest(id);
                if (notifId == 0)
                    return Content("");

                var apiResult = NotificationServices.GetDetailAsync(notifId).Result;

                await Task.FromResult(apiResult);
                if (apiResult != null && apiResult.value != null)
                {
                    model.NotifInfo = JsonConvert.DeserializeObject<IdentityNotification>(apiResult.value.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetNewNotification because: " + ex.ToString());
                return Content("");
            }

            return PartialView("Partials/_MiniNotification", model);
        }

        [IsValidURLRequest]
        [PreventCrossOrigin]
        public async Task<ActionResult> ReadNotif(int? id = 0)
        {
            var model = new JobSeekerNotificationModel();
            try
            {
                var notifId = Utils.ConvertToIntFromQuest(id);
                if (notifId == 0)
                    return RedirectToErrorPage();

                var apiModel = new ApiMarkIsReadNotificationModel();
                apiModel.id = notifId;
                apiModel.user_id = AccountHelper.GetCurrentUserId();
                apiModel.is_read = true;

                IdentityNotification info = null;

                var apiResult = NotificationServices.MarkIsReadAsync(apiModel).Result;
                await Task.FromResult(apiResult);
                if (apiResult != null && apiResult.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityNotification>(apiResult.value.ToString());
                    if (info == null)
                        return RedirectToErrorPage();

                    if (info.target_type == (int)EnumNotifTargetType.Job)
                    {
                        if (info.action_type == (int)EnumNotifActionTypeForJobSeeker.Application_Rejected || info.action_type == (int)EnumNotifActionTypeForJobSeeker.Application_Accepted)
                        {
                            return RedirectToAction("applied", "member");
                        }
                        //if (info.action_type == (int)EnumNotifActionTypeForJobSeeker.Invitation_Received)
                        //{
                        //    return RedirectToAction("invitations", "member");
                        //}
                        return Redirect(SecurityHelper.GenerateSecureLink("Job", "Detail", new { id = info.target_id, is_invitation = 1 }));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to ReadNotification because: " + ex.ToString());
            }

            return Content("");
        }

        [HttpPost]
        [ActionName("ChangePwd")]
        public async Task<ActionResult> ChangePwd(WebAccountChangePasswordModel model)
        {
            //var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            var message = string.Empty;
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
                //if (!this.IsCaptchaValid(""))
                //{
                //    ModelState.AddModelError("CaptchaInputText", captchaMessage);
                //    this.AddNotification(captchaMessage, NotificationType.ERROR);
                //    return View(model);
                //}
                var apiModel = new ApiChangePasswordModel();
                var userInfo = AccountHelper.GetCurrentUser();

                if (userInfo != null)
                {
                    apiModel.UserId = userInfo.Id;
                    apiModel.Token = userInfo.TokenKey;
                    apiModel.NewPwd = Utility.Md5HashingData(model.NewPassword);
                    apiModel.OldPwd1 = Utility.Md5HashingData(model.OldPassword);
                    apiModel.PwdType = PasswordLevelType.Level1;
                    apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

                    var apiUserResult = await AccountServices.ChangePasswordAsync(apiModel);
                    if (apiUserResult != null)
                    {
                        message = apiUserResult.Msg;
                        if (string.IsNullOrEmpty(message))
                            message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                        if (apiUserResult.Code == EnumCommonCode.Success)
                        {
                            this.AddNotification(UserWebResource.COMMON_CHANGEPWD_SUCCESS, NotificationType.SUCCESS);

                            ModelState.Clear();
                            return View();
                        }
                        else
                        {
                            this.AddNotification(message, NotificationType.ERROR);
                        }
                    }
                    else
                    {
                        this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                    }
                }
                else
                {
                    return RedirectToErrorPage();
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to ChangePassword because: {0}", ex.ToString()));
            }

            return View(model);
        }

        public async Task<ActionResult> InviteUsing()
        {
            JobSeekerInviteFriendModel model = new JobSeekerInviteFriendModel();
            try
            {
                model.job_seeker_id = AccountHelper.GetCurrentUserId();

                //model.token = SecurityHelper.GenerateUrlToken("Job", "InviteFriend", new { id = id });

                await Task.FromResult(model.job_seeker_id);
            }
            catch (Exception ex)
            {
                logger.Error("Failed for show InviteUsing form: " + ex.ToString());
            }

            return PartialView("Partials/_InviteUsing", model);
        }

        [HttpPost]
        [ActionName("InviteUsing")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUsing(JobSeekerInviteFriendModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiFriendInvitationModel();
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.Emails = new List<string>();
                apiModel.Emails.Add(model.email);

                apiModel.note = model.note;

                var apiReturned = await JobSeekerServices.InviteFriendUsingSystemAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SENT_SUCCESSFULLY, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetInvitedFriends()" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec InviteUsing because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        [IsValidURLRequest]
        public async Task<ActionResult> InvitationResend(JobSeekerInviteResendModel model)
        {
            try
            {
                var token = Request["tk"] != null ? Request["tk"].ToString() : string.Empty;

                await Task.FromResult(token);
                model.token = token;
            }
            catch (Exception ex)
            {
                logger.Error("Failed for show InvitationResend form: " + ex.ToString());
            }

            return PartialView("Partials/_InvitationResend", model);
        }

        [HttpPost]
        [ActionName("InvitationResend")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InvitationResend_Confirm(JobSeekerInviteResendModel model)
        {
            var message = string.Empty;
            var isSuccess = false;

            try
            {
                var apiModel = new ApiFriendInvitationModel();
                apiModel.invite_id = model.invite_id;
                apiModel.job_id = model.job_id;
                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.Emails = new List<string>();
                apiModel.Emails.Add(model.email);
                apiModel.note = model.note;

                var currentToken = SecurityHelper.GenerateUrlToken("Member", "InvitationResend", new { invite_id = model.invite_id, email = model.email, job_id = model.job_id });

                if (model.token != currentToken)
                {
                    return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_DATA_INVALID, title = UserWebResource.LB_NOTIFICATION });
                }

                var apiReturned = await JobSeekerServices.InvitationResendAsync(apiModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SENT_SUCCESSFULLY, title = UserWebResource.LB_NOTIFICATION, clientcallback = "GetInvitedFriends();" });
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

        private ApiJobSeekerModel ExtractFormData(JobSeekerUpdateProfileModel model)
        {
            var info = new ApiJobSeekerModel();
            info.user_id = AccountHelper.GetCurrentUserId();
            info.job_seeker_id = info.user_id;
            info.email = model.email;
            info.phone = model.phone;
            info.japanese_level_number = model.japanese_level_number;
            info.marriage = model.marriage;
            info.dependent_num = model.dependent_num;
            info.fullname = model.fullname;
            info.fullname_furigana = model.fullname_furigana;
            info.display_name = model.display_name;
            info.image = model.image;
            info.birthday = model.birthday;
            info.gender = model.gender;
            info.id_card = model.id_card;
            info.note = model.note;
            info.video_path = model.video_path;
            info.expected_job_title = model.expected_job_title;
            info.expected_salary_min = model.expected_salary_min;
            info.expected_salary_max = model.expected_salary_max;
            info.work_status = model.work_status;
            info.qualification_id = model.qualification_id;
            info.job_seeking_status_id = model.job_seeking_status_id;
            info.salary_type_id = model.salary_type_id;
            info.nationality_id = model.nationality_id;
            info.visa_id = model.visa_id;
            info.duration_visa = model.duration_visa;
            info.religion = (model.religion == true ? 1 : 0);
            info.religion_detail = model.religion_detail;

            info.Addresses = new List<IdentityJobSeekerAddress>();

            var address = new IdentityJobSeekerAddress();
            address.id = model.address.id;
            address.job_seeker_id = info.user_id;
            address.country_id = model.address.country_id;
            address.region_id = model.address.region_id;
            address.prefecture_id = model.address.prefecture_id;
            address.city_id = model.address.city_id;
            address.postal_code = model.address.postal_code;
            address.detail = model.address.detail;
            address.furigana = model.address.furigana;
            address.train_line_id = model.address.train_line_id;
            address.station_id = model.address.station_id;
            address.is_contact_address = true;

            var address_contact = new IdentityJobSeekerAddress();
            address_contact.id = model.address_contact.id;
            address_contact.job_seeker_id = info.user_id;
            address_contact.country_id = model.address_contact.country_id;
            address_contact.region_id = model.address_contact.region_id;
            address_contact.prefecture_id = model.address_contact.prefecture_id;
            address_contact.city_id = model.address_contact.city_id;
            address_contact.postal_code = model.address_contact.postal_code;
            address_contact.detail = model.address_contact.detail;
            address_contact.furigana = model.address_contact.furigana;
            address_contact.train_line_id = model.address_contact.train_line_id;
            address_contact.station_id = model.address_contact.station_id;

            info.Addresses.Add(address);
            info.Addresses.Add(address_contact);

            return info;
        }

        private ApiJobSeekerWishModel ExtractFormData(JobSeekerWishModel model)
        {
            var info = new ApiJobSeekerWishModel();
            info.job_seeker_id = AccountHelper.GetCurrentUserId();
            if (model.employment_type_ids.HasData())
                info.employment_type_ids = string.Join(",", model.employment_type_ids);
            if (model.prefecture_ids.HasData())
                info.prefecture_ids = string.Join(",", model.prefecture_ids);
            if (model.sub_field_ids.HasData())
                info.sub_field_ids = string.Join(",", model.sub_field_ids);
            info.salary_min = model.salary_min;
            info.salary_max = model.salary_max;
            info.start_date = model.start_date;
            return info;
        }
        private JobSeekerUpdateProfileModel ParseDataToForm(IdentityJobSeeker identity)
        {
            var model = new JobSeekerUpdateProfileModel();

            model.email = identity.email;
            model.phone = identity.phone;
            model.marriage = identity.marriage;
            model.dependent_num = identity.dependent_num;
            model.fullname = identity.fullname;
            model.fullname_furigana = identity.fullname_furigana;
            model.display_name = identity.display_name;
            model.image = identity.image;
            model.birthday = identity.birthday.DateTimeQuestToString(DATE_FORMAT);
            model.gender = identity.gender;
            model.id_card = identity.id_card;
            model.note = identity.note;
            model.video_path = identity.video_path;
            model.expected_job_title = identity.expected_job_title;
            model.expected_salary_min = identity.expected_salary_min;
            model.expected_salary_max = identity.expected_salary_max;
            model.work_status = identity.work_status;
            model.qualification_id = identity.qualification_id;
            model.job_seeking_status_id = identity.job_seeking_status_id;
            model.salary_type_id = identity.salary_type_id;
            model.japanese_level_number = identity.japanese_level_number;
            model.nationality_id = identity.nationality_id;
            model.visa_id = identity.visa_id;
            model.duration_visa = identity.duration_visa.DateTimeQuestToString(DATE_FORMAT);
            model.religion = identity.religion;
            model.religion_detail = identity.religion_detail;

            model.Extensions = identity.Extensions;
            return model;
        }

        #endregion
    }
}
