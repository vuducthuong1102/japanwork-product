//using Autofac;
//using Manager.SharedLibs.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Models;
//using Manager.WebApp.Models.Business;
//using Manager.WebApp.Settings;
//using MsSql.AspNet.Identity.Entities;
//using MsSql.AspNet.Identity.MsSqlStores;
//using System;
//using System.Web.Mvc;

//namespace Manager.WebApp.Controllers
//{
//    public class DemoController : BaseAuthedController
//    {
//        private readonly ILog logger = LogProvider.For<DemoController>();

//        public DemoController()
//        {
//            //_cacheReportByYearExpiredTime = DateTimeOffset.Now.AddMinutes(SystemSettings.CacheExpireDataInDashBoard); 
//        }

//        public ActionResult Index()
//        {            
//            return View();
//        }
        
//        public ActionResult File()
//        {
//            return View();
//        }
//    }
//}