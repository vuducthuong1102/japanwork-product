using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityVisa : IdentityCommon
    {
        public int id { get; set; }

        public string visa { get; set; }

        public List<IdentityVisaLang> LangList { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public IdentityVisa()
        {
            LangList = new List<IdentityVisaLang>();
        }
    }

    public class IdentityVisaLang
    {
        public int id { get; set; }
        public int visa_id { get; set; }
        public string visa { get; set; }
        public string language_code { get; set; }
    }
}
