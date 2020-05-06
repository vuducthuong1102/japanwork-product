
using Manager.SharedLibs;
using Manager.SharedLibs.Logging;
using Manager.WebApp.ActionResults;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace Manager.WebApp.Controllers
{
    //[Authorize]
    [RoutePrefix("api/upload")]
    public class ApiUploadFileController : ApiController
    {
        private readonly ILog logger = LogProvider.For<ApiUploadFileController>();

        [AllowAnonymous]
        [HttpGet]
        [Route("abc")]
        public IHttpActionResult Abc()
        {
            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject("OK"));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("images")]
        public IHttpActionResult UploadImages()
        {
            var httpRequest = HttpContext.Current.Request;
            var uploadedList = new List<string>();
            var returnModel = new ResponseApiModel();

            if (httpRequest.Files.Count < 1)
            {
                returnModel.Code = EnumCommonCode.Error;
                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            try
            {
                var objectId = 0;
                var subDir = "img";
                var includeDatePath = true;
                NameValueCollection vals2 = null;
                var formData = HttpContext.Current.Request.Params["MyFormData"];
                if (formData != null)
                {
                    string vals = HttpContext.Current.Request.Form.GetValues("MyFormData").First();
                    vals2 = HttpUtility.ParseQueryString(vals);
                }

                if (vals2 != null)
                {
                    objectId = Utils.ConvertToInt32(vals2.GetValues("ObjectId").First());
                    subDir = vals2.GetValues("SubDir").First().ToString();
                    var rawInclude = vals2.GetValues("InCludeDatePath");
                    if (rawInclude != null)
                        includeDatePath = Utils.ConvertToBoolean(rawInclude.First());
                    else
                        includeDatePath = false;
                }

                if (objectId <= 0)
                    objectId = Utils.ConvertToInt32(httpRequest["ObjectId"]);

                if (objectId <= 0)
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = "Upload failed. ObjectId not found";

                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var fileDir = string.Empty;
                    var fileName = FileUploadHelper.UploadPostedFile(postedFile, "/" + subDir + "/" + objectId, includeDatePath);

                    uploadedList.Add(fileName);
                }

                returnModel.Code = EnumCommonCode.Success;
                returnModel.Data = uploadedList;
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPLOAD IMAGES: " + ex.Message;
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;
                logger.Error(strError);

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("postimages")]
        public IHttpActionResult UploadPostFiles()
        {
            var httpRequest = HttpContext.Current.Request;
            var uploadedList = new List<string>();
            var returnModel = new ResponseApiModel();

            if (httpRequest.Files.Count < 1)
            {
                returnModel.Code = -1;
                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            try
            {
                var userId = HttpContext.Current.Request.Params["UserId"];
                if (string.IsNullOrEmpty(userId))
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = "Upload failed. UserId not found";

                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var fileName = FileUploadHelper.UploadPostedFile(postedFile, "/Trips/" + userId);

                    //var fileUrl = ShareLibCdnHelper.GetFullImgPath(fileName);
                    uploadedList.Add(fileName);
                }

                returnModel.Code = EnumCommonCode.Success;
                returnModel.Data = uploadedList;
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPLOAD FILES: " + ex.Message;

                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;
                logger.Error(strError);

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("postvideos")]
        //public IHttpActionResult UploadPostVideos()
        //{
        //    var httpRequest = HttpContext.Current.Request;
        //    var uploadedList = new List<FileUploadResponseModel>();
        //    var returnModel = new ResponseApiModel();

        //    if (httpRequest.Files.Count < 1)
        //    {
        //        returnModel.Code = -1;
        //        return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        //    }

        //    try
        //    {
        //        var userId = HttpContext.Current.Request.Params["UserId"];
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            returnModel.Code = EnumCommonCode.Error;
        //            returnModel.Msg = "Upload failed. UserId not found";

        //            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        //        }

        //        foreach (string file in httpRequest.Files)
        //        {
        //            var postedFile = httpRequest.Files[file];
        //            var cover = string.Empty;
        //            var fileName = FileUploadHelper.UploadVideo(postedFile, "/Trips/" + userId, out cover);

        //            var fileItem = new FileUploadResponseModel();
        //            fileItem.FilePath = fileName;
        //            fileItem.Cover = cover;

        //            uploadedList.Add(fileItem);
        //        }

        //        returnModel.Code = EnumCommonCode.Success;
        //        returnModel.Data = uploadedList;
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = "Failed for UPLOAD VIDEO FILES: " + ex.Message;

        //        returnModel.Code = EnumCommonCode.Error;
        //        returnModel.Msg = strError;
        //        logger.Error(strError);

        //        return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        //    }

        //    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        //}
    }
}