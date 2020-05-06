using Autofac;
using Manager.SharedLibs;
using Manager.SharedLibs.Caching.Providers;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Models.Business;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class HomeController : BaseAuthedController
    {
        private readonly string _reportInYear = "REPORT_IN_YEAR";
        private readonly string _reportInWeek = "REPORT_IN_WEEK";
        private readonly ILog logger = LogProvider.For<HomeController>();

        public HomeController()
        {
            
        }

        public ActionResult Index()
        {            
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

                //Agency statistics 
                //var listHang = SystemSettings.AirlinesReportInDashBoard.Split(',').ToList();
                //var listHangLabels = SystemSettings.AirlinesLabelInDashBoard.Split(',').ToList();
                //var airlinesCounter = listHang.Count;

                //finalData.AgencyData.ListHang = listHang;
                //finalData.AgencyData.ListHangLabels = listHangLabels;
                //finalData.AgencyData.TotalData = new int[airlinesCounter];

                var listData = new List<IdentityReportByYear>();

                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                DateTime lastDay = new DateTime(year, 12, 31);
                DateTime dtNow = DateTime.Now;

                var _storeReport = GlobalContainer.IocContainer.Resolve<IStoreReport>();
                var filter = new ReportFilter();

                filter.FromDate = firstDay;
                filter.ToDate = lastDay;

                listData = _storeReport.GetStatisticsForDashBoard(filter);

                if (listData.HasData())
                {
                    foreach (var item in listData)
                    {
                        //Get data statistics for year
                        for (int i = 0; i < finalData.InYearData.successData.Length; i++)
                        {
                            //if (item.NgayBook.Value.Month == (i + 1))
                            //{
                            //    if (item.TinhTrang == (int)EnumTinhTrangVe.DaXuatVe)
                            //    {
                            //        finalData.InYearData.successData[i]++;
                            //    }
                            //    else if (item.TinhTrang == (int)EnumTinhTrangVe.DaHuy)
                            //    {
                            //        finalData.InYearData.failedData[i]++;
                            //    }
                            //    else
                            //    {
                            //        finalData.InYearData.processingData[i]++;
                            //    }
                            //}

                            if (item.CurrentMonth == (i + 1))
                            {
                                finalData.InYearData.successData[i] = item.PublicCount;
                                finalData.InYearData.processingData[i] = item.ProcessingCount;
                                finalData.InYearData.failedData[i] = item.UnProcessedCount;
                            }
                        }                        
                    }
                }

                finalData.UpdatedTime = dtNow.ToString("dd/MM/yyyy HH:mm");
                finalData.NextUpdate = dtNow.AddMinutes(SystemSettings.CacheExpireDataInDashBoard).ToString("dd/MM/yyyy HH:mm");

                //Save to cache: Expire time default is: 30 minutes
                cacheProvider.Set(_reportInYear, finalData, SystemSettings.CacheExpireDataInDashBoard);
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
                var listData = new List<IdentityReportInWeek>();

                var _storeReport = GlobalContainer.IocContainer.Resolve<IStoreReport>();
                var filter = new ReportFilter();              
                var listDaysInWeek = CommonHelpers.GetListDaysInCurrentWeek();

                if (listDaysInWeek.HasData())
                {
                    filter.FromDate = listDaysInWeek[0];

                    filter.ToDate = listDaysInWeek[listDaysInWeek.Count - 1];
                    filter.ToDate = filter.ToDate.AddDays(1).Date.AddSeconds(-1);

                    filter.ListDays = string.Join(",", listDaysInWeek.Select(x => x.Day).ToList());
                }

                finalData.FromDateStr = filter.FromDate.ToString("dd/MM/yyyy");
                finalData.ToDateStr = filter.ToDate.ToString("dd/MM/yyyy");

                listData = _storeReport.GetStatisticsInWeek(filter);                

                if (listData.HasData())
                {
                    foreach (var item in listData)
                    {
                        //Get data statistics for year
                        //for (int i = 2; i <= finalData.InWeekData.successData.Length + 2; i++)
                        //{
                        //    if (item.Idx == i)
                        //    {
                        //        finalData.InWeekData.successData[i - 2] = item.PublicCount;
                        //        finalData.InWeekData.processingData[i - 2] = item.ProcessingCount;
                        //        finalData.InWeekData.failedData[i - 2] = item.UnProcessedCount;
                        //    }
                        //}
                        foreach (var dt in listDaysInWeek)
                        {
                            if (item.CurrentDate == dt.Day)
                            {
                                finalData.InWeekData.successData[8 - item.Idx] = item.PublicCount;
                                finalData.InWeekData.processingData[8 - item.Idx] = item.ProcessingCount;
                                finalData.InWeekData.failedData[8 - item.Idx] = item.UnProcessedCount;
                            }
                        }
                    }
                }

                finalData.UpdatedTime = dtNow.ToString("dd/MM/yyyy HH:mm");
                finalData.NextUpdate = dtNow.AddMinutes(SystemSettings.CacheExpireDataInDashBoard).ToString("dd/MM/yyyy HH:mm");

                //Save to cache: Expire time default is: 30 minutes
                //cacheProvider.Set(_reportInWeek, finalData, SystemSettings.CacheExpireDataInDashBoard);
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