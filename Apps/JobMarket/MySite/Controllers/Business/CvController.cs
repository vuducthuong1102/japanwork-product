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
using MySite.Settings;

namespace MySite.Controllers
{
    [Authorize]
    public class CvController : BaseAuthenticatedController
    {
        private readonly ILog logger = LogProvider.For<CvController>();
        private readonly string currentLang = UserCookieManager.GetCurrentLanguageOrDefault();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        #region CV

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerifyLoggedInUser]
        public ActionResult GetSuggestionCvs()
        {
            List<CvDropDownItemModel> returnList = new List<CvDropDownItemModel>();
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;                
                var apiInputModel = new ApiGetListByPageModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;
                apiInputModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = CvServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        List<IdentityCv> cvList = JsonConvert.DeserializeObject<List<IdentityCv>>(apiReturned.value.ToString());

                        if (cvList.HasData())
                        {
                            foreach (var item in cvList)
                            {
                                var itemModel = new CvDropDownItemModel();
                                itemModel.id = item.id;
                                itemModel.cv_title = item.cv_title;

                                returnList.Add(itemModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionCvs because: " + ex.ToString());
            }

            return Json(new { success = true, data = returnList });
        }

        private CvUpdateModel PreparingCvViewModel(int cv_id)
        {
            CvUpdateModel model = null;
            try
            {
                var apiReturned = CvServices.GetDetailAsync(cv_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        IdentityCv info = JsonConvert.DeserializeObject<IdentityCv>(apiReturned.value.ToString());
                        if (info != null)
                        {
                            model = new CvUpdateModel();

                            model = ParseDataToForm(info);
                            //model.Countries = CommonHelpers.GetListCountries();
                            //model.Regions = CommonHelpers.GetListRegions();
                            //model.Qualifications = CommonHelpers.GetListQualifications();

                            //if (info.address != null)
                            //{
                            //    var addJson = JsonConvert.SerializeObject(info.address);
                            //    if(!string.IsNullOrEmpty(addJson))
                            //        model.address = JsonConvert.DeserializeObject<IdentityJobSeekerAddress>(addJson);
                            //}

                            //if (info.address_contact != null)
                            //{
                            //    var addJson = JsonConvert.SerializeObject(info.address_contact);
                            //    if (!string.IsNullOrEmpty(addJson))
                            //        model.address_contact = JsonConvert.DeserializeObject<IdentityJobSeekerAddress>(addJson);
                            //}

                            //if (model.address == null)
                            //    model.address = new IdentityJobSeekerAddress();

                            //if (model.address_contact == null)
                            //    model.address_contact = new IdentityJobSeekerAddress();

                            //if (model.address != null)
                            //{
                            //    if (model.address.country_id == (int)EnumCountry.Japan)
                            //    {
                            //        if (model.address.city_id > 0)
                            //        {
                            //            var trainLineApiReturn = TrainLineServices.GetListByCityIdAsync(model.address.city_id).Result;
                            //            if (trainLineApiReturn != null)
                            //            {
                            //                if (trainLineApiReturn.value != null)
                            //                    model.train_lines = JsonConvert.DeserializeObject<List<IdentityTrainLine>>(trainLineApiReturn.value.ToString());
                            //            }
                            //        }

                            //        if (model.address.station_id > 0)
                            //        {
                            //            var stationApiReturn = StationServices.GetDetailAsync(model.address.station_id).Result;
                            //            if (stationApiReturn != null)
                            //            {
                            //                if (stationApiReturn.value != null)
                            //                    model.station_info = JsonConvert.DeserializeObject<IdentityStation>(stationApiReturn.value.ToString());
                            //            }
                            //        }
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display PreparingCvViewModel because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return model;
        }

        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> CvProfilePartial(int? id)
        {
            CvUpdateModel model = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var cv_id = Utils.ConvertToIntFromQuest(id);
                if (cv_id <= 0)
                    return Content("");

                if (Request["read_only"] != null)
                {
                    model.read_only = Utils.ConvertToBoolean(Request["read_only"]);
                }

                model = PreparingCvViewModel(cv_id);

                await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display MyProfilePartial Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_Profile", model);
        }

        [IsValidURLRequest]
        public ActionResult UpdateCv(int? id)
        {
            CvUpdateModel model = new CvUpdateModel();
            try
            {
                var cv_id = Utils.ConvertToIntFromQuest(id);
                if (cv_id <= 0)
                    return RedirectToErrorPage();

                model.id = cv_id;
                model = PreparingCvViewModel(cv_id);

                //model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
                //if (model == null)
                //    return RedirectToErrorPage();
                //else
                //    return View(model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display Update Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }

        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCv(CvUpdateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            //return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
            try
            {
                var apiCvModel = ExtractCvFormData(model);
                //if (model.image_file_upload != null)
                //{
                //    ApiCvUploadImageModel apiImgModel = new ApiCvUploadImageModel();
                //    apiImgModel.job_seeker_id = apiCvModel.cv.job_seeker_id;
                //    apiImgModel.cv_id = model.id;
                //    var apiUploadReturned = await CvServices.UploadImageAsync(apiImgModel, model.image_file_upload);
                //    if (apiUploadReturned != null)
                //    {
                //        if (apiUploadReturned.value != null)
                //        {
                //            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                //            if (images.HasData())
                //            {
                //                apiCvModel.cv.image = images[0].Path;
                //            }
                //        }
                //    }
                //}

                var apiReturned = await CvServices.UpdateAsync(apiCvModel);
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
                            return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = "location.reload();" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec UpdateCv because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult CvCreationMethod()
        {
            var model = new CvCreationMethodModel();
            try
            {
                
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display ConfirmCreateCv because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_CvCreationMethod", model);
        }

        [HttpPost]
        [ActionName("ConfirmCvCreationMethod")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCvCreationMethod(CvCreationMethodModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                if(model.creation_method == (int)EnumCvCreationMethod.CreateNew)
                {
                    return RedirectToAction("CreateCV", new { form = model.form });
                }
                else
                {
                    if (model.cv_id > 0)
                    {
                        var currentUserId = AccountHelper.GetCurrentUserId();
                        var apiReturn = CvServices.CloneAsync(new ApiCvModel { id = model.cv_id, job_seeker_id = currentUserId }).Result;
                        if (apiReturn != null && apiReturn.value != null)
                        {
                            var newId = Utils.ConvertToInt32(apiReturn.value.ToString());

                            var tk = SecurityHelper.GenerateUrlToken("CV", "UpdateCV", new { id = newId });

                            this.AddNotification(apiReturn.message, NotificationType.SUCCESS);

                            //Clone by existed CV
                            return RedirectToAction("UpdateCV", new { id = newId, tk = tk });
                        }

                        this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                    }
                }              
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmCvCreationMethod because: " + ex.ToString());                
            }

            return RedirectToAction("CreateCV", new { form = model.form });
        }

        public ActionResult CreateCV(int? form = 0)
        {
            var model = new CvUpdateModel();
            try
            {
                model.form = Utils.ConvertToInt32(form);
                model.EduHistories = new List<JobSeekerEduHistoryModel>();
                model.created_date = DateTime.Now.ToString(DATE_FORMAT);
                model.JapaneseLevels = CommonHelpers.GetListJapaneseLevels();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display CreateCV Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }

        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCV(CvUpdateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            
            try
            {
                var apiCvModel = ExtractCvFormData(model);
                if (model.image_file_upload != null)
                {
                    ApiCvUploadImageModel apiImgModel = new ApiCvUploadImageModel();
                    apiImgModel.job_seeker_id = apiCvModel.cv.job_seeker_id;
                    var apiUploadReturned = await CvServices.UploadImageAsync(apiImgModel, model.image_file_upload);
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiCvModel.cv.image = images[0].Path;
                            }
                        }
                    }               
                }
                var apiReturned = await CvServices.CreateAsync(apiCvModel);
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
                            if (!string.IsNullOrEmpty(apiReturned.message))
                            {
                                return Json(new { success = true, message = apiReturned.message, title = UserWebResource.LB_NOTIFICATION, clientcallback = "RedirectTo('/Member/MyCV')" });
                            }
                            else
                            {
                                return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = "RedirectTo('/Member/MyCV')" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec CreateCV because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #region Edu History

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> DefaultEduHistories()
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
                            var counter = 0;
                            foreach (var item in myList)
                            {
                                //item.isDefault = true;
                                item.id = 0;
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

                                counter++;
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

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EduHistories(int? id)
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

        public ActionResult UpdateCvEduHistory(int id = 0, int cv_id = 0)
        {
            var model = new CvEduHistoryModel();
            try
            {
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Majors = CommonHelpers.GetListMajors();
                model.id = id;
                model.cv_id = cv_id;

                if (model.id > 0)
                {
                    var apiModel = new ApiCvEduHistoryModel();
                    apiModel.id = model.id;
                    apiModel.cv_id = model.cv_id;
                    //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = EduHistoryServices.CvGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityCvEduHistory info = JsonConvert.DeserializeObject<IdentityCvEduHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.cv_id = info.cv_id;
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
                var strError = string.Format("Failed when trying display UpdateCvEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCvEduHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateCvEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCvEduHistory(CvEduHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCvEduHistoryModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;
                apiModel.school = model.school;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;
                apiModel.qualification_id = model.qualification_id;
                apiModel.major_id = model.major_id;
                apiModel.major_custom = model.major_custom;
               
                if (apiModel.cv_id > 0)
                {
                    var apiReturned = await EduHistoryServices.CvUpdateAsync(apiModel);
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
                                var newId = Utils.ConvertToInt32(apiReturned.value);
                                model.id = newId;
                            }
                            //else
                            //{

                            //    return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION });
                            //}
                        }
                    }

                    model.isDefault = true;
                }

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

                htmlReturn = PartialViewAsString("../Cv/Partials/_CvEduHistoryItem", model);

                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCvEduHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }
        }

        public ActionResult DeleteCvEduHistory(int id = 0, int cv_id = 0)
        {
            var model = new CvEduHistoryModel();
            try
            {               
                model.id = id;
                model.cv_id = cv_id;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCvEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteCvEduHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteCvEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCvEduHistory(CvEduHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCvEduHistoryModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;
               
                if (apiModel.id > 0)
                {
                    var apiReturned = await EduHistoryServices.CvDeleteAsync(apiModel);
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

                                return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback= " MemberGlobal.eduHistories();" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec DeleteCvEduHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #region Work History

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> DefaultWorkHistories()
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
                        if (myList.HasData())
                        {
                            foreach (var item in myList)
                            {
                                item.id = 0;
                            }
                        }
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

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> WorkHistories(int? id)
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

        public ActionResult UpdateCvWorkHistory(int id = 0, int cv_id = 0)
        {
            var model = new CvWorkHistoryModel();
            try
            {
                model.id = id;
                model.cv_id = cv_id;

                if (model.id > 0)
                {
                    var apiModel = new ApiCvWorkHistoryModel();
                    apiModel.id = model.id;
                    apiModel.cv_id = model.cv_id;
                    //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = WorkHistoryServices.CvGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityCvWorkHistory info = JsonConvert.DeserializeObject<IdentityCvWorkHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.cv_id = info.cv_id;
                                model.company = info.company;
                                model.form = info.form;
                                model.content_work = info.content_work;
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
                var strError = string.Format("Failed when trying display UpdateCvWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCvWorkHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateCvWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCvWorkHistory(CvWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiCvWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.company = model.company;
                apiModel.content_work = model.content_work;
                apiModel.form = model.form;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;

                if (apiModel.cv_id > 0)
                {
                    var apiReturned = await WorkHistoryServices.CvUpdateAsync(apiModel);
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
                                var newId = Utils.ConvertToInt32(apiReturned.value);
                                model.id = newId;
                            }
                            //else
                            //{
                            //    return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION });
                            //}
                        }

                        model.isDefault = true;
                    }
                }

                model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                htmlReturn = PartialViewAsString("../Cv/Partials/_CvWorkHistoryItem", model);
                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCvWorkHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }
        }

        public ActionResult DeleteCvWorkHistory(int id = 0, int cv_id = 0)
        {
            var model = new CvWorkHistoryModel();
            try
            {
                model.id = id;
                model.cv_id = cv_id;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCvWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteCvWorkHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteCvWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCvWorkHistory(CvWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCvWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;

                if (apiModel.cv_id > 0)
                {
                    var apiReturned = await WorkHistoryServices.CvDeleteAsync(apiModel);
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

                                return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = "  MemberGlobal.workHistories();" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec DeleteCvWorkHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #region Certificate

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> DefaultCertificates()
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
                        if (myList.HasData())
                        {
                            foreach (var item in myList)
                            {
                                item.id = 0;
                            }
                        }
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

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> Certificates(int? id)
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

        public ActionResult UpdateCvCertificate(int id = 0, int cv_id = 0)
        {
            var model = new CvCertificateModel();
            try
            {
                model.id = id;
                model.cv_id = cv_id;

                if (model.id > 0)
                {
                    var apiModel = new ApiCvCertificateModel();
                    apiModel.id = model.id;
                    apiModel.cv_id = model.cv_id;
                    //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = CertificateServices.CvGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityCvCertificate info = JsonConvert.DeserializeObject<IdentityCvCertificate>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.name = info.name;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.pass = info.pass;
                                model.point = info.point;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateCvCertificate because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCvCertificate", model);
        }

        [HttpPost]
        [ActionName("UpdateCvCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCvCertificate(CvCertificateModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiCvCertificateModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.name = model.name;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.pass = model.pass;
                apiModel.point = model.point;

                if (apiModel.cv_id > 0)
                {
                    var apiReturned = await CertificateServices.CvUpdateAsync(apiModel);
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
                                var newId = Utils.ConvertToInt32(apiReturned.value);
                                model.id = newId;
                            }
                            //else
                            //{
                            //    return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION });
                            //}
                        }

                        model.isDefault = true;
                    }
                }

                model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                htmlReturn = PartialViewAsString("../Cv/Partials/_CvCertificateItem", model);
                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCvCertificate because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            //return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult DeleteCvCertificate(int id = 0, int cv_id = 0)
        {
            var model = new CvCertificateModel();
            try
            {
                model.id = id;
                model.cv_id = cv_id;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCvCertificate because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteCvCertificate", model);
        }

        [HttpPost]
        [ActionName("DeleteCvCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCvCertificate(CvCertificateModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCvCertificateModel();
                apiModel.id = model.id;
                apiModel.cv_id = model.cv_id;

                if (apiModel.id > 0)
                {
                    var apiReturned = await CertificateServices.CvDeleteAsync(apiModel);
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

                                return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_DELETE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = "  MemberGlobal.certificates();" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec DeleteCvCertificate because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #endregion

        #endregion

        #region Helpers

        private ApiJobSeekerModel ExtractFormData(JobSeekerUpdateProfileModel model)
        {
            var info = new ApiJobSeekerModel();
            info.user_id = AccountHelper.GetCurrentUserId();
            info.job_seeker_id = info.user_id;
            info.email = model.email;
            info.phone = model.phone;
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

        private CvUpdateModel ParseDataToForm(IdentityCv identity)
        {
            var model = new CvUpdateModel();
            model.id = identity.id;
            model.title = identity.cv_title;
            model.form = identity.form;
            model.hobby_skills = identity.hobby_skills;
            model.pr = identity.pr;
            model.reason = identity.reason;
            model.time_work = identity.time_work;
            model.aspiration = identity.aspiration;

            model.created_date = identity.date.DateTimeQuestToString(DATE_FORMAT);

            //model.email = identity.email;
            //model.japanese_level_number = identity.japanese_level_number;
            //model.phone = identity.phone;
            //model.marriage = (identity.marriage) ? 1 : 0;
            //model.dependent_num = identity.dependent_num;
            //model.fullname = identity.fullname;
            //model.fullname_furigana = identity.fullname_furigana;
            ////model.display_name = identity.display_name;

            //model.image = identity.image;
            //model.image_full_path = identity.image_full_path;
            //model.birthday = identity.birthday.DateTimeQuestToString(DATE_FORMAT);
            //model.gender = identity.gender;
            //model.qualification_id = identity.highest_edu;

            //model.id_card = identity.id_card;
            //model.note = identity.note;
            //model.video_path = identity.video_path;
            //model.expected_job_title = identity.expected_job_title;
            //model.expected_salary_min = identity.expected_salary_min;
            //model.expected_salary_max = identity.expected_salary_max;
            //model.work_status = identity.work_status;
            //model.qualification_id = identity.qualification_id;
            //model.job_seeking_status_id = identity.job_seeking_status_id;
            //model.salary_type_id = identity.salary_type_id;

            //model.Extensions = identity.Extensions;
            return model;
        }

        private ApiCvUpdateModel ExtractCvFormData(CvUpdateModel model)
        {
            var updateInfo = new ApiCvUpdateModel();

            var info = new ApiCvModel();
            info.id = model.id;
            info.cv_title = model.title;

            var dt = Utils.ConvertStringToDateTimeQuestByFormat(model.created_date, DATE_FORMAT);
            if (dt != null)
            {
                info.date = dt.DateTimeQuestToString(DATE_FORMAT);
            }
            info.form = model.form;

            info.highest_edu = model.qualification_id;            
            info.hobby_skills = model.hobby_skills;
            info.pr = model.pr;
            info.reason = model.reason;
            info.time_work = model.time_work;
            info.aspiration = model.aspiration;            
            info.job_seeker_id = AccountHelper.GetCurrentUserId();

            //info.email = model.email;
            //info.japanese_level_number = model.japanese_level_number;
            //info.phone = model.phone;
            //info.marriage = model.marriage;
            //info.dependent_num = model.dependent_num;
            //info.fullname = model.fullname;
            //info.fullname_furigana = model.fullname_furigana;            
            //info.image = model.image;
            //info.birthday = model.birthday;
            //info.gender = model.gender;

            //var address = new ApiCvAddressModel();
            //address.id = model.address.id;
            //address.job_seeker_id = info.job_seeker_id;
            //address.country_id = model.address.country_id;
            //address.region_id = model.address.region_id;
            //address.prefecture_id = model.address.prefecture_id;
            //address.city_id = model.address.city_id;
            //address.postal_code = model.address.postal_code;
            //address.detail = model.address.detail;
            //address.furigana = model.address.furigana;
            //address.train_line_id = model.address.train_line_id;
            //address.station_id = model.address.station_id;            

            //var address_contact = new ApiCvAddressModel();
            //address_contact.id = model.address_contact.id;
            //address_contact.job_seeker_id = info.job_seeker_id;
            //address_contact.country_id = model.address_contact.country_id;
            //address_contact.region_id = model.address_contact.region_id;
            //address_contact.prefecture_id = model.address_contact.prefecture_id;
            //address_contact.city_id = model.address_contact.city_id;
            //address_contact.postal_code = model.address_contact.postal_code;
            //address_contact.detail = model.address_contact.detail;
            //address_contact.furigana = model.address_contact.furigana;
            //address_contact.train_line_id = model.address_contact.train_line_id;
            //address_contact.station_id = model.address_contact.station_id;
            //address_contact.is_contact_address = true;

            updateInfo.cv = info;
            //updateInfo.address = address;
            //updateInfo.address_contact = address_contact;

            //updateInfo.edu_history = ExtractCvEduHistory(model);
            //updateInfo.work_history = ExtractCvWorkHistory(model);
            //updateInfo.certification = ExtractCvCertificate(model);

            return updateInfo;
        }

        private List<ApiCvEduHistoryModel> ExtractCvEduHistory(CvUpdateModel model)
        {
            List<ApiCvEduHistoryModel> myList = null;
            if (model == null)
                return null;

            if (model.EduHistories.HasData())
            {
                myList = new List<ApiCvEduHistoryModel>();

                foreach (var item in model.EduHistories)
                {
                    if (!item.isDefault && model.id != 0)
                        continue;

                    var newItem = new ApiCvEduHistoryModel();
                    newItem.id = item.id;
                    newItem.school = item.school;
                    newItem.start_date = item.start_date_str;
                    newItem.end_date = item.end_date_str;
                    newItem.qualification_id = item.qualification_id;
                    newItem.major_id = item.major_id;
                    newItem.status = item.status;
                    newItem.address = item.address;

                    myList.Add(newItem);
                }
            }

            return myList;
        }

        private List<ApiCvWorkHistoryModel> ExtractCvWorkHistory(CvUpdateModel model)
        {
            List<ApiCvWorkHistoryModel> myList = null;
            if (model == null)
                return null;

            if (model.WorkHistories.HasData())
            {
                myList = new List<ApiCvWorkHistoryModel>();

                foreach (var item in model.WorkHistories)
                {
                    if (!item.isDefault && model.id != 0)
                        continue;

                    var newItem = new ApiCvWorkHistoryModel();
                    newItem.id = item.id;
                    newItem.company = item.company;
                    newItem.content_work = item.content_work;
                    newItem.start_date = item.start_date_str;
                    newItem.end_date = item.end_date_str;
                    newItem.status = item.status;
                    newItem.address = item.address;

                    myList.Add(newItem);
                }
            }

            return myList;
        }

        private List<ApiCvCertificateModel> ExtractCvCertificate(CvUpdateModel model)
        {
            List<ApiCvCertificateModel> myList = null;
            if (model == null)
                return null;

            if (model.Certificates.HasData())
            {
                myList = new List<ApiCvCertificateModel>();

                foreach (var item in model.Certificates)
                {
                    if (!item.isDefault && model.id != 0)
                        continue;

                    var newItem = new ApiCvCertificateModel();
                    newItem.id = item.id;
                    newItem.name = item.name;
                    newItem.start_date = item.start_date_str;
                    newItem.end_date = item.end_date_str;
                    newItem.pass = item.pass;
                    newItem.point = item.point;

                    myList.Add(newItem);
                }
            }

            return myList;
        }

        #endregion
    }
}
