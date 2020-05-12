using System;
using System.Web.Mvc;
using StackExchange.Redis.Extensions.Core;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using System.Collections.Generic;
using Manager.WebApp.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Manager.WebApp.Helpers;
using ApiJobMarket.DB.Sql.Entities;

namespace Manager.WebApp.Controllers
{
    public class StatisticsController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<StatisticsController>();
        private const string _allUsersCacheKey = "MYCLOUD_ALL_USERS";

        public StatisticsController()
        {

        }   

        [AccessRoleChecker]
        public ActionResult UsersOnline()
        {
            var model = new StatisticsUsersOnlineModel();

            try
            {
                var responseApiModel = MyCloudServices.GetUsersOnline();
                if(responseApiModel != null)
                {
                    if(responseApiModel.Data != null)
                    {
                        model.ListUser = JsonConvert.DeserializeObject<List<Connector>>(responseApiModel.Data.ToString());
                    }

                    if (model.ListUser != null && model.ListUser.Count > 0)
                    {
                        model.ListUser = model.ListUser.GroupBy(x => x.UserId)
                        .Select(grp => grp.First())
                        .ToList();
                    }
                }                
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not get UsersOnline because: {0}", ex.ToString()));
            }

            return View(model);
        }

        public ActionResult InvoiceDetail()
        {
            return View();
        }

        public ActionResult GetInvoice(ManageStatisticsModel model)
        {            
            try
            {
                ApiStatisticsModel apiModel = new ApiStatisticsModel();

            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetInvoice: " + ex.ToString());
            }

            return PartialView("Partials/_PublishedJobByYear", model);
        }

        public async Task<ActionResult> PublishedJobByYear(ManageStatisticsModel model)
        {
            IdentityStatisticsJobByYear data = new IdentityStatisticsJobByYear();
            try
            {
                ApiStatisticsModel apiModel = new ApiStatisticsModel();
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.year = model.year;

                var result = StatisticsServices.PublishedJobByYearAsync(apiModel).Result;

                await Task.FromResult(result);

                data = result.ConvertData<IdentityStatisticsJobByYear>();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to PublishedJobByYear: " + ex.ToString());
            }

            return PartialView("Partials/_PublishedJobByYear", data);
        }

        public ActionResult Refresh()
        {
            var model = new StatisticsUsersOnlineModel();

            try
            {
                var responseApiModel = MyCloudServices.GetUsersOnline();
                if (responseApiModel != null)
                {
                    if (responseApiModel.Data != null)
                    {
                        model.ListUser = JsonConvert.DeserializeObject<List<Connector>>(responseApiModel.Data.ToString());
                    }

                    if (model.ListUser != null && model.ListUser.Count > 0)
                    {
                        model.ListUser = model.ListUser.GroupBy(x => x.UserId)
                        .Select(grp => grp.First())
                        .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not get UsersOnline because: {0}", ex.ToString()));
            }

            return PartialView("_Users", model);
        }
    }
}