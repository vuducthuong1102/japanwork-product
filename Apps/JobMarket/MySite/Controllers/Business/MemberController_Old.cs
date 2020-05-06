//using System.Web.Mvc;
//using MySite.Logging;
//using MySite.Models;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using System;
//using MySite.Helpers;
//using MySite.Settings;
//using System.Threading.Tasks;
//using MySite.ShareLibs;
//using MySite.Attributes;
//using MySite.Services;
//using MySite.Caching;
//using System.Linq;
//using System.Net;
//using ApiJobMarket.DB.Sql.Entities;
//using MySite.Resources;
//using System.Web;
//using MySite.ShareLibs.Extensions;
//using System.Drawing.Imaging;

//namespace MySite.Controllers
//{
//    public class MemberController_Old : BaseAuthenticatedController
//    {
//        private readonly ILog logger = LogProvider.For<MemberController>();
//        private readonly string currentLang = UserCookieManager.GetCurrentLanguageOrDefault();

//        private JobSeekerUpdateProfileModel PreparingMyProfileViewModel()
//        {
//            var model = new JobSeekerUpdateProfileModel();
//            try
//            {
//                var apiModel = new ApiJobSeekerGetDetailModel();
//                apiModel.id = AccountHelper.GetCurrentUserId();

//                var apiReturned = JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
//                if (apiReturned != null)
//                {
//                    if (apiReturned.value != null)
//                    {
//                        IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiReturned.value.ToString());
//                        if (info != null)
//                        {
//                            model = ParseDataToForm(info);
//                            model.Countries = CommonHelpers.GetListCountries();
//                            model.Regions = CommonHelpers.GetListRegions();
//                            model.Qualifications = CommonHelpers.GetListQualifications();

//                            if (info.Addresses.HasData())
//                            {
//                                model.address = info.Addresses.Where(x => x.is_contact_address == true).FirstOrDefault();
//                                model.address_contact = info.Addresses.Where(x => x.is_contact_address == false).FirstOrDefault();

//                                if (model.address != null)
//                                {
//                                    if (model.address.country_id == (int)EnumCountry.Japan)
//                                    {
//                                        if (model.address.train_line_id > 0)
//                                        {
//                                            var trainLineApiReturn = TrainLineServices.GetDetailAsync(model.address.train_line_id).Result;
//                                            if (trainLineApiReturn != null)
//                                            {
//                                                if (trainLineApiReturn.value != null)
//                                                    model.train_line_info = JsonConvert.DeserializeObject<IdentityTrainLine>(trainLineApiReturn.value.ToString());
//                                            }
//                                        }

//                                        if (model.address.station_id > 0)
//                                        {
//                                            var stationApiReturn = StationServices.GetDetailAsync(model.address.station_id).Result;
//                                            if (stationApiReturn != null)
//                                            {
//                                                if (stationApiReturn.value != null)
//                                                    model.station_info = JsonConvert.DeserializeObject<IdentityStation>(stationApiReturn.value.ToString());
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display PreparingMyProfileViewModel because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return model;
//        }

//        public async Task<ActionResult> MyProfile()
//        {
//            JobSeekerUpdateProfileModel model = null;
//            try
//            {
//                model = PreparingMyProfileViewModel();

//                await Task.FromResult(model);
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display MyProfile Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return View(model);
//        }

//        [PreventSpam(DelayRequest = 1)]
//        public async Task<ActionResult> MyProfilePartial()
//        {
//            JobSeekerUpdateProfileModel model = null;

//            if (!ModelState.IsValid)
//            {
//                var message = string.Join(" | ", ModelState.Values
//                    .SelectMany(v => v.Errors)
//                    .Select(e => e.ErrorMessage));
//                return Content(message);
//            }

//            try
//            {
//                model = PreparingMyProfileViewModel();

//                await Task.FromResult(model);
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display MyProfilePartial Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("Sub/_Profile", model);
//        }

//        public ActionResult Resume()
//        {
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display Resume Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return View();
//        }

//        public ActionResult JobAlert()
//        {
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display Job Alert Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return View();
//        }

//        public ActionResult MyCV()
//        {
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display MyCV Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return View();
//        }

//        public ActionResult CreateCV()
//        {
//            var model = new CvUpdateModel();
//            try
//            {
//                model.created_date = DateTime.Now.ToString("dd-MM-yyyy");
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display CreateCV Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return View("Sub/CreateCV", model);
//        }

//        public ActionResult ChangePwd()
//        {
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display ChangePwd Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return View();
//        }

//        public ActionResult Applied()
//        {
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display Applied Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [PreventCrossOrigin]
//        [PreventSpam(DelayRequest = 1)]
//        public async Task<ActionResult> AppliedJobs()
//        {
//            var isSuccess = false;
//            var htmlReturn = string.Empty;
//            var returnModel = new ApplicationSearchResultModel();
//            var total = 0;

