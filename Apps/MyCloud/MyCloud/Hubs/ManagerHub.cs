using Autofac;
using Microsoft.AspNet.SignalR;
using MyCloud.Models;
using MyCloud.Settings;
using MyCloud.SharedLib.Caching.Providers;
using MyCloud.SharedLib.Logging;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloud.Hubs
{
    public class ManagerHub : Hub
    {
        #region Common data
        private readonly ICacheClient _myCache;
        private readonly ILog logger = LogProvider.For<ManagerHub>();

        private readonly DateTimeOffset _cacheExpiredTime;
        private readonly string _allUsersCacheKey = "ALL_USERS";

        #endregion

        #region Methods

        public ManagerHub()
        {
            _myCache = GlobalContainer.IocContainer.Resolve<ICacheClient>();
            _allUsersCacheKey = string.Format("{0}{1}", SystemSettings.DefaultCacheKeyPrefix, _allUsersCacheKey);

            _cacheExpiredTime = DateTimeOffset.Now.AddMinutes(10080); //One week
        }

        public void Connect(dynamic connector)
        {
            var connectionId = Context.ConnectionId;
            var serverVars = Context.Request.GetHttpContext().Request.ServerVariables;
            var currentIP = serverVars["REMOTE_ADDR"];

            var uInfo = new Connector();
            uInfo.ConnectionId = connectionId;

            if (connector != null)
            {
                uInfo.user_name = connector.user_name;
                uInfo.id = connector.id;
                //uInfo.Avatar = connector.Avatar;
                uInfo.last_connected = DateTime.UtcNow;
                uInfo.last_ip_connected = currentIP;
            }

            //Storage user to cache
            AddUserToCache(uInfo);

            // send to caller
            Clients.Caller.onConnected(connectionId);

            // send to all except caller client
            Clients.AllExcept(connectionId).onNewUserConnected(connectionId, connector.user_name);
        }

        private List<Connector> GetAllUsersFromCache()
        {
            var strError = string.Empty;
            List<Connector> listUser = null;

            try
            {
                listUser = _myCache.Get<List<Connector>>(_allUsersCacheKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetAllUsersFromCache because: {0}", ex.ToString());
                logger.Error(strError);
            }

            if (listUser == null)
                listUser = new List<Connector>();

            return listUser;
        }

        private Connector GetUserById(int userId)
        {
            var listUser = GetAllUsersFromCache();
            if (listUser != null)
            {
                var user = listUser.FirstOrDefault(x => x.id == userId);

                return user;
            }

            return null;
        }

        private void AddUserToCache(Connector newUser)
        {
            var strError = string.Empty;

            var listUser = GetAllUsersFromCache();
            if (listUser != null && listUser.Count > 0)
            {
                //Remove if any
                listUser.RemoveAll(x => x.id == newUser.id);
            }

            //Add new user
            listUser.Add(newUser);

            try
            {
                //Save users
                _myCache.Add(_allUsersCacheKey, listUser, _cacheExpiredTime);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not AddUserToCache because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        private ConnectorMessages GetInboxMessages(int senderId, int receiverId)
        {
            var strError = string.Empty;
            ConnectorMessages converSation = null;

            try
            {
                var listKeys = _myCache.SearchKeys("Thread_*");
                var commonKey = string.Empty;

                if (listKeys.Count() > 0)
                {
                    commonKey = listKeys.FirstOrDefault(x => (x.Contains(senderId.ToString()) && x.Contains(receiverId.ToString())));
                }

                if (!string.IsNullOrEmpty(commonKey))
                    converSation = _myCache.Get<ConnectorMessages>(commonKey);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetInboxMessages because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return converSation;
        }

        private void AddMessageToCache(int senderId, int receiverId, MessageDetail detail)
        {
            var strError = string.Empty;
            var cacheKey = string.Format("Thread_{0}_{1}", senderId, receiverId);

            var connectorMessages = GetInboxMessages(senderId, receiverId);

            if (connectorMessages == null)
            {
                connectorMessages = new ConnectorMessages
                {
                    SenderId = senderId,
                    ReceiverId = receiverId
                };
            }

            //Add new user
            connectorMessages.ListMessage.Add(detail);

            try
            {
                var listKeys = _myCache.SearchKeys("Thread_*");
                var commonKey = string.Empty;
                if (listKeys.Count() > 0)
                {
                    commonKey = listKeys.FirstOrDefault(x => (x.Contains(senderId.ToString()) && x.Contains(receiverId.ToString())));
                }

                if (string.IsNullOrEmpty(commonKey))
                    commonKey = cacheKey;

                //Save to cache
                _myCache.Add(commonKey, connectorMessages, _cacheExpiredTime);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not AddMessageToCache because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        private MessageDetail CreateNewMessage(Connector fromUser, Connector toUser, string message)
        {
            var detail = new MessageDetail();
            detail.SentTime = DateTime.UtcNow;
            detail.Message = message;
            detail.SenderName = fromUser.user_name;
            detail.ReceiverName = toUser.user_name;
            detail.ReceivedTime = null;

            return detail;
        }

        #region Public client methods

        public void OpenPrivateConversation(int fromUser, int toUser)
        {
            var connectorMessages = GetInboxMessages(fromUser, toUser);

            // send to caller user
            Clients.Caller.openPrivateConversation(connectorMessages);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;
            var listUser = GetAllUsersFromCache();

            var toUser = listUser.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = listUser.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                //Create new message record
                var messageDetail = CreateNewMessage(fromUser, toUser, message);

                // store messages in cache
                AddMessageToCache(fromUser.id, toUser.id, messageDetail);

                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.user_name, message);

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.user_name, message);
            }

        }

        //public void SendApprovalRequest(ApprovalRequestBookingModel model)
        //{
        //    //Save to database


        //    // send to specific user
        //    var connectionId = Context.ConnectionId;
        //    Clients.AllExcept(Context.ConnectionId).receivedApprovalRequest(JsonConvert.SerializeObject(model));
        //}

        //public void BookingRequestApproved(ApprovalRequestBookingModel model)
        //{
        //    //Save to database


        //    // send to specific user
        //    var connectionId = Context.ConnectionId;
        //    Clients.AllExcept(Context.ConnectionId).bookingRequestApproved(JsonConvert.SerializeObject(model));
        //}

        #endregion

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var listUser = GetAllUsersFromCache();
            var item = listUser.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                //Remove user online
                listUser.Remove(item);

                //Save to cache
                _myCache.Add(_allUsersCacheKey, listUser, _cacheExpiredTime);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.user_name);
            }

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region private Messages

        private void AddMessageinCache(string userName, string message)
        {
            //CurrentMessage.Add(new MessageDetail { user_name = userName, Message = message });

            //if (CurrentMessage.Count > 100)
            //    CurrentMessage.RemoveAt(0);
        }

        #endregion

    }
}