using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCommon
    {
        [JsonIgnore]
        public string keyword { get; set; }

        //[JsonIgnore]
        public int total_count { get; set; }

        [JsonIgnore]
        public DateTime? from_date { get; set; }

        [JsonIgnore]
        public DateTime? to_date { get; set; }

        //[JsonIgnore]
        public string language_code { get; set; }

        public int staff_id { get; set; }

        public bool is_show_info { get; set; }

        [JsonIgnore]
        public int ishiring { get; set; }
    }

    public class BaseOnlineUserIdentity
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Avatar { get; set; }
    }
}
