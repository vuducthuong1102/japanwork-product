using MyCloud.Helpers;
using MyCloud.Models;
using MyCloud.Resources;
using MyCloud.SharedLib;
using MyCloud.SharedLib.Logging;
using MyCloud.SharedLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MyCloud.Hubs
{
    public class CommonGateHub : CommonHub
    {
        private readonly ILog logger = LogProvider.For<CommonGateHub>();

        #region Public server methods
        public void SendMessage()
        {
            Clients.All.updateNotification();
        }

        public void TestSendMessageToAll(string userName, string message)
        {
            // Broad cast message
            Clients.All.messageTestReceived(userName, message);
        }

        public void SendNotifSignToUsers(string userIds, int notifId)
        {
            var returnUsers = new List<Connector>();
            var connections = GetListConnectionsFromUsers(userIds, ref returnUsers);

            //logger.Debug(string.Format("Send notif to User: {0}", userIds));

            Clients.Clients(connections).updateNotification(notifId);

            //Create notification json files
            //CreateFileForNotification(userIds, notifInfo);
        }

        public void TestMessage(string message)
        {
            var connector = new Connector();
            connector.id = 1;
            connector.user_name = "bangvl";
            Clients.All.receivedMessage(message);
            var currentDir = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Notifications/ToSend");

            var filePath = Path.Combine(currentDir, "test.json");

            NotificationFileHelper<Connector>.WriteToJsonFile(connector, filePath);

            var newConnector = NotificationFileHelper<Connector>.ReadJsonFile(filePath);
        }

        public void CreateFileForNotification(string listIds, dynamic data)
        {
            var currentDir = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Notifications/ToSend");
            var allConnections = new List<ConnectionInfo>();
            var receivers = listIds.Split(',');
            if (receivers != null && receivers.Length > 0)
            {
                foreach (var userId in receivers)
                {
                    var devices = CommonGateHelpers.GetDevicesByUser(Utils.ConvertToInt32(userId));
                    if (devices != null && devices.Count > 0)
                    {
                        foreach (var item in devices)
                        {
                            var filePath = Path.Combine(currentDir, string.Format("{0}_{1}_{2}.json", userId, DateTime.UtcNow.ToString("ddMMyyyyHHmmss"), item.device_id));
                            var inputData = ParseNotifDataFromConnection(item, data);
                            NotificationFileHelper<NotificationInputModel>.WriteToJsonFile(inputData, filePath);
                        }
                    }
                }
            }
        }

        private NotificationInputModel ParseNotifDataFromConnection(JobSeekerDevice device, dynamic data)
        {
            var notif = new NotificationInputModel();
            notif.device_id = device.device_id;
            notif.registration_id = device.registration_id;
            notif.Data = data;
            notif.language_code = device.language_code;

            return notif;
        }

        private List<string> GetListConnectionsFromUsers(string listUsers, ref List<Connector> returnUsers)
        {
            var receivers = listUsers.Split(',');
            var listUser = CommonGateHelpers.GetAllUsersFromCache();
            var listConnections = new List<string>();

            if (listUser != null)
            {
                returnUsers = new List<Connector>();
                if (receivers != null && receivers.Length > 0)
                {
                    foreach (var userId in receivers)
                    {
                        var currentUserId = Utils.ConvertToInt32(userId);
                        var currentUser = listUser.Where(x => x.id == currentUserId).FirstOrDefault();

                        var matchedsConnections = listUser.Where(x => x.id == currentUserId).ToList();
                        if (currentUser != null)
                        {
                            returnUsers.Add(currentUser);
                            if (currentUser.Connections != null && currentUser.Connections.Count > 0)
                            {
                                var allConnections = currentUser.Connections.Select(x => x.ConnectionId).ToList();
                                listConnections.AddRange(allConnections);
                            }
                        }
                    }
                }
            }

            return listConnections;
        }

        #endregion
    }
}