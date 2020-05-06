using Autofac;
using Manager.SharedLibs;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Services;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Manager.WebApp.Controllers
{
    public class MessengerController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<MessengerController>();

        public MessengerController()
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam]
        public async Task<ActionResult> GetOperatorsList(CommonPagingModel model)
        {
            var isSuccess = true;
            var htmlReturn = string.Empty;
            var msg = string.Empty;
            var listIds = string.Empty;

            List<BaseOnlineUserIdentity> returnList = new List<BaseOnlineUserIdentity>();
            try
            {
                var page = Utils.ConvertToInt32(Request["page"]);
                if (page == 0)
                    model.CurrentPage = 1;
                else
                    model.CurrentPage = page;

                if (model.PageSize > SystemSettings.DefaultPageSize || model.PageSize == 0)
                    model.PageSize = SystemSettings.DefaultPageSize;
                var apiModel = new ApiCommonFilterModel();
                apiModel.keyword = model.Keyword;
                apiModel.page_index = model.CurrentPage;
                apiModel.page_size = model.PageSize;

                var currentUser = GetCurrentUser();
                List<IdentityUser> listOperators = null;
                var apiReturn = MyAgencyServices.GetListOperatorsForMessengerAsync(apiModel).Result;

                if(apiReturn != null && apiReturn.value != null)
                {
                    listOperators = JsonConvert.DeserializeObject<List<IdentityUser>>(apiReturn.value.ToString());
                }

                await Task.FromResult(listOperators);

                if (listOperators.HasData())
                {
                    foreach (var item in listOperators)
                    {
                        var userOnline = ParseUserOnlineInfo(item);
                        returnList.Add(userOnline);
                    }
                }

                htmlReturn = PartialViewAsString("../Widgets/Contact/_OperatorsList", returnList);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                var strError = string.Format("Failed when trying GetOperatorsList because: {0}", ex.ToString());
                logger.Error(strError);
                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> OpenConversation()
        {
            var receiverId = 0;

            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;

            try
            {
                if (Request["receiver"] != null)
                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

                if (receiverId == 0)
                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, html = htmlReturn }, JsonRequestBehavior.AllowGet);

                var userInfo = GetCurrentUser();
                if (userInfo == null)
                {
                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;

                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                IdentityUser receiverInfo = null;

                var targetUserResult = MyAgencyServices.GetOperatorProfileAsync(new ApiCommonOperatorInfoModel { Id = receiverId }).Result;
                if (targetUserResult != null && targetUserResult.value != null)
                {
                    receiverInfo = JsonConvert.DeserializeObject<IdentityUser>(targetUserResult.value.ToString());
                }
                //var receiverInfo = AccountHelper.GetByStaffId(receiverId);

                await Task.FromResult(receiverInfo);

                var boxModel = new ConversationModel();
                if (receiverInfo != null)
                {
                    boxModel.TargetUser = receiverInfo;

                    isSuccess = true;
                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_ChatBoxTemp", boxModel);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetCurrentConversation because: {0}", ex.ToString());
                logger.Error(strError);
                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetCurrentConversation()
        {
            var receiverId = 0;
            var isSuccess = true;
            var htmlReturn = string.Empty;
            var msg = string.Empty;

            try
            {
                if (Request["receiver"] != null)
                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

                if (receiverId == 0)
                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, html = htmlReturn }, JsonRequestBehavior.AllowGet);

                var userInfo = AccountHelper.GetCurrentUser();
                if (userInfo == null)
                {
                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;
                    isSuccess = false;
                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                var apiModel = new ApiConversationModel();
                apiModel.UserOne = userInfo.StaffId;
                apiModel.UserTwo = receiverId;
                apiModel.Ip = Request.UserHostAddress;

                var objectList = InitConversation(receiverId);

                await Task.FromResult(objectList);
                var conId = 0;
                if (!objectList.HasData())
                {
                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
                    isSuccess = false;
                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                conId = objectList[0].ConversationId;

                if (conId == 0)
                {
                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
                    isSuccess = false;
                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                var filterModel = GetMessageFilter();
                filterModel.OwnerId = (objectList[0].ObjectType == ((int)EnumMessengerObjectType.Admin) ? objectList[0].ObjectRootId : objectList[1].ObjectRootId);
                filterModel.UserTwo = (objectList[0].ObjectType == ((int)EnumMessengerObjectType.Agency) ? objectList[0].ObjectRootId : objectList[1].ObjectRootId);
                filterModel.ConversationId = conId;

                var store = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();
                
                var myList = store.GetByPage(filterModel);
                if (myList.HasData())
                {
                    var userOne = ParseUserOnlineInfo(userInfo);
                    userOne.Avatar = SocialCdnHelper.GetAvatarOrDefault(userOne.Avatar);

                    BaseOnlineUserIdentity userTwo = new BaseOnlineUserIdentity();

                    var targetUserResult = MyAgencyServices.GetOperatorProfileAsync(new ApiCommonOperatorInfoModel { Id = receiverId }).Result;
                    if(targetUserResult != null && targetUserResult.value != null)
                    {
                        var targetUser = JsonConvert.DeserializeObject<IdentityUser>(targetUserResult.value.ToString());
                        userTwo = ParseUserOnlineInfo(targetUser);
                    }

                    foreach (var item in myList)
                    {
                        item.UserOneInfo = userOne;
                        item.UserTwoInfo = userTwo;
                    }

                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageList", myList);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetCurrentConversation because: {0}", ex.ToString());
                logger.Error(strError);
                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> LoadMoreMessages()
        {
            var receiverId = 0;
            var isSuccess = true;
            var htmlReturn = string.Empty;
            var msg = string.Empty;

            try
            {
                if (Request["receiver"] != null)
                    receiverId = Utils.ConvertToInt32(Request["receiver"]);

                if (receiverId == 0)
                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, html = htmlReturn }, JsonRequestBehavior.AllowGet);

                var userInfo = AccountHelper.GetCurrentUser();
                if (userInfo == null)
                {
                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;
                    isSuccess = false;
                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }


                var apiModel = new ApiConversationModel();
                apiModel.UserOne = userInfo.StaffId;
                apiModel.UserTwo = receiverId;
                apiModel.Ip = Request.UserHostAddress;

                var objectList = InitConversation(receiverId);

                await Task.FromResult(objectList);
                var conId = 0;
                if (!objectList.HasData())
                {
                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
                    isSuccess = false;
                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                conId = objectList[0].ConversationId;

                if (conId == 0)
                {
                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
                    isSuccess = false;
                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                if (conId > 0)
                {
                    var filterModel = GetMessageFilter();

                    filterModel.OwnerId = (objectList[0].ObjectType == ((int)EnumMessengerObjectType.Admin) ? objectList[0].ObjectRootId : objectList[1].ObjectRootId);
                    filterModel.UserTwo = (objectList[0].ObjectType == ((int)EnumMessengerObjectType.Agency) ? objectList[0].ObjectRootId : objectList[1].ObjectRootId);
                    filterModel.ConversationId = objectList[0].ConversationId;
                    //filterModel.OwnerId = userInfo.StaffId;
                    //filterModel.UserTwo = receiverId;

                    var store = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();
                    var myList = store.GetByPage(filterModel);
                    if (myList.HasData())
                    {
                        var userOne = ParseUserOnlineInfo(userInfo);
                        userOne.Avatar = SocialCdnHelper.GetAvatarOrDefault(userOne.Avatar);

                        BaseOnlineUserIdentity userTwo = new BaseOnlineUserIdentity();

                        var targetUserResult = MyAgencyServices.GetOperatorProfileAsync(new ApiCommonOperatorInfoModel { Id = receiverId }).Result;
                        if (targetUserResult != null && targetUserResult.value != null)
                        {
                            var targetUser = JsonConvert.DeserializeObject<IdentityUser>(targetUserResult.value.ToString());
                            userTwo = ParseUserOnlineInfo(targetUser);
                        }


                        foreach (var item in myList)
                        {
                            item.UserOneInfo = userOne;
                            item.UserTwoInfo = userTwo;
                        }

                        htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageList", myList);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying LoadMoreMessages because: {0}", ex.ToString());
                logger.Error(strError);
                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Send(MessageSendingModel model)
        {
            var message = string.Empty;
            var conversationId = 0;
            var receiverId = 0;

            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;

            try
            {
                var record = new IdentityConversationReply();

                var userInfo = AccountHelper.GetCurrentUser();
                if (userInfo == null)
                {
                    msg = ManagerResource.ERROR_LOGIN_SESSION_ENDED;

                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                receiverId = Utils.ConvertToInt32(model.receiver);
                conversationId = Utils.ConvertToInt32(model.conversation);

                if (conversationId == 0)
                {
                    var objectList = InitConversation(receiverId);

                    await Task.FromResult(objectList);
                    if (!objectList.HasData())
                    {
                        msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;
                        isSuccess = false;
                        return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                    }

                    conversationId = objectList[0].ConversationId;

                    record.UserId = userInfo.StaffId;
                    record.UserObjectId = (objectList[0].ObjectType == ((int)EnumMessengerObjectType.Admin) ? objectList[0].ObjectId : objectList[1].ObjectId);

                    record.TargetObjectType = (int)EnumMessengerObjectType.Agency;
                    record.TargetObjectId = (objectList[0].ObjectType == ((int)EnumMessengerObjectType.Agency) ? objectList[0].ObjectId : objectList[1].ObjectId);
                    record.TargetId = receiverId;
                }   

                if (conversationId == 0)
                {
                    msg = ManagerResource.ERROR_COULD_NOT_CREATE_CHAT;

                    return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
                }

                var replyStore = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();
               
                record.Ip = Request.UserHostAddress;
                record.ConversationId = conversationId;               
                record.UserObjectType = (int)EnumMessengerObjectType.Admin;
                record.UserTwo = receiverId;

                record.Content = model.message;
                record.Type = EnumMessageTypes.Text;

                var result = replyStore.Insert(record);

                await Task.FromResult(result);
                isSuccess = true;
                record.Id = result;

                return Json(new { success = isSuccess, message = msg, html = htmlReturn, msgItem = record }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying Send because: {0}", ex.ToString());
                logger.Error(strError);
                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [PreventSpam]
        public async Task<ActionResult> NewMessage(IdentityConversationReply msgItem)
        {
            var isSuccess = false;
            var htmlReturn = string.Empty;
            var msg = string.Empty;

            try
            {
                var currentUserId = GetCurrentStaffId();
                var replyStore = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();

                var newMsg = replyStore.GetDetailByConversation(msgItem.ConversationId, msgItem.Id);
                await Task.FromResult(newMsg);

                var itemModel = new MessageItemModel();
                itemModel.CurrentUserId = msgItem.UserId;
                itemModel.MessageItem = msgItem;

                //var receiver = ParseUserOnlineInfo(GetCurrentUser());
                IdentityUser senderInfo = new IdentityUser();
                IdentityUser receiverInfo = new IdentityUser();

                if (newMsg != null)
                {
                    if (newMsg.UserObjectType == (int)EnumMessengerObjectType.Admin)
                    {
                        senderInfo = GetByStaffId(msgItem.UserId);
                    }
                    else if (newMsg.UserObjectType == (int)EnumMessengerObjectType.Agency)
                    {
                        var targetUserResult = MyAgencyServices.GetOperatorProfileAsync(new ApiCommonOperatorInfoModel { Id = newMsg.UserId }).Result;
                        if (targetUserResult != null && targetUserResult.value != null)
                        {
                            senderInfo = JsonConvert.DeserializeObject<IdentityUser>(targetUserResult.value.ToString());
                        }
                    }

                    if (newMsg.TargetId == (int)EnumMessengerObjectType.Admin)
                    {
                        receiverInfo = GetByStaffId(msgItem.UserId);
                    }
                    else if (newMsg.TargetObjectType == (int)EnumMessengerObjectType.Agency)
                    {
                        var targetUserResult = MyAgencyServices.GetOperatorProfileAsync(new ApiCommonOperatorInfoModel { Id = newMsg.TargetId }).Result;
                        if (targetUserResult != null && targetUserResult.value != null)
                        {
                            receiverInfo = JsonConvert.DeserializeObject<IdentityUser>(targetUserResult.value.ToString());
                        }
                    }
                }

                var sender = ParseUserOnlineInfo(senderInfo);
                var receiver = ParseUserOnlineInfo(receiverInfo);

                itemModel.MessageItem.UserOneInfo = sender;
                itemModel.MessageItem.UserTwoInfo = receiver;

                if (currentUserId == msgItem.UserId && msgItem.UserObjectType == (int)EnumMessengerObjectType.Admin)
                {
                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageMine", itemModel);
                }
                else
                {
                    htmlReturn = PartialViewAsString("../Messenger/Partials/ChatBox/_MessageGuest", itemModel);
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying ShowMsgContent because: {0}", ex.ToString());
                logger.Error(strError);
                msg = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private List<IdentityConversationObject> InitConversation(int receiverId)
        {
            var conId = 0;
            var returnList = new List<IdentityConversationObject>();
            try
            {
                var sender = GetCurrentUser();
                var from = new IdentityConversationObject();
                from.ObjectRootId = sender.StaffId;
                from.ObjectType = (int)EnumMessengerObjectType.Admin;

                var to = new IdentityConversationObject();
                to.ObjectRootId = receiverId;
                to.ObjectType = (int)EnumMessengerObjectType.Agency;

                var store = GlobalContainer.IocContainer.Resolve<IStoreConversation>();
                var returnObjects = store.JoinToConversation(from, to);
                if(returnObjects.HasData())
                    conId = returnObjects[0].ConversationId;

                from.ConversationId = conId;
                to.ConversationId = conId;

                returnList.Add(from);
                returnList.Add(to);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying InitConversation because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return returnList;
        }

        private IdentityConversationReply GetMessageFilter()
        {
            var filter = new IdentityConversationReply();
            filter.PageIndex = 1;
            if (Request["page"] != null)
                filter.PageIndex = Utils.ConvertToInt32(Request["page"]);

            filter.PageSize = SystemSettings.DefaultPageSize;
            return filter;
        }

        private BaseOnlineUserIdentity ParseUserOnlineInfo(IdentityUser info)
        {
            BaseOnlineUserIdentity userOnline = new BaseOnlineUserIdentity();
            userOnline.Id = info.StaffId;
            userOnline.UserName = info.UserName;
            userOnline.DisplayName = info.FullName;
            userOnline.Avatar = info.Avatar;

            return userOnline;
        }

        #endregion
    }
}