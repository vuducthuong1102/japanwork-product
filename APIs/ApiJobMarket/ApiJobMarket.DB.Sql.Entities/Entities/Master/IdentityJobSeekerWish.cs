using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityJobSeekerWish : IdentityCommon
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }

        public string employment_type_ids { get; set; }

        public string prefecture_ids { get; set; }
        public string sub_field_ids { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int employment_type_id { get; set; }
        public DateTime? start_date { get; set; }
    }
}