//            if (!ModelState.IsValid)
//            {
//                var message = string.Join(" | ", ModelState.Values
//                    .SelectMany(v => v.Errors)
//                    .Select(e => e.ErrorMessage));
//                return Json(new { success = isSuccess, html = message, }, JsonRequestBehavior.AllowGet);
//            }

//            try
//            {
//                var filter = GetFilterConfig();
//                var apiFilterModel = new ApiCommonFilterModel();
//                apiFilterModel.page_index = filter.page_index;
//                apiFilterModel.page_size = filter.page_size;
//                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var hasLoggedIn = User.Identity.IsAuthenticated;
//                var apiReturned = await JobSeekerServices.GetAllApplicationsAsync(apiFilterModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.value != null)
//                    {
//                        returnModel.ApplicationList = JsonConvert.DeserializeObject<List<ApplicationInfoModel>>(apiReturned.value.ToString());

//                        if (returnModel.ApplicationList.HasData())
//                        {
//                            returnModel.TotalCount = apiReturned.total;
//                            returnModel.CurrentPage = apiFilterModel.page_index;
//                            returnModel.PageSize = apiFilterModel.page_size;
//                            total = apiReturned.total;

//                            //var regionIds = new List<int>();
//                            var prefectureIds = new List<int>();
//                            var cityIds = new List<int>();
//                            var stationIds = new List<int>();
//                            var listEmployments = CommonHelpers.GetListEmploymentTypes();

//                            foreach (var item in returnModel.ApplicationList)
//                            {
//                                if (item.job_info != null)
//                                {
//                                    if (item.job_info.Addresses.HasData())
//                                    {
//                                        prefectureIds.AddRange(item.job_info.Addresses.Select(x => x.prefecture_id).ToList());
//                                        cityIds.AddRange(item.job_info.Addresses.Select(x => x.city_id).ToList());

//                                        foreach (var add in item.job_info.Addresses)
//                                        {
//                                            stationIds.AddRange(add.Stations.Select(x => x.station_id).ToList());
//                                        }
//                                    }
//                                }
//                            }

//                            //var allRegions = CommonHelpers.GetListRegions(regionIds);
//                            var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
//                            var allCities = CommonHelpers.GetListCities(cityIds);
//                            var allStations = CommonHelpers.GetListStations(stationIds);
//                            var hasStations = allStations.HasData();

//                            foreach (var item in returnModel.ApplicationList)
//                            {
//                                if (item.job_info != null)
//                                {
//                                    var employmentType = listEmployments.Where(x => x.id == item.job_info.employment_type_id).FirstOrDefault();
//                                    if (employmentType != null)
//                                    {
//                                        var typeName = string.Empty;
//                                        if (employmentType.LangList.HasData())
//                                            typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

//                                        if (string.IsNullOrEmpty(typeName))
//                                            typeName = employmentType.employment_type;

//                                        item.employment_type_label = typeName;
//                                        item.employment_type_show_trains = employmentType.show_trains;
//                                    }
//                                }

//                                if (item.job_info.Addresses.HasData())
//                                {
//                                    foreach (var add in item.job_info.Addresses)
//                                    {
//                                        //add.region_info = allRegions.Where(x => x.id == add.region_id).FirstOrDefault();
//                                        add.prefecture_info = allPrefectures.Where(x => x.id == add.prefecture_id).FirstOrDefault();
//                                        add.city_info = allCities.Where(x => x.id == add.city_id).FirstOrDefault();
//                                        if (add.Stations.HasData() && item.employment_type_show_trains)
//                                        {
//                                            foreach (var stat in add.Stations)
//                                            {
//                                                var matchedStation = allStations.Where(x => x.id == stat.station_id).FirstOrDefault();
//                                                if (matchedStation != null)
//                                                {
//                                                    stat.detail = matchedStation.detail;
//                                                    stat.furigana = matchedStation.furigana;
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }

//                            htmlReturn = PartialViewAsString("../Member/Sub/_AppliedJobs", returnModel);
//                        }

//                        isSuccess = true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying get AppliedJobs because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
//        }

//        [IsValidURLRequest]
//        //[RestrictCopyRequest]
//        public async Task<ActionResult> Detail(JobViewDetailModel model)
//        {
//            try
//            {
//                if (model.id <= 0)
//                    return RedirectToErrorPage();

//                var apiInputModel = new ApiJobGetDetailModel();
//                apiInputModel.job_seeker_id = AccountHelper.GetCurrentUserId();
//                apiInputModel.id = model.id;

//                var apiReturned = await JobServices.GetDetailAsync(apiInputModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.value != null)
//                    {
//                        model.job_info = JsonConvert.DeserializeObject<JobInfoModel>(apiReturned.value.ToString());
//                    }
//                }

