using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityPrefecture : IdentityCommon
    {
        public int id { get; set; }

        public int region_id { get; set; }

        public string prefecture { get; set; }

        public string furigana { get; set; }

        public List<IdentityCity> Cities { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

    }
}
