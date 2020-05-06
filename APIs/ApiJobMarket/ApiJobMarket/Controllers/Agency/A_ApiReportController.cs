using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using ApiJobMarket.DB.Sql.Stores;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using Autofac;
using System.Collections.Generic;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using System.Web;
using System.Dynamic;
using ApiJobMarket.Caching.Providers;
using ApiJobMarket.Settings;
using System.Linq;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/A_report")]
    public class A_ApiReportController : BaseApiController
    {
        private readonly string _reportInYear = "REPORT_IN_YEAR_{0}";
        private readonly string _reportInWeek = "REPORT_IN_WEEK_{1}";

        private readonly ILog logger = LogProvider.For<A_ApiReportController>();

        [HttpPost]
        [Route("job_by_year")]
        public async Task<IHttpActionResult> GetStatisticsByYear(ApiAgencyReportByYearModel model)
        {
            var requestName = "A_report-GetStatisticsByYear";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {                
                var result = StatisticsByYear(model);

                await Task.FromResult(result);

                returnModel.value = result;
                jsonString = JsonConvert.SerializeObject(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("job_by_week")]
        public async Task<IHttpActionResult> GetStatisticsByWeek(ApiAgencyReportByWeekModel model)
        {
            var requestName = "A_report-GetStatisticsByWeek";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                var result = StatisticsByWeek(model);

                await Task.FromResult(result);

                returnModel.value = result;
                jsonString = JsonConvert.SerializeObject(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("application_by_week")]
        public async Task<IHttpActionResult> GetStatisticsApplicationByWeek(ApiAgencyReportByWeekModel model)
        {
            var requestName = "A_report-GetStatisticsApplicationByWeek";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                var result = StatisticsApplicationByWeek(model);

                await Task.FromResult(result);

                returnModel.value = result;
                jsonString = JsonConvert.SerializeObject(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);


                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        private List<IdentityReportByYear> StatisticsByYear(ApiAgencyReportByYearModel model)
        {
            var listData = new List<IdentityReportByYear>();

            var year = Utils.ConvertToIntFromQuest(model.year);
            if(year <= 0)
                year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            DateTime dtNow = DateTime.Now;

            var _storeReport = GlobalContainer.IocContainer.Resolve<IStoreReport>();
            var filter = new IdentityReportFilter();

            filter.AgencyId = Utils.ConvertToIntFromQuest(model.agency_id);

            filter.FromDate = firstDay;
            filter.ToDate = lastDay;

            listData = _storeReport.GetStatisticsForDashBoard(filter);

            return listData;
        }

        private List<IdentityReportByWeek> StatisticsByWeek(ApiAgencyReportByWeekModel model)
        {
            var listData = new List<IdentityReportByWeek>();

            var _storeReport = GlobalContainer.IocContainer.Resolve<IStoreReport>();
            var filter = new IdentityReportFilter();
            var listDaysInWeek = CommonHelpers.GetListDaysInCurrentWeek();
            filter.AgencyId = Utils.ConvertToIntFromQuest(model.agency_id);

            if (listDaysInWeek.HasData())
            {
                filter.FromDate = listDaysInWeek[0];

                filter.ToDate = listDaysInWeek[listDaysInWeek.Count - 1];
                filter.ToDate = filter.ToDate.AddDays(1).Date.AddSeconds(-1);

                filter.ListDays = string.Join(",", listDaysInWeek.Select(x => x.Day).ToList());
            }

            listData = _storeReport.GetStatisticsByWeek(filter);

            return listData;
        }

        private List<IdentityReportApplicationByWeek> StatisticsApplicationByWeek(ApiAgencyReportByWeekModel model)
        {
            var listData = new List<IdentityReportApplicationByWeek>();

            var _storeReport = GlobalContainer.IocContainer.Resolve<IStoreReport>();
            var filter = new IdentityReportFilter();
            var listDaysInWeek = CommonHelpers.GetListDaysInCurrentWeek();
            filter.AgencyId = Utils.ConvertToIntFromQuest(model.agency_id);

            if (listDaysInWeek.HasData())
            {
                filter.FromDate = listDaysInWeek[0];

                filter.ToDate = listDaysInWeek[listDaysInWeek.Count - 1];
                filter.ToDate = filter.ToDate.AddDays(1).Date.AddSeconds(-1);

                filter.ListDays = string.Join(",", listDaysInWeek.Select(x => x.Day).ToList());
            }

            listData = _storeReport.GetStatisticsApplicationByWeek(filter);

            return listData;
        }
    }
}
