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
    [RoutePrefix("api/statistics")]
    public class ApiStatisticsController : BaseApiController
    {
        private readonly string _inYearKey = "STATISTICS_JOB_IN_YEAR_{0}";

        private readonly ILog logger = LogProvider.For<ApiStatisticsController>();

        [HttpPost]
        [Route("published_job_by_year")]
        public async Task<IHttpActionResult> GetStatisticsJobByYear(ApiStatisticsModel model)
        {
            var requestName = "Statistics-GetStatisticsJobByYear";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {                
                var result = StatisticsPublishedJobByYear(model);

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

        private IdentityStatisticsJobByYear StatisticsPublishedJobByYear(ApiStatisticsModel model)
        {
            var data = new IdentityStatisticsJobByYear();

            var year = Utils.ConvertToIntFromQuest(model.year);
            if(year <= 0)
                year = DateTime.Now.Year;

            var _storeStatistics = GlobalContainer.IocContainer.Resolve<IStoreStatistics>();
            var filter = new IdentityStatisticsFilter();

            filter.agency_id = model.agency_id;
            filter.year = year;
            
            data = _storeStatistics.PublishedJobByYear(filter);

            return data;
        }        
    }
}
