using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityMajor : IdentityCommon
    {
        public int id { get; set; }

        public string major { get; set; }

        public List<IdentityMajorLang> LangList { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public IdentityMajor()
        {
            LangList = new List<IdentityMajorLang>();
        }
    }

    public class IdentityMajorLang
    {
        public int id { get; set; }
        public int major_id { get; set; }
        public string major { get; set; }
        public string language_code { get; set; }
    }
}