//                if (model.job_info == null)
//                    return RedirectToErrorPage();
//                else
//                {
//                    var prefectureIds = new List<int>();
//                    var cityIds = new List<int>();
//                    var stationIds = new List<int>();

//                    model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();

//                    var listEmployments = CommonHelpers.GetListEmploymentTypes();
//                    var employmentType = listEmployments.Where(x => x.id == model.job_info.employment_type_id).FirstOrDefault();
//                    if (employmentType != null)
//                    {
//                        var typeName = string.Empty;
//                        if (employmentType.LangList.HasData())
//                            typeName = employmentType.LangList.Where(x => x.language_code == currentLang).Select(x => x.employment_type).FirstOrDefault();

//                        if (string.IsNullOrEmpty(typeName))
//                            typeName = employmentType.employment_type;

//                        model.job_info.employment_type_label = typeName;
//                        model.job_info.employment_type_show_trains = employmentType.show_trains;
//                    }

//                    //Get company address info
//                    if (model.job_info.company_info != null)
//                    {
//                        if (model.job_info.company_info.prefecture_id > 0)
//                            prefectureIds.Add(model.job_info.company_info.prefecture_id);

//                        if (model.job_info.company_info.city_id > 0)
//                            cityIds.Add(model.job_info.company_info.city_id);
//                    }

//                    var jobHasAddress = (model.job_info.Addresses.HasData());
//                    if (jobHasAddress)
//                    {
//                        var currentPrefectIds = model.job_info.Addresses.Select(x => x.prefecture_id).ToList();
//                        if (currentPrefectIds.HasData())
//                        {
//                            prefectureIds.AddRange(currentPrefectIds);
//                        }

//                        var currentCityIds = model.job_info.Addresses.Select(x => x.city_id).ToList();
//                        if (currentCityIds.HasData())
//                        {
//                            cityIds.AddRange(currentCityIds);
//                        }

//                        if (model.job_info.Addresses.HasData())
//                        {
//                            foreach (var add in model.job_info.Addresses)
//                            {
//                                var currentStationIds = add.Stations.Select(x => x.station_id).ToList();
//                                if (currentStationIds.HasData())
//                                {
//                                    stationIds.AddRange(currentStationIds);
//                                }
//                            }
//                        }
//                    }

//                    var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
//                    var allCities = CommonHelpers.GetListCities(cityIds);
//                    var allStations = CommonHelpers.GetListStations(stationIds);
//                    var hasStations = allStations.HasData();

//                    if (jobHasAddress)
//                    {
//                        foreach (var add in model.job_info.Addresses)
//                        {
//                            //add.region_info = allRegions.Where(x => x.id == add.region_id).FirstOrDefault();
//                            add.prefecture_info = allPrefectures.Where(x => x.id == add.prefecture_id).FirstOrDefault();
//                            add.city_info = allCities.Where(x => x.id == add.city_id).FirstOrDefault();
//                            if (add.Stations.HasData() && model.job_info.employment_type_show_trains)
//                            {
//                                foreach (var stat in add.Stations)
//                                {
//                                    var matchedStation = allStations.Where(x => x.id == stat.station_id).FirstOrDefault();
//                                    if (matchedStation != null)
//                                    {
//                                        stat.detail = matchedStation.detail;
//                                        stat.furigana = matchedStation.furigana;
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    if (model.job_info.company_info != null)
//                    {
//                        model.job_info.company_info.address_info = new IdentityAddress();
//                        model.job_info.company_info.address_info.prefecture_info = allPrefectures.Where(x => x.id == model.job_info.company_info.prefecture_id).FirstOrDefault();
//                        model.job_info.company_info.address_info.city_info = allCities.Where(x => x.id == model.job_info.company_info.city_id).FirstOrDefault();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying get Detail because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return View(model);
//        }

//        public ActionResult CancelApplication(JobApplicationModel model)
//        {
//            return PartialView("Partials/_CancelApplication", model);
//        }

//        [HttpPost]
//        [ActionName("CancelApplication")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmCancelApplication(JobApplicationModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = new ApiApplicationCancelModel();
//                apiModel.id = model.id;
//                apiModel.job_id = model.job_id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturned = await ApplicationServices.CancelAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.LB_APPLICATION_CANCELED_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec CancelApplication because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        [HttpPost]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> UpdateProfile(JobSeekerUpdateProfileModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = ExtractFormData(model);
//                if (model.image_file_upload != null)
//                {
//                    var apiUploadReturned = await JobSeekerServices.UploadImageAsync(apiModel, model.image_file_upload);
//                    if (apiUploadReturned != null)
//                    {
//                        if (apiUploadReturned.value != null)
//                        {
//                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
//                            if (images.HasData())
//                            {
//                                apiModel.image = images[0].Path;
//                            }
//                        }
//                    }
//                }

