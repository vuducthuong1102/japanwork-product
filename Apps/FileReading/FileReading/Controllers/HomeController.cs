using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileReading.Controllers
{
    public class HomeController : Controller
    {
        //[AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(CacheProfile = "DefaultOutputCache")]
        public ActionResult Index()
        {
            try
            {
                var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString();
                var path = $@"" + mediaPath + HttpUtility.UrlDecode(Request.Url.AbsolutePath);
                var bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "image/jpeg");
            }
            catch
            {
                Response.Cache.SetNoStore();
                Response.Cache.SetNoServerCaching();

                var path = Server.MapPath("~/Content/images/default-image.jpg");
                var bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "image/jpeg");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [OutputCache(CacheProfile = "DefaultOutputCache")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}