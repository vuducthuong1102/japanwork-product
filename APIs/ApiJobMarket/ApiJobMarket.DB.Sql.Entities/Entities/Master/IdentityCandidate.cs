using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCandidate : IdentityCommon
    {
        public int id { get; set; }
        public DateTime? request_time { get; set; }
        public DateTime? applied_time { get; set; }
        public int cv_id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int type { get; set; }

        public int type_job_seeker { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //Extends
        public string status_label { get; set; }

        public IdentityJob job_info { get; set; }

        public IdentityJobSeeker job_seeker_info { get; set; }

        public int agency_id { get; set; }
        public int agency_parent_id { get; set; }
        public int pic_id { get; set; }
        public int pic_job_id { get; set; }
        public int company_id { get; set; }
        public string list_job_seeker_ids { get; set; }
        public string list_job_ids { get; set; }
        public List<IdentityInterviewProcess> ListInterviewProcess { get; set; }
        public int interview_status_id { get; set; }
        public string code { get; set; }
    }
}
