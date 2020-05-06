using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class ApiConversationModel
    {
        public int Id { get; set; }

        public int UserOne { get; set; }

        public int UserTwo { get; set; }

        public string Ip { get; set; }

        public int Status { get; set; }

        public int OwnerId { get; set; }
    }

    public class ApiConversationReplyModel
    {
        public long Id { get; set; }

        public int ConversationId { get; set; }

        public int Type { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public string Ip { get; set; }

        public int Status { get; set; }
    }

    public class ApiGetMessagesModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int OwnerId { get; set; }
        public int UserTwo { get; set; }
    }
}