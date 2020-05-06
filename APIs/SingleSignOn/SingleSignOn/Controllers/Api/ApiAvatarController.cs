using Autofac;
using Manager.WebApp.Helpers;
using Newtonsoft.Json;
using SingleSignOn.ActionResults;
using SingleSignOn.Caching;
using SingleSignOn.Caching.Providers;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Helpers;
using SingleSignOn.Logging;
using SingleSignOn.Models;
using SingleSignOn.Resources;
using SingleSignOn.Services;
using SingleSignOn.Settings;
using SingleSignOn.ShareLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace SingleSignOn.Controllers
{
    [RoutePrefix("api/file")]
    public class ApiAvatarController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiAvatarController>();
        private const int AvatarStoredWidth = 100;  // ToDo - Change the size of the stored avatar image
        private const int AvatarStoredHeight = 100; // ToDo - Change the size of the stored avatar image
        private const int AvatarScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private string TempFolder = "";
        private string AvatarPath = "";

        private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };

        private IStoreUser _userStore;

        public ApiAvatarController(IStoreUser userStore)
        {
            AvatarPath = ImageSettings.AvatarFolder;
            TempFolder = AvatarPath + "/Temp/";

            _userStore = userStore;
        }

        [HttpPost]
        [Route("upload")]
        public IHttpActionResult UploadFiles()
        {

            var uploadedList = new List<string>();
            var returnModel = new ResponseApiModel();

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count < 1)
            {
                returnModel.Code = EnumCommonCode.Success;
                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            try
            {
                var userId = HttpContext.Current.Request.Params["UserId"];
                if (string.IsNullOrEmpty(userId))
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;

                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                var file = new HttpPostedFileWrapper(httpRequest.Files[0]);
                if (file == null) return Json(new { success = false, errorMessage = "No file uploaded." });
                if (file == null || !IsImage(file)) return Json(new { success = false, errorMessage = "File is of wrong format." });
                if (file.ContentLength <= 0) return Json(new { success = false, errorMessage = "File cannot be zero length." });
                var tempFileName = GetTempSavedFilePath(file, Utils.ConvertToInt32(userId));
                //mistertommat - 18 Nov '15 - replacing '\' to '//' results in incorrect image url on firefox and IE,
                //therefore replacing '\\' to '/' so that a proper web url is returned. 
                var baseFileName = ImageSettings.ImageContainerServer + "/" + ImageSettings.CdnReadContentLink + Path.Combine(TempFolder, tempFileName);


                returnModel.Code = EnumCommonCode.Success;
                returnModel.Data = baseFileName;
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPLOAD FILES: " + ex.Message;
                logger.ErrorException(strError, ex);

                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_UPLOAD_FILES;

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        [HttpPost]
        [Route("saveavatar")]
        public IHttpActionResult SaveAvatar()
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
                var imgPath = string.Format("{0}/{1}/", SystemSettings.MediaFileFolder, ImageSettings.AvatarFolder) + fileName;
                var imgTempPath = string.Format("{0}/{1}/", SystemSettings.MediaFileFolder, TempFolder) + fileName;
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

                img.Crop(top, left, cropHeight, cropWidth);

                // ... and save the new one.                
                var newFilePath = string.Format("{0}/{1}/", SystemSettings.MediaFileFolder, ImageSettings.AvatarFolder) + newFileName;
                //var newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFilePath)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
                }

                img.Save(newFilePath);

                // ... delete the temporary file,...
                //System.IO.File.Delete(imgTempPath);
                //CleanUpTempFolder(1);
                //newFileName = newFileName.Replace("\\", "/");
                var physicalPath = ImageSettings.AvatarFolder + "/" + newFileName;
                var newFileLocation = ImageSettings.ImageContainerServer + "/" + ImageSettings.CdnReadContentLink + "/" + physicalPath;

                if (currentUser != 0)
                {
                    var userIdentity = _userStore.GetUserById(currentUser);
                    userIdentity.Avatar = physicalPath;

                    _userStore.UpdateProfile(userIdentity);
                }

                returnModel.Code = EnumCommonCode.Success;
                returnModel.Data = newFileLocation;
            }
            catch (Exception ex)
            {
                string strError = "Failed for SAVE AVATAR: " + ex.Message;
                logger.ErrorException(strError, ex);

                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_UPLOAD_FILES;

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }
            finally
            {
                //Clear profile of this user
                AccountHelper.ClearUserCache(currentUser);

                //Clear login data of this user
                ClearLoginDataCached();
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        private void ClearLoginDataCached()
        {           
            try
            {
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                cacheProvider.ClearAll(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY);
            }
            catch (Exception ex)
            {
                var strError = "Could not ClearLoginDataCached to CleareUserCache due to: {0}";
                strError = string.Format(ex.Message);
            }
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file == null) return false;
            return file.ContentType.Contains("image") ||
                _imageFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string GetTempSavedFilePath(HttpPostedFileBase file,int userId)
        {
            // Define destination
            //var serverPath = HttpContext.Server.MapPath(TempFolder);
            var serverPath = string.Format("{0}/{1}/", SystemSettings.MediaFileFolder, TempFolder);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            //CleanUpTempFolder(1);
            //return Path.Combine(TempFolder, fileName);
            return fileName;
        }

        private static string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {            
            try
            {
                fileName = FileUploadHelper.ResizeImageByScreenCroping(file, AvatarScreenWidth, serverPath);             
            }
            catch
            {

            }

            return fileName;
        }



        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                var currentUtcNow = DateTime.UtcNow;
                //var serverPath = HttpContext.Server.MapPath("/Upload/Avatars/Temp");
                var serverPath = TempFolder;
                if (!Directory.Exists(serverPath)) return;
                var fileEntries = Directory.GetFiles(serverPath);
                foreach (var fileEntry in fileEntries)
                {
                    var fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                    var res = currentUtcNow - fileCreationTime;
                    if (res.TotalHours > hoursOld)
                    {
                        System.IO.File.Delete(fileEntry);
                    }
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }
    }
}