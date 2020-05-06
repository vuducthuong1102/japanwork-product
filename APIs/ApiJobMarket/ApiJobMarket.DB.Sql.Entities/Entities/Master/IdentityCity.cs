using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCity : IdentityCommon
    {
        public int id { get; set; }

        public int prefecture_id { get; set; }

        public string city { get; set; }

        public string furigana { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        //public List<IdentityCityLang> LangList { get; set; }
                
        //public IdentityCity()
        //{
        //    LangList = new List<IdentityCityLang>();
        //}
    }

    public class IdentityCityLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string Name { get; set; }
        public string UrlFriendly { get; set; }
        public int CategoryId { get; set; }
    }
}
