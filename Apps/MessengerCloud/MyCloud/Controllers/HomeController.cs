using Autofac;
using MyCloud.Models;
using MyCloud.SharedLib.Caching.Providers;
using MyCloud.SharedLib.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace MyCloud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILog logger = LogProvider.For<HomeController>();
        private const string _allUsersCacheKey = "MYCLOUD_ONLINE_USERS";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manager()
        {
            return View();
        }

        public ActionResult Accountant()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();

        }

        [HttpGet]
        public ActionResult UsersOnline()
        {
            List<Connector> listUser = null;
            var responseModel = new ApiResponseCommonModel();
            responseModel.status = (int)HttpStatusCode.OK;
            try
            {
                var cacheProvider = Startup.AutofacContainer.Resolve<ICacheProvider>();
                cacheProvider.Get(_allUsersCacheKey,out listUser);

                responseModel.value = listUser;                               
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not get UsersOnline because: {0}", ex.ToString()));
            }

            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }
    }
}