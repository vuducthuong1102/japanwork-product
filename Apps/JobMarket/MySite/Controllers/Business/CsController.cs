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
    public class CsController : BaseAuthenticatedController
    {
        private readonly ILog logger = LogProvider.For<CsController>();
        private readonly string currentLang = UserCookieManager.GetCurrentLanguageOrDefault();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        #region CS

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerifyLoggedInUser]
        public ActionResult GetSuggestionCss()
        {
            List<CsDropDownItemModel> returnList = new List<CsDropDownItemModel>();
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;                
                var apiInputModel = new ApiGetListByPageModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;
                apiInputModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                var apiReturned = CsServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        List<IdentityCs> csList = JsonConvert.DeserializeObject<List<IdentityCs>>(apiReturned.value.ToString());

                        if (csList.HasData())
                        {
                            foreach (var item in csList)
                            {
                                var itemModel = new CsDropDownItemModel();
                                itemModel.id = item.id;
                                itemModel.cs_title = item.cs_title;

                                returnList.Add(itemModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionCss because: " + ex.ToString());
            }

            return Json(new { success = true, data = returnList });
        }

        private CsUpdateModel PreparingCsViewModel(int cs_id)
        {
            CsUpdateModel model = null;
            try
            {
                var apiReturned = CsServices.GetDetailAsync(cs_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        IdentityCs info = JsonConvert.DeserializeObject<IdentityCs>(apiReturned.value.ToString());
                        if (info != null)
                        {
                            model = new CsUpdateModel();

                            model = ParseDataToForm(info);
                            model.Countries = CommonHelpers.GetListCountries();
                            model.Regions = CommonHelpers.GetListRegions();
                            model.Qualifications = CommonHelpers.GetListQualifications();

                            if (info.address != null)
                            {
                                var addJson = JsonConvert.SerializeObject(info.address);
                                if(!string.IsNullOrEmpty(addJson))
                                    model.address = JsonConvert.DeserializeObject<IdentityJobSeekerAddress>(addJson);
                            }

                            //if (info.address_contact != null)
                            //{
                            //    var addJson = JsonConvert.SerializeObject(info.address_contact);
                            //    if (!string.IsNullOrEmpty(addJson))
                            //        model.address_contact = JsonConvert.DeserializeObject<IdentityJobSeekerAddress>(addJson);
                            //}

                            if (model.address == null)
                                model.address = new IdentityJobSeekerAddress();

                            if (model.address_contact == null)
                                model.address_contact = new IdentityJobSeekerAddress();

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
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display PreparingCsViewModel because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return model;
        }

        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> CsProfilePartial(int? id)
        {
            CsUpdateModel model = null;

            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content(message);
            }

            try
            {
                var cs_id = Utils.ConvertToIntFromQuest(id);
                if (cs_id <= 0)
                    return Content("");

                model = PreparingCsViewModel(cs_id);

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
        public ActionResult UpdateCs(int? id)
        {
            CsUpdateModel model = new CsUpdateModel();
            try
            {
                var cs_id = Utils.ConvertToIntFromQuest(id);
                if (cs_id <= 0)
                    return RedirectToErrorPage();

                model.id = cs_id;
                model = PreparingCsViewModel(cs_id);

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
        public async Task<ActionResult> UpdateCs(CsUpdateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            //return Json(new { success = isSuccess, message = message, title = UserWebResource.LB_NOTIFICATION });
            try
            {
                var apiCsModel = ExtractCsFormData(model);
                if (model.image_file_upload != null)
                {
                    ApiCsUploadImageModel apiImgModel = new ApiCsUploadImageModel();
                    apiImgModel.job_seeker_id = apiCsModel.cs.job_seeker_id;
                    apiImgModel.cs_id = model.id;
                    var apiUploadReturned = await CsServices.UploadImageAsync(apiImgModel, model.image_file_upload);
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiCsModel.cs.image = images[0].Path;
                            }
                        }
                    }
                }

                var apiReturned = await CsServices.UpdateAsync(apiCsModel);
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

                logger.Error("Failed to exec UpdateCs because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult CsCreationMethod()
        {
            var model = new CsCreationMethodModel();
            try
            {
                
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display ConfirmCreateCs because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_CsCreationMethod", model);
        }

        [HttpPost]
        [ActionName("ConfirmCsCreationMethod")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCsCreationMethod(CsCreationMethodModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                if(model.creation_method == (int)EnumCsCreationMethod.CreateNew)
                {
                    return RedirectToAction("CreateCS", new { form = model.form });
                }
                else
                {
                    if (model.cs_id > 0)
                    {
                        var currentUserId = AccountHelper.GetCurrentUserId();
                        var apiReturn = CsServices.CloneAsync(new ApiCsModel { id = model.cs_id, job_seeker_id = currentUserId }).Result;
                        if (apiReturn != null && apiReturn.value != null)
                        {
                            var newId = Utils.ConvertToInt32(apiReturn.value.ToString());

                            var tk = SecurityHelper.GenerateUrlToken("CS", "UpdateCS", new { id = newId });

                            this.AddNotification(apiReturn.message, NotificationType.SUCCESS);

                            //Clone by existed CS
                            return RedirectToAction("UpdateCS", new { id = newId, tk = tk });
                        }

                        this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                    }
                }              
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmCsCreationMethod because: " + ex.ToString());                
            }

            return RedirectToAction("CreateCS", new { form = model.form });
        }

        public ActionResult CreateCS()
        {
            var model = new CsUpdateModel();
            try
            {
                model.EduHistories = new List<JobSeekerEduHistoryModel>();
                model.created_date = DateTime.Now.ToString(DATE_FORMAT);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display CreateCS Page because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return View(model);
        }

        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCS(CsUpdateModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            
            try
            {
                var apiCsModel = ExtractCsFormData(model);
                if (model.image_file_upload != null)
                {
                    ApiCsUploadImageModel apiImgModel = new ApiCsUploadImageModel();
                    apiImgModel.job_seeker_id = apiCsModel.cs.job_seeker_id;
                    var apiUploadReturned = await CsServices.UploadImageAsync(apiImgModel, model.image_file_upload);
                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiCsModel.cs.image = images[0].Path;
                            }
                        }
                    }               
                }
                
                var apiReturned = await CsServices.CreateAsync(apiCsModel);
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
                                return Json(new { success = true, message = apiReturned.message, title = UserWebResource.LB_NOTIFICATION, clientcallback = "RedirectTo('/Member/MyCS')" });
                            }
                            else
                            {
                                return Json(new { success = true, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION, clientcallback = "RedirectTo('/Member/MyCS')" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec CreateCS because: " + ex.ToString());

                return Json(new { success = isSuccess, message = message });
            }

            return Json(new { success = isSuccess, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        #region Edu History

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public async Task<ActionResult> DefaultEduHistories()
        {
            List<CsEduHistoryModel> myList = null;
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
                        myList = JsonConvert.DeserializeObject<List<CsEduHistoryModel>>(apiReturn.value.ToString());

                        if (myList.HasData())
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            var majors = CommonHelpers.GetListMajors();
                            var counter = 0;
                            foreach (var item in myList)
                            {
                                item.id = 0;
                                //item.isDefault = true;
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
            List<CsEduHistoryModel> myList = null;
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
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                var cs_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await CsServices.GetEduHistoryAsync(apiModel, cs_id);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<CsEduHistoryModel>>(apiReturn.value.ToString());

                        if (myList.HasData())
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            var majors = CommonHelpers.GetListMajors();
                            var counter = 0;
                            foreach (var item in myList)
                            {
                                item.isDefault = true;
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

        public ActionResult UpdateCsEduHistory(int id = 0, int cs_id = 0)
        {
            var model = new CsEduHistoryModel();
            try
            {
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Majors = CommonHelpers.GetListMajors();
                model.id = id;
                model.cs_id = cs_id;

                if (model.id > 0)
                {
                    var apiModel = new ApiCsEduHistoryModel();
                    apiModel.id = model.id;
                    apiModel.cs_id = model.cs_id;
                    //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = EduHistoryServices.CsGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityCsEduHistory info = JsonConvert.DeserializeObject<IdentityCsEduHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.cs_id = info.cs_id;
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
                var strError = string.Format("Failed when trying display UpdateCsEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCsEduHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateCsEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCsEduHistory(CsEduHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCsEduHistoryModel();
                apiModel.id = model.id;
                apiModel.cs_id = model.cs_id;
                apiModel.school = model.school;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;
                apiModel.qualification_id = model.qualification_id;
                apiModel.major_id = model.major_id;
                apiModel.major_custom = model.major_custom;
               
                if (apiModel.cs_id > 0)
                {
                    var apiReturned = await EduHistoryServices.CsUpdateAsync(apiModel);
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

                htmlReturn = PartialViewAsString("../Cs/Partials/_CsEduHistoryItem", model);

                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCsEduHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }
        }

        public ActionResult DeleteCsEduHistory(int id = 0, int cs_id = 0)
        {
            var model = new CsEduHistoryModel();
            try
            {               
                model.id = id;
                model.cs_id = cs_id;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCsEduHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteCsEduHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteCsEduHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCsEduHistory(CsEduHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCsEduHistoryModel();
                apiModel.id = model.id;
                apiModel.cs_id = model.cs_id;
               
                if (apiModel.id > 0)
                {
                    var apiReturned = await EduHistoryServices.CsDeleteAsync(apiModel);
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

                logger.Error("Failed to exec DeleteCsEduHistory because: " + ex.ToString());

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
            List<CsWorkHistoryModel> myList = null;
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
                        myList = JsonConvert.DeserializeObject<List<CsWorkHistoryModel>>(apiReturn.value.ToString());
                        if (myList.HasData())
                        {
                            var idx = 0;
                            foreach (var item in myList)
                            {
                                idx++;
                                item.id = 0;
                                item.idx = (int)EpochTime.GetIntDate(DateTime.Now) + idx;
                                item.json_obj = JsonConvert.SerializeObject(item);
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
            List<CsWorkHistoryModel> myList = null;
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
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                var cs_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await CsServices.GetWorkHistoryAsync(apiModel, cs_id);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<CsWorkHistoryModel>>(apiReturn.value.ToString());
                        if (myList.HasData())
                        {
                            foreach (var item in myList)
                            {
                                item.isDefault = true;
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

        public ActionResult  UpdateCsWorkHistory(int id = 0, int cs_id = 0)
        {
            var model = new CsWorkHistoryModel();
            try
            {
                model.id = id;
                model.cs_id = cs_id;
                model.Fields = CommonHelpers.GetListFields();
                model.Industries = CommonHelpers.GetListIndustries();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                if (model.id > 0)
                {
                    var apiModel = new ApiCsWorkHistoryModel();
                    apiModel.id = model.id;
                    apiModel.cs_id = model.cs_id;
                    //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = WorkHistoryServices.CsGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityCsWorkHistory info = JsonConvert.DeserializeObject<IdentityCsWorkHistory>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                model.id = info.id;
                                model.cs_id = info.cs_id;
                                model.company = info.company;
                                model.sub_field_id = info.sub_field_id;
                                model.sub_industry_id = info.sub_industry_id;
                                model.employment_type_id = info.employment_type_id;
                                model.employees_number = info.employees_number;
                                model.resign_reason = info.resign_reason;
                                model.start_date_str = info.start_date.DateTimeQuestToString(DATE_FORMAT);
                                model.end_date_str = info.end_date.DateTimeQuestToString(DATE_FORMAT);
                                model.status = Utils.ConvertToIntFromQuest(info.status);
                                model.address = info.address;

                                model.Details = new List<CsWorkHistoryDetailModel>();
                                if (info.Details.HasData())
                                {
                                    foreach (var dt in info.Details)
                                    {
                                        var detailMd = new CsWorkHistoryDetailModel();
                                        detailMd.id = dt.id;
                                        detailMd.department = dt.department;
                                        detailMd.position = dt.position;
                                        detailMd.content_work = dt.content_work;
                                        detailMd.salary = dt.salary;
                                        detailMd.start_date = dt.start_date.DateTimeQuestToString("dd-MM-yyyy");
                                        detailMd.end_date = dt.end_date.DateTimeQuestToString("dd-MM-yyyy");

                                        model.Details.Add(detailMd);
                                    }
                                }
                                else
                                {
                                    var detailMd = new CsWorkHistoryDetailModel();
                                    model.Details.Add(detailMd);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var detailMd = new CsWorkHistoryDetailModel();
                    model.Details.Add(detailMd);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateCsWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCsWorkHistory", model);
        }

        [HttpPost]
        [ActionName("UpdateCsWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCsWorkHistory(CsWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiCsWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.cs_id = model.cs_id;
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.company = model.company;
                apiModel.sub_field_id = model.sub_field_id;
                apiModel.sub_industry_id = model.sub_industry_id;
                apiModel.employment_type_id = model.employment_type_id;
                apiModel.employees_number = model.employees_number;
                apiModel.resign_reason = model.resign_reason;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.address = model.address;

                if (model.Details.HasData())
                {
                    apiModel.Details = new List<ApiCsWorkHistoryDetailModel>();
                    foreach (var item in model.Details)
                    {
                        var dt = new ApiCsWorkHistoryDetailModel();
                        dt.id = item.id;
                        dt.cs_work_history_id = Utils.ConvertToIntFromQuest(model.id);
                        dt.department = item.department;
                        dt.position = item.position;
                        dt.content_work = item.content_work;
                        dt.salary = item.salary;
                        dt.start_date = item.start_date;
                        dt.end_date = item.end_date;

                        apiModel.Details.Add(dt);
                    }
                }

                var newId = 0;
                var apiReturned = await WorkHistoryServices.CsUpdateAsync(apiModel);
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
                            newId = Utils.ConvertToInt32(apiReturned.value);
                            model.id = newId;
                        }
                        //else
                        //{
                        //    return Json(new { success = true, html = htmlReturn, message = UserWebResource.LB_SAVE_SUCCESS, title = UserWebResource.LB_NOTIFICATION });
                        //}
                    }

                    model.isDefault = true;
                }

                model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                model.idx = (int)EpochTime.GetIntDate(DateTime.Now);
                model.json_obj = JsonConvert.SerializeObject(model);

                htmlReturn = PartialViewAsString("../Cs/Partials/_CsWorkHistoryItem", model);
                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCsWorkHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }
        }

        [HttpPost]
        public ActionResult UpdateDefaultCsWorkHistory()
        {
            var model = new CsWorkHistoryModel();
            try
            {
                var json_obj = string.Empty;
                CsWorkHistoryModel oldData = new CsWorkHistoryModel();
                if (Request["json_obj"] != null)
                    json_obj = Request["json_obj"].ToString();

                if (!string.IsNullOrEmpty(json_obj))
                {
                    oldData = JsonConvert.DeserializeObject<CsWorkHistoryModel>(json_obj);
                }                

                model = oldData;

                model.start_date_str = model.start_date.DateTimeQuestToString(DATE_FORMAT);
                model.end_date_str = model.end_date.DateTimeQuestToString(DATE_FORMAT);

                if (!model.Details.HasData())
                {
                    model.Details = new List<CsWorkHistoryDetailModel>();
                    model.Details.Add(new CsWorkHistoryDetailModel());
                }

                model.json_obj = JsonConvert.SerializeObject(model);

                model.Fields = CommonHelpers.GetListFields();
                model.Industries = CommonHelpers.GetListIndustries();
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();             
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display UpdateDefaultCsWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCsWorkHistory", model);
        }

        [HttpPost]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateDefaultCsWorkHistory(CsWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var jsonObj = JsonConvert.SerializeObject(model);
                await Task.FromResult(jsonObj);

                model.start_date = Utils.ConvertStringToDateTimeQuestByFormat(model.start_date_str, DATE_FORMAT);
                model.end_date = Utils.ConvertStringToDateTimeQuestByFormat(model.end_date_str, DATE_FORMAT);

                var detailList = new List<CsWorkHistoryDetailModel>();
                if (model.Details.HasData())
                {
                    foreach (var item in model.Details)
                    {
                        if(!string.IsNullOrEmpty(item.department) || !string.IsNullOrEmpty(item.position) || !string.IsNullOrEmpty(item.content_work))
                        {
                            detailList.Add(item);
                        }
                    }

                    model.Details = detailList;
                }

                model.idx = (int)EpochTime.GetIntDate(DateTime.Now);
                model.json_obj = JsonConvert.SerializeObject(model);

                htmlReturn = PartialViewAsString("../Cs/Partials/_CsWorkHistoryItem", model);

                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateDefaultCsWorkHistory because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }
        }

        public ActionResult DeleteCsWorkHistory(int id = 0, int cs_id = 0)
        {
            var model = new CsWorkHistoryModel();
            try
            {
                model.id = id;
                model.cs_id = cs_id;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCsWorkHistory because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteCsWorkHistory", model);
        }

        [HttpPost]
        [ActionName("DeleteCsWorkHistory")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCsWorkHistory(CsWorkHistoryModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCsWorkHistoryModel();
                apiModel.id = model.id;
                apiModel.cs_id = model.cs_id;

                if (apiModel.cs_id > 0)
                {
                    var apiReturned = await WorkHistoryServices.CsDeleteAsync(apiModel);
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

                logger.Error("Failed to exec DeleteCsWorkHistory because: " + ex.ToString());

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
            List<CsCertificateModel> myList = null;
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
                        myList = JsonConvert.DeserializeObject<List<CsCertificateModel>>(apiReturn.value.ToString());
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
            List<CsCertificateModel> myList = null;
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
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                var cs_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await CsServices.GetCertificateAsync(apiModel, cs_id);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<CsCertificateModel>>(apiReturn.value.ToString());
                        if (myList.HasData())
                        {
                            foreach (var item in myList)
                            {
                                item.isDefault = true;
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

        public ActionResult UpdateCsCertificate(int id = 0, int cs_id = 0)
        {
            var model = new CsCertificateModel();
            try
            {
                model.id = id;
                model.cs_id = cs_id;

                if (model.id > 0)
                {
                    var apiModel = new ApiCsCertificateModel();
                    apiModel.id = model.id;
                    apiModel.cs_id = model.cs_id;
                    //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();

                    var apiReturned = CertificateServices.CsGetDetailAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityCsCertificate info = JsonConvert.DeserializeObject<IdentityCsCertificate>(apiReturned.value.ToString());
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
                var strError = string.Format("Failed when trying display UpdateCsCertificate because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_UpdateCsCertificate", model);
        }

        [HttpPost]
        [ActionName("UpdateCsCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmUpdateCsCertificate(CsCertificateModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;
            try
            {
                var apiModel = new ApiCsCertificateModel();
                apiModel.id = model.id;
                apiModel.cs_id = model.cs_id;
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                apiModel.name = model.name;
                apiModel.start_date = model.start_date_str;
                apiModel.end_date = model.end_date_str;
                apiModel.status = Utils.ConvertToIntFromQuest(model.status);
                apiModel.pass = model.pass;
                apiModel.point = model.point;

                if (apiModel.cs_id > 0)
                {
                    var apiReturned = await CertificateServices.CsUpdateAsync(apiModel);
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

                htmlReturn = PartialViewAsString("../Cs/Partials/_CsCertificateItem", model);
                return Json(new { success = true, html = htmlReturn });
            }
            catch (Exception ex)
            {
                message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to exec ConfirmUpdateCsCertificate because: " + ex.ToString());

                return Json(new { success = false, message = message });
            }

            //return Json(new { success = false, message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, title = UserWebResource.LB_ERROR });
        }

        public ActionResult DeleteCsCertificate(int id = 0, int cs_id = 0)
        {
            var model = new CsCertificateModel();
            try
            {
                model.id = id;
                model.cs_id = cs_id;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying display DeleteCsCertificate because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Partials/_DeleteCsCertificate", model);
        }

        [HttpPost]
        [ActionName("DeleteCsCertificate")]
        [VerifyLoggedInUser]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteCsCertificate(CsCertificateModel model)
        {
            var message = string.Empty;
            var htmlReturn = string.Empty;

            try
            {
                var apiModel = new ApiCsCertificateModel();
                apiModel.id = model.id;
                apiModel.cs_id = model.cs_id;

                if (apiModel.id > 0)
                {
                    var apiReturned = await CertificateServices.CsDeleteAsync(apiModel);
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

                logger.Error("Failed to exec DeleteCsCertificate because: " + ex.ToString());

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

            //var address_contact = new IdentityJobSeekerAddress();
            //address_contact.id = model.address_contact.id;
            //address_contact.job_seeker_id = info.user_id;
            //address_contact.country_id = model.address_contact.country_id;
            //address_contact.region_id = model.address_contact.region_id;
            //address_contact.prefecture_id = model.address_contact.prefecture_id;
            //address_contact.city_id = model.address_contact.city_id;
            //address_contact.postal_code = model.address_contact.postal_code;
            //address_contact.detail = model.address_contact.detail;
            //address_contact.furigana = model.address_contact.furigana;
            //address_contact.train_line_id = model.address_contact.train_line_id;
            //address_contact.station_id = model.address_contact.station_id;

            info.Addresses.Add(address);
            //info.Addresses.Add(address_contact);

            return info;
        }

        private CsUpdateModel ParseDataToForm(IdentityCs identity)
        {
            var model = new CsUpdateModel();
            model.id = identity.id;
            model.title = identity.cs_title;          

            model.created_date = identity.date.DateTimeQuestToString(DATE_FORMAT);
            model.email = identity.email;
            model.phone = identity.phone;
            model.fullname = identity.fullname;
            model.fullname_furigana = identity.fullname_furigana;
            model.image = identity.image;
            model.image_full_path = identity.image_full_path;
            model.birthday = identity.birthday.DateTimeQuestToString(DATE_FORMAT);
            model.gender = identity.gender;
            model.qualification_id = identity.highest_edu;
            
            return model;
        }

        private ApiCsUpdateModel ExtractCsFormData(CsUpdateModel model)
        {
            var updateInfo = new ApiCsUpdateModel();

            var info = new ApiCsModel();
            info.id = model.id;
            info.cs_title = model.title;
            info.highest_edu = model.qualification_id;         

            var dt = Utils.ConvertStringToDateTimeQuestByFormat(model.created_date, DATE_FORMAT);
            if(dt != null)
            {
                info.date = dt.DateTimeQuestToString("yyyy-MM-dd");
            }

            info.job_seeker_id = AccountHelper.GetCurrentUserId();

            //info.email = model.email;
            //info.phone = model.phone;
            //info.fullname = model.fullname;
            //info.fullname_furigana = model.fullname_furigana;            
            //info.image = model.image;
            //info.birthday = model.birthday;
            //info.gender = model.gender;
            //info.train_line_id = model.address.train_line_id;
            //info.station_id = model.address.station_id;

            //var address = new ApiCsAddressModel();
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

            //var address_contact = new ApiCsAddressModel();
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

            updateInfo.cs = info;
            //updateInfo.address = address;
            //updateInfo.address_contact = address_contact;

            //updateInfo.edu_history = ExtractCsEduHistory(model);
            updateInfo.work_history = ExtractCsWorkHistory(model);
            //updateInfo.certification = ExtractCsCertificate(model);

            return updateInfo;
        }

        private List<ApiCsEduHistoryModel> ExtractCsEduHistory(CsUpdateModel model)
        {
            List<ApiCsEduHistoryModel> myList = null;
            if (model == null)
                return null;

            if (model.EduHistories.HasData())
            {
                myList = new List<ApiCsEduHistoryModel>();

                foreach (var item in model.EduHistories)
                {
                    if (!item.isDefault && model.id != 0)
                        continue;

                    var newItem = new ApiCsEduHistoryModel();
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

        private List<ApiCsWorkHistoryModel> ExtractCsWorkHistory(CsUpdateModel model)
        {
            List<ApiCsWorkHistoryModel> myList = null;
            if (model == null)
                return null;

            if (model.WorkHistories.HasData())
            {
                myList = new List<ApiCsWorkHistoryModel>();

                foreach (var item in model.WorkHistories)
                {
                    //if (!item.isDefault && model.id != 0)
                    //    continue;

                    if (!string.IsNullOrEmpty(item.json_obj))
                    {
                        var myObj = JsonConvert.DeserializeObject<CsWorkHistoryModel>(item.json_obj);

                        var newItem = new ApiCsWorkHistoryModel();
                        newItem.id = myObj.id;
                        newItem.company = myObj.company;
                        newItem.sub_field_id = myObj.sub_field_id;
                        newItem.sub_industry_id = myObj.sub_industry_id;
                        newItem.employment_type_id = myObj.employment_type_id;
                        newItem.employees_number = myObj.employees_number;
                        newItem.resign_reason = myObj.resign_reason;
                        newItem.start_date = myObj.start_date_str;
                        newItem.end_date = myObj.end_date_str;
                        newItem.status = myObj.status;
                        newItem.address = myObj.address;

                        if (myObj.Details.HasData())
                        {
                            newItem.Details = new List<ApiCsWorkHistoryDetailModel>();
                            foreach (var dt in myObj.Details)
                            {
                                var dtApi = new ApiCsWorkHistoryDetailModel();
                                dtApi.id = dt.id;
                                dtApi.department = dt.department;
                                dtApi.position = dt.position;
                                dtApi.content_work = dt.content_work;
                                dtApi.salary = dt.salary;
                                dtApi.start_date = dt.start_date;
                                dtApi.end_date = dt.end_date;

                                newItem.Details.Add(dtApi);
                            }
                        }

                        myList.Add(newItem);
                    }                    
                }
            }

            return myList;
        }

        private List<ApiCsCertificateModel> ExtractCsCertificate(CsUpdateModel model)
        {
            List<ApiCsCertificateModel> myList = null;
            if (model == null)
                return null;

            if (model.Certificates.HasData())
            {
                myList = new List<ApiCsCertificateModel>();

                foreach (var item in model.Certificates)
                {
                    if (!item.isDefault && model.id != 0)
                        continue;

                    var newItem = new ApiCsCertificateModel();
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
