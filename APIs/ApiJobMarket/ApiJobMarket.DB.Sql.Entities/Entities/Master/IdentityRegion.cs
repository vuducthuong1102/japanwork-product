using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityRegion : IdentityCommon
    {
        public int id { get; set; }

        public string region { get; set; }

        public string furigana { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        public List<IdentityPrefecture> Prefectures { get; set; }

        //public List<IdentityRegionLang> LangList { get; set; }

        //public IdentityRegion()
        //{
        //    LangList = new List<IdentityRegionLang>();
        //}
    }

    public class IdentityRegionLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string Name { get; set; }
        public string UrlFriendly { get; set; }
        public int CategoryId { get; set; }
    }
}
