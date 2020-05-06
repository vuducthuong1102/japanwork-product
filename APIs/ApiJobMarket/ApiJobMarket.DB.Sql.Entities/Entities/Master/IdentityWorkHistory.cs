using System;
namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityWorkHistory : IdentityCommon
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public string company { get; set; }
        public string content_work { get; set; }
        public int form { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string address { get; set; }
    }
   
    public class IdentityJobSeekerWorkHistory : IdentityWorkHistory
    {
        public int job_seeker_id { get; set; }
    }
}
