using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityEmploymentType : IdentityCommon
    {
        public int id { get; set; }

        public string employment_type { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public bool show_trains { get; set; }
        public int calculate_by { get; set; }

        public List<IdentityEmploymentTypeLang> LangList { get; set; }

        public IdentityEmploymentType()
        {
            LangList = new List<IdentityEmploymentTypeLang>();
        }
    }

    public class IdentityEmploymentTypeLang
    {
        public int id { get; set; }
        public int employment_type_id { get; set; }
        public string employment_type { get; set; }
        public string language_code { get; set; }
    }
}
