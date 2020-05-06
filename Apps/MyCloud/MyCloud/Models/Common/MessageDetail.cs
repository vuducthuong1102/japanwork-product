using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCloud
{
    public class ConnectorMessages
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public List<MessageDetail> ListMessage { get; set; }
        public ConnectorMessages()
        {
            ListMessage = new List<MessageDetail>();
        }         
    }


    public class MessageDetail
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string Message { get; set; }
        public DateTime SentTime { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public int AffectedId { get; set; }
    }
}