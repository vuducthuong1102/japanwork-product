using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentitySuggest : IdentityCommon
    {
        public int id { get; set; }
        public int form { get; set; }
        public int type { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public bool isDescription { get; set; }
        public string icon { get; set; }
        public List<IdentitySuggestLang> LangList { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public IdentitySuggest()
        {
            LangList = new List<IdentitySuggestLang>();
        }
    }

    public class IdentitySuggestLang
    {
        public int id { get; set; }
        public int suggest_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string language_code { get; set; }
    }
}