//                var apiReturned = await JobSeekerServices.UpdateProfileAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.MS_UPDATE_PROFILE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec UpdateProfile because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        #region CV

//        [HttpPost]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> CreateCV(CvUpdateModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiCvModel = ExtractCvFormData(model);
//                if (model.image_file_upload != null)
//                {
//                    ApiCvUploadImageModel apiImgModel = new ApiCvUploadImageModel();
//                    //var myImg = FileUpload.ConvertHttpPostedFileBaseToImage(model.image_file_upload);
//                    //apiImgModel.image = FileUpload.ImageToBase64(myImg, ImageFormat.Png);
//                    apiImgModel.job_seeker_id = apiCvModel.cv.job_seeker_id;
//                    var apiUploadReturned = await CvServices.UploadImageAsync(apiImgModel, model.image_file_upload);
//                    if (apiUploadReturned != null)
//                    {
//                        if (apiUploadReturned.value != null)
//                        {
//                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
//                            if (images.HasData())
//                            {
//                                apiCvModel.cv.image = images[0].Path;
//                            }
//                        }
//                    }
//                }

//                var apiReturned = await CvServices.CreateAsync(apiCvModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            if (!string.IsNullOrEmpty(apiReturned.message))
//                            {
//                                return Json(new { success = true, message = apiReturned.message, title = UserWebResource.LB_NOTIFICATION, clientcallback = "RedirectTo('/Member/MyCV')" });
//                            }
//                            else
//                            {
//                                return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = "RedirectTo('/Member/MyCV')" });
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec CreateCV because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [PreventSpam(DelayRequest = 1)]
//        public async Task<ActionResult> MyCVs()
//        {
//            var isSuccess = false;
//            var htmlReturn = string.Empty;
//            var returnModel = new CvSearchResultModel();
//            var total = 0;

//            if (!ModelState.IsValid)
//            {
//                var message = string.Join(" | ", ModelState.Values
//                    .SelectMany(v => v.Errors)
//                    .Select(e => e.ErrorMessage));
//                return Json(new { success = isSuccess, html = message, }, JsonRequestBehavior.AllowGet);
//            }

//            try
//            {
//                var filter = GetFilterConfig();
//                var apiFilterModel = new ApiGetListByPageModel();
//                apiFilterModel.page_index = filter.page_index;
//                apiFilterModel.page_size = filter.page_size;
//                apiFilterModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturned = await JobSeekerServices.GetAllCVsAsync(apiFilterModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.value != null)
//                    {
//                        returnModel.CvList = JsonConvert.DeserializeObject<List<CvInfoModel>>(apiReturned.value.ToString());

//                        if (returnModel.CvList.HasData())
//                        {
//                            returnModel.TotalCount = apiReturned.total;
//                            returnModel.CurrentPage = apiFilterModel.page_index;
//                            returnModel.PageSize = apiFilterModel.page_size;
//                            total = apiReturned.total;

//                            htmlReturn = PartialViewAsString("../Member/Sub/_CVs", returnModel);
//                        }

//                        isSuccess = true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying get MyCVs because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return Json(new { success = isSuccess, html = htmlReturn, total = total }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult DeleteCv(ApiCvModel model)
//        {
//            return PartialView("Partials/_DeleteCv", model);
//        }

//        [HttpPost]
//        [ActionName("DeleteCv")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmDeleteCv(ApiCvModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = new ApiCvDeleteModel();
//                apiModel.cv_id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturned = await CvServices.DeleteAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetMyCVs();" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmDeleteCv because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        public ActionResult AddCvEduHistory()
//        {
//            var model = new JobSeekerEduHistoryModel();
//            try
//            {
//                model.Qualifications = CommonHelpers.GetListQualifications();
//                model.Majors = CommonHelpers.GetListMajors();

//                if (model.id > 0)
//                {
//                    var apiModel = new ApiJobSeekerEduHistoryModel();
//                    apiModel.id = model.id;
//                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                    var apiReturned = EduHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
//                    if (apiReturned != null)
//                    {
//                        if (apiReturned.value != null)
//                        {
//                            IdentityJobSeekerEduHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerEduHistory>(apiReturned.value.ToString());
//                            if (info != null)
//                            {
//                                model.id = info.id;
//                                model.school = info.school;
//                                model.start_date_str = info.start_date.DateTimeQuestToString("dd-MM-yyyy");
//                                model.end_date_str = info.end_date.DateTimeQuestToString("dd-MM-yyyy");
//                                model.status = Utils.ConvertToIntFromQuest(info.status);
//                                model.address = info.address;
//                                model.qualification_id = info.qualification_id;
//                                model.major_id = info.major_id;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying display AddCvEduHistory because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("Partials/CV/_AddCvEduHistory", model);
//        }

