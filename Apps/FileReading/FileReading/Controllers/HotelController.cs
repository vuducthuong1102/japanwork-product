using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FileReading.Controllers
{
    public class HotelController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(CacheProfile = "DefaultOutputCache")]
        public ActionResult ShowImage(string imageUrl)
        {
            try
            {
                //var dir = Server.MapPath("/Uploads/Hotel");
                //var path = Path.Combine(dir, imageUrl); //validate the path for security or use other means to generate the path.
                //return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");

                var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/Hotel/";
                var path = $@"" + mediaPath;
                if (Request["imageUrl"] != null)
                {
                    path += Request["imageUrl"];
                }

                //if (System.IO.File.Exists(path))
                //{
                //    var bytes = System.IO.File.ReadAllBytes(path);
                //    return File(bytes, "image/jpeg");
                //}

                if (IsValidImageFromLocal(path))
                {
                    return GetOriginalFile(path);
                }
            }
            catch
            {
                
            }

            return GetDefaultImage();
        }

        private FileContentResult GetOriginalFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "image/jpeg");
            }

            return GetDefaultImage();
        }

        private bool IsValidImageFromLocal(string filePath)
        {
            try
            {
                Image newImage = Image.FromFile(filePath);
            }
            catch 
            {
                // Image.FromFile will throw this if file is invalid. 
                // Don't ask me why. return false; 

                return false;
            }

            return true;
        }

        private bool IsAValidImageFileByUrl(string aUrl)
        {
            WebRequest aresponse = HttpWebRequest.Create(aUrl);
            try
            {
                WebResponse anotherrresponse = aresponse.GetResponse();
                
                return anotherrresponse.ContentType == "image/jpeg";
            }
            catch
            {
                return false;
            }
        }

        private FileContentResult GetDefaultImage()
        {
            Response.Cache.SetNoStore();
            Response.Cache.SetNoServerCaching();

            var path = Server.MapPath("~/Content/images/default-image.jpg");
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "image/jpeg");
        } 

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(CacheProfile = "10Minutes")]
        public ActionResult ShowImageShortCache(string imageUrl)
        {
            try
            {
                var dir = Server.MapPath("/Uploads/Hotel");
                var path = Path.Combine(dir, imageUrl); //validate the path for security or use other means to generate the path.
                return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");
            }
            catch (Exception ex)
            {
                Response.Cache.SetNoStore();
                Response.Cache.SetNoServerCaching();

                return Content("An error occurred. Image could not read because: " + ex.ToString());
            }
        }
    }
}