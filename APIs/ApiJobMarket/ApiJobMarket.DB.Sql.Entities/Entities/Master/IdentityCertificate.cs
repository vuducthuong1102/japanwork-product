using Newtonsoft.Json;
using System;
namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCertificate : IdentityCommon
    {
        public int id { get; set; }        
        public string name { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string point { get; set; }
        public int pass { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }
        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }
   
    public class IdentityJobSeekerCertificate : IdentityCertificate
    {
        public int job_seeker_id { get; set; }
    }

}
