using Newtonsoft.Json;
using System;

namespace SingleSignOn.DB.Sql.Entities
{
    public class IdentityCommon
    {
        //[JsonIgnore]
        public int TotalCount { get; set; }

        [JsonIgnore]
        public DateTime? FromDate { get; set; }

        [JsonIgnore]
        public DateTime? ToDate { get; set; }
    }
}
