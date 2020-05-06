using Microsoft.AspNet.SignalR;
using MyCloud.Helpers;
using MyCloud.Models;
using MyCloud.SharedLib.Logging;
using MyCloud.SharedLibs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloud.Hubs
{
    public class CommonHub : Hub
    {
        private readonly ILog logger = LogProvider.For<CommonHub>();

        #region Methods

        public void Connect(Connector connector)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                var serverVars = Context.Request.GetHttpContext().Request.ServerVariables;
                var currentIP = serverVars["REMOTE_ADDR"];

                var uInfo = new Connector();
                uInfo.ConnectionId = connectionId;

                if (connector != null)
                {
                    //logger.Debug(string.Format("New device connected: {0}", JsonConvert.SerializeObject(connector)));

                    //uInfo.user_name = connector.user_name;
                    //uInfo.DisplayName = connector.DisplayName;
                    uInfo.id = connector.id;
                    //uInfo.Avatar = connector.Avatar;
                    uInfo.last_connected = DateTime.UtcNow;
                    uInfo.last_ip_connected = currentIP;

                    var conn = new ConnectionInfo();
                    conn.ConnectionId = connectionId;
                    conn.connected_time = uInfo.last_connected;
                    conn.ip = currentIP;
                    conn.device_name = (connector.device_name == null) ? serverVars["HTTP_USER_AGENT"].ToString() : connector.device_name;
                    conn.device_type = connector.device_type;
                    conn.device_id = connector.device_id;
                    conn.registration_id = connector.registration_id;

                    uInfo.Connections.Add(conn);
                }

                var newConnFlag = false;
                if (!string.IsNullOrEmpty(uInfo.user_name))
                {
                    if (uInfo.user_name.ToLower() != "admin")
                        newConnFlag = true;
                }
                else
                    newConnFlag = true;

                if (newConnFlag)
                {
                    //Storage user to cache
                    CommonGateHelpers.AddUserToCache(uInfo);

                    // send to caller
                    Clients.Caller.onConnected(connectionId);

                    // send to all except caller client
                    //Clients.AllExcept(connectionId).onNewUserConnected(connectionId, connector.user_name);

                    if (connector.device_id != null)
                    {
                        var deviceInfo = new JobSeekerDevice();
                        deviceInfo.job_seeker_id = connector.id;
                        deviceInfo.device_id = connector.device_id;
                        deviceInfo.device_name = (connector.device_name == null) ? serverVars["HTTP_USER_AGENT"].ToString() : connector.device_name;
                        deviceInfo.registration_id = connector.registration_id;
                        deviceInfo.device_type = Utils.ConvertToIntFromQuest(connector.device_type);

                        deviceInfo.language_code = connector.language_code;

                        //add device
                        CommonGateHelpers.NewDeviceConnected(deviceInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not connect to CommonHub because {0}", ex.ToString()));
            }
        }

        protected Connector GetUserById(int userId)
        {
            var listUser = CommonGateHelpers.GetAllUsersFromCache();
            if (listUser != null)
            {
                var user = listUser.FirstOrDefault(x => x.id == userId);

                return user;
            }

            return null;
        }

        public void SendMessageToAll(string userName, string message)
        {
            // Broad cast message
            Clients.All.messageReceived(userName, message);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var listUser = CommonGateHelpers.GetAllUsersFromCache();
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

                        ////Set to offline device
                        //if (!string.IsNullOrEmpty(currentConnection.device_id))
                        //{
                        //    user.Connections = new List<ConnectionInfo>();
                        //    user.Connections.Add(currentConnection);
                        //}

                        //Remove current connection (current tab)
                        user.Connections.Remove(currentConnection);
                        if (user.Connections == null || user.Connections.Count <= 0)
                        {
                            listUser.Remove(user);
                            currentUser.has_logout = true;
                        }

                        break;
                    }
                }
            }

            //Save to cache
            CommonGateHelpers.StorageListUsersToCache(listUser);

            return base.OnDisconnected(stopCalled);
        }

        #endregion
    }
}