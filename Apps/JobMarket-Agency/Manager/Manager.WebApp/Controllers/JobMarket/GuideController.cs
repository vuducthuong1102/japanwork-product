using System.Web.Mvc;
using System.Collections.Generic;
using System;
using ApiJobMarket.DB.Sql.Entities;
using System.Linq;
using Newtonsoft.Json;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using Manager.SharedLibs;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;

using Manager.WebApp.Resources;
using System.Globalization;
using System.Threading;

namespace Manager.WebApp.Controllers
{
    public class GuideController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<MasterController>();

        public ActionResult Index()
        {            
            return View();
        }
    }
}
