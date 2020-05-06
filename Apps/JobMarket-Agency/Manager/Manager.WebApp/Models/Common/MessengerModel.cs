using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class MessengerModel
    {
        public BaseOnlineUserIdentity UserInfo { get; set; }
        public string Message { get; set; }
        public int MessageType { get; set; }
    }

    public class ConversationModel
    {
        public int ConversationId { get; set; }
        public IdentityUser TargetUser { get; set; }
    }

    public class MessageSendingModel
    {
        [AllowHtml]
        public string message { get; set; }

        public int receiver { get; set; }
        public int conversation { get; set; }
    }

    public class MessageItemModel
    {
        public int CurrentUserId { get; set; }
        public IdentityConversationReply MessageItem { get; set; }
        public List<IdentityConversationReply> NextMessages { get; set; }

        public MessageItemModel()
        {
            NextMessages = new List<IdentityConversationReply>();
        }
    }
}