using ApiJobMarket.DB.Sql.Entities;
using Autofac;
using Manager.SharedLibs;
using Manager.SharedLibs.Caching.Providers;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class HomeController : BaseAuthedController
    {
        private readonly string _reportInYear = "REPORT_IN_YEAR";
        private readonly string _reportInWeek = "REPORT_IN_WEEK";
        private readonly string _reportAppInWeek = "REPORT_APP_IN_WEEK";
        private readonly ILog logger = LogProvider.For<HomeController>();

        public HomeController()
        {
            
        }

        public ActionResult Index()
        {
            //if (PermissionHelper.CheckPermission("JobSeeker", "Process"))
            //{
            //    return RedirectToAction("JobSeeker", "Process");
            //}

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DashBoardStatistics()
        {
            DashBoardStatisticsModel finalData = null;

            try
            {
                finalData = GetStatisticsByYear();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when get Report for Dashboard because: {0}", ex.ToString()));
            }

            return Json(finalData, JsonRequestBehavior.AllowGet);
        }

        private DashBoardStatisticsModel GetStatisticsByYear()
        {
            DashBoardStatisticsModel finalData = null;

            //Check from cache
            var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

            cacheProvider.Get(_reportInYear, out finalData);

            if (finalData == null)
            {
                finalData = new DashBoardStatisticsModel();
                
                var listData = new List<IdentityReportByYear>();                
                var dtNow = DateTime.Now;

                var apiFilter = new ApiAgencyReportByYearModel();
                apiFilter.agency_id = GetCurrentAgencyId();
                apiFilter.year = dtNow.Year;

                var apiReturn = A_ReportServices.JobByYearAsync(apiFilter).Result;

                if (apiReturn != null && apiReturn.value != null)
                    listData = JsonConvert.DeserializeObject<List<IdentityReportByYear>>(apiReturn.value.ToString());

                if (listData.HasData())
                {
                    foreach (var item in listData)
                    {
                        //Get data statistics for year
                        for (int i = 0; i < finalData.InYearData.successData.Length; i++)
                        {                            
                            if (item.CurrentMonth == (i + 1))
                            {
                                finalData.InYearData.successData[i] = item.PublicCount;
                                finalData.InYearData.processingData[i] = item.ProcessingCount;
                                finalData.InYearData.failedData[i] = item.UnProcessedCount;
                            }
                        }
                    }

                    finalData.UpdatedTime = dtNow.ToString("dd/MM/yyyy HH:mm");
                    finalData.NextUpdate = dtNow.AddMinutes(SystemSettings.CacheExpireDataInDashBoard).ToString("dd/MM/yyyy HH:mm");

                    //Save to cache: Expire time default is: 30 minutes
                    cacheProvider.Set(_reportInYear, finalData, SystemSettings.CacheExpireDataInDashBoard);
                }                
            }

            return finalData;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DashBoardStatisticsByWeek()
        {
            DashBoardStatisticsByWeekModel finalData = null;

            try
            {
                finalData = GetStatisticsByWeek();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when get Report By Week for Dashboard because: {0}", ex.ToString()));
            }

            return Json(finalData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DashBoardStatisticsAppByWeek()
        {
            DashBoardStatisticsAppByWeekModel finalData = null;

            try
            {
                finalData = GetStatisticsAppByWeek();
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Failed when get Report Application By Week for Dashboard because: {0}", ex.ToString()));
            }

            return Json(finalData, JsonRequestBehavior.AllowGet);
        }

        private DashBoardStatisticsByWeekModel GetStatisticsByWeek()
        {
            DashBoardStatisticsByWeekModel finalData = null;

            //Check from cache
            var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
            var dtNow = DateTime.Now;

            cacheProvider.Get(_reportInWeek, out finalData);

            if (finalData == null)
            {
                finalData = new DashBoardStatisticsByWeekModel();
                var listData = new List<IdentityReportByWeek>();

                var apiFilter = new ApiAgencyReportByWeekModel();
                apiFilter.agency_id = GetCurrentAgencyId();

                var apiReturn = A_ReportServices.JobByWeekAsync(apiFilter).Result;

                if (apiReturn != null && apiReturn.value != null)
                    listData = JsonConvert.DeserializeObject<List<IdentityReportByWeek>>(apiReturn.value.ToString());

                var listDaysInWeek = CommonHelpers.GetListDaysInCurrentWeek();

                if (listDaysInWeek.HasData())
                {
                    var fromDate = listDaysInWeek[0];

                    var toDate = listDaysInWeek[listDaysInWeek.Count - 1];
                    toDate = toDate.AddDays(1).Date.AddSeconds(-1);
                    
                    finalData.FromDateStr = fromDate.ToString("dd/MM/yyyy");
                    finalData.ToDateStr = toDate.ToString("dd/MM/yyyy");
                }

                if (listData.HasData())
                {
                    var counter = 8;
                    foreach (var item in listData)
                    {
                        foreach (var dt in listDaysInWeek)
                        {
                            if (item.CurrentDate == dt.Day)
                            {
                                finalData.InWeekData.successData[8 - item.Idx] = item.PublicCount;
                                finalData.InWeekData.processingData[8 - item.Idx] = item.ProcessingCount;
                                finalData.InWeekData.failedData[8 - item.Idx] = item.UnProcessedCount;

                                finalData.InWeekData.successTotal += item.PublicCount;
                                finalData.InWeekData.processingTotal += item.ProcessingCount;
                                finalData.InWeekData.failedTotal += item.UnProcessedCount;
                            }

                            counter -= 2;
                        }                       
                    }

                    finalData.UpdatedTime = dtNow.ToString("dd/MM/yyyy HH:mm");
                    finalData.NextUpdate = dtNow.AddMinutes(SystemSettings.CacheExpireDataInDashBoard).ToString("dd/MM/yyyy HH:mm");

                    //Save to cache: Expire time default is: 30 minutes
                    cacheProvider.Set(_reportInWeek, finalData, SystemSettings.CacheExpireDataInDashBoard);
                }              
            }

            return finalData;
        }

        private DashBoardStatisticsAppByWeekModel GetStatisticsAppByWeek()
        {
            DashBoardStatisticsAppByWeekModel finalData = null;

            //Check from cache
            var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
            var dtNow = DateTime.Now;

            cacheProvider.Get(_reportAppInWeek, out finalData);

            if (finalData == null)
            {
                finalData = new DashBoardStatisticsAppByWeekModel();
                var listData = new List<IdentityReportApplicationByWeek>();

                var apiFilter = new ApiAgencyReportByWeekModel();
                apiFilter.agency_id = GetCurrentAgencyId();

                var apiReturn = A_ReportServices.ApplicationByWeekAsync(apiFilter).Result;

                if (apiReturn != null && apiReturn.value != null)
                    listData = JsonConvert.DeserializeObject<List<IdentityReportApplicationByWeek>>(apiReturn.value.ToString());

                var listDaysInWeek = CommonHelpers.GetListDaysInCurrentWeek();

                if (listDaysInWeek.HasData())
                {
                    var fromDate = listDaysInWeek[0];

                    var toDate = listDaysInWeek[listDaysInWeek.Count - 1];
                    toDate = toDate.AddDays(1).Date.AddSeconds(-1);

                    finalData.FromDateStr = fromDate.ToString("dd/MM/yyyy");
                    finalData.ToDateStr = toDate.ToString("dd/MM/yyyy");
                }

                if (listData.HasData())
                {
                    var counter = 8;
                    foreach (var item in listData)
                    {
                        foreach (var dt in listDaysInWeek)
                        {
                            if (item.CurrentDate == dt.Day)
                            {
                                finalData.InWeekData.waitingData[8 - item.Idx] = item.WaitingCount;
                                finalData.InWeekData.approvedData[8 - item.Idx] = item.ApprovedCount;
                                finalData.InWeekData.ignoredData[8 - item.Idx] = item.IgnoredCount;

                                finalData.InWeekData.waitingTotal += item.WaitingCount;
                                finalData.InWeekData.approvedTotal += item.ApprovedCount;
                                finalData.InWeekData.ignoredTotal += item.IgnoredCount;
                            }

                            counter -= 2;
                        }
                    }

                    finalData.UpdatedTime = dtNow.ToString("dd/MM/yyyy HH:mm");
                    finalData.NextUpdate = dtNow.AddMinutes(SystemSettings.CacheExpireDataInDashBoard).ToString("dd/MM/yyyy HH:mm");

                    //Save to cache: Expire time default is: 30 minutes
                    cacheProvider.Set(_reportAppInWeek, finalData, SystemSettings.CacheExpireDataInDashBoard);
                }
            }

            return finalData;
        }

        public ActionResult Demo()
        {
            return View();
        }

        public ActionResult ItemLabelExport()
        {
            return View();
        }
    }
}