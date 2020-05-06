//using ApiJobMarket.DB.Sql.Entities;
//using Manager.SharedLibs;
//using Manager.SharedLibs.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Models;
//using Manager.WebApp.Resources;
//using Manager.WebApp.Services;
//using Manager.WebApp.Settings;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Web.Mvc;

//namespace Manager.WebApp.Controllers
//{
//    public class MessengerController : BaseAuthedController
//    {
//        private readonly ILog logger = LogProvider.For<MessengerController>();

//        public MessengerController()
//        {

//        }

//        public ActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [PreventSpam]
//        public async Task<ActionResult> GetFriendLists()
//        {
//            var isSuccess = true;
//            var htmlReturn = string.Empty;
//            var msg = string.Empty;
//            var listIds = string.Empty;

//            List<BaseOnlineUserIdentity> friends = new List<BaseOnlineUserIdentity>();
//            try
//            {
//                //var apiFilter = GetFilter();
//                //apiFilter.UserId = AccountHelper.GetCurrentUserId();

//                //var apiReturned = await FriendServices.GetByPageAsync(apiFilter);
//                //if (apiReturned != null)
//                //{
//                //    if (apiReturned.Data != null)
//                //    {
//                //        friends = JsonConvert.DeserializeObject<List<IdentityUser>>(apiReturned.Data.ToString());
//                //        friends = CheckUserOnline(friends);

//                //        friends = friends.OrderByDescending(x => x.IsOnline).ThenByDescending(x => x.LastOnline).ToList();

//                //        htmlReturn = PartialViewAsString("../Widgets/Contact/_FriendList", friends);
//                //    }
//                //}

//                var currentUser = GetCurrentUser();
//                var listMembers = CommonHelpers.GetListUser(currentUser.ParentId);
//                if (listMembers.HasData())
//                {
//                    foreach (var item in listMembers)
//                    {
//                        if(item.StaffId != currentUser.StaffId)
//                        {
//                            var userOnline = new BaseOnlineUserIdentity();
//                            userOnline.Id = item.StaffId;
//                            userOnline.UserName = item.UserName;
//                            userOnline.DisplayName = item.FullName;
//                            userOnline.Avatar = "/Content/Avatars/default-avatar.png";

//                            friends.Add(userOnline);
//                        }                        
//                    }
//                }

//                htmlReturn = PartialViewAsString("../Widgets/Contact/_FriendList", friends);
//            }
//            catch (Exception ex)
//            {
//                isSuccess = false;
//                var strError = string.Format("Failed when trying GetFriendList because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public async Task<ActionResult> OpenConversation()
//        {
//            var receiverId = 0;

//            var isSuccess = false;
//            var htmlReturn = string.Empty;
//            var msg = string.Empty;

//            try
//            {
//                if (Request["receiver"] != null)
//                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

//                if (receiverId == 0)
//                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, html = htmlReturn }, JsonRequestBehavior.AllowGet);

//                var userInfo = GetCurrentUser();
//                if (userInfo == null)
//                {
//                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;

//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }

//                var apiReturned = await AccountServices.GetUserProfileAsync(new ApiUserModel { UserId = receiverId });
//                if (apiReturned != null)
//                {
//                    if (apiReturned.Data != null)
//                    {
//                        var receiverInfo = JsonConvert.DeserializeObject<IdentityUser>(apiReturned.Data.ToString());

//                        var boxModel = new ConversationModel();
//                        if (receiverInfo != null)
//                        {
//                            boxModel.TargetUser = receiverInfo;

//                            isSuccess = true;
//                            htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_ChatBoxTemp", boxModel);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying GetCurrentConversation because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public async Task<ActionResult> GetCurrentConversation()
//        {
//            var receiverId = 0;
//            var isSuccess = true;
//            var htmlReturn = string.Empty;
//            var msg = string.Empty;

//            try
//            {
//                if (Request["receiver"] != null)
//                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

//                if (receiverId == 0)
//                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, html = htmlReturn }, JsonRequestBehavior.AllowGet);

//                var userInfo = AccountHelper.GetCurrentUser();
//                if (userInfo == null)
//                {
//                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;
//                    isSuccess = false;
//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }


//                var apiModel = new ApiConversationModel();
//                apiModel.UserOne = userInfo.StaffId;
//                apiModel.UserTwo = receiverId;
//                apiModel.Ip = Request.UserHostAddress;

//                var conversationId = InitConversation(userInfo.StaffId, receiverId);
//                if (conversationId == 0)
//                {
//                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
//                    isSuccess = false;
//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }

