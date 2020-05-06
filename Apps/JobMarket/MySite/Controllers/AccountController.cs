using System;
using System.Web.Mvc;
using MySite.Logging;
using MySite.Services;
using Newtonsoft.Json;
using MySite.Helpers;
using MySite.Resources;
using System.Threading.Tasks;
using MySite.Models.Account;
using System.Collections.Generic;
using MySite.ShareLibs;
using MySite.Models;
using MySite.Settings;
using MySite.Helpers.Validation;
using System.Linq;
using System.Web;
using MySite.Attributes;
using SingleSignOn.DB.Sql.Entities;

namespace MySite.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ILog logger = LogProvider.For<AccountController>();

        public AccountController()
        {
            //Constructor 
        }

        public ActionResult ViewProfile(string accountlink, int id)
        {
            var returnModel = new ProfileModel();
            var checkExists = false;
            if (string.IsNullOrEmpty(accountlink) || id == 0)
            {
                return RedirectToErrorPage();
            }
            try
            {
                int CurrentUserId = AccountHelper.GetCurrentUserId();
                var apiInputModel = new ApiUserModel { UserId = id, CurrentUserId = CurrentUserId };
                var apiReturned = AccountServices.GetUserProfileAsync(apiInputModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        var userInfo = JsonConvert.DeserializeObject<IdentityUser>(apiReturned.Data.ToString());
                        returnModel.UserInfo = userInfo;
                        returnModel.UserId = id;
                        returnModel.OwnerId = CurrentUserId;
                        if ("/" + accountlink + "-" + id.ToString() == UrlFriendly.getUrlByUserProfile(userInfo.DisplayName, userInfo.Id))
                        {
                            checkExists = true;

                            if (CurrentUserId > 0)
                            {
                                var apiChecked = AccountServices.GetFeedCounterAsync(apiInputModel).Result;

                                if (apiChecked != null)
                                {
                                    if (apiChecked.Data != null)
                                    {
                                        var result = JsonConvert.DeserializeObject<IdentityUserData>(apiChecked.Data.ToString());
                                        if (result != null)
                                        {

                                            //if (result.IsFriend || result.RequestFriendId == 0)
                                            //{
                                            //    if (id != CurrentUserId)
                                            //    {
                                            //        returnModel.UserFriendId = id;
                                            //    }
                                            //}
                                            //else if (result.RequestFriendId > 0)
                                            //{
                                            //    if (result.UserSendId == CurrentUserId)
                                            //    {
                                            //        returnModel.IsOwnerRequest = true;
                                            //        returnModel.UserFriendId = id;
                                            //    }
                                            //    else
                                            //    {
                                            //        returnModel.IsOwnerRequest = false;
                                            //    }
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                if (!checkExists)
                {
                    return RedirectToErrorPage();
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Get user profile because: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return View(returnModel);
        }

        public ActionResult ShowInfo(int id)
        {
            var returnModel = new ProfileModel();
            var checkExists = false;
            if (id == 0)
            {
                return RedirectToErrorPage();
            }
            try
            {
                var apiInputModel = new ApiUserModel { UserId = id };
                var apiReturned = AccountServices.GetUserProfileAsync(apiInputModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        var userInfo = JsonConvert.DeserializeObject<IdentityUser>(apiReturned.Data.ToString());
                        returnModel.UserInfo = userInfo;
                        checkExists = true;
                    }

                }
                if (!checkExists)
                {
                    return RedirectToErrorPage();
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Get user profile because: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return View("ViewProfile", returnModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult GetListFollowers()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            List<IdentityUser> myList = null;
            var msg = string.Empty;
            var id = 0;

            if (Request["ownerId"] != null)
                id = Utils.ConvertToInt32(Request["ownerId"]);

            if (id <= 0)
            {
                return RedirectToErrorPage();
            }

            try
            {
                var apiFollower = new ApiFollowUserModel();
                apiFollower.UserId = AccountHelper.GetCurrentUserId();
                apiFollower.OwnerId = id;
                if (Request["page"] != null)
                    apiFollower.PageIndex = Utils.ConvertToInt32(Request["page"]);

                apiFollower.PageSize = SystemSettings.DefaultPageSize;
                var apiReturned = UserServices.GetFollowers(apiFollower).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<IdentityUser>>(apiReturned.Data.ToString());
                    }
                    if (myList != null && myList.Count > 0)
                    {
                        isSuccess = true;
                        htmlReturn = PartialViewAsString("../Account/Element/_FollowersList", myList);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetListFollowers because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult GetListFollowing()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            List<IdentityUser> myList = null;
            var msg = string.Empty;
            var id = 0;

            if (Request["ownerId"] != null)
                id = Utils.ConvertToInt32(Request["ownerId"]);

            if (id <= 0)
            {
                return RedirectToErrorPage();
            }

            try
            {
                var apiFollower = new ApiFollowUserModel();
                apiFollower.UserId = AccountHelper.GetCurrentUserId();
                apiFollower.OwnerId = id;
                if (Request["page"] != null)
                    apiFollower.PageIndex = Utils.ConvertToInt32(Request["page"]);

                apiFollower.PageSize = SystemSettings.DefaultPageSize;
                var apiReturned = UserServices.GetFollowings(apiFollower).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        myList = JsonConvert.DeserializeObject<List<IdentityUser>>(apiReturned.Data.ToString());
                    }
                    if (myList != null && myList.Count > 0)
                    {
                        var returnModel = new ProfileModel();
                        returnModel.UserId = apiFollower.UserId;
                        returnModel.OwnerId = apiFollower.OwnerId;
                        returnModel.ListFollowings = myList;

                        isSuccess = true;
                        htmlReturn = PartialViewAsString("../Account/Element/_FollowingList", returnModel);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetListFollowing because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Follower(int id)
        {
            var returnModel = new ProfileModel();

            try
            {
                var UserId = AccountHelper.GetCurrentUserId();
                var apiInputModel = new ApiUserModel { UserId = id };
                var apiReturned = AccountServices.GetUserProfileAsync(apiInputModel).Result;

                var apiFollower = new ApiFollowUserModel { OwnerId = id, UserId = UserId, PageIndex = 1, PageSize = 20 };
                var listFollower = UserServices.GetFollowers(apiFollower).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        returnModel.UserInfo = JsonConvert.DeserializeObject<IdentityUser>(apiReturned.Data.ToString());
                    }
                    else
                    {
                        return RedirectToErrorPage();
                    }
                }

                if (listFollower != null)
                {
                    if (listFollower.Data != null)
                    {
                        returnModel.ListFollowers = JsonConvert.DeserializeObject<List<IdentityUser>>(listFollower.Data.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Get List Followers because: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return View(returnModel);
        }

        public ActionResult Following(int id)
        {
            var returnModel = new ProfileModel();

            try
            {
                var UserId = AccountHelper.GetCurrentUserId();
                var apiInputModel = new ApiUserModel { UserId = id };
                var apiReturned = AccountServices.GetUserProfileAsync(apiInputModel).Result;

                var apiFollower = new ApiFollowUserModel { OwnerId = id, UserId = UserId, PageIndex = 1, PageSize = 20 };
                var listFollowing = UserServices.GetFollowings(apiFollower).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        returnModel.UserInfo = JsonConvert.DeserializeObject<IdentityUser>(apiReturned.Data.ToString());
                    }
                    else
                    {
                        return RedirectToErrorPage();
                    }
                }

                if (listFollowing != null)
                {
                    if (listFollowing.Data != null)
                    {
                        returnModel.ListFollowings = JsonConvert.DeserializeObject<List<IdentityUser>>(listFollowing.Data.ToString());
                    }
                    returnModel.OwnerId = apiFollower.OwnerId;
                    returnModel.UserId = apiFollower.UserId;
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Get List Following because: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return View(returnModel);
        }

        public ActionResult EditProfile()
        {
            var returnModel = new ProfileModel();
            try
            {
                var userLogin = AccountHelper.GetCurrentUser();
                if (userLogin != null)
                {
                    var apiInputModel = new ApiUserModel { UserId = userLogin.Id };
                    var apiReturned = AccountServices.GetUserProfileAsync(apiInputModel).Result;

                    if (apiReturned != null)
                    {
                        if (apiReturned.Data != null)
                        {
                            returnModel.UserInfo = JsonConvert.DeserializeObject<IdentityUser>(apiReturned.Data.ToString());
                        }
                        else
                        {
                            return RedirectToErrorPage();
                        }
                    }
                }
                else
                {
                    return RedirectToErrorPage("Unauthorized");
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Get user profile because: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return View(returnModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult Update()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            try
            {
                var apiModel = GetUpdateProfileData();
                var apiReturned = AccountServices.UpdateProfileAsync(apiModel).Result;
                if (apiReturned != null)
                {
                    if (apiReturned.Code == EnumCommonCode.Success)
                    {
                        isSuccess = true;

                        //Update cached data
                        AccountHelper.UpdateUserCachedData(apiModel.FieldName, apiModel.FieldValue);
                    }

                    msg = apiReturned.Msg;
                }
                else
                {
                    msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
                    logger.Error("Api UpdateProfile return null value");
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying UpdateProfile because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult UploadAvatar()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            try
            {
                var fileUpload = Request.Files;
                var apiModel = new ApiUploadAvatarModel();
                var userId = Utils.ConvertToInt32(Request["UserId"]);
                var listFileUpload = new List<HttpPostedFileBase>();

                //Upload file
                if (fileUpload != null && fileUpload.Count > 0)
                {
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];

                        listFileUpload.Add(file);
                    }

                    apiModel.Files = listFileUpload;
                    apiModel.UserId = userId;

                    var apiResult = AccountServices.UploadAvatarTempAsync(apiModel).Result;
                    if (apiResult != null)
                    {
                        if (apiResult.Code == EnumCommonCode.Success)
                        {
                            isSuccess = true;

                            return Json(new { success = isSuccess, message = msg, data = apiResult.Data, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying UploadAvatar because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult CropAvatar()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            try
            {
                var fileUpload = Request.Files;
                var apiModel = new ApiCropAvatarModel();

                apiModel.userId = AccountHelper.GetCurrentUserId().ToString();
                apiModel.fileName = Request["fileName"];
                apiModel.t = Request["t"];
                apiModel.l = Request["l"];
                apiModel.h = Request["h"];
                apiModel.w = Request["w"];

                var apiResult = AccountServices.CropAvatarTempAsync(apiModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.Code == EnumCommonCode.Success)
                    {
                        isSuccess = true;

                        return Json(new { success = isSuccess, message = msg, data = apiResult.Data, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying CropAvatarTempAsync because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult UpdateAvatar()
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            try
            {
                var fieldName = string.Empty;
                var fieldValue = string.Empty;

                if (Request["field"] != null)
                    fieldName = Request["field"];

                if (Request["value"] != null)
                    fieldValue = Request["value"];

                //Update cached data
                AccountHelper.UpdateUserCachedData(fieldName, fieldValue);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying UpdateAvatar because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateCachedUserInfo()
        {
            try
            {
                var fieldName = Request["FieldName"];
                var fieldValue = Request["FieldValue"];

                //Update cached data
                AccountHelper.UpdateUserCachedData(fieldName, fieldValue);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying UpdateCachedUserInfo because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return Json(new { success = true, msg = "UpdateCacheDone" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult GetCounter()
        {
            var userId = 0;

            if (Request["userId"] != null)
            {
                userId = Utils.ConvertToInt32(Request["userId"]);
            }

            if (userId == 0)
            {
                userId = AccountHelper.GetCurrentUserId();
            }

            var returnModel = new IdentityUserData();
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            try
            {
                var apiInputModel = new ApiUserModel { UserId = userId };
                var apiReturned = AccountServices.GetCounterAsync(apiInputModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        isSuccess = true;
                        returnModel = JsonConvert.DeserializeObject<IdentityUserData>(apiReturned.Data.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Get user counter because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return Json(new { success = isSuccess, message = msg, data = returnModel }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult GetFeedCounter()
        {
            var userId = 0;

            if (Request["userId"] != null)
            {
                userId = Utils.ConvertToInt32(Request["userId"]);
            }

            if (userId == 0)
            {
                userId = AccountHelper.GetCurrentUserId();
            }

            var returnModel = new IdentityUserData();
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            try
            {
                var apiInputModel = new ApiUserModel { UserId = userId };
                var apiReturned = AccountServices.GetFeedCounterAsync(apiInputModel).Result;

                if (apiReturned != null)
                {
                    if (apiReturned.Data != null)
                    {
                        isSuccess = true;
                        returnModel = JsonConvert.DeserializeObject<IdentityUserData>(apiReturned.Data.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetFeedCounter because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return Json(new { success = isSuccess, message = msg, data = returnModel }, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult GetNotification()
        //{
        //    var isSuccess = false;
        //    var htmlReturn = string.Empty;
        //    List<IdentityNotification> notifs = null;
        //    var msg = string.Empty;
        //    try
        //    {
        //        var apiFilterModel = new ApiFilterModel();
        //        var currentUser = AccountHelper.GetCurrentUser();
        //        if (currentUser != null)
        //        {
        //            apiFilterModel.UserId = currentUser.Id;
        //            apiFilterModel.PageIndex = 1;
        //            apiFilterModel.PageSize = SystemSettings.NotifPageSize;
        //            var apiReturned = AccountServices.GetNotificationAsync(apiFilterModel).Result;

        //            if (apiReturned != null)
        //            {
        //                if (apiReturned.Data != null)
        //                {
        //                    notifs = JsonConvert.DeserializeObject<List<IdentityNotification>>(apiReturned.Data.ToString());
        //                }
        //                isSuccess = true;
        //                htmlReturn = PartialViewAsString("../Widgets/_Notifications", notifs);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying GetNotification because: {0}", ex.ToString());
        //        logger.Error(strError);
        //        msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
        //        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
        //    }

        //    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ViewAllNotif()
        //{
        //    return View("ViewNotifications");
        //}

        //public ActionResult GetNotificationsByPage()
        //{
        //    var isSuccess = false;
        //    var htmlReturn = string.Empty;
        //    List<IdentityNotification> notifs = null;
        //    var msg = string.Empty;
        //    try
        //    {
        //        var apiFilterModel = new ApiFilterModel();
        //        var currentUser = AccountHelper.GetCurrentUser();
        //        if (currentUser != null)
        //        {
        //            if (Request["page"] != null)
        //                apiFilterModel.PageIndex = Utils.ConvertToInt32(Request["page"]);

        //            apiFilterModel.PageSize = SystemSettings.DefaultPageSize;

        //            apiFilterModel.UserId = currentUser.Id;
        //            var apiReturned = AccountServices.GetNotificationAsync(apiFilterModel).Result;

        //            if (apiReturned != null)
        //            {
        //                if (apiReturned.Data != null)
        //                {
        //                    notifs = JsonConvert.DeserializeObject<List<IdentityNotification>>(apiReturned.Data.ToString());
        //                }
        //                if (notifs != null && notifs.Count > 0)
        //                {
        //                    isSuccess = true;
        //                    htmlReturn = PartialViewAsString("../Widgets/_Notifications", notifs);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying Notifications because: {0}", ex.ToString());
        //        logger.Error(strError);
        //        msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
        //        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
        //    }

        //    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetInviteByPage()
        //{
        //    var isSuccess = false;
        //    var htmlReturn = string.Empty;
        //    IdentityNotifFriend requestFriends = null;
        //    var msg = string.Empty;
        //    try
        //    {
        //        var apiFilterModel = new ApiFilterModel();
        //        var currentUser = AccountHelper.GetCurrentUser();
        //        if (currentUser != null)
        //        {
        //            if (Request["page"] != null)
        //                apiFilterModel.PageIndex = Utils.ConvertToInt32(Request["page"]);

        //            apiFilterModel.PageSize = SystemSettings.DefaultPageSize;

        //            apiFilterModel.UserId = currentUser.Id;
        //            var apiReturned = FriendServices.GetInviteByPage(apiFilterModel).Result;

        //            if (apiReturned != null)
        //            {
        //                if (apiReturned.Data != null)
        //                {
        //                    requestFriends = JsonConvert.DeserializeObject<IdentityNotifFriend>(apiReturned.Data.ToString());
        //                }
        //                if (requestFriends != null)
        //                {
        //                    isSuccess = true;
        //                    htmlReturn = PartialViewAsString("../Account/Element/_FriendRequestLists", requestFriends);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying _FriendRequestLists because: {0}", ex.ToString());
        //        logger.Error(strError);
        //        msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
        //        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
        //    }

        //    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult NotifDetail(string id)
        //{
        //    try
        //    {
        //        var notifId = 0;
        //        notifId = Utils.ConvertToInt32(id);

        //        if (notifId <= 0)
        //        {
        //            return RedirectToErrorPage();
        //        }

        //        var model = new IdentityNotification();
        //        model.Id = notifId;
        //        model.UserId = AccountHelper.GetCurrentUserId();

        //        IdentityNotification notifInfo = null;
        //        var apiReturned = AccountServices.ViewNotificatioAsync(model).Result;

        //        if (apiReturned != null)
        //        {
        //            if (apiReturned.Data != null)
        //            {
        //                notifInfo = JsonConvert.DeserializeObject<IdentityNotification>(apiReturned.Data.ToString());

        //                if (notifInfo != null)
        //                {
        //                    var objectUrl = string.Empty;
        //                    if (notifInfo.ObjectType == CommonObject.Post || notifInfo.ObjectType == CommonObject.Comment)
        //                    {
        //                        if (!string.IsNullOrEmpty(notifInfo.UrlRedirect))
        //                        {
        //                            objectUrl = string.Format("/post/getinfo/{0}?url={1}", notifInfo.ObjectId, HttpUtility.UrlEncode(notifInfo.UrlRedirect));
        //                        }
        //                        else
        //                        {
        //                            objectUrl = string.Format("/post/getinfo/{0}", notifInfo.ObjectId);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var userId = Utils.ConvertToInt32(notifInfo.ObjectId);
        //                        var apiUser = AccountServices.GetUserProfileAsync(new ApiUserModel(), userId).Result;
        //                        if (apiUser != null)
        //                        {
        //                            if (apiUser.Data != null)
        //                            {
        //                                var userInfo = JsonConvert.DeserializeObject<IdentityUser>(apiUser.Data.ToString());
        //                                objectUrl = AccountHelper.GetUserProfileUrl(userInfo);
        //                            }
        //                        }

        //                    }

        //                    return Redirect(objectUrl);
        //                }
        //                else
        //                {
        //                    return RedirectToErrorPage();
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToErrorPage();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying Get NotifDetail because: {0}", ex.ToString());
        //        logger.Error(strError);

        //        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
        //    }

        //    return RedirectToErrorPage();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[PreventCrossOrigin]
        //public async Task<ActionResult> GetListPosted()
        //{
        //    List<IdentityPost> posts = null;
        //    var isSuccess = false;
        //    var htmlReturn = string.Empty;
        //    var msg = string.Empty;
        //    try
        //    {
        //        var filter = GetPostFilter();
        //        var postApiReturned = await PostServices.GetListPostedAsync(filter);

        //        if (postApiReturned != null)
        //        {
        //            if (postApiReturned.Data != null)
        //            {
        //                posts = JsonConvert.DeserializeObject<List<IdentityPost>>(postApiReturned.Data.ToString());
        //            }

        //            if (posts != null && posts.Count > 0)
        //            {
        //                isSuccess = true;
        //                htmlReturn = PartialViewAsString("../Widgets/Post/_NewsFeedPosts", posts);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying Get list posted because: {0}", ex.ToString());
        //        logger.Error(strError);
        //        msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
        //    }

        //    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[PreventCrossOrigin]
        //public async Task<ActionResult> GetUploadedImages()
        //{
        //    List<IdentityImage> images = null;
        //    var isSuccess = false;
        //    var htmlReturn = string.Empty;
        //    var msg = string.Empty;
        //    try
        //    {
        //        var filter = GetPostFilter();
        //        var postApiReturned = await PostServices.GetUploadedImagesAsync(filter);
        //        if (postApiReturned != null)
        //        {
        //            if (postApiReturned.Data != null)
        //            {
        //                images = JsonConvert.DeserializeObject<List<IdentityImage>>(postApiReturned.Data.ToString());
        //            }

        //            if (images != null && images.Count > 0)
        //            {
        //                isSuccess = true;
        //                htmlReturn = PartialViewAsString("../Account/_UploadedImages", images);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying Get uploaded images because: {0}", ex.ToString());
        //        logger.Error(strError);
        //        msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
        //    }

        //    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[PreventCrossOrigin]
        //public async Task<ActionResult> GetListSaved()
        //{
        //    List<IdentityPost> posts = null;
        //    var isSuccess = false;
        //    var htmlReturn = string.Empty;
        //    var msg = string.Empty;
        //    try
        //    {
        //        var filter = GetPostFilter();
        //        var postApiReturned = await PostServices.GetListSavedAsync(filter);
        //        if (postApiReturned != null)
        //        {
        //            if (postApiReturned.Data != null)
        //            {
        //                posts = JsonConvert.DeserializeObject<List<IdentityPost>>(postApiReturned.Data.ToString());
        //            }

        //            if (posts != null && posts.Count > 0)
        //            {
        //                isSuccess = true;
        //                htmlReturn = PartialViewAsString("../Widgets/Post/_NewsFeedPosts", posts);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed when trying Get list Saved because: {0}", ex.ToString());
        //        logger.Error(strError);
        //        msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
        //    }

        //    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public async Task<ActionResult> ChangePassword(WebAccountChangePasswordModel model)
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            var returnModel = new ResponseApiModel();

            if (!ModelState.IsValid)
            {
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
                return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userInfo = AccountHelper.GetCurrentUser();
                if (userInfo == null)
                {
                    msg = UserWebResource.COMMON_ERROR_SESSION_ENDED;
                }
                else
                {
                    model.UserId = userInfo.Id;
                    model.TokenKey = userInfo.TokenKey;

                    var apiModel = ChangePasswordConvertToApiData(model);

                    var apiReturned = await AccountServices.ChangePasswordAsync(apiModel);
                    if (apiReturned != null)
                    {
                        if (apiReturned != null)
                        {
                            if (apiReturned.Code == EnumCommonCode.Success)
                            {
                                isSuccess = true;
                            }

                            msg = apiReturned.Msg;
                        }
                        else
                        {
                            msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
                            logger.Error("Api changepwd return null value");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying change password because: {0}", ex.ToString());
                logger.Error(strError);
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private ApiUpdateProfileModel GetUpdateProfileData()
        {
            var userInfo = AccountHelper.GetCurrentUser();
            var apiModel = new ApiUpdateProfileModel();
            if (Request["field"] != null)
                apiModel.FieldName = Request["field"];

            if (Request["value"] != null)
                apiModel.FieldValue = Request["value"];

            if (userInfo != null)
            {
                apiModel.UserId = userInfo.Id;
                apiModel.Token = userInfo.TokenKey;
                apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);
            }

            return apiModel;
        }

        private string GetUpdateProfleResponseMessage(int statusCode)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserWebResource.MS_UPDATE_PROFILE_SUCCESS;
            }
            else if (statusCode == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                message = UserWebResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }

            return message;
        }

        private ApiChangePasswordModel ChangePasswordConvertToApiData(WebAccountChangePasswordModel inputModel)
        {
            var model = new ApiChangePasswordModel();
            model.UserId = inputModel.UserId;
            model.Token = inputModel.TokenKey;
            model.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);
            model.PwdType = PasswordLevelType.Level1;
            model.NewPwd = Utility.Md5HashingData(inputModel.NewPassword);
            model.OldPwd1 = Utility.Md5HashingData(inputModel.OldPassword);

            return model;
        }

        #endregion
    }
}
