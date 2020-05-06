using System;
namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityEduHistory : IdentityCommon
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public string school { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string address { get; set; }
        public int qualification_id { get; set; }
        public int major_id { get; set; }
        public string major_custom { get; set; }
    }
   
    public class IdentityJobSeekerEduHistory : IdentityEduHistory
    {
        public int job_seeker_id { get; set; }
    }
}
