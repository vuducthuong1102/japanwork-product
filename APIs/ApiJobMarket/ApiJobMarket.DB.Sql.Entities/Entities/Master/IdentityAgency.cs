using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityAgency : IdentityCommon
    {
        public int id { get; set; }
        public string agency { get; set; }
        public int agency_id { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public string address { get; set; }
        public int constract_id { get; set; }  
        public string logo_path { get; set; }
        public string logo_full_path { get; set; }

        public int status { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }
}
