using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityEmailMessage : CommonIdentity
    {
        public int Id { get; set; }        
        public int AgencyId { get; set; }
        public int StaffId { get; set; }

        //MessageId hashing from mail server
        public string MessageIdHash { get; set; }

        //MessageId Parent from mail server
        public string ParentMessageIdHash { get; set; }

        //MessageId from mail server
        public string MessageId { get; set; }

        //MessageId Parent from mail server
        public string ParentMessageId { get; set; }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string ShortMessage { get; set; }
        public string Attachments { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? FetchedDate { get; set; }

        //Local parent Id
        public int EmailMessageId { get; set; }

        public int TotalChilds { get; set; }

        public List<IdentityEmailMessage> MessageParts { get; set; }
        public List<IdentityEmailAttachment> AttachedFiles { get; set; }

        public IdentityEmailMessage()
        {
            MessageParts = new List<IdentityEmailMessage>();
        }
    }

    public class IdentityEmailAttachment
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
    }

    //public class IdentityEmailMessagePart : CommonIdentity
    //{
    //    public int Id { get; set; }
    //    public int EmailMessageId { get; set; }
    //    public string MessageId { get; set; }
    //    public string ParentMessageId { get; set; }
    //    public string Subject { get; set; }
    //    public string BodyContent { get; set; }
    //    public string Attachment { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //}

    public class IdentityEmailAddress
    {
        public string DisplayName { get; set; }
        public string Address { get; set; }
    }
}
