using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityTokenFireBase : IdentityCommon
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string token { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }
}
