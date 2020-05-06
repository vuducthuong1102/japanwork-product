using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityAgencyNotificationBase : IdentityCommon
    {
        public int id { get; set; }
        public int action_type { get; set; }
        public int sender_id { get; set; }
        public int target_type { get; set; }
        public int target_id { get; set; }        
        public string content { get; set; }
        public int company_id { get; set; }
        public int agency_id { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class IdentityAgencyNotification : IdentityAgencyNotificationBase
    {
        public int notification_id { get; set; }
        public bool is_viewed { get; set; }
        public bool is_read { get; set; }
        public bool is_sent { get; set; }
        public int status { get; set; }
        public DateTime? read_at { get; set; }
        public int action_id { get; set; }
    }
}
