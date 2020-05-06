using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MyCloud.Helpers;
using MyCloud.Models;
using MyCloud.SharedLib;
using System.Linq;

namespace MyCloud.Hubs
{
    public class MessengerHub : BaseMessengerHub
    {
        public void SendMessageToAll(string userName, string message)
        {
            Clients.All.messageReceived(userName, message);
        }

        public void SendPrivateMessage(IdentityConversationReply data)
        {
            var connectedUsers = MessengerHelpers.GetAllUsersFromCache();
            var toUser = connectedUsers.FirstOrDefault(x => x.id == data.TargetId && x.t == data.TargetObjectType);
            var fromUser = connectedUsers.FirstOrDefault(x => x.id == data.UserId && x.t == data.UserObjectType);

            if (connectedUsers != null && connectedUsers.Count > 0)
            {
                if(fromUser != null && fromUser.Connections.CheckListHasData())
                {
                    foreach (var senderConn in fromUser.Connections)
                    {
                        Clients.Client(senderConn.ConnectionId).sendPrivateMessage(data);
                    }
                }

                if (toUser != null && toUser.Connections.CheckListHasData())
                {
                    foreach (var receiverConn in toUser.Connections)
                    {
                        // Broad cast message
                        Clients.Client(receiverConn.ConnectionId).messageReceived(data);
                    }
                }
            }

        }

        public void UserIsTyping(Connector uInfo, int targetId, int t)
        {
            var connectedUsers = MessengerHelpers.GetAllUsersFromCache();
            var toUser = connectedUsers.FirstOrDefault(x => x.id == targetId && x.t == t);
            var fromUser = connectedUsers.FirstOrDefault(x => x.id == uInfo.id && x.t == uInfo.t);

            if (toUser != null && fromUser != null)
            {
                if (connectedUsers != null && connectedUsers.Count > 0)
                {
                    foreach (var item in connectedUsers)
                    {
                        if (item.id == toUser.id)
                        {
                            if (item.Connections != null && item.Connections.Count > 0)
                            {
                                foreach (var conn in item.Connections)
                                {
                                    // Broad cast message
                                    Clients.Client(conn.ConnectionId).userIsTyping(fromUser);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void MessageTypingDone(Connector uInfo, int targetId, int t)
        {
            var connectedUsers = MessengerHelpers.GetAllUsersFromCache();
            var toUser = connectedUsers.FirstOrDefault(x => x.id == targetId && x.t == t);
            var fromUser = connectedUsers.FirstOrDefault(x => x.id == uInfo.id && x.t == uInfo.t);

            if (toUser != null && fromUser != null)
            {
                if (connectedUsers != null && connectedUsers.Count > 0)
                {
                    foreach (var item in connectedUsers)
                    {
                        if (item.id == toUser.id)
                        {
                            if (item.Connections != null && item.Connections.Count > 0)
                            {
                                foreach (var conn in item.Connections)
                                {
                                    // Broad cast message
                                    Clients.Client(item.ConnectionId).messageTypingDone(fromUser);
                                }
                            }
                        }
                    }
                }
            }
        }

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