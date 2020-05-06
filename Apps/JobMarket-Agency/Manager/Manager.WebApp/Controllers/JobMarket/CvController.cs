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
using System.Threading.Tasks;
using Rotativa;

namespace Manager.WebApp.Controllers
{
    public class CvController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<CvController>();

        [IsValidURLRequest]
        public ActionResult Preview(int? id, int? status, int? show_more, int type_job_seeker = 0, int hide_pf = 0)
        {
            var model = new ManageCvPreviewModel();
            try
            {
                if (id <= 0)
                    RedirectToErrorPage();
                if (type_job_seeker == 0)
                {

                    var cvId = Utils.ConvertToIntFromQuest(id);
                    var apiResult = CvServices.GetDetailAsync(cvId).Result;
                    var job_seeker_id = 0;
                    if (apiResult != null)
                    {
                        if (apiResult.value != null)
                        {
                            IdentityCv resultCvInfo = JsonConvert.DeserializeObject<IdentityCv>(apiResult.value.ToString());
                            if (resultCvInfo != null)
                            {
                                if (resultCvInfo.jobseeker != null)
                                {
                                    model = ParseDataToForm(resultCvInfo.jobseeker);
                                    model.CvInfo.hobby_skills = resultCvInfo.hobby_skills;
                                    model.CvInfo.pr = resultCvInfo.pr;
                                    model.CvInfo.reason = resultCvInfo.reason;
                                    model.CvInfo.reason_pr = resultCvInfo.reason_pr;
                                    model.CvInfo.id = resultCvInfo.id;
                                    job_seeker_id = model.CvInfo.job_seeker_id;
                                }
                            }
                            //if (model.CvInfo != null)
                            //{
                            //    model.CvInfo.status = Utils.ConvertToIntFromQuest(status);
                            //    var qualifications = CommonHelpers.GetListQualifications();
                            //    if (qualifications.HasData())
                            //    {
                            //        var matchedQualifi = qualifications.Where(x => x.id == model.CvInfo.highest_edu).FirstOrDefault();
                            //        if (matchedQualifi != null)
                            //        {
                            //            if (matchedQualifi.LangList.HasData())
                            //                model.qualification_label = matchedQualifi.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();

                            //            if (string.IsNullOrEmpty(model.qualification_label))
                            //                model.qualification_label = matchedQualifi.qualification;
                            //        }
                            //    }

                            //    var countryIds = new List<int>();
                            //    var regionIds = new List<int>();
                            //    var prefectureIds = new List<int>();
                            //    var cityIds = new List<int>();
                            //    var stationIds = new List<int>();

                            //    if (model.CvInfo.address != null)
                            //    {
                            //        if (model.CvInfo.address.country_id > 0)
                            //            countryIds.Add(model.CvInfo.address.country_id);

                            //        if (model.CvInfo.address.region_id > 0)
                            //            regionIds.Add(model.CvInfo.address.region_id);

                            //        if (model.CvInfo.address.prefecture_id > 0)
                            //            prefectureIds.Add(model.CvInfo.address.prefecture_id);

                            //        if (model.CvInfo.address.city_id > 0)
                            //            prefectureIds.Add(model.CvInfo.address.city_id);

                            //        if (model.CvInfo.address.station_id > 0)
                            //            stationIds.Add(model.CvInfo.address.station_id);
                            //    }

                            //    if (model.CvInfo.address_contact != null)
                            //    {
                            //        if (model.CvInfo.address_contact.country_id > 0)
                            //            countryIds.Add(model.CvInfo.address_contact.country_id);

                            //        if (model.CvInfo.address_contact.region_id > 0)
                            //            regionIds.Add(model.CvInfo.address_contact.region_id);

                            //        if (model.CvInfo.address_contact.prefecture_id > 0)
                            //            prefectureIds.Add(model.CvInfo.address_contact.prefecture_id);

                            //        if (model.CvInfo.address_contact.city_id > 0)
                            //            prefectureIds.Add(model.CvInfo.address_contact.city_id);

                            //        if (model.CvInfo.address_contact.station_id > 0)
                            //            stationIds.Add(model.CvInfo.address_contact.station_id);
                            //    }

                            //    //var allCountries = CommonHelpers.GetListCountries();
                            //    var allRegions = CommonHelpers.GetListRegions(regionIds);
                            //    var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
                            //    var allCities = CommonHelpers.GetListCities(cityIds);
                            //    //var allStations = CommonHelpers.GetListStations(stationIds);

                            //    if (model.CvInfo.address != null)
                            //    {
                            //        model.address = new ManageCvPreviewAddressModel();
                            //        model.address.AddressInfo = model.CvInfo.address;

                            //        //model.address.country_name = allCountries.Where(x => x.id == model.CvInfo.address.country_id).Select(x => x.country).FirstOrDefault();
                            //        if (model.CvInfo.address.region_id > 0)
                            //            model.address.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address.region_id).FirstOrDefault();

                            //        if (model.CvInfo.address.prefecture_id > 0)
                            //            model.address.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address.prefecture_id).FirstOrDefault();

                            //        if (model.CvInfo.address.city_id > 0)
                            //            model.address.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address.city_id).FirstOrDefault();
                            //    }

                            //    if (model.CvInfo.address_contact != null)
                            //    {
                            //        model.address_contact = new ManageCvPreviewAddressModel();
                            //        model.address_contact.AddressInfo = model.CvInfo.address_contact;

                            //        //model.address_contact.country_name = allCountries.Where(x => x.id == model.CvInfo.address_contact.country_id).Select(x => x.country).FirstOrDefault();
                            //        if (model.CvInfo.address_contact.region_id > 0)
                            //            model.address_contact.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address_contact.region_id).FirstOrDefault();

                            //        if (model.CvInfo.address_contact.prefecture_id > 0)
                            //            model.address_contact.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address_contact.prefecture_id).FirstOrDefault();

                            //        if (model.CvInfo.address_contact.city_id > 0)
                            //            model.address_contact.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address_contact.city_id).FirstOrDefault();
                            //    }
                            //}
                        }
                    }

                    if (job_seeker_id > 0)
                    {
                        var apiModel = new ApiGetListByPageModel();
                        apiModel.agency_id = GetCurrentAgencyId();
                        apiModel.job_seeker_id = job_seeker_id;

                        var apiReturned = JobSeekerServices.GetAllCVsSentToAgencyAsync(apiModel).Result;
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
                }
                else
                {
                    var jobSeekerId = Utils.ConvertToIntFromQuest(id);
                    var apiModel = new ApiJobSeekerModel();
                    apiModel.id = jobSeekerId;

                    var apiReturned = A_JobSeekerServices.GetDetailForUpdateAsync(apiModel).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            IdentityJobSeeker info = JsonConvert.DeserializeObject<IdentityJobSeeker>(apiReturned.value.ToString());
                            if (info != null)
                            {
                                if (info.agency_id != GetCurrentAgencyId())
                                    return RedirectToErrorPage();

                                model = ParseDataToForm(info);

                                model.CvInfo.job_seeker_id = info.id;
                                model.type_job_seeker = type_job_seeker;
                                model.CvInfo.jobseeker = info;
                                var listUser = CommonHelpers.GetListUser(GetCurrentAgencyId());
                                if (listUser.HasData())
                                {
                                    model.staff_info = listUser.FirstOrDefault(s => s.StaffId == info.staff_id);
                                }
                            }
                            else
                            {
                                return RedirectToErrorPage();
                            }
                        }
                    }
                }

                //if (model.WishInfo != null)
                //{
                //    var listIds = Utils.ConvertStringToListInt(model.WishInfo.prefecture_ids);
                //    model.Prefectures = CommonHelpers.GetListPrefectures(listIds);
                //}

                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.SubFields = CommonHelpers.GetListSubFields();
                model.Countries = CommonHelpers.GetListCountries();
                model.Visas = CommonHelpers.GetListVisas();
                model.hide_pf = hide_pf;

                model.WishList = new List<IdentityJobSeekerWish>();
                var listIds = new List<int>();
                if (id > 0)
                {
                    var jobSeekerId = Utils.ConvertToIntFromQuest(id);
                    var apiReturned = A_JobSeekerWishServices.GetDetailAsync(jobSeekerId).Result;
                    if (apiReturned != null)
                    {
                        if (apiReturned.value != null)
                        {
                            List<IdentityJobSeekerWish> info = JsonConvert.DeserializeObject<List<IdentityJobSeekerWish>>(apiReturned.value.ToString());
                            if (info.HasData())
                            {
                                model.WishList = info;

                                foreach (var item in info)
                                {
                                    if (!string.IsNullOrEmpty(item.prefecture_ids))
                                    {
                                        var list_prefecture_ids = Utils.ConvertStringToListInt(item.prefecture_ids);
                                        foreach (var prefecture_id in list_prefecture_ids)
                                        {
                                            if (listIds.IndexOf(prefecture_id) == -1)
                                            {
                                                listIds.Add(prefecture_id);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                model.Prefectures = CommonHelpers.GetListPrefectures(listIds);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to Preview because: " + ex.ToString());

                return View(model);
            }
            model.show_more = Utils.ConvertToIntFromQuest(show_more);
            return View(model);
        }

        private ManageCvPreviewModel ParseDataToForm(IdentityJobSeeker identity)
        {
            var result = new ManageCvPreviewModel();
            result.CvInfo = new IdentityCv();
            result.CvInfo.job_seeker_id = identity.user_id;
            result.CvInfo.id = identity.user_id;
            result.CvInfo.email = identity.email;
            result.CvInfo.phone = identity.phone;
            result.CvInfo.marriage = (identity.marriage == 0 ? false : true);
            result.CvInfo.dependent_num = identity.dependent_num;
            result.CvInfo.fullname = identity.fullname;
            result.CvInfo.fullname_furigana = identity.fullname_furigana;
            result.CvInfo.image = identity.image;
            result.CvInfo.birthday = identity.birthday;
            result.CvInfo.date = identity.created_at;
            result.CvInfo.gender = identity.gender;
            result.CvInfo.status = 1;
            result.CvInfo.qualification_id = identity.qualification_id;
            result.CvInfo.nationality_id = identity.nationality_id;
            result.CvInfo.visa_id = identity.visa_id;
            result.CvInfo.duration_visa = identity.duration_visa;
            result.CvInfo.religion_detail = identity.religion_detail;
            result.created_at = identity.created_at;

            var qualifications = CommonHelpers.GetListQualifications();
            if (qualifications.HasData())
            {
                var matchedQualifi = qualifications.Where(x => x.id == result.CvInfo.qualification_id).FirstOrDefault();
                if (matchedQualifi != null)
                {
                    if (matchedQualifi.LangList.HasData())
                        result.qualification_label = matchedQualifi.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();

                    if (string.IsNullOrEmpty(result.qualification_label))
                        result.qualification_label = matchedQualifi.qualification;
                }
            }
            if (identity.WishInfo != null)
            {
                result.WishInfo = identity.WishInfo;
            }
            //result.CvInfo.video_path = identity.video_path;
            //result.CvInfo.expected_job_title = identity.expected_job_title;
            //result.CvInfo.expected_salary_min = identity.expected_salary_min;
            //result.CvInfo.expected_salary_max = identity.expected_salary_max;
            //result.CvInfo.work_status = identity.work_status;
            //result.CvInfo.qualification_id = identity.qualification_id;
            //result.CvInfo.job_seeking_status_id = identity.job_seeking_status_id;
            //result.CvInfo.salary_type_id = identity.salary_type_id;
            result.CvInfo.japanese_level_number = identity.japanese_level_number;
            //result.pi = identity.pic_id;
            result.address = new ManageCvPreviewAddressModel();
            result.address_contact = new ManageCvPreviewAddressModel();
            if (identity.Addresses.HasData())
            {
                result.address.address_full = identity.Addresses[0].address_furigana;
                var currentLang = GetCurrentLanguageOrDefault();
                if (currentLang == "ja-JP")
                {
                    result.address.address_full = identity.Addresses[0].address;
                }
            }
            if (identity.Addresses != null && identity.Addresses.Count > 1)
            {
                result.address_contact.address_full = identity.Addresses[1].address_furigana;
                var currentLang = GetCurrentLanguageOrDefault();
                if (currentLang == "ja-JP")
                {
                    result.address_contact.address_full = identity.Addresses[1].address;
                }
            }
            result.CvInfo.Extensions = identity.Extensions;
            return result;
        }

        [IsValidURLRequest]
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
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to Preview because: " + ex.ToString());
            }
            if (model.CvInfo.form == 0)
            {
                return new PartialViewAsPdf("Partials/_FullTime", model)
                {
                    FileName = "CvFullTime.pdf"
                };
            }
            else
            {
                return new PartialViewAsPdf("Partials/_PartTime", model)
                {
                    FileName = "CvPartTime.pdf"
                };
            }

        }

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EduHistories(int? id, int? type)
        {
            List<CvEduHistoryModel> myList = null;
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
                apiModel.job_seeker_id = Utils.ConvertToIntFromQuest(id);
                if (type == 0)
                {
                    //var cv_id = Utils.ConvertToIntFromQuest(id);
                    //var apiReturn = await CvServices.GetEduHistoryAsync(apiModel, cv_id);
                    //if (apiReturn != null)
                    //{
                    //    if (apiReturn.value != null)
                    //    {
                    //        myList = JsonConvert.DeserializeObject<List<CvEduHistoryModel>>(apiReturn.value.ToString());

                    //        if (myList.HasData())
                    //        {
                    //            var qualifications = CommonHelpers.GetListQualifications();
                    //            var majors = CommonHelpers.GetListMajors();
                    //            var counter = 0;
                    //            foreach (var item in myList)
                    //            {
                    //                item.isDefault = true;
                    //                var qualification = qualifications.Where(x => x.id == item.qualification_id).FirstOrDefault();
                    //                if (qualification != null)
                    //                {
                    //                    item.qualification_label = qualification.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();
                    //                    if (string.IsNullOrEmpty(item.qualification_label))
                    //                        item.qualification_label = qualification.qualification;
                    //                }

                    //                var major = majors.Where(x => x.id == item.major_id).FirstOrDefault();
                    //                if (major != null)
                    //                {
                    //                    item.major_label = major.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.major).FirstOrDefault();
                    //                    if (string.IsNullOrEmpty(item.major_label))
                    //                        item.major_label = major.major;
                    //                }

                    //                counter++;
                    //            }
                    //        }
                    //    }
                    //}

                    var apiReturn = await JobSeekerServices.GetEduHistoryAsync(apiModel);
                    if (apiReturn != null)
                    {
                        if (apiReturn.value != null)
                        {
                            myList = JsonConvert.DeserializeObject<List<CvEduHistoryModel>>(apiReturn.value.ToString());

                            if (myList.HasData())
                            {
                                var qualifications = CommonHelpers.GetListQualifications();
                                var majors = CommonHelpers.GetListMajors();
                                foreach (var item in myList)
                                {
                                    var qualification = qualifications.Where(x => x.id == item.qualification_id).FirstOrDefault();
                                    if (qualification != null)
                                    {
                                        item.qualification_label = qualification.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();
                                        if (string.IsNullOrEmpty(item.qualification_label))
                                            item.qualification_label = qualification.qualification;
                                    }

                                    var major = majors.Where(x => x.id == item.major_id).FirstOrDefault();
                                    if (major != null)
                                    {
                                        item.major_label = major.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.major).FirstOrDefault();
                                        if (string.IsNullOrEmpty(item.major_label))
                                            item.major_label = major.major;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var apiReturn = await A_JobSeekerServices.GetEduHistoryAsync(apiModel);
                    if (apiReturn != null)
                    {
                        if (apiReturn.value != null)
                        {
                            myList = JsonConvert.DeserializeObject<List<CvEduHistoryModel>>(apiReturn.value.ToString());

                            if (myList.HasData())
                            {
                                var qualifications = CommonHelpers.GetListQualifications();
                                var majors = CommonHelpers.GetListMajors();
                                foreach (var item in myList)
                                {
                                    var qualification = qualifications.Where(x => x.id == item.qualification_id).FirstOrDefault();
                                    if (qualification != null)
                                    {
                                        item.qualification_label = qualification.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();
                                        if (string.IsNullOrEmpty(item.qualification_label))
                                            item.qualification_label = qualification.qualification;
                                    }

                                    var major = majors.Where(x => x.id == item.major_id).FirstOrDefault();
                                    if (major != null)
                                    {
                                        item.major_label = major.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.major).FirstOrDefault();
                                        if (string.IsNullOrEmpty(item.major_label))
                                            item.major_label = major.major;
                                    }
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

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> WorkHistories(int? id, int? type)
        {
            List<CvWorkHistoryModel> myList = null;
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
                apiModel.job_seeker_id = Utils.ConvertToIntFromQuest(id);

                if (type == 0)
                {

                    var apiReturn = await JobSeekerServices.GetWorkHistoryAsync(apiModel);
                    if (apiReturn != null)
                    {
                        if (apiReturn.value != null)
                        {
                            myList = JsonConvert.DeserializeObject<List<CvWorkHistoryModel>>(apiReturn.value.ToString());
                        }
                    }
                    //var cv_id = Utils.ConvertToIntFromQuest(id);
                    //var apiReturn = await CvServices.GetWorkHistoryAsync(apiModel, cv_id);
                    //if (apiReturn != null)
                    //{
                    //    if (apiReturn.value != null)
                    //    {
                    //        myList = JsonConvert.DeserializeObject<List<CvWorkHistoryModel>>(apiReturn.value.ToString());
                    //        if (myList.HasData())
                    //        {
                    //            foreach (var item in myList)
                    //            {
                    //                item.isDefault = true;
                    //            }
                    //        }
                    //    }
                    //}
                }
                else
                {
                    var apiReturn = await A_JobSeekerServices.GetWorkHistoryAsync(apiModel);
                    if (apiReturn != null)
                    {
                        if (apiReturn.value != null)
                        {
                            myList = JsonConvert.DeserializeObject<List<CvWorkHistoryModel>>(apiReturn.value.ToString());
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
        public async Task<ActionResult> Certificates(int? id, int? type)
        {
            List<CvCertificateModel> myList = null;
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
                apiModel.job_seeker_id = Utils.ConvertToIntFromQuest(id);
                if (type == 0)
                {
                    var apiReturn = await JobSeekerServices.GetCertificateAsync(apiModel);
                    if (apiReturn != null)
                    {
                        if (apiReturn.value != null)
                        {
                            myList = JsonConvert.DeserializeObject<List<CvCertificateModel>>(apiReturn.value.ToString());
                        }
                    }
                }
                else
                {
                    var apiReturn = await A_JobSeekerServices.GetCertificateAsync(apiModel);
                    if (apiReturn != null)
                    {
                        if (apiReturn.value != null)
                        {
                            myList = JsonConvert.DeserializeObject<List<CvCertificateModel>>(apiReturn.value.ToString());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListCvInDropdown()
        {
            var strError = string.Empty;
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;

                int currentPage = 1;
                int pageSize = SystemSettings.DefaultPageSize;

                var apiFilterModel = new ApiCvSuggestionModel
                {
                    job_id = jobId,
                    keyword = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : null,
                    page_index = currentPage,
                    page_size = pageSize
                };

                var apiResult = CvServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;
                List<IdentityCv> listData = new List<IdentityCv>();
                if (apiResult != null && apiResult.value != null)
                {
                    listData = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                }

                var returnList = new List<CvItemInDropdownListModel>();
                if (listData.HasData())
                {
                    foreach (var record in listData)
                    {
                        var item = new CvItemInDropdownListModel();
                        item.id = record.id;
                        item.fullname = record.fullname;
                        item.fullname_furigana = record.fullname_furigana;
                        item.phone = record.phone;
                        item.email = record.email;
                        item.id = record.id;

                        returnList.Add(item);
                    }
                }

                return Json(new { success = true, data = returnList });

            }
            catch (Exception ex)
            {
                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to GetListCvInDropdown because: " + ex.ToString());

                return Json(new { success = false, data = string.Empty, message = strError });
            }
        }

        public ActionResult ChoosenCv(CvChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var apiFilterModel = new ApiCvSuggestionModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize
            };

            try
            {
                var apiResult = CvServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                }

                if (model.SearchResults.HasData())
                {
                    model.TotalCount = model.SearchResults[0].total_count;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to show ChoosenCv form: " + ex.ToString());
            }

            return PartialView("Partials/_ChoosenCv", model);
        }

        [HttpPost]
        public ActionResult ChoosenCvSearch(CvChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            if (Request["CurrentPage"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);
            }

            var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;

            var apiFilterModel = new ApiCvSuggestionModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                job_id = jobId,
                page_index = currentPage,
                page_size = pageSize
            };

            try
            {
                var apiResult = CvServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                }
                if (model.SearchResults.HasData())
                {
                    model.TotalCount = model.SearchResults[0].total_count;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to show _ChoosenCvList form: " + ex.ToString());
            }

            return PartialView("Partials/_ChoosenCvList", model);
        }

        #region Helpers


        #endregion

    }
}