using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Collections.Generic;
using System;
using MySite.Helpers;
using ApiJobMarket.DB.Sql.Entities;
using MySite.Caching;
using System.Linq;
using MySite.Settings;
using MySite.Services;
using Newtonsoft.Json;
using MySite.ShareLibs;
using System.Globalization;
using System.Threading;
using MySite.Resources;
using MySite.Attributes;
using System.Threading.Tasks;

namespace MySite.Controllers
{
    public class MasterController : BaseController
    {
        private readonly string currentLang = UserCookieManager.GetCurrentLanguageOrDefault();

        private readonly ILog logger = LogProvider.For<MasterController>();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public ActionResult GetSearchJobBox()
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Content("<div class='SearchJobError'>" + message + "</div>");
            }

            JobSearchModel model = new JobSearchModel();

            if (model.CurrentPage <= 0)
                model.CurrentPage = 1;

            if (string.IsNullOrEmpty(model.sorting_date))
                model.sorting_date = "desc";

            try
            {
                model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
                model.FieldLists = CommonHelpers.GetListFields();
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display GetSearchJobBox because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("~/Views/Widgets/_SearchJobBox.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public JsonResult GetResources()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(currentLang);
            var resources = ResourceSerialiser.ToJson(typeof(UserWebResource), currentLang);

            return Json(resources, JsonRequestBehavior.AllowGet);
        }

        private List<TreeViewItemModel> GetListChildOfIndustry(IdentityIndustry currentItem, List<int> checkedList)
        {
            List<TreeViewItemModel> listData = new List<TreeViewItemModel>();
            foreach (var item in currentItem.Sub_industries)
            {
                var treeItem = new TreeViewItemModel();
                treeItem.id = "sub_industry_" + item.id;

                var matchedChecked = checkedList.Where(x => x == item.id).FirstOrDefault();
                if (matchedChecked > 0)
                    treeItem.@checked = true;

                var currentSubIndustryName = item.LangList.Where(x => x.language_code == currentLang).Select(x => x.sub_industry).FirstOrDefault();

                if (string.IsNullOrEmpty(currentSubIndustryName))
                    currentSubIndustryName = item.sub_industry;

                treeItem.text = currentSubIndustryName + "<span class='sub-industry hidden' data-id='" + item.id + "'></span>";
                treeItem.population = item.industry_id;

                listData.Add(treeItem);
            }

            return listData;
        }

        private List<TreeViewItemModel> GetListChildOfField(IdentityField currentItem, List<int> checkedList)
        {
            List<TreeViewItemModel> listData = new List<TreeViewItemModel>();
            foreach (var item in currentItem.Sub_fields)
            {
                var treeItem = new TreeViewItemModel();
                treeItem.id = "sub_field_" + item.id;

                var matchedChecked = checkedList.Where(x => x == item.id).FirstOrDefault();
                if (matchedChecked > 0)
                    treeItem.@checked = true;

                var currentSubFieldName = item.LangList.Where(x => x.language_code == currentLang).Select(x => x.sub_field).FirstOrDefault();
                if (string.IsNullOrEmpty(currentSubFieldName))
                    currentSubFieldName = item.sub_field;

                treeItem.text = currentSubFieldName + "<span class='sub-field hidden' data-id='" + item.id + "'></span>";
                treeItem.population = item.field_id;

                listData.Add(treeItem);
            }

            return listData;
        }

        [HttpGet]
        public ActionResult GetListIndustriesAsTreeData()
        {            
            List<TreeViewItemModel> listData = new List<TreeViewItemModel>();
            try
            {
                var checkedItems = (Request["CheckedItems"] != null) ? Request["CheckedItems"].ToString() : string.Empty;
                var checkedList = new List<int>();

                if (!string.IsNullOrEmpty(checkedItems))
                    checkedList = checkedItems.Split(',').Select(Int32.Parse).ToList();

                var rawList = CommonHelpers.GetListIndustries();
                if (rawList.HasData())
                {
                    foreach (var item in rawList)
                    {                        
                        var currentIndustryName = item.LangList.Where(x => x.language_code == currentLang).Select(x => x.industry).FirstOrDefault();
                        var treeItem = new TreeViewItemModel();
                        treeItem.id = "industry_" + item.id;                     
                        
                        if(string.IsNullOrEmpty(currentIndustryName))
                            currentIndustryName = item.industry;

                        treeItem.text = currentIndustryName;                        
                        if (item.Sub_industries.HasData())
                        {
                            treeItem.hasChildren = true;
                            treeItem.children = GetListChildOfIndustry(item, checkedList);
                        }

                        listData.Add(treeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetListIndustriesAsTreeData because: " + ex.ToString());
            }

            return Json(listData, JsonRequestBehavior.AllowGet);
            //return View();
        }

        [HttpGet]
        public ActionResult GetListFieldsAsTreeData()
        {
            List<TreeViewItemModel> listData = new List<TreeViewItemModel>();
            try
            {
                var checkedItems = (Request["CheckedItems"] != null) ? Request["CheckedItems"].ToString() : string.Empty;
                var checkedList = new List<int>();

                if (!string.IsNullOrEmpty(checkedItems))
                    checkedList = checkedItems.Split(',').Select(Int32.Parse).ToList();

                var rawList = CommonHelpers.GetListFields();
                if (rawList.HasData())
                {
                    foreach (var item in rawList)
                    {
                        var currentFieldName = item.LangList.Where(x => x.language_code == currentLang).Select(x => x.field).FirstOrDefault();
                        if (string.IsNullOrEmpty(currentFieldName))
                            currentFieldName = item.field;

                        var treeItem = new TreeViewItemModel();

                        treeItem.id = "field_" + item.id;
                        treeItem.text = currentFieldName;
                        if (item.Sub_fields.HasData())
                        {
                            treeItem.hasChildren = true;
                            treeItem.children = GetListChildOfField(item, checkedList);
                        }

                        listData.Add(treeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetListFieldsAsTreeData because: " + ex.ToString());
            }

            return Json(listData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionPlaces()
        {
            List<IdentitySearchPlace> returnList = null;
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var apiInputModel = new ApiGetListByPageModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;

                var apiReturned = SearchPlaceServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentitySearchPlace>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionPlaces because: " + ex.ToString());
            }

            return Json(new { success = true, data = returnList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public ActionResult GetFields()
        {

            JobSearchModel model = new JobSearchModel();
            try
            {
                model.FieldLists = CommonHelpers.GetListFieldCounts();
                //if (model.FieldLists.HasData())
                //{
                //    model.FieldLists = model.FieldLists.OrderByDescending(s => s.count_num).ToList();
                //}
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display GetFields because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("~/Views/Widgets/_Field.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public ActionResult GetListHots()
        {

            JobSearchModel model = new JobSearchModel();
            try
            {
                var jobSeekerId = AccountHelper.GetCurrentUserId();
                model.JobLists = CommonHelpers.GetListHots(jobSeekerId, 6);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when display GetListHots because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("~/Views/Widgets/_JobItem.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionTrainLines()
        {
            List<IdentityTrainLine> returnList = new List<IdentityTrainLine>();
            try
            {
                var city_id = (Request["id"] != null) ? Utils.ConvertToInt32(Request["id"]) : 0;
                var trainLineApiReturn = TrainLineServices.GetListByCityIdAsync(city_id).Result;
                if (trainLineApiReturn != null)
                {
                    if (trainLineApiReturn.value != null)
                        returnList = JsonConvert.DeserializeObject<List<IdentityTrainLine>>(trainLineApiReturn.value.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionTrainLines because: " + ex.ToString());
            }
            return PartialView("~/Views/Widgets/Items/Option/_TrainLineList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionStations()
        {
            List<IdentityStation> returnList = new List<IdentityStation>();
            try
            {
                var line_id = (Request["line_id"] != null) ? Utils.ConvertToInt32(Request["line_id"]) : 0;
                var city_id = (Request["city_id"] != null) ? Utils.ConvertToInt32(Request["city_id"]) : 0;

                var apiReturned = StationServices.GetListByPositionAsync(line_id,city_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityStation>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionStations because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/Option/_StationList.cshtml", returnList);
        }

        [HttpGet]
        public async Task<ActionResult> AcceptInvitation(string token)
        {
            var result = EnumCommonCode.Success;
            var returnModel = new ApiResponseCommonModel();
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
                return View();
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    ViewBag.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
                    return View();
                }

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 4)
                {
                    var apiModel = new ApiFriendInvitationAcceptModel();

                    apiModel.job_id = Utils.ConvertToInt32(userData[0]);
                    apiModel.job_seeker_id = Utils.ConvertToInt32(userData[1]);
                    apiModel.invite_id = Utils.ConvertToInt32(userData[2]);
                    apiModel.email = userData[3];

                    returnModel = await JobSeekerServices.AcceptFriendInvitationAsync(apiModel);
                }

                if(returnModel != null)
                {
                    if (returnModel.status == EnumCommonCode.Success)
                    {
                        if(returnModel.value != null)
                        {
                            IdentityFriendInvitation info = JsonConvert.DeserializeObject<IdentityFriendInvitation>(returnModel.value.ToString());
                            if(info != null)
                            {
                                if(info.job_id <= 0)
                                {
                                    return Redirect("~");
                                }
                                else
                                {
                                    var previewLink = SecurityHelper.GenerateSecureLink("job", "preview", new { id = info.job_id });
                                    return Redirect(previewLink);
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
                    return View();
                }                
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to AcceptFriendInvitation due to: {0}", ex.ToString());
                logger.Error(strError);

                ViewBag.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
                return View();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionStationByTrainLines()
        {
            List<IdentityStation> returnList = new List<IdentityStation>();
            try
            {
                var line_id = (Request["line_id"] != null) ? Utils.ConvertToInt32(Request["line_id"]) : 0;
                var apiReturned = StationServices.GetListByPositionAsync(line_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityStation>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionStations because: " + ex.ToString());
            }
            return PartialView("~/Views/Widgets/Items/Option/_StationList.cshtml", returnList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionTrainLineByCitys()
        {
            List<IdentityTrainLine> returnList = new List<IdentityTrainLine>();
            try
            {
                var city_id = 0;
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                if (string.IsNullOrEmpty(keyword))
                {
                    city_id = (Request["city_id"] != null) ? Utils.ConvertToInt32(Request["city_id"]) : 0;
                }
                var trainLineApiReturn = TrainLineServices.GetListByCityIdAsync(city_id, keyword).Result;
                if (trainLineApiReturn != null)
                {
                    if (trainLineApiReturn.value != null)
                        returnList = JsonConvert.DeserializeObject<List<IdentityTrainLine>>(trainLineApiReturn.value.ToString());
                    if (returnList.HasData())
                    {
                        foreach (var item in returnList)
                        {
                            if (currentLang == "ja-JP")
                            {
                                item.furigana = item.train_line;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionTrainLines because: " + ex.ToString());
            }
            return Json(new { success = true, data = returnList });
            //return PartialView("~/Views/Widgets/Items/Option/_TrainLineList.cshtml", returnList);
        }
        #region For filtering

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult St_FilterGetRegions()
        {
            List<IdentityRegion> returnList = null;
            try
            {
                var stRegionIds = Request["st_region_ids"].ToString();
                var selectedRegionIds = new List<int>();
                if (!string.IsNullOrEmpty(stRegionIds))
                {
                    selectedRegionIds = stRegionIds.Split(',').Select(Int32.Parse).ToList();
                }

                var apiReturned = RegionServices.GetListAsync().Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                        if (selectedRegionIds.HasData() && returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                var matchedItem = selectedRegionIds.Where(x => x == item.id).FirstOrDefault();
                                if (matchedItem > 0)
                                    item.IsSelected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to St_FilterGetRegions because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/StationFilter/_RegionList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult St_FilterGetPrefecturesByRegion()
        {
            List<IdentityPrefecture> returnList = null;
            try
            {
                var stPrefectureIds = Request["st_prefecture_ids"].ToString();
                var selectedPrefectureIds = new List<int>();
                if (!string.IsNullOrEmpty(stPrefectureIds))
                {
                    selectedPrefectureIds = stPrefectureIds.Split(',').Select(Int32.Parse).ToList();
                }

                var id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = RegionServices.GetPrefecturesAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityPrefecture>>(apiReturned.value.ToString());
                        if (selectedPrefectureIds.HasData() && returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                var matchedItem = selectedPrefectureIds.Where(x => x == item.id).FirstOrDefault();
                                if (matchedItem > 0)
                                    item.IsSelected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to St_FilterGetPrefecturesByRegion because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/StationFilter/_PrefectureList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult St_FilterGetTrainLinesByPrefecture()
        {
            List<IdentityTrainLine> returnList = null;
            try
            {
                var stStationIds = Request["station_ids"].ToString();
                var selectedStationIds = new List<int>();
                if (!string.IsNullOrEmpty(stStationIds))
                {
                    selectedStationIds = stStationIds.Split(',').Select(Int32.Parse).ToList();
                }

                var id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = PrefectureServices.GetTrainLinesAndStationsAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityTrainLine>>(apiReturned.value.ToString());
                        if (returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                if (item.Stations.HasData())
                                {
                                    foreach (var station in item.Stations)
                                    {
                                        var matchedItem = selectedStationIds.Where(x => x == station.id).FirstOrDefault();
                                        if (matchedItem > 0)
                                        {
                                            station.IsSelected = true;
                                            item.IsSelected = true;
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
                logger.Error("Failed to St_FilterGetTrainLinesByPrefecture because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/StationFilter/_TrainLineAndStationList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ct_FilterGetRegions()
        {
            List<IdentityRegion> returnList = null;
            try
            {
                var ctRegionIds = Request["ct_region_ids"].ToString();
                var selectedRegionIds = new List<int>();
                if (!string.IsNullOrEmpty(ctRegionIds))
                {
                    selectedRegionIds = ctRegionIds.Split(',').Select(Int32.Parse).ToList();
                }

                var apiReturned = RegionServices.GetListAsync().Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                        if (selectedRegionIds.HasData() && returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                var matchedItem = selectedRegionIds.Where(x => x == item.id).FirstOrDefault();
                                if (matchedItem > 0)
                                    item.IsSelected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to Ct_FilterGetRegions because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/CityFilter/_RegionList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ct_FilterGetPrefecturesByRegion()
        {
            List<IdentityPrefecture> returnList = null;
            try
            {
                var ctPrefectureIds = Request["ct_prefecture_ids"].ToString();
                var selectedPrefectureIds = new List<int>();
                if (!string.IsNullOrEmpty(ctPrefectureIds))
                {
                    selectedPrefectureIds = ctPrefectureIds.Split(',').Select(Int32.Parse).ToList();
                }

                var id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = RegionServices.GetPrefecturesAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityPrefecture>>(apiReturned.value.ToString());
                        if (selectedPrefectureIds.HasData() && returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                var matchedItem = selectedPrefectureIds.Where(x => x == item.id).FirstOrDefault();
                                if (matchedItem > 0)
                                    item.IsSelected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to Ct_FilterGetPrefecturesByRegion because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/CityFilter/_PrefectureList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ct_FilterGetCitiesByPrefecture()
        {
            List<IdentityCity> returnList = null;
            try
            {
                var ctCityIds = Request["city_ids"].ToString();
                var selectedCityIds = new List<int>();
                if (!string.IsNullOrEmpty(ctCityIds))
                {
                    selectedCityIds = ctCityIds.Split(',').Select(Int32.Parse).ToList();
                }

                var id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = PrefectureServices.GetCitiesAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityCity>>(apiReturned.value.ToString());
                        if (selectedCityIds.HasData() && returnList.HasData())
                        {
                            foreach (var item in returnList)
                            {
                                var matchedItem = selectedCityIds.Where(x => x == item.id).FirstOrDefault();
                                if (matchedItem > 0)
                                    item.IsSelected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to Ct_FilterGetCitiesByPrefecture because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/CityFilter/_CityList.cshtml", returnList);
        }

        #endregion


        #region Options List

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRegionsByCountry()
        {
            List<IdentityRegion> returnList = null;
            try
            {
                var country_id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = RegionServices.GetListAsync(country_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityRegion>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetRegionsByCountry because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/Option/_RegionList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPrefecturesByRegion()
        {
            List<IdentityPrefecture> returnList = null;
            try
            {
                var region_id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = RegionServices.GetPrefecturesAsync(region_id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityPrefecture>>(apiReturned.value.ToString());                        
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetPrefecturesByRegion because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/Option/_PrefectureList.cshtml", returnList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCitiesByPrefecture()
        {
            List<IdentityCity> returnList = null;
            try
            {                
                var id = Utils.ConvertToInt32(Request["id"]);
                var apiReturned = PrefectureServices.GetCitiesAsync(id).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityCity>>(apiReturned.value.ToString());                        
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetCitiesByPrefectures because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/Option/_CityList.cshtml", returnList);
        }

        #endregion

    }
}
