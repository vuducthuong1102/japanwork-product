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

namespace Manager.WebApp.Controllers
{
    public class CvController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<CvController>();

        [AccessRoleChecker]
        [IsValidURLRequest]
        public ActionResult Preview(int? id)
        {
            var model = new ManageCvPreviewModel();
            try
            {
                if (id <= 0)
                    RedirectToErrorPage();

                var cvId = Utils.ConvertToIntFromQuest(id);
                
                var apiResult = CvServices.GetDetailAsync(cvId).Result;

                if(apiResult != null)
                {
                    if(apiResult.value != null)
                    {
                        model.CvInfo = JsonConvert.DeserializeObject<IdentityCv>(apiResult.value.ToString());
                        if(model.CvInfo != null)
                        {
                            var qualifications = CommonHelpers.GetListQualifications();
                            if (qualifications.HasData())
                            {
                                var matchedQualifi = qualifications.Where(x => x.id == model.CvInfo.highest_edu).FirstOrDefault();
                                if(matchedQualifi != null)
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

                            if(model.CvInfo.address != null)
                            {
                                model.address = new ManageCvPreviewAddressModel();
                                model.address.AddressInfo = model.CvInfo.address;

                                //model.address.country_name = allCountries.Where(x => x.id == model.CvInfo.address.country_id).Select(x => x.country).FirstOrDefault();
                                if(model.CvInfo.address.region_id > 0)
                                    model.address.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address.region_id).FirstOrDefault();

                                if (model.CvInfo.address.prefecture_id > 0)
                                    model.address.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address.prefecture_id).FirstOrDefault();

                                if (model.CvInfo.address.city_id > 0)
                                    model.address.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address.city_id).FirstOrDefault();                               
                            }

                            if(model.CvInfo.address_contact != null)
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

                return View(model);
            }

            return View(model);
        }

        //[IsValidURLRequest]
        //public ActionResult PrintPDF(int? id)
        //{
        //    var model = new ManageCvPreviewModel();
        //    try
        //    {
        //        if (id <= 0)
        //            RedirectToErrorPage();

        //        var cvId = Utils.ConvertToIntFromQuest(id);

        //        var apiResult = CvServices.GetDetailAsync(cvId).Result;

        //        if (apiResult != null)
        //        {
        //            if (apiResult.value != null)
        //            {
        //                model.CvInfo = JsonConvert.DeserializeObject<IdentityCv>(apiResult.value.ToString());
        //                if (model.CvInfo != null)
        //                {
        //                    var qualifications = CommonHelpers.GetListQualifications();
        //                    if (qualifications.HasData())
        //                    {
        //                        var matchedQualifi = qualifications.Where(x => x.id == model.CvInfo.highest_edu).FirstOrDefault();
        //                        if (matchedQualifi != null)
        //                        {
        //                            if (matchedQualifi.LangList.HasData())
        //                                model.qualification_label = matchedQualifi.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.qualification).FirstOrDefault();

        //                            if (string.IsNullOrEmpty(model.qualification_label))
        //                                model.qualification_label = matchedQualifi.qualification;
        //                        }
        //                    }

        //                    var countryIds = new List<int>();
        //                    var regionIds = new List<int>();
        //                    var prefectureIds = new List<int>();
        //                    var cityIds = new List<int>();
        //                    var stationIds = new List<int>();

        //                    if (model.CvInfo.address != null)
        //                    {
        //                        if (model.CvInfo.address.country_id > 0)
        //                            countryIds.Add(model.CvInfo.address.country_id);

        //                        if (model.CvInfo.address.region_id > 0)
        //                            regionIds.Add(model.CvInfo.address.region_id);

        //                        if (model.CvInfo.address.prefecture_id > 0)
        //                            prefectureIds.Add(model.CvInfo.address.prefecture_id);

        //                        if (model.CvInfo.address.city_id > 0)
        //                            prefectureIds.Add(model.CvInfo.address.city_id);

        //                        if (model.CvInfo.address.station_id > 0)
        //                            stationIds.Add(model.CvInfo.address.station_id);
        //                    }

        //                    if (model.CvInfo.address_contact != null)
        //                    {
        //                        if (model.CvInfo.address_contact.country_id > 0)
        //                            countryIds.Add(model.CvInfo.address_contact.country_id);

        //                        if (model.CvInfo.address_contact.region_id > 0)
        //                            regionIds.Add(model.CvInfo.address_contact.region_id);

        //                        if (model.CvInfo.address_contact.prefecture_id > 0)
        //                            prefectureIds.Add(model.CvInfo.address_contact.prefecture_id);

        //                        if (model.CvInfo.address_contact.city_id > 0)
        //                            prefectureIds.Add(model.CvInfo.address_contact.city_id);

        //                        if (model.CvInfo.address_contact.station_id > 0)
        //                            stationIds.Add(model.CvInfo.address_contact.station_id);
        //                    }

        //                    //var allCountries = CommonHelpers.GetListCountries();
        //                    var allRegions = CommonHelpers.GetListRegions(regionIds);
        //                    var allPrefectures = CommonHelpers.GetListPrefectures(prefectureIds);
        //                    var allCities = CommonHelpers.GetListCities(cityIds);
        //                    //var allStations = CommonHelpers.GetListStations(stationIds);

        //                    if (model.CvInfo.address != null)
        //                    {
        //                        model.address = new ManageCvPreviewAddressModel();
        //                        model.address.AddressInfo = model.CvInfo.address;

        //                        //model.address.country_name = allCountries.Where(x => x.id == model.CvInfo.address.country_id).Select(x => x.country).FirstOrDefault();
        //                        if (model.CvInfo.address.region_id > 0)
        //                            model.address.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address.region_id).FirstOrDefault();

        //                        if (model.CvInfo.address.prefecture_id > 0)
        //                            model.address.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address.prefecture_id).FirstOrDefault();

        //                        if (model.CvInfo.address.city_id > 0)
        //                            model.address.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address.city_id).FirstOrDefault();
        //                    }

        //                    if (model.CvInfo.address_contact != null)
        //                    {
        //                        model.address_contact = new ManageCvPreviewAddressModel();
        //                        model.address_contact.AddressInfo = model.CvInfo.address_contact;

        //                        //model.address_contact.country_name = allCountries.Where(x => x.id == model.CvInfo.address_contact.country_id).Select(x => x.country).FirstOrDefault();
        //                        if (model.CvInfo.address_contact.region_id > 0)
        //                            model.address_contact.AddressInfo.region_info = allRegions.Where(x => x.id == model.CvInfo.address_contact.region_id).FirstOrDefault();

        //                        if (model.CvInfo.address_contact.prefecture_id > 0)
        //                            model.address_contact.AddressInfo.prefecture_info = allPrefectures.Where(x => x.id == model.CvInfo.address_contact.prefecture_id).FirstOrDefault();

        //                        if (model.CvInfo.address_contact.city_id > 0)
        //                            model.address_contact.AddressInfo.city_info = allCities.Where(x => x.id == model.CvInfo.address_contact.city_id).FirstOrDefault();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

        //        logger.Error("Failed to Preview because: " + ex.ToString());
        //    }
        //    if (model.CvInfo.form == 0)
        //    {
        //        return new PartialViewAsPdf("Partials/_FullTime", model)
        //        {
        //            FileName = "CvFullTime.pdf"
        //        };
        //    }
        //    else
        //    {
        //        return new PartialViewAsPdf("Partials/_PartTime", model)
        //        {
        //            FileName = "CvPartTime.pdf"
        //        };
        //    }
            
        //}

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EduHistories(int? id)
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
                var cv_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await CvServices.GetEduHistoryAsync(apiModel, cv_id);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<CvEduHistoryModel>>(apiReturn.value.ToString());

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
        public async Task<ActionResult> WorkHistories(int? id)
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
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                var cv_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await CvServices.GetWorkHistoryAsync(apiModel, cv_id);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<CvWorkHistoryModel>>(apiReturn.value.ToString());
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

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> Certificates(int? id)
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
                //apiModel.job_seeker_id = AccountHelper.GetCurrentUserId();
                var cv_id = Utils.ConvertToIntFromQuest(id);

                var apiReturn = await CvServices.GetCertificateAsync(apiModel, cv_id);
                if (apiReturn != null)
                {
                    if (apiReturn.value != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<CvCertificateModel>>(apiReturn.value.ToString());
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

        #region Helpers


        #endregion

    }
}