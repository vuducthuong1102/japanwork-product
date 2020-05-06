using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityNotificationBase : IdentityCommon
    {
        public int id { get; set; }
        public int action_type { get; set; }
        public int sender_id { get; set; }
        public int target_type { get; set; }
        public int target_id { get; set; }
        public string content { get; set; }

        public DateTime? created_at { get; set; }
    }

    public class IdentityNotification : IdentityNotificationBase
    {
        public int user_id { get; set; }
        public int notification_id { get; set; }
        public bool is_viewed { get; set; }
        public bool is_read { get; set; }
        public int status { get; set; }
        public DateTime? read_at { get; set; }
    }
}
