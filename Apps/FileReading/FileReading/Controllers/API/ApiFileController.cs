using FileReading.ActionResults;
using FileReading.Helpers;
using FileReading.Logging;
using FileReading.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using FileReading.Settings;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Helpers;
using System.Text.RegularExpressions;

namespace FileReading.Controllers
{
    //[Authorize]
    [RoutePrefix("api/file")]
    public class ApiFileController : ApiController
    {
        private readonly ILog logger = LogProvider.For<ApiFileController>();

        [AllowAnonymous]
        [HttpPost]
        [Route("uploadavatar")]
        public IHttpActionResult UploadAvatar()
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

                var requestUrl = HttpContext.Current.Request.Url;
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var fileName = FileUploadHelper.UploadSingleImage(postedFile, "/Avatars/Temp");

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        //uploadedList.Add(fileName);
                        var fullPath = string.Format("{0}://{1}/Content/ShowImage?url={2}", requestUrl.Scheme, requestUrl.Authority, fileName);

                        returnModel.Data = new { path = fileName, fullPath = fullPath };

                        break;
                    }
                }

                returnModel.Code = EnumCommonCode.Success;
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

        [HttpPost]
        [Route("cutavatar")]
        public IHttpActionResult CutAvatar()
        {
            //string t, string l, string h, string w, string fileName
            var t = HttpContext.Current.Request["t"];
            var l = HttpContext.Current.Request["l"];
            var h = HttpContext.Current.Request["h"];
            var w = HttpContext.Current.Request["w"];
            var fileName = HttpContext.Current.Request["fileName"];
            var userId = HttpContext.Current.Request["userId"];
            var returnModel = new ResponseApiModel();
            var currentUser = 0;

            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    currentUser = Utils.ConvertToInt32(userId);
                }
                // Calculate dimensions
                var top = Convert.ToInt32(t.Replace("-", "").Replace("px", ""));
                var left = Convert.ToInt32(l.Replace("-", "").Replace("px", ""));
                var height = Convert.ToInt32(h.Replace("-", "").Replace("px", ""));
                var width = Convert.ToInt32(w.Replace("-", "").Replace("px", ""));

                var newFileName = string.Empty;

                var match = Regex.Match(fileName, @"(Avatars.*)");
                fileName = match.Groups[1].Value;
                fileName = fileName.Replace("Avatars/Temp/", string.Empty);
                // Get file from temporary folder
                //var fn = Path.Combine(Server.MapPath(MapTempFolder), Path.GetFileName(fileName));
                var imgPath = string.Format("{0}/{1}/", SystemSettings.MediaFileUrl, ImageSettings.AvatarFolder) + fileName;
                var imgTempPath = string.Format("{0}/{1}", SystemSettings.MediaFileUrl, HttpContext.Current.Request["fileName"]) ;
                var fn = imgTempPath;
                // ...get image and resize it, ...
                var img = new WebImage(fn);
                if (currentUser != 0)
                {
                    newFileName = string.Format("{0}_{1}{2}", currentUser, EpochTime.GetIntDate(DateTime.Now), Path.GetExtension(fileName));
                }
                else
                {
                    newFileName = string.Format("{0}{1}", EpochTime.GetIntDate(DateTime.Now), Path.GetExtension(fileName));
                }

                img.Resize(width, height);

                // ... crop the part the user selected, ...
                var cropHeight = img.Height - top - ImageSettings.AvatarHeight;
                var cropWidth = img.Width - left - ImageSettings.AvatarWidth;
                if (cropHeight <= 0) cropHeight = img.Height;
                if (cropWidth <= 0) cropWidth = img.Width;
                if (top == 0) top = 1;
                if (left == 0) left = 1;
                img.Crop(top, left, cropHeight, cropWidth);

                // ... and save the new one.                
                var newFilePath = string.Format("{0}/{1}/", SystemSettings.MediaFileUrl, ImageSettings.AvatarFolder) + newFileName;
                //var newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFilePath)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
                }

                img.Save(newFilePath);

                var physicalPath = ImageSettings.AvatarFolder + "/" + newFileName;
                var requestUrl = HttpContext.Current.Request.Url;
                //var newFileLocation = ImageSettings.ImageContainerServer + "/" + ImageSettings.CdnReadContentLink + "/" + physicalPath;
                var newFileLocation = string.Format("{0}://{1}/Content/ShowImage?url={2}", requestUrl.Scheme, requestUrl.Authority, physicalPath);

                returnModel.Data = new { path = physicalPath, fullPath = newFileLocation };
                returnModel.Code = EnumCommonCode.Success;
            }
            catch (Exception ex)
            {
                string strError = "Failed for SAVE AVATAR: " + ex.Message;
                logger.ErrorException(strError, ex);

                returnModel.Code = EnumCommonCode.Error;

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("delete")]
        public IHttpActionResult DeleteFiles(FilesDeleteModel model)
        {
            var returnModel = new ResponseApiModel();
            try
            {
                if(model == null || model.FilesPath == null)
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = "Model is invalid. Model could not be null";

                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                if(model.FilesPath.Count > 0)
                {
                    foreach (var itemPath in model.FilesPath)
                    {
                        DeleteFile(itemPath);
                    }
                }
                returnModel.Code = EnumCommonCode.Success;
            }
            catch (Exception ex)
            {
                string strError = "Failed for DELETE FILES: " + ex.Message;
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;
                logger.Error(strError);

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        #region Helpers

        private bool DeleteFile(string filePath)
        {
            try
            {
                var mediaPath = ConfigurationManager.AppSettings["MediaFileUrl"].ToString() + "/";
                var fullPath = $@"" + mediaPath;
                if (!string.IsNullOrEmpty(filePath))
                {
                    fullPath += filePath;
                }

                if (System.IO.File.Exists(fullPath))
                {
                    //Delete file if existed
                    System.IO.File.Delete(fullPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for DELETE [{0}]. Because: {1}", filePath, ex.Message);
                logger.Error(strError);
            }

            return false;
        }

        #endregion
    }
}