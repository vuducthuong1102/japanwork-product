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

using Manager.WebApp.Resources;
using System.Globalization;
using System.Threading;

namespace Manager.WebApp.Controllers
{
    public class MasterController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<MasterController>();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        public JsonResult GetResources()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_currentLanguage);
            var resources = ResourceSerialiser.ToJson(typeof(ManagerResource), _currentLanguage);

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

                        if (string.IsNullOrEmpty(currentIndustryName))
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
                        var currentLang = GetCurrentLanguageOrDefault();
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetSuggestionStations()
        //{
        //    List<IdentityStation> returnList = new List<IdentityStation>();
        //    try
        //    {
        //        var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
        //        var place_id = (Request["place_id"] != null) ? Utils.ConvertToInt32(Request["place_id"]) : 0;
        //        var apiInputModel = new ApiGetStationByPageModel();
        //        apiInputModel.keyword = keyword;
        //        apiInputModel.page_index = 1;
        //        apiInputModel.page_size = SystemSettings.DefaultPageSize;
        //        apiInputModel.place_id = place_id;

        //        var apiReturned = StationServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
        //        if (apiReturned != null)
        //        {
        //            if (apiReturned.value != null)
        //            {
        //                returnList = JsonConvert.DeserializeObject<List<IdentityStation>>(apiReturned.value.ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Failed to GetSuggestionStations because: " + ex.ToString());
        //    }

        //    return Json(new { success = true, data = returnList });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionStations()
        {
            List<IdentityStation> returnList = new List<IdentityStation>();
            try
            {

                var city_id = (Request["city_id"] != null) ? Utils.ConvertToInt32(Request["city_id"]) : 0;
                var apiReturned = StationServices.GetListByCityIdAsync(city_id).Result;
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
        //  [ValidateAntiForgeryToken]
        public ActionResult GetJobsByCompany(InterviewProcessInsertModel model)
        {
            List<IdentityJob> returnList = new List<IdentityJob>();
            try
            {
                model.PageSize = SystemSettings.DefaultPageSize;
                if (model.CurrentPage == 0) model.CurrentPage = 1;
                var apiModel = new ApiJobModel()
                {
                    type_job_seeker = model.type_job_seeker,
                    page_size = model.PageSize,
                    agency_id = GetCurrentAgencyId(),
                    company_id = model.company_id,
                    job_seeker_id = model.job_seeker_id,
                    keyword = model.Keyword,
                    page_index = model.CurrentPage
                };

                var listStatus = new List<int>();
                listStatus.Add((int)EnumJobStatus.Published);
                if (model.type_job_seeker == 1)
                {
                    listStatus.Add((int)EnumJobStatus.Draft);
                    listStatus.Add((int)EnumJobStatus.Saved);

                }
                apiModel.list_status = string.Join(",", listStatus);

                var apiReturned = JobServices.GetListAssignmentWorkByPageAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJob>>(apiReturned.value.ToString());
                        model.TotalCount = Utils.ConvertToInt32(apiReturned.total);
                        if (model.SearchResults.HasData())
                        {
                            List<int> listCompanyIds = model.SearchResults.Select(x => x.company_id).ToList();
                            if (listCompanyIds.HasData())
                            {
                                var companyReturnApi = CompanyServices.GetListByIdsAsync(new ApiGetListByIdsModel { ListIds = listCompanyIds }).Result;
                                if (companyReturnApi != null && companyReturnApi.value != null)
                                    model.Companies = JsonConvert.DeserializeObject<List<IdentityCompany>>(companyReturnApi.value.ToString());
                            }
                        }
                    }
                }
                model.Staffs = CommonHelpers.GetListUser(GetCurrentAgencyId());
                model.SubFields = CommonHelpers.GetListSubFields();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to ListAssignmentWork because: " + ex.ToString());
            }
            return PartialView("~/Views/Widgets/Items/Option/_JobList.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionTags()
        {
            List<IdentityTag> returnList = new List<IdentityTag>();
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var apiInputModel = new ApiGetListByPageModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;

                var apiReturned = TagServices.GetSuggestionsByPagingAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityTag>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionTags because: " + ex.ToString());
            }

            return Json(new { success = true, data = returnList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSuggestionCompanies()
        {
            List<IdentityCompany> returnList = new List<IdentityCompany>();

            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;

                var apiInputModel = new ApiCompanyModel();
                apiInputModel.keyword = keyword;
                apiInputModel.page_index = 1;
                apiInputModel.page_size = SystemSettings.DefaultPageSize;
                apiInputModel.agency_id = GetCurrentAgencyId();

                var apiReturned = AgencyServices.GetCompaniesAsync(apiInputModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.value != null)
                    {
                        returnList = JsonConvert.DeserializeObject<List<IdentityCompany>>(apiReturned.value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetSuggestionCompanies because: " + ex.ToString());
            }

            returnList.Insert(0, new IdentityCompany { company_name = ManagerResource.LB_ALL, id = -1 });

            return Json(new { success = true, data = returnList });
        }

        [HttpPost]
        public ActionResult ScheduleStaffList()
        {
            List<MsSql.AspNet.Identity.IdentityUser> returnList = new List<MsSql.AspNet.Identity.IdentityUser>();

            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var agency_id = GetCurrentAgencyId();
                List<MsSql.AspNet.Identity.IdentityUser> tmpList = null;
                if (agency_id > 0)
                    tmpList = CommonHelpers.GetListUser(agency_id);

                if (tmpList.HasData())
                {
                    foreach (var item in tmpList)
                    {
                        if (item.StaffId != agency_id)
                        {
                            returnList.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to ScheduleStaffList because: " + ex.ToString());
            }

            return PartialView("~/Views/Widgets/Items/Option/_ScheduleMonitorStaffList.cshtml", returnList);
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
        public ActionResult GetFieldsByEmploymentType()
        {
            var returnList = new List<IdentityField>();
            var form = 0;

            try
            {
                var employment_type = Utils.ConvertToInt32(Request["employment_type"]);
                form = Utils.ConvertNumberCommaToInt32(Request["form"]);

                if (employment_type > 0)
                {
                    returnList = CommonHelpers.GetListFieldsByEmploymentType(employment_type);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetFieldsByEmploymentType because: " + ex.ToString());
            }

            if (form == 1)
            {
                return PartialView("~/Views/Widgets/Items/Option/_FieldListPopupSearch.cshtml", returnList);
            }

            return PartialView("~/Views/Widgets/Items/Option/_FieldList.cshtml", returnList);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListJobSeekerInDropdown()
        {
            var strError = string.Empty;
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;
                int currentPage = 1;
                int pageSize = SystemSettings.DefaultPageSize;

                var apiFilterModel = new ApiJobSeekerSuggestionModel
                {
                    keyword = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : null,
                    page_index = currentPage,
                    page_size = pageSize,
                    agency_id = GetCurrentAgencyId(),
                    job_id = jobId
                };

                var apiResult = JobSeekerServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;
                List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();
                if (apiResult != null && apiResult.value != null)
                {
                    listData = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(apiResult.value.ToString());
                }

                var returnList = new List<JobSeekerItemInDropdownListModel>();
                if (listData.HasData())
                {
                    foreach (var record in listData)
                    {
                        var item = new JobSeekerItemInDropdownListModel();
                        item.id = record.user_id;
                        item.fullname = record.fullname;
                        item.fullname_furigana = record.fullname_furigana;
                        item.phone = record.phone;
                        item.email = record.email;
                        item.is_invited = record.Extensions.is_invited;

                        returnList.Add(item);
                    }
                }

                return Json(new { success = true, data = returnList });

            }
            catch (Exception ex)
            {
                strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error("Failed to GetListJobSeekerInDropdown because: " + ex.ToString());

                return Json(new { success = false, data = string.Empty, message = strError });
            }
        }

        public ActionResult ChoosenJobSeeker(JobSeekerChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["Page"] != null)
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);

            var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;
            var apiFilterModel = new ApiJobSeekerSuggestionModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
                agency_id = GetCurrentAgencyId(),
                job_id = jobId
            };

            try
            {
                var apiResult = JobSeekerServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(apiResult.value.ToString());
                    model.TotalCount = apiResult.total;
                }

                model.CurrentPage = currentPage;
                model.PageSize = pageSize;
                model.job_id = jobId;

            }
            catch (Exception ex)
            {
                logger.Error("Failed to show ChoosenJobSeeker form: " + ex.ToString());
            }

            return PartialView("~/Views/JobSeeker/Partials/_ChoosenJobSeeker.cshtml", model);
        }

        [HttpPost]
        public ActionResult ChoosenJobSeekerSearch(JobSeekerChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;
            var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;

            if (Request["Page"] != null)
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);

            if (Request["CurrentPage"] != null)
                currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);

            var apiFilterModel = new ApiJobSeekerSuggestionModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
                agency_id = GetCurrentAgencyId(),
                job_id = jobId
            };

            try
            {
                var apiResult = JobSeekerServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityJobSeeker>>(apiResult.value.ToString());
                    model.TotalCount = apiResult.total;
                }

                model.CurrentPage = currentPage;
                model.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to show _ChoosenJobSeekerList form: " + ex.ToString());
            }

            return PartialView("~/Views/JobSeeker/Partials/_ChoosenJobSeekerList.cshtml", model);
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
                    keyword = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : null,
                    page_index = currentPage,
                    page_size = pageSize,
                    agency_id = GetCurrentAgencyId(),
                    job_id = jobId
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
                        item.job_seeker_id = record.job_seeker_id;
                        item.fullname = record.fullname;
                        item.fullname_furigana = record.fullname_furigana;
                        item.phone = record.phone;
                        item.email = record.email;
                        item.is_invited = record.Extensions.is_invited;

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetListTrainLineInDropdown()
        //{
        //    var strError = string.Empty;
        //    try
        //    {
        //        var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
        //        var city_id = (Request["city_id"] != null) ? Utils.ConvertToInt32(Request["city_id"]) : 0;
        //        int currentPage = 1;
        //        int pageSize = SystemSettings.DefaultPageSize;

        //        var apiFilterModel = new ApiCvSuggestionModel
        //        {
        //            keyword = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : null,
        //            page_index = currentPage,
        //            page_size = pageSize,
        //            agency_id = GetCurrentAgencyId(),
        //            city_id = city_id
        //        };

        //        var apiResult = CvServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;
        //        List<IdentityCv> listData = new List<IdentityCv>();
        //        if (apiResult != null && apiResult.value != null)
        //        {
        //            listData = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
        //        }

        //        var returnList = new List<CvItemInDropdownListModel>();
        //        if (listData.HasData())
        //        {
        //            foreach (var record in listData)
        //            {
        //                var item = new CvItemInDropdownListModel();
        //                item.id = record.id;
        //                item.job_seeker_id = record.job_seeker_id;
        //                item.fullname = record.fullname;
        //                item.fullname_furigana = record.fullname_furigana;
        //                item.phone = record.phone;
        //                item.email = record.email;
        //                item.is_invited = record.Extensions.is_invited;

        //                returnList.Add(item);
        //            }
        //        }

        //        return Json(new { success = true, data = returnList });

        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

        //        logger.Error("Failed to GetListCvInDropdown because: " + ex.ToString());

        //        return Json(new { success = false, data = string.Empty, message = strError });
        //    }
        //}

        public ActionResult ChoosenCv(CvChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;
            pageSize = 20;
            if (Request["Page"] != null)
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);

            var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;
            var apiFilterModel = new ApiCvSuggestionModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
                agency_id = GetCurrentAgencyId(),
                job_id = jobId
            };

            try
            {
                var apiResult = CvServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                    model.TotalCount = apiResult.total;
                }
                model.JapanseLevels = CommonHelpers.GetListJapaneseLevels();
                model.Majors = CommonHelpers.GetListMajors();
                model.Qualifications = CommonHelpers.GetListQualifications();
                model.Countries = CommonHelpers.GetListCountries();
                model.CurrentPage = currentPage;
                model.PageSize = pageSize;
                model.job_id = jobId;

            }
            catch (Exception ex)
            {
                logger.Error("Failed to show ChoosenCv form: " + ex.ToString());
            }

            return PartialView("~/Views/Cv/Partials/_ChoosenCv.cshtml", model);
        }
        [HttpPost]
        public ActionResult ChoosenCvSearch(CvChoosenModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;
            var jobId = (Request["job_id"] != null) ? Utils.ConvertToInt32(Request["job_id"]) : 0;

            if (Request["Page"] != null)
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);

            if (Request["CurrentPage"] != null)
                currentPage = Utils.ConvertToInt32(Request["CurrentPage"], 1);

            var apiFilterModel = new ApiCvSuggestionModel
            {
                keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                page_index = currentPage,
                page_size = pageSize,
                agency_id = GetCurrentAgencyId(),
                job_id = jobId
            };

            try
            {
                var apiResult = CvServices.GetSuggestionsForInvitationAsync(apiFilterModel).Result;

                if (apiResult != null && apiResult.value != null)
                {
                    model.SearchResults = JsonConvert.DeserializeObject<List<IdentityCv>>(apiResult.value.ToString());
                    model.TotalCount = apiResult.total;
                }

                model.CurrentPage = currentPage;
                model.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to show _ChoosenCvList form: " + ex.ToString());
            }

            return PartialView("~/Views/Cv/Partials/_ChoosenCvList.cshtml", model);
        }

        #endregion

        [PreventCrossOrigin]
        public ActionResult AddCustomField()
        {
            var model = new CommonCustomFieldModel();
            return PartialView("../Widgets/FormControls/CustomField/_AddCustomField", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("AddCustomField")]
        public ActionResult AddCustomField_Confirm(CommonCustomFieldModel model)
        {
            var htmlReturn = string.Empty;
            try
            {
                htmlReturn = PartialViewAsString("../Widgets/FormControls/CustomField/_CustomFieldInfo", model);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying AddCustomField Confirm because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = true, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }
    }
}
