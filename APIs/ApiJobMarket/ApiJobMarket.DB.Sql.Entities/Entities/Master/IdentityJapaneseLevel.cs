using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityJapaneseLevel : IdentityCommon
    {
        public int id { get; set; }

        public string level { get; set; }

        public int level_number { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }
}