//                if (conversationId > 0)
//                {
//                    var filterModel = GetMessageFilter();
//                    filterModel.OwnerId = userInfo.StaffId;
//                    filterModel.UserTwo = receiverId;

//                    var returnMsgApi = await ConversationServices.GetMessagesByPageAsync(filterModel);
//                    if (returnMsgApi != null)
//                    {
//                        var myList = JsonConvert.DeserializeObject<List<IdentityConversationReply>>(returnMsgApi.Data.ToString());
//                        if (myList != null && myList.Count > 0)
//                        {
//                            htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageList", myList);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying GetCurrentConversation because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public async Task<ActionResult> LoadMoreMessages()
//        {
//            var receiverId = 0;
//            var isSuccess = true;
//            var htmlReturn = string.Empty;
//            var msg = string.Empty;

//            try
//            {
//                if (Request["receiver"] != null)
//                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

//                if (receiverId == 0)
//                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, html = htmlReturn }, JsonRequestBehavior.AllowGet);

//                var userInfo = AccountHelper.GetCurrentUser();
//                if (userInfo == null)
//                {
//                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;
//                    isSuccess = false;
//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }


//                var apiModel = new ApiConversationModel();
//                apiModel.UserOne = userInfo.StaffId;
//                apiModel.UserTwo = receiverId;
//                apiModel.Ip = Request.UserHostAddress;

//                var conversationId = InitConversation(userInfo.StaffId, receiverId);
//                if (conversationId == 0)
//                {
//                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
//                    isSuccess = false;
//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }

//                if (conversationId > 0)
//                {
//                    var filterModel = GetMessageFilter();
//                    filterModel.OwnerId = userInfo.StaffId;
//                    filterModel.UserTwo = receiverId;

//                    var returnMsgApi = await ConversationServices.GetMessagesByPageAsync(filterModel);
//                    if (returnMsgApi != null)
//                    {
//                        var myList = JsonConvert.DeserializeObject<List<IdentityConversationReply>>(returnMsgApi.Data.ToString());
//                        if (myList != null && myList.Count > 0)
//                        {
//                            htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageList", myList);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying LoadMoreMessages because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public async Task<ActionResult> Send()
//        {
//            var message = string.Empty;
//            var conversationId = 0;
//            var receiverId = 0;

//            var isSuccess = false;
//            var htmlReturn = string.Empty;
//            var msg = string.Empty;

//            try
//            {
//                var userInfo = AccountHelper.GetCurrentUser();
//                if (userInfo == null)
//                {
//                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;

//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }

//                if (Request["message"] != null)
//                    message = Request["message"].ToString();

//                if (Request["receiver"] != null)
//                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

//                if (Request["conversation"] != null)
//                    conversationId = Utils.ConvertToInt32(Request["conversation"]);

//                if (conversationId == 0)
//                    conversationId = InitConversation(userInfo.StaffId, receiverId);

//                if (conversationId == 0)
//                {
//                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;

//                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//                }

//                var apiMsgModel = new ApiConversationReplyModel();
//                apiMsgModel.Ip = Request.UserHostAddress;
//                apiMsgModel.ConversationId = conversationId;
//                apiMsgModel.UserId = userInfo.StaffId;
//                apiMsgModel.Content = message;
//                apiMsgModel.Type = EnumMessageTypes.Text;

//                var apiReturned = await ConversationServices.SendMessageAsync(apiMsgModel);

//                if (apiReturned != null)
//                {
//                    if (apiReturned.Data != null)
//                    {
//                        isSuccess = (apiReturned.Code == EnumCommonCode.Success);
//                        msg = apiReturned.Msg;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying Send because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        #region Helpers

//        private int InitConversation(int fromUserId, int receiverId)
//        {
//            var apiModel = new ApiConversationModel();
//            apiModel.UserOne = fromUserId;
//            apiModel.UserTwo = receiverId;
//            apiModel.Ip = Request.UserHostAddress;

//            var conversationId = 0;
//            try
//            {
//                var apiReturned = ConversationServices.GetCurrentAsync(apiModel).Result;

//                if (apiReturned != null)
//                {
//                    if (apiReturned.Data != null)
//                    {
//                        conversationId = Utils.ConvertToInt32(apiReturned.Data);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying InitConversation because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return conversationId;
//        }

//        private ApiGetMessagesModel GetMessageFilter()
//        {
//            var filter = new ApiGetMessagesModel();
//            filter.PageIndex = 1;
//            if (Request["page"] != null)
//                filter.PageIndex = Utils.ConvertToInt32(Request["page"]);

//            filter.PageSize = SystemSettings.DefaultPageSize;
//            return filter;
//        }

//        #endregion
//    }
//}