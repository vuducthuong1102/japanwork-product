//using Autofac;
//using Manager.SharedLibs;
//using Manager.SharedLibs.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Models;
//using Manager.WebApp.Resources;
//using Manager.WebApp.Services;
//using Manager.WebApp.Settings;
//using MsSql.AspNet.Identity;
//using MsSql.AspNet.Identity.Entities;
//using MsSql.AspNet.Identity.MsSqlStores;
//using MsSql.AspNet.Identity.Stores;
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
//                            var userOnline = ParseUserOnlineInfo(item);
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

//                var receiverInfo = AccountHelper.GetByStaffId(receiverId);

//                var boxModel = new ConversationModel();
//                if (receiverInfo != null)
//                {
//                    boxModel.TargetUser = receiverInfo;

//                    boxModel.TargetUser.Avatar = "/Content/Avatars/messenger-avatar.png";

//                    isSuccess = true;
//                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_ChatBoxTemp", boxModel);
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
                    
//                    var store = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();

//                    var myList = store.GetByPage(filterModel);
//                    if (myList.HasData())
//                    {
//                        var userOne = ParseUserOnlineInfo(userInfo);
//                        var targetUser = GetByStaffId(receiverId);
//                        var userTwo = ParseUserOnlineInfo(targetUser);     
                        
//                        foreach (var item in myList)
//                        {
//                            item.UserOneInfo = userOne;
//                            item.UserTwoInfo = userTwo;
//                        }

//                        htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageList", myList);
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

//                    var store = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();
//                    var myList = store.GetByPage(filterModel);
//                    if (myList.HasData())
//                    {
//                        var userOne = ParseUserOnlineInfo(userInfo);
//                        var targetUser = GetByStaffId(receiverId);
//                        var userTwo = ParseUserOnlineInfo(targetUser);

//                        foreach (var item in myList)
//                        {
//                            item.UserOneInfo = userOne;
//                            item.UserTwoInfo = userTwo;
//                        }

//                        htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageList", myList);
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

//                var replyStore = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();

//                var record = new IdentityConversationReply();
//                record.Ip = Request.UserHostAddress;
//                record.ConversationId = conversationId;
//                record.UserId = userInfo.StaffId;
//                record.UserTwo = receiverId;
//                record.Content = message;
//                record.Type = EnumMessageTypes.Text;

//                var result = replyStore.Insert(record);

//                await Task.FromResult(result);
//                isSuccess = true;
//                record.Id = result;

//                return Json(new { success = isSuccess, message = msg, html = htmlReturn, msgItem = record }, JsonRequestBehavior.AllowGet);

//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying Send because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost, ValidateInput(false)]
//        [ValidateAntiForgeryToken]
//        [PreventSpam]
//        public async Task<ActionResult> NewMessage(IdentityConversationReply msgItem)
//        {
//            var isSuccess = false;
//            var htmlReturn = string.Empty;
//            var msg = string.Empty;

//            try
//            {
//                var currentUserId = GetCurrentStaffId();
//                var replyStore = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();
//                var newMsg = replyStore.GetDetailByConversation(msgItem.ConversationId, msgItem.Id);

//                var itemModel = new MessageItemModel();
//                itemModel.CurrentUserId = msgItem.UserId;
//                itemModel.MessageItem = msgItem;               

//                //var receiver = ParseUserOnlineInfo(GetCurrentUser());
//                var senderInfo = GetByStaffId(msgItem.UserId);
//                var receiverInfo = GetByStaffId(msgItem.UserTwo);

//                var sender = ParseUserOnlineInfo(senderInfo);
//                var receiver = ParseUserOnlineInfo(receiverInfo);

//                itemModel.MessageItem.UserOneInfo = sender;
//                itemModel.MessageItem.UserTwoInfo = receiver;

//                if (currentUserId == msgItem.UserId)
//                {
//                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageMine", itemModel);
//                }
//                else
//                {
//                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageGuest", itemModel);
//                }

//                isSuccess = true;
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying ShowMsgContent because: {0}", ex.ToString());
//                logger.Error(strError);
//                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
//            }

//            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
//        }

//        #region Helpers

//        private int InitConversation(int fromUserId, int receiverId)
//        {           
//            var conversationId = 0;
//            try
//            {
//                var filter = new IdentityConversation();
//                filter.UserOne = fromUserId;
//                filter.UserTwo = receiverId;
//                filter.Ip = Request.UserHostAddress;

//                var store = GlobalContainer.IocContainer.Resolve<IStoreConversation>();
//                conversationId = store.GetCurrentConversation(filter);
//            }
//            catch (Exception ex)
//            {
//                var strError = string.Format("Failed when trying InitConversation because: {0}", ex.ToString());
//                logger.Error(strError);
//            }

//            return conversationId;
//        }

//        private IdentityConversationReply GetMessageFilter()
//        {
//            var filter = new IdentityConversationReply();
//            filter.PageIndex = 1;
//            if (Request["page"] != null)
//                filter.PageIndex = Utils.ConvertToInt32(Request["page"]);

//            filter.PageSize = SystemSettings.DefaultPageSize;
//            return filter;
//        }

//        private BaseOnlineUserIdentity ParseUserOnlineInfo(IdentityUser info)
//        {
//            BaseOnlineUserIdentity userOnline = new BaseOnlineUserIdentity();
//            userOnline.Id = info.StaffId;
//            userOnline.UserName = info.UserName;
//            userOnline.DisplayName = info.FullName;
//            userOnline.Avatar = "/Content/Avatars/messenger-avatar.png";

//            return userOnline;
//        }

//        #endregion
//    }
//}