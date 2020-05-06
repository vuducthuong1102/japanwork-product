using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityApplication : IdentityCommon
    {
        public int id { get; set; }
        public DateTime? interview_accept_time { get; set; }
        public DateTime? cancelled_time { get; set; }
        public int cv_id { get; set; }
        public int type { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //Extends
        public string status_label { get; set; }
        public IdentityJob job_info { get; set; }
        public bool is_saved { get; set; }
        public int pic_id { get; set; }
        public int job_pic_id { get; set; }
        public string fullname { get; set; }
        public string qualification { get; set; }
        public string major { get; set; }
        public int gender { get; set; }
        public DateTime? birthday { get; set; }
        public int japanese_level_number { get; set; }
        public string email { get; set; }
        public string code { get; set; }
       public bool is_show_info { get; set; }
    }
}
