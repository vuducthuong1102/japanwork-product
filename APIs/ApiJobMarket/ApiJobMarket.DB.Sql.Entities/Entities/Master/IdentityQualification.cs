using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityQualification : IdentityCommon
    {
        public int id { get; set; }

        public string qualification { get; set; }

        public bool show_major { get; set; }

        public List<IdentityQualificationLang> LangList { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public IdentityQualification()
        {
            LangList = new List<IdentityQualificationLang>();
        }
    }

    public class IdentityQualificationLang
    {
        public int id { get; set; }
        public int qualification_id { get; set; }
        public string qualification { get; set; }
        public string language_code { get; set; }
    }
}
