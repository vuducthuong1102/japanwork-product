using Autofac;
using MyCloud.Services;
using Microsoft.AspNet.SignalR;
using MyCloud.Helpers;
using MyCloud.Models;
using MyCloud.SharedLib.Logging;
using MyCloud.SharedLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Transports;

namespace MyCloud.Hubs
{
    public class BaseMessengerHub : Hub
    {
        private readonly ILog logger = LogProvider.For<MessengerHub>();

        #region Methods

        public BaseMessengerHub()
        {
            //_myCache = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
            //_allUsersCacheKey = string.Format("{0}{1}", SystemSettings.DefaultCacheKeyPrefix, _allUsersCacheKey);           
        }

        public void Connect(dynamic connector)
        {
            var connectionId = Context.ConnectionId;
            var serverVars = Context.Request.GetHttpContext().Request.ServerVariables;
            var currentIP = serverVars["REMOTE_ADDR"];

            try
            {
                var uInfo = new Connector();
                uInfo.ConnectionId = connectionId;

                if (connector != null)
                {
                    uInfo.user_name = connector.user_name;
                    //uInfo.DisplayName = connector.DisplayName;
                    uInfo.id = connector.id;
                    //uInfo.Avatar = connector.Avatar;
                    uInfo.last_connected = DateTime.Now;
                    uInfo.last_ip_connected = currentIP;

                    var conn = new ConnectionInfo();
                    conn.ConnectionId = connectionId;
                    conn.connected_time = uInfo.last_connected;
                    conn.ip = currentIP;
                    conn.device_name = serverVars["HTTP_USER_AGENT"].ToString();

                    conn.device_id = connector.device_id;
                    conn.registration_id = connector.registration_id;
                    //if (connector.iosDevice != null)
                    //    conn.iosDevice = Utils.ConvertToBoolean(connector.iosDevice.ToString(), false);

                    uInfo.Connections.Add(conn);
                }

                if (!string.IsNullOrEmpty(uInfo.user_name))
                {
                    if (uInfo.user_name.ToLower() != "admin")
                    {
                        var listUser = MessengerHelpers.AddUserToCache(uInfo);
                        //var uniqueUsers = GetUniqueUsers(listUser);

                        // send to caller
                        //Clients.Caller.onMessengerConnected(listUser);

                        // send to all except caller client
                        //Clients.AllExcept(connectionId).onMessengerNewUserConnected(connectionId, uInfo);
                        Clients.All.NewUserConnected(uInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not Connect because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        protected List<Connector> GetUniqueUsers(List<Connector> listUser)
        {
            if (listUser != null)
            {
                listUser = listUser.Where(x => x.user_name != "admin").GroupBy(x => x.id).Select(y => y.First()).ToList();
            }

            return listUser;
        }

        protected Connector GetUserById(int userId)
        {
            var listUser = MessengerHelpers.GetAllUsersFromCache();
            if (listUser != null)
            {
                var user = listUser.FirstOrDefault(x => x.id == userId);

                return user;
            }

            return null;
        }



        //private ConnectorMessages GetInboxMessages(int senderId, int receiverId)
        //{
        //    var strError = string.Empty;
        //    ConnectorMessages converSation = null;

        //    try
        //    {
        //        var listKeys = _myCache.SearchKeys("Thread_*");
        //        var commonKey = string.Empty;

        //        if (listKeys.Count() > 0)
        //        {
        //            commonKey = listKeys.FirstOrDefault(x => (x.Contains(senderId.ToString()) && x.Contains(receiverId.ToString())));
        //        }

        //        if (!string.IsNullOrEmpty(commonKey))
        //            converSation = _myCache.Get<ConnectorMessages>(commonKey);
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = string.Format("Could not GetInboxMessages because: {0}", ex.ToString());
        //        logger.Error(strError);
        //    }

        //    return converSation;
        //}

        //private void AddMessageToCache(int senderId, int receiverId, MessageDetail detail)
        //{
        //    var strError = string.Empty;
        //    var cacheKey = string.Format("Thread_{0}_{1}", senderId, receiverId);

        //    var connectorMessages = GetInboxMessages(senderId, receiverId);

        //    if (connectorMessages == null)
        //    {
        //        connectorMessages = new ConnectorMessages
        //        {
        //            SenderId = senderId,
        //            ReceiverId = receiverId
        //        };
        //    }

        //    //Add new user
        //    connectorMessages.ListMessage.Add(detail);

        //    try
        //    {
        //        var listKeys = _myCache.SearchKeys("Thread_*");
        //        var commonKey = string.Empty;
        //        if (listKeys.Count() > 0)
        //        {
        //            commonKey = listKeys.FirstOrDefault(x => (x.Contains(senderId.ToString()) && x.Contains(receiverId.ToString())));
        //        }

        //        if (string.IsNullOrEmpty(commonKey))
        //            commonKey = cacheKey;

        //        //Save to cache
        //        _myCache.Add(commonKey, connectorMessages, _cacheExpiredTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = string.Format("Could not AddMessageToCache because: {0}", ex.ToString());
        //        logger.Error(strError);
        //    }
        //}

        private MessageDetail CreateNewMessage(Connector fromUser, Connector toUser, string message)
        {
            var detail = new MessageDetail();
            detail.SentTime = DateTime.Now;
            detail.Message = message;
            detail.SenderName = fromUser.user_name;
            detail.ReceiverName = toUser.user_name;
            detail.ReceivedTime = null;

            return detail;
        }

        #region Public server methods

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

        public override Task OnDisconnected(bool stopCalled)
        {
            Task mytask = Task.Run(() =>
            {
                UserDisconnected(Context.ConnectionId);
            });

            return base.OnDisconnected(stopCalled);
        }

        private async void UserDisconnected(string connectionId)
        {
            await Task.Delay(1000);
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            var allConnections = heartBeat.GetConnections();
            var hasOnlineUsers = (allConnections != null && allConnections.Count > 0);

            var listUser = MessengerHelpers.GetAllUsersFromCache();
            Connector currentUser = null;
            var id = Context.ConnectionId;
            if (listUser != null && listUser.Count > 0)
            {
                foreach (var user in listUser)
                {
                    var currentConnection = user.Connections.FirstOrDefault(x => x.ConnectionId == id);
                    if (currentConnection != null)
                    {
                        currentUser = user;
                        //Remove current connection (current tab)
                        user.Connections.Remove(currentConnection);
                        //if (user.Connections == null || user.Connections.Count <= 0)
                        //{
                        //    listUser.Remove(user);
                        //    currentUser.has_logout = true;

                        //    Task.FromResult(MemberServices.UpdateOnlineTimeAsync(new ApiJobSeekerDeviceModel { job_seeker_id = user.id }));

                        //    Clients.AllExcept(id).onMessengerUserOffline(user.id);
                        //}

                        //break;
                    }

                    if (hasOnlineUsers)
                    {
                        if (user.Connections.CheckListHasData())
                        {
                            foreach (var onlineConn in allConnections)
                            {
                                var matchedConn = user.Connections.Where(x => x.ConnectionId == onlineConn.ConnectionId).FirstOrDefault();
                                if (matchedConn != null && !onlineConn.IsAlive)
                                {
                                    user.Connections.Remove(matchedConn);
                                }
                            }
                        }
                    }
                }
            }

            if (currentUser != null)
            {
                //Save to cache
                MessengerHelpers.StorageListUsersToCache(listUser);

                Clients.All.onMessengerUserDisconnected(id, currentUser);
            }
        }

        public override Task OnReconnected()
        {
            var listUser = MessengerHelpers.GetAllUsersFromCache();
            Connector currentUser = null;
            var id = Context.ConnectionId;
            if (listUser != null && listUser.Count > 0)
            {
                foreach (var user in listUser)
                {
                    var currentConnection = user.Connections.FirstOrDefault(x => x.ConnectionId == id);
                    if (currentConnection != null)
                    {
                        currentUser = user;

                        break;
                    }
                }
            }

            if (currentUser != null)
            {
                //Save to cache
                MessengerHelpers.StorageListUsersToCache(listUser);

                Clients.AllExcept(id).onMessengerUserReconnected(currentUser.id);
            }

            return base.OnReconnected();
        }

        #endregion
    }
}