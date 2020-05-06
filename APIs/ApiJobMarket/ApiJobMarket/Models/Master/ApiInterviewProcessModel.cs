using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
{
    public class ApiInterviewProcessEditModel : ApiInterviewProcessModel
    {
        public int id { get; set; }
    }
    public class ApiInterviewProcessInsertModel : ApiInterviewProcessModel
    {
    }
    public class ApiInterviewProcessModel
    {
        public string note { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int status_id { get; set; }
        public int job_id { get; set; }
        public int cv_id { get; set; }
        public int agency_id { get; set; }
        public int candidate_id { get; set; }
        public DateTime? modified_at { get; set; }
        public DateTime? created_at { get; set; }

        public int staff_id { get; set; }
    }

    public class ApiInterviewProcessSearchModel : ApiCommonModel
    {

    }
}