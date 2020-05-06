using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityField : IdentityCommon
    {
        public int id { get; set; }
        public string field { get; set; }
        public string icon { get; set; }
        public int employment_type { get; set; }

        public List<IdentitySubField> Sub_fields { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }
        public int count_num { get; set; }
        public int count_add_today { get; set; }
        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public List<IdentityFieldLang> LangList { get; set; }

        public IdentityField()
        {
            LangList = new List<IdentityFieldLang>();
        }
    }

    public class IdentitySubField : IdentityCommon
    {
        public int id { get; set; }
        public int field_id { get; set; }
        public string sub_field { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public List<IdentitySubFieldLang> LangList { get; set; }

        public IdentitySubField()
        {
            LangList = new List<IdentitySubFieldLang>();
        }
    }

    public class IdentityJobSubField : IdentitySubField
    {
        public int sub_field_id { get; set; }
        public int job_id { get; set; }
    }

    public class IdentityFieldLang
    {
        public int id { get; set; }
        public int field_id { get; set; }
        public string field { get; set; }
        public string language_code { get; set; }
    }

    public class IdentitySubFieldLang
    {
        public int id { get; set; }
        public int sub_field_id { get; set; }
        public string sub_field { get; set; }
        public string language_code { get; set; }
    }
}