//        [HttpPost]
//        [ActionName("AddCvEduHistory")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmAddCvEduHistory(JobSeekerEduHistoryModel model)
//        {
//            var message = string.Empty;
//            var htmlReturn = string.Empty;
//            try
//            {
//                var apiModel = new ApiJobSeekerEduHistoryModel();
//                apiModel.id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
//                apiModel.school = model.school;
//                apiModel.start_date = model.start_date_str;
//                apiModel.end_date = model.end_date_str;
//                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
//                apiModel.address = model.address;
//                apiModel.qualification_id = model.qualification_id;
//                apiModel.major_id = model.major_id;

//                if (apiModel.id > 0)
//                {
//                    var apiReturned = await EduHistoryServices.JobSeekerUpdateAsync(apiModel);
//                    if (apiReturned != null)
//                    {
//                        if (apiReturned.status == (int)HttpStatusCode.OK)
//                        {
//                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                            {
//                                message = apiReturned.error.message;

//                                return Json(new { success = false, html = htmlReturn, message = message, title = UserWebResource.LB_NOTIFICATION });
//                            }
//                            else
//                            {
//                                return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    model.Qualifications = CommonHelpers.GetListQualifications();
//                    model.Majors = CommonHelpers.GetListMajors();

//                    model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, "dd-MM-yyyy");
//                    model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, "dd-MM-yyyy");

//                    var qualification = model.Qualifications.Where(x => x.id == model.qualification_id).FirstOrDefault();
//                    if (qualification != null)
//                    {
//                        model.qualification_label = qualification.LangList.Where(x => x.language_code == currentLang).Select(x => x.qualification).FirstOrDefault();
//                        if (string.IsNullOrEmpty(model.qualification_label))
//                            model.qualification_label = qualification.qualification;
//                    }

//                    var major = model.Majors.Where(x => x.id == model.major_id).FirstOrDefault();
//                    if (major != null)
//                    {
//                        model.major_label = major.LangList.Where(x => x.language_code == currentLang).Select(x => x.major).FirstOrDefault();
//                        if (string.IsNullOrEmpty(model.major_label))
//                            model.major_label = major.major;
//                    }

//                    htmlReturn = PartialViewAsString("../Member/Partials/CV/_CvEduHistoryItem", model);
//                    return Json(new { success = true, html = htmlReturn });
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmAddCvEduHistory because: " + ex.ToString());

//                return Json(new { success = false, message = message });
//            }

//            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        public ActionResult AddCvWorkHistory()
//        {
//            var model = new JobSeekerWorkHistoryModel();
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying display AddCvWorkHistory because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("Partials/CV/_AddCvWorkHistory", model);
//        }

//        [HttpPost]
//        [ActionName("AddCvWorkHistory")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmAddCvWorkHistory(JobSeekerWorkHistoryModel model)
//        {
//            var message = string.Empty;
//            var htmlReturn = string.Empty;
//            try
//            {
//                var apiModel = new ApiJobSeekerWorkHistoryModel();
//                apiModel.id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
//                apiModel.company = model.company;
//                apiModel.content_work = model.content_work;
//                apiModel.form = model.form;
//                apiModel.start_date = model.start_date_str;
//                apiModel.end_date = model.end_date_str;
//                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
//                apiModel.address = model.address;

//                if (apiModel.id > 0)
//                {
//                    var apiReturned = await WorkHistoryServices.JobSeekerUpdateAsync(apiModel);
//                    if (apiReturned != null)
//                    {
//                        if (apiReturned.status == (int)HttpStatusCode.OK)
//                        {
//                            if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                            {
//                                message = apiReturned.error.message;

//                                return Json(new { success = false, message = message, title = UserWebResource.LB_NOTIFICATION });
//                            }
//                            else
//                            {
//                                return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, "dd-MM-yyyy");
//                    model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, "dd-MM-yyyy");

//                    htmlReturn = PartialViewAsString("../Member/Partials/CV/_CvWorkHistoryItem", model);
//                    return Json(new { success = true, html = htmlReturn });
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmAddCvWorkHistory because: " + ex.ToString());

//                return Json(new { success = false, message = message });
//            }

//            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        #endregion

//        #region Edu history

//        [PreventCrossOrigin]
//        [PreventSpam(DelayRequest = 1)]
//        public async Task<ActionResult> EduHistories()
//        {
//            List<JobSeekerEduHistoryModel> myList = null;
//            if (!ModelState.IsValid)
//            {
//                var message = string.Join(" | ", ModelState.Values
//                    .SelectMany(v => v.Errors)
//                    .Select(e => e.ErrorMessage));
//                return Content(message);
//            }

