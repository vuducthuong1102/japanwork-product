using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityIndustry : IdentityCommon
    {
        public int id { get; set; }
        public string industry { get; set; }

        public List<IdentitySubIndustry> Sub_industries { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public List<IdentityIndustryLang> LangList { get; set; }

        public IdentityIndustry()
        {
            LangList = new List<IdentityIndustryLang>();
        }
    }

    public class IdentitySubIndustry : IdentityCommon
    {
        public int id { get; set; }
        public int industry_id { get; set; }
        public string sub_industry { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public List<IdentitySubIndustryLang> LangList { get; set; }

        public IdentitySubIndustry()
        {
            LangList = new List<IdentitySubIndustryLang>();
        }
    }

    public class IdentityIndustryLang
    {
        public int id { get; set; }
        public int industry_id { get; set; }
        public string industry { get; set; }
        public string language_code { get; set; }
    }

    public class IdentitySubIndustryLang
    {
        public int id { get; set; }
        public int sub_industry_id { get; set; }
        public string sub_industry { get; set; }
        public string language_code { get; set; }
    }
}
