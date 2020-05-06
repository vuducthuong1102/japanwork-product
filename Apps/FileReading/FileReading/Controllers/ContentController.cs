using FileReading.Extensions;
using FileReading.Helpers;
using FileReading.Logging;
using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FileReading.Controllers
{
    //[Compress]
    public class ContentController : Controller
    {
        private readonly ILog logger = LogProvider.For<ContentController>();
        static string[] mediaExtensions = {
            //".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", //etc
            ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", //etc
            ".AVI", ".MP4", ".DIVX", ".WMV", //etc
        };

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(CacheProfile = "DefaultOutputCache")]
        public ActionResult ShowImage(string url)
        {
            try
            {
                var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/";
                var path = $@"" + mediaPath;
                if (Request["url"] != null)
                {
                    path += Request["url"];
                }

                return GetOriginalFile(path);
            }
            catch(Exception ex)
            {
                string strError = "Failed for show content: " + ex.Message;
                logger.Error(strError);
            }

            return GetDefaultImage();
        }

        //[OutputCache(CacheProfile = "DefaultOutputCache")]
        //public MyActionResult ShowImage(string url)
        //{
        //    return new MyActionResult();
        //}

        //[AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(CacheProfile = "DefaultOutputCache")]
        //public ActionResult Media(string url)
        //{
        //    try
        //    {
        //        var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/";
        //        var path = $@"" + mediaPath;
        //        if (Request["url"] != null)
        //        {
        //            path += Request["url"];
        //        }

        //        return GetOriginalMediaFile(path);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = "Failed for show Media content: " + ex.Message;
        //        logger.Error(strError);
        //    }

        //    return GetDefaultImage();
        //}

        [OutputCache(CacheProfile = "DefaultOutputCache")]
        public ActionResult Media(string url)
        {            
            try
            {
                var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/";
                var path = $@"" + mediaPath;
                if (Request["url"] != null)
                {
                    path += Request["url"];
                }

                if (System.IO.File.Exists(path))
                {
                    string contentType = MimeMapping.GetMimeMapping(path);
                    //var cd = new System.Net.Mime.ContentDisposition
                    //{
                    //    FileName = fileName,
                    //    Inline = true,
                    //};

                    //Response.AppendHeader("Content-Disposition", cd.ToString());

                    var lastMod = EpochTime.GetIntDate(System.IO.File.GetLastWriteTimeUtc(path));
                    var md5Path = CalculateMD5Hash(path);
                    String etag = CalculateMD5Hash(md5Path + lastMod); //e.g. "00amyWGct0y_ze4lIsj2Mw"

                    var currentMatch = string.Empty;
                    if (!String.IsNullOrEmpty(Request.Headers["If-None-Match"]))
                        currentMatch = Request.Headers["If-None-Match"].ToString();

                    if (!String.IsNullOrEmpty(Request.Headers["if-none-match"]))
                        currentMatch = Request.Headers["if-none-match"].ToString();

                    if (!String.IsNullOrEmpty(currentMatch))
                    {
                        if (currentMatch == etag)
                        {
                            // Response.StatusCode = 304;
                            // Response.StatusDescription = "Not Modified";
                            // return new EmptyResult();
                            //// return new HttpStatusCodeResult(HttpStatusCode.NotModified);
                            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
                        }
                    }
                    else
                    {
                        var bytes = System.IO.File.ReadAllBytes(path);
                        var fileName = Path.GetFileName(path);
                        Response.Cache.SetETag(etag);
                        Response.Cache.SetCacheability(HttpCacheability.Public);

                        return File(bytes, contentType);
                    }
                }
                else
                {
                    return GetDefaultImage();
                }

            }
            catch (Exception ex)
            {
                string strError = "Failed for show Media content: " + ex.Message;
                logger.Error(strError);
            }

            return GetDefaultImage();
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }

        private FileContentResult GetOriginalFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                String etag = CalculateMD5Hash(DateTime.Now.ToString()); //e.g. "00amyWGct0y_ze4lIsj2Mw"
                Response.Cache.SetETag(etag);

                var bytes = System.IO.File.ReadAllBytes(path);

                if (IsMediaFile(path))
                {
                    return File(bytes, "video/mp4");
                }
                return File(bytes, "image/jpeg");
            }

            return GetDefaultImage();
        }

        private FileContentResult GetOriginalMediaFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                String etag = CalculateMD5Hash(DateTime.Now.ToString()); //e.g. "00amyWGct0y_ze4lIsj2Mw"
                Response.Cache.SetETag(etag);

                var bytes = System.IO.File.ReadAllBytes(path);
                var fileName = Path.GetFileName(path);

                string contentType = MimeMapping.GetMimeMapping(path);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(bytes, contentType);

                //return File(bytes, "video/mp4");
            }

            return GetDefaultImage();
        }

        static bool IsMediaFile(string path)
        {
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }

        //private FileStreamResult GetOriginalFile(string path)
        //{
        //    if (System.IO.File.Exists(path))
        //    {
        //        path = Path.Combine(path);
        //        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        //        return new FileStreamResult(fileStream, MimeMapping.GetMimeMapping(path));
        //    }

        //    return GetDefaultImageNew();
        //}

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

        private FileStreamResult GetDefaultImageNew()
        {
            Response.Cache.SetNoStore();
            Response.Cache.SetNoServerCaching();
           
            var path = Server.MapPath("~/Content/images/default-image.jpg");
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, MimeMapping.GetMimeMapping(path));
        }

        public ActionResult GetFile(string url)
        {
            var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/";
            var path = $@"" + mediaPath;
            if (Request["url"] != null)
            {
                path += Request["url"];
            }

            if (System.IO.File.Exists(path))
            {
                return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));
            }
            else
            {
                return Redirect("~/Error/FileNotFound");
            }            
        }
    }

    //public class MyActionResult : ActionResult
    //{
    //    public override void ExecuteResult(ControllerContext controllerContext)
    //    {
    //        var context = controllerContext.HttpContext;

    //        var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/";
    //        var fileName = string.Empty;
    //        var path = $@"" + mediaPath;
    //        if (context.Request["url"] != null)
    //        {
    //            path += context.Request["url"];
    //            fileName = context.Request["url"];
    //        }
    //        context.Response.ContentType = "image\\jpeg";
    //        if (System.IO.File.Exists(path))
    //        {
    //            string extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
    //            context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(600));
    //            context.Response.Cache.SetCacheability(HttpCacheability.Public);
    //            context.Response.Cache.SetValidUntilExpires(false);
                
    //            context.Response.AddHeader("content-disposition", "inline; filename=" + fileName);
    //            context.Response.WriteFile(path);
    //        }                
    //        else
    //        {
    //            context.Response.Cache.SetNoStore();
    //            context.Response.Cache.SetNoServerCaching();
    //            context.Response.WriteFile("~/Content/images/default-image.jpg");
    //        }               
    //    }
    //}
}