//            try
//            {
//                var apiModel = new ApiGetListByPageModel();
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturn = await JobSeekerServices.GetEduHistoryAsync(apiModel);
//                if (apiReturn != null)
//                {
//                    if (apiReturn.value != null)
//                    {
//                        myList = JsonConvert.DeserializeObject<List<JobSeekerEduHistoryModel>>(apiReturn.value.ToString());

//                        if (myList.HasData())
//                        {
//                            var qualifications = CommonHelpers.GetListQualifications();
//                            var majors = CommonHelpers.GetListMajors();
//                            foreach (var item in myList)
//                            {
//                                var qualification = qualifications.Where(x => x.id == item.qualification_id).FirstOrDefault();
//                                if (qualification != null)
//                                {
//                                    item.qualification_label = qualification.LangList.Where(x => x.language_code == currentLang).Select(x => x.qualification).FirstOrDefault();
//                                    if (string.IsNullOrEmpty(item.qualification_label))
//                                        item.qualification_label = qualification.qualification;
//                                }

//                                var major = majors.Where(x => x.id == item.major_id).FirstOrDefault();
//                                if (major != null)
//                                {
//                                    item.major_label = major.LangList.Where(x => x.language_code == currentLang).Select(x => x.major).FirstOrDefault();
//                                    if (string.IsNullOrEmpty(item.major_label))
//                                        item.major_label = major.major;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display EduHistories Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("Sub/_EduHistories", myList);
//        }

//        public ActionResult UpdateEduHistory(int id = 0)
//        {
//            var model = new JobSeekerEduHistoryModel();
//            try
//            {
//                model.id = id;
//                model.Qualifications = CommonHelpers.GetListQualifications();
//                model.Majors = CommonHelpers.GetListMajors();

//                if (model.id > 0)
//                {
//                    var apiModel = new ApiJobSeekerEduHistoryModel();
//                    apiModel.id = model.id;
//                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                    var apiReturned = EduHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
//                    if (apiReturned != null)
//                    {
//                        if (apiReturned.value != null)
//                        {
//                            IdentityJobSeekerEduHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerEduHistory>(apiReturned.value.ToString());
//                            if (info != null)
//                            {
//                                model.id = info.id;
//                                model.school = info.school;
//                                model.start_date_str = info.start_date.DateTimeQuestToString("dd-MM-yyyy");
//                                model.end_date_str = info.end_date.DateTimeQuestToString("dd-MM-yyyy");
//                                model.status = Utils.ConvertToIntFromQuest(info.status);
//                                model.address = info.address;
//                                model.qualification_id = info.qualification_id;
//                                model.major_id = info.major_id;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying display UpdateEduHistory because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return PartialView("Partials/_UpdateEduHistory", model);
//        }

//        [HttpPost]
//        [ActionName("UpdateEduHistory")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmUpdateEduHistory(JobSeekerEduHistoryModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = new ApiJobSeekerEduHistoryModel();
//                apiModel.id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
//                apiModel.school = model.school;
//                apiModel.start_date = model.start_date_str;
//                apiModel.end_date = model.end_date_str;
//                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
//                apiModel.address = model.address;
//                apiModel.qualification_id = model.qualification_id;
//                apiModel.major_id = model.major_id;

//                var apiReturned = await EduHistoryServices.JobSeekerUpdateAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmUpdateEduHistory because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        public ActionResult DeleteEduHistory(JobSeekerEduHistoryModel model)
//        {
//            return PartialView("Partials/_DeleteEduHistory", model);
//        }

//        [HttpPost]
//        [ActionName("DeleteEduHistory")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmDeleteEduHistory(JobSeekerEduHistoryModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = new ApiJobSeekerEduHistoryModel();
//                apiModel.id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturned = await EduHistoryServices.JobSeekerDeleteAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetEduHistories();" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmDeleteEduHistory because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        #endregion

//        #region Work history

//        [PreventCrossOrigin]
//        [PreventSpam(DelayRequest = 1)]
//        public async Task<ActionResult> WorkHistories()
//        {
//            List<JobSeekerWorkHistoryModel> myList = null;

//            if (!ModelState.IsValid)
//            {
//                var message = string.Join(" | ", ModelState.Values
//                    .SelectMany(v => v.Errors)
//                    .Select(e => e.ErrorMessage));
//                return Content(message);
//            }
//            try
//            {
//                var apiModel = new ApiGetListByPageModel();
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturn = await JobSeekerServices.GetWorkHistoryAsync(apiModel);
//                if (apiReturn != null)
//                {
//                    if (apiReturn.value != null)
//                    {
//                        myList = JsonConvert.DeserializeObject<List<JobSeekerWorkHistoryModel>>(apiReturn.value.ToString());
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when display WorkHistories Page because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return PartialView("Sub/_WorkHistories", myList);
//        }

