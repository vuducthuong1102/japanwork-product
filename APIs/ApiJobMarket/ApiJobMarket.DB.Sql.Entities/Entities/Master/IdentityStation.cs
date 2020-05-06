using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityStation : IdentityCommon
    {
        public int id { get; set; }
        public string station { get; set; }
        public string furigana { get; set; }
        public string address { get; set; }
        public string postal_code { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        //Extends
        public int train_line_id { get; set; }

        //public List<IdentityCityLang> LangList { get; set; }
                
        //public IdentityCity()
        //{
        //    LangList = new List<IdentityCityLang>();
        //}
    }

    public class IdentityJobAddressStation 
    {
        public int id { get; set; }
        public int job_id { get; set; }
        public int job_address_id { get; set; }
        public int station_id { get; set; }
        public string station { get; set; }
        public string furigana { get; set; }
        public string detail { get; set; }
    }
}
