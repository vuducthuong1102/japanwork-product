using FileReading.ActionResults;
using FileReading.Helpers;
using FileReading.Logging;
using FileReading.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace FileReading.Controllers
{
    //[Authorize]
    [RoutePrefix("api/upload")]
    public class ApiUploadFileController : ApiController
    {
        private readonly ILog logger = LogProvider.For<ApiUploadFileController>();

        [AllowAnonymous]
        [HttpPost]
        [Route("images_old")]
        public IHttpActionResult UploadImages_Old()
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

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var fileDir = string.Empty;
                    var subDirPath = (objectId > 0) ? "/" + subDir + "/" + objectId : "/" + subDir;
                    var fileName = FileUploadHelper.UploadSingleImage(postedFile, subDirPath, includeDatePath);

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
        [Route("images")]
        public IHttpActionResult UploadImages()
        {
            var uploadedList = new List<FileUploadResponseModel>();
            var httpRequest = HttpContext.Current.Request;
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

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var fileDir = string.Empty;
                    var subDirPath = (objectId > 0) ? "/" + subDir + "/" + objectId : "/" + subDir;
                    var returnFile = FileUploadHelper.UploadPostedFile(postedFile, subDirPath, includeDatePath);

                    if (returnFile != null)
                        uploadedList.Add(returnFile);
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
        [Route("base64_images")]
        public IHttpActionResult UploadBase64Images()
        {            
            var uploadedList = new List<FileUploadResponseModel>();
            var returnModel = new ResponseApiModel();
            var httpRequest = HttpContext.Current.Request;
            try
            {
                var objectId = 0;
                var subDir = "img";
                var includeDatePath = true;
                var filesPosted = string.Empty;
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

                    filesPosted = vals2.GetValues("FilesInString").First().ToString();
                }

                if (objectId <= 0)
                    objectId = Utils.ConvertToInt32(httpRequest["ObjectId"]);

                var uploadInputFiles = JsonConvert.DeserializeObject<List<string>>(filesPosted);
                if(uploadInputFiles != null && uploadInputFiles.Count > 0)
                {
                    foreach (var file in uploadInputFiles)
                    {
                        var subDirPath = (objectId > 0) ? "/" + subDir + "/" + objectId : "/" + subDir;
                        var returnFile = FileUploadHelper.UploadFrombase64(file, subDirPath, includeDatePath);

                        if (returnFile != null)
                            uploadedList.Add(returnFile);
                    }
                }                

                returnModel.Code = EnumCommonCode.Success;
                returnModel.Data = uploadedList;
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPLOAD IMAGES FROM BASE64: " + ex.Message;
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
                    var fileName = FileUploadHelper.UploadSingleImage(postedFile, "/Trips/" + userId);

                    //var fileUrl = ShareLibCdnHelper.GetFullImgPath(fileName);
                    if (!string.IsNullOrEmpty(fileName))
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

        [AllowAnonymous]
        [HttpPost]
        [Route("videos")]
        public IHttpActionResult UploadVideos()
        {
            var httpRequest = HttpContext.Current.Request;
            var uploadedList = new List<FileUploadResponseModel>();
            var returnModel = new ResponseApiModel();

            if (httpRequest.Files.Count < 1)
            {
                returnModel.Code = -1;
                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            try
            {
                var objectId = 0;
                var subDir = "videos";
                var includeDatePath = true;
                var generateThumb = false;
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

                    var rawGenerate = vals2.GetValues("GenerateThumb");
                    if (rawGenerate != null)
                        generateThumb = Utils.ConvertToBoolean(rawGenerate.First());
                    else
                        generateThumb = false;
                }

                if (objectId <= 0)
                    objectId = Utils.ConvertToInt32(httpRequest["ObjectId"]);

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var subDirPath = (objectId > 0) ? "/" + subDir + "/" + objectId : "/" + subDir;
                    var cover = string.Empty;
                    var fileName = FileUploadHelper.UploadVideo(postedFile, subDirPath, out cover, generateThumb, includeDatePath);
                    
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var fileItem = new FileUploadResponseModel();
                        fileItem.FileName = postedFile.FileName;
                        fileItem.Path = fileName;
                        fileItem.CoverPath = cover;

                        uploadedList.Add(fileItem);
                    }
                }

                returnModel.Code = EnumCommonCode.Success;
                returnModel.Data = uploadedList;
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPLOAD VIDEO FILES: " + ex.Message;

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