//        public ActionResult UpdateWorkHistory(int id = 0)
//        {
//            var model = new JobSeekerWorkHistoryModel();
//            try
//            {
//                model.id = id;
//                if (model.id > 0)
//                {
//                    var apiModel = new ApiJobSeekerWorkHistoryModel();
//                    apiModel.id = model.id;
//                    apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                    var apiReturned = WorkHistoryServices.JobSeekerGetDetailAsync(apiModel).Result;
//                    if (apiReturned != null)
//                    {
//                        if (apiReturned.value != null)
//                        {
//                            IdentityJobSeekerWorkHistory info = JsonConvert.DeserializeObject<IdentityJobSeekerWorkHistory>(apiReturned.value.ToString());
//                            if (info != null)
//                            {
//                                model.id = info.id;
//                                model.company = info.company;
//                                model.content_work = info.content_work;
//                                model.form = info.form;
//                                model.start_date_str = info.start_date.DateTimeQuestToString("dd-MM-yyyy");
//                                model.end_date_str = info.end_date.DateTimeQuestToString("dd-MM-yyyy");
//                                model.status = Utils.ConvertToIntFromQuest(info.status);
//                                model.address = info.address;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying display UpdateWorkHistory because: {0}", ex.ToString());
//                logger.Error(strError);
//            }
//            return PartialView("Partials/_UpdateWorkHistory", model);
//        }

//        [HttpPost]
//        [ActionName("UpdateWorkHistory")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmUpdateWorkHistory(JobSeekerWorkHistoryModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = new ApiJobSeekerWorkHistoryModel();
//                apiModel.id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
//                apiModel.company = model.company;
//                apiModel.content_work = model.content_work;
//                apiModel.form = model.form;
//                apiModel.start_date = model.start_date_str;
//                apiModel.end_date = model.end_date_str;
//                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
//                apiModel.address = model.address;

//                var apiReturned = await WorkHistoryServices.JobSeekerUpdateAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmUpdateWorkHistory because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        public ActionResult DeleteWorkHistory(JobSeekerWorkHistoryModel model)
//        {
//            return PartialView("Partials/_DeleteWorkHistory", model);
//        }

//        [HttpPost]
//        [ActionName("DeleteWorkHistory")]
//        [VerifyLoggedInUser]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ConfirmDeleteWorkHistory(JobSeekerWorkHistoryModel model)
//        {
//            var message = string.Empty;
//            var isSuccess = false;

//            try
//            {
//                var apiModel = new ApiJobSeekerWorkHistoryModel();
//                apiModel.id = model.id;
//                apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

//                var apiReturned = await WorkHistoryServices.JobSeekerDeleteAsync(apiModel);
//                if (apiReturned != null)
//                {
//                    if (apiReturned.status == (int)HttpStatusCode.OK)
//                    {
//                        if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
//                        {
//                            message = apiReturned.error.message;

//                            return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
//                        }
//                        else
//                        {
//                            return Json(new { success = true, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = " GetWorkHistories();" });
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

//                logger.Error("Failed to exec ConfirmDeleteWorkHistory because: " + ex.ToString());

//                return Json(new { success = isSuccess, message = message });
//            }

//            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
//        }

//        #endregion

//        #region Helpers

//        private ApiJobSeekerModel ExtractFormData(JobSeekerUpdateProfileModel model)
//        {
//            var info = new ApiJobSeekerModel();
//            info.user_id = AccountHelper.GetCurrentUserId();
//            info.job_seeker_id = info.user_id;
//            info.email = model.email;
//            info.phone = model.phone;
//            info.marriage = model.marriage;
//            info.dependent_num = model.dependent_num;
//            info.fullname = model.fullname;
//            info.fullname_furigana = model.fullname_furigana;
//            info.display_name = model.display_name;
//            info.image = model.image;
//            info.birthday = model.birthday;
//            info.gender = model.gender;
//            info.id_card = model.id_card;
//            info.note = model.note;
//            info.video_path = model.video_path;
//            info.expected_job_title = model.expected_job_title;
//            info.expected_salary_min = model.expected_salary_min;
//            info.expected_salary_max = model.expected_salary_max;
//            info.work_status = model.work_status;
//            info.qualification_id = model.qualification_id;
//            info.job_seeking_status_id = model.job_seeking_status_id;
//            info.salary_type_id = model.salary_type_id;

//            info.Addresses = new List<IdentityJobSeekerAddress>();

