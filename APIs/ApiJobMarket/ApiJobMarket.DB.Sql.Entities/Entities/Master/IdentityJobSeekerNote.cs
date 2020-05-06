using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{

    public class IdentityJobSeekerNote : IdentityCommon
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public int type { get; set; }
        public string note { get; set; }
        public DateTime? created_at { get; set; }
        public int type_job_seeker { get; set; }
        public int agency_id { get; set; }
    }
}
