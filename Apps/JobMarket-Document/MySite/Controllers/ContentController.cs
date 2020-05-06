using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using MySite.Helpers;
using MySite.Resources;
using MySite.Settings;
using System.Threading.Tasks;
using MySite.Caching;
using MySite.ShareLibs;
using MySite.Services;
using System.Linq;
using MySite.Models.External.OpenWeather;
using System.Web;
using System.Globalization;

namespace MySite.Controllers
{
    public class ContentController : BaseController
    {
        private readonly ILog logger = LogProvider.For<ContentController>();

        [HttpGet]
        public ActionResult Candidates()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Company()
        {
            return View();
        }
        [HttpGet]
        public ActionResult staff()
        {
            return View();
        }
        [HttpGet]
        public ActionResult job()
        {
            return View();
        }
        [HttpGet]
        public ActionResult apply()
        {
            return View();
        }
        [HttpGet]
        public ActionResult matching()
        {
            return View();
        }

        [HttpGet]
        public ActionResult schedule()
        {
            return View();
        }

        [HttpGet]
        public ActionResult task()
        {
            return View();
        }

        [HttpGet]
        public ActionResult file()
        {
            return View();
        }

        [HttpGet]
        public ActionResult import()
        {
            return View();
        }

        [HttpGet]
        public ActionResult data()
        {
            return View();
        }

        [HttpGet]
        public ActionResult sales()
        {
            return View();
        }

        [HttpGet]
        public ActionResult invoice()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DataOption()
        {
            return View();
        }

        [HttpGet]
        public ActionResult map()
        {
            return View();
        }

        [HttpGet]
        public ActionResult sms()
        {
            return View();
        }
        [HttpGet]
        public ActionResult facebook()
        {
            return View();
        }

        [HttpGet]
        public ActionResult line()
        {
            return View();
        }

        [HttpGet]
        public ActionResult introduce()
        {
            return View();
        }
        [HttpGet]
        public ActionResult cloudplan()
        {
            return View();
        }
        [HttpGet]
        public ActionResult installationplan()
        {
            return View();
        }
        public ActionResult candidates_option()
        {
            return View();
        }
    }
}
