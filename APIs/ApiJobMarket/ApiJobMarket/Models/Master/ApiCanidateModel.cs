using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Models
{
    public class ApiCandidateModel : ApiCommonModel
    {
        public int agency_id { get; set; }
        public string langcode { get; set; }
    }
    public class ApiCandidateInsertModel
    {
        public int cv_id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int type { get; set; }
        public int company_id { get; set; }
        public int agency_parent_id { get; set; }
        public int agency_id { get; set; }
        public int pic_id { get; set; }
        public int staff_id { get; set; }
        public string list_job_seeker_ids { get; set; }
        public string list_job_ids { get; set; }
    }
    public class ApiCandidateDeleteModel
    {
        public int job_id { get; set; }
        public int agency_id { get; set; }
    }
}