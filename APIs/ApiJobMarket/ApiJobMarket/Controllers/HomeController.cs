using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ApiJobMarket.Services;
using ApiJobMarket.Logging;
using ApiJobMarket.Models;
using ApiJobMarket.Helpers;
using ApiJobMarket.DB.Sql.Repositories;

namespace ApiJobMarket.Controllers
{    
    public class HomeController : Controller
    {
        private readonly ILog logger = LogProvider.For<HomeController>();
        private readonly ISampleService _sampleService;

        public HomeController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult HashData(HashDataModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ActionName("HashData")]
        public ActionResult HashData_Post(HashDataModel model)
        {
            model.UserName = model.UserName.Trim();
            model.Result = HashingSignInData(model);
            return View(model);
        }

        public string HashingSignInData(HashDataModel model)
        {
            var rawDataToHash = string.Format("{0}|{1}|{2}", model.UserName, model.PasswordHash, model.Time);

            return Utility.Md5HashingData(rawDataToHash);
        }

        public ActionResult ChangeLanguage(string lang)
        {
            if(Request != null)
            {
                if(Request.UrlReferrer != null)
                {
                    var currentUrl = Request.UrlReferrer.ToString();
                    if (!string.IsNullOrEmpty(Request.UrlReferrer.ToString()))
                    {
                        new LanguageMessageHandler().SetLanguage(lang);

                        return Redirect(currentUrl);
                    }
                }              
            }

            new LanguageMessageHandler().SetLanguage(lang);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Demo()
        {
            //var places = new List<int>();
            //places.Add("ChIJCYt4ckCXMzERG6VDtBHsxOk");
            //places.Add("ChIJwc6exbUZQjERGUMAVgGQ0g8");

            //RpsPost rps = new RpsPost();
            //rps.PlacesInsertBulk(places, 1002040);

            return Content("Ok");
        }
    }
}
