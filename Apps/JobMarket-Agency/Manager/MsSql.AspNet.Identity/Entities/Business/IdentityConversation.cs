using System;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityConversationObject
    {
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public int ObjectRootId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ConversationId { get; set; }
    }

    public class IdentityConversation : IdentityCommon
    {
        public int Id { get; set; }

        public int UserOne { get; set; }

        public int UserTwo { get; set; }

        public string Ip { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int Status { get; set; }

        public bool OneViewed { get; set; }

        public bool TwoViewed { get; set; }

        public bool OneRead { get; set; }

        public bool TwoRead { get; set; }

        public bool OneDeleted { get; set; }

        public bool TwoDeleted { get; set; }

        public DateTime? OneReadDate { get; set; }

        public DateTime? TwoReadDate { get; set; }

        public int OwnerId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public BaseOnlineUserIdentity UserOneInfo { get; set; }

        public BaseOnlineUserIdentity UserTwoInfo { get; set; }

        public IdentityConversationReply FirstConversationReply { get; set; }
    }

    public class IdentityConversationReply : IdentityCommon
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int Type { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }
        public int UserObjectId { get; set; }
        public int UserObjectType { get; set; }

        public int TargetId { get; set; }
        public int TargetObjectId { get; set; }
        public int TargetObjectType { get; set; }

        public string Ip { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int Status { get; set; }

        public bool OneDeleted { get; set; }

        public bool TwoDeleted { get; set; }

        public BaseOnlineUserIdentity UserOneInfo { get; set; }

        public BaseOnlineUserIdentity UserTwoInfo { get; set; }

        public int OwnerId { get; set; }
        public int UserTwo { get; set; }
    }
}
