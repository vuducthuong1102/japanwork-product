using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityInterviewProcess
    {
        public int id { get; set; }
        public string note { get; set; }
        public int status_id { get; set; }
        public int job_id { get; set; }
        public int cv_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? modified_at { get; set; }
        public int agency_id { get; set; }
        public int candidate_id { get; set; }
        public int total_count { get; set; }
        public int staff_id { get; set; }
    }
}
