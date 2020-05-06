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
using Autofac;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.Controllers
{
    public class SearchController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<SearchController>();

        public SearchController()
        {

        }

        //[AccessRoleChecker]
        public ActionResult Index(ManageSearchModel model)
        {
            //int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            //if (Request["Page"] != null)
            //{
            //    currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            //}

            //var filter = new ApiCompanyModel
            //{
            //    keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
            //    page_index = currentPage,
            //    page_size = pageSize,
            //    agency_id = GetCurrentAgencyId()
            //};

            try
            {
                if (Request["TsbSearchKeyword"] != null)
                    model.Keyword = Request["TsbSearchKeyword"].ToString();

                model.Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null;
                var searchType = Utils.ConvertToInt32(Request["TsbSearchType"]);

                if(searchType == 0)
                    searchType = (int)EnumSearchType.JobSeeker;

                Session["SearchType"] = searchType;
                Session["SearchKeyword"] = model.Keyword;

                if (searchType == ((int)EnumSearchType.JobSeeker))
                {
                    return RedirectToAction("Index", "JobSeeker", new { Keyword = model.Keyword });
                }
                else if(searchType == ((int)EnumSearchType.Job))
                {
                    return RedirectToAction("Index", "Job", new { Keyword = model.Keyword });
                }
                else if(searchType == ((int)EnumSearchType.Company))
                {
                    return RedirectToAction("Index", "Company", new {  Keyword = model.Keyword });
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                //return View(model);
            }

            return View(model);
        }
    }
}