//            var address = new IdentityJobSeekerAddress();
//            address.id = model.address.id;
//            address.job_seeker_id = info.user_id;
//            address.country_id = model.address.country_id;
//            address.region_id = model.address.region_id;
//            address.prefecture_id = model.address.prefecture_id;
//            address.city_id = model.address.city_id;
//            address.postal_code = model.address.postal_code;
//            address.detail = model.address.detail;
//            address.furigana = model.address.furigana;
//            address.train_line_id = model.address.train_line_id;
//            address.station_id = model.address.station_id;
//            address.is_contact_address = true;

//            var address_contact = new IdentityJobSeekerAddress();
//            address_contact.id = model.address_contact.id;
//            address_contact.job_seeker_id = info.user_id;
//            address_contact.country_id = model.address_contact.country_id;
//            address_contact.region_id = model.address_contact.region_id;
//            address_contact.prefecture_id = model.address_contact.prefecture_id;
//            address_contact.city_id = model.address_contact.city_id;
//            address_contact.postal_code = model.address_contact.postal_code;
//            address_contact.detail = model.address_contact.detail;
//            address_contact.furigana = model.address_contact.furigana;
//            address_contact.train_line_id = model.address_contact.train_line_id;
//            address_contact.station_id = model.address_contact.station_id;

//            info.Addresses.Add(address);
//            info.Addresses.Add(address_contact);

//            return info;
//        }

//        private JobSeekerUpdateProfileModel ParseDataToForm(IdentityJobSeeker identity)
//        {
//            var model = new JobSeekerUpdateProfileModel();

//            model.email = identity.email;
//            model.phone = identity.phone;
//            model.marriage = identity.marriage;
//            model.dependent_num = identity.dependent_num;
//            model.fullname = identity.fullname;
//            model.fullname_furigana = identity.fullname_furigana;
//            model.display_name = identity.display_name;
//            model.image = identity.image;
//            model.birthday = identity.birthday.DateTimeQuestToString("dd-MM-yyyy");
//            model.gender = identity.gender;
//            model.id_card = identity.id_card;
//            model.note = identity.note;
//            model.video_path = identity.video_path;
//            model.expected_job_title = identity.expected_job_title;
//            model.expected_salary_min = identity.expected_salary_min;
//            model.expected_salary_max = identity.expected_salary_max;
//            model.work_status = identity.work_status;
//            model.qualification_id = identity.qualification_id;
//            model.job_seeking_status_id = identity.job_seeking_status_id;
//            model.salary_type_id = identity.salary_type_id;

//            model.Extensions = identity.Extensions;
//            return model;
//        }

//        private ApiCvUpdateModel ExtractCvFormData(CvUpdateModel model)
//        {
//            var updateInfo = new ApiCvUpdateModel();

//            var info = new ApiCvModel();
//            info.cv_title = model.title;
//            info.highest_edu = model.qualification_id;
//            var dt = Utils.ConvertStringToDateTimeQuestByFormat(model.created_date, "dd-MM-yyyy");
//            if (dt != null)
//            {
//                info.date = dt.DateTimeQuestToString("yyyy-MM-dd");
//            }

//            info.job_seeker_id = AccountHelper.GetCurrentUserId();
//            info.email = model.email;
//            info.phone = model.phone;
//            info.marriage = model.marriage;
//            info.dependent_num = model.dependent_num;
//            info.fullname = model.fullname;
//            info.fullname_furigana = model.fullname_furigana;
//            info.image = model.image;
//            info.birthday = model.birthday;
//            info.gender = model.gender;

//            var address = new ApiCvAddressModel();
//            address.id = model.address.id;
//            address.job_seeker_id = info.job_seeker_id;
//            address.country_id = model.address.country_id;
//            address.region_id = model.address.region_id;
//            address.prefecture_id = model.address.prefecture_id;
//            address.city_id = model.address.city_id;
//            address.postal_code = model.address.postal_code;
//            address.detail = model.address.detail;
//            address.furigana = model.address.furigana;
//            address.train_line_id = model.address.train_line_id;
//            address.station_id = model.address.station_id;
//            address.is_contact_address = true;

//            var address_contact = new ApiCvAddressModel();
//            address_contact.id = model.address_contact.id;
//            address_contact.job_seeker_id = info.job_seeker_id;
//            address_contact.country_id = model.address_contact.country_id;
//            address_contact.region_id = model.address_contact.region_id;
//            address_contact.prefecture_id = model.address_contact.prefecture_id;
//            address_contact.city_id = model.address_contact.city_id;
//            address_contact.postal_code = model.address_contact.postal_code;
//            address_contact.detail = model.address_contact.detail;
//            address_contact.furigana = model.address_contact.furigana;
//            address_contact.train_line_id = model.address_contact.train_line_id;
//            address_contact.station_id = model.address_contact.station_id;

//            updateInfo.cv = info;
//            updateInfo.address = address;
//            updateInfo.address_contact = address_contact;

//            return updateInfo;
//        }

//        #endregion
//    }
//}
