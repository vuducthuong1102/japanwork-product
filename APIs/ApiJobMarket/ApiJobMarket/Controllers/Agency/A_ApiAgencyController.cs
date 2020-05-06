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

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/A_agencies")]
    public class A_ApiAgenyController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<A_ApiAgenyController>();

        [HttpPost]
        [Route("schedules_by_staff")]
        public async Task<IHttpActionResult> GetSchedulesByStaff(ApiScheduleByStaffModel model)
        {
            var requestName = "Agencies-GetSchedulesByStaff";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                filter.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                filter.start_time = null;
                filter.end_time = null;

                try
                {

                    filter.start_time = Utils.ConvertStringToDateTimeQuestByFormat(model.start_time, "yyyy-MM-dd");
                    filter.end_time = Utils.ConvertStringToDateTimeQuestByFormat(model.end_time, "yyyy-MM-dd");
                }
                catch
                {
                }
                
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSchedule>();

                List<IdentitySchedule> listData = myStore.GetByStaff(model);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;                    
                }

                returnModel.value = listData;
                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
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
        [Route("todayschedules_by_staff")]
        public async Task<IHttpActionResult> GetTodaySchedulesByStaff(ApiScheduleByStaffModel model)
        {
            var requestName = "Agencies-GetSchedulesByStaff";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.agency_id = Utils.ConvertToIntFromQuest(model.agency_id);
                filter.staff_id = Utils.ConvertToIntFromQuest(model.staff_id);
                filter.start_time = null;
                filter.end_time = null;

                try
                {
                    filter.start_time = Utils.ConvertStringToDateTimeQuestByFormat(model.start_time, "yyyy-MM-dd");
                    filter.end_time = Utils.ConvertStringToDateTimeQuestByFormat(model.end_time, "yyyy-MM-dd");
                }
                catch
                {
                }
                
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreSchedule>();

                List<IdentitySchedule> listData = myStore.GetTodayByStaff(model);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;                    
                }

                returnModel.value = listData;
                jsonString = JsonConvert.SerializeObject(returnModel);
                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
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
    }
}
