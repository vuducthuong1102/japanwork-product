using System.Web.Mvc;
using System.Collections.Generic;
using System;
using ApiJobMarket.DB.Sql.Entities;
using System.Linq;
using Newtonsoft.Json;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using Manager.SharedLibs;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class MasterController : BaseController
    {
        private readonly ILog logger = LogProvider.For<MasterController>();

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

                var currentSubIndustryName = item.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.sub_industry).FirstOrDefault();

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

                var currentSubFieldName = item.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.sub_field).FirstOrDefault();
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
                        var currentIndustryName = item.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.industry).FirstOrDefault();
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
                        var currentFieldName = item.LangList.Where(x => x.language_code == _currentLanguage).Select(x => x.field).FirstOrDefault();
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
        public ActionResult GetSuggestionTrainLines()
        {
            List<IdentityTrainLine> returnList = new List<IdentityTrainLine>();
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var place_id = (Request["place_id"] != null) ? Utils.ConvertToInt32(Request["place_id"]) : 0;
                var apiInputModel = new ApiGetTrainLineByPageModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;
                apiInputModel.place_id = place_id;
                var apiReturned = TrainLineServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityTrainLine>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionTrainLines because: " + ex.ToString());
            }

            return Json(new { success = true, data = returnList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionStations()
        {
            List<IdentityStation> returnList = new List<IdentityStation>();
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var place_id = (Request["place_id"] != null) ? Utils.ConvertToInt32(Request["place_id"]) : 0;
                var apiInputModel = new ApiGetStationByPageModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;
                apiInputModel.place_id = place_id;

                var apiReturned = StationServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
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

            return Json(new { success = true, data = returnList });
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
