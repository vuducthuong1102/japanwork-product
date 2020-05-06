using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityJobSeeker : IdentityCommon
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int marriage { get; set; }
        public int dependent_num { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public string display_name { get; set; }
        public string image { get; set; }
        public DateTime? birthday { get; set; }
        public int gender { get; set; }
        public string id_card { get; set; }
        public string note { get; set; }
        public string video_path { get; set; }
        public string expected_job_title { get; set; }
        public int expected_salary_min { get; set; }
        public int expected_salary_max { get; set; }
        public int work_status { get; set; }
        public string google_id { get; set; }
        public string facebook_id { get; set; }
        public int view_count { get; set; }
        public int status { get; set; }
        public int qualification_id { get; set; }
        public int job_seeking_status_id { get; set; }
        public int salary_type_id { get; set; }        
        public int agency_id { get; set; }
        public int company_count { get; set; }
        //[JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public dynamic Extensions { get; set; }

        public List<IdentityJobSeekerAddress> Addresses { get; set; }

        public string school { get; set; }
        public string major { get; set; }
        public int japanese_level_number { get; set; }
        public int pic_id { get; set; }

        public bool is_assignmented { get; set; }

        public string code { get; set; }
        public IdentityJobSeekerWish WishInfo { get; set; }
        public IdentityJobSeeker()
        {
            Addresses = new List<IdentityJobSeekerAddress>();
            Extensions = new ExpandoObject();
            Extensions.image_full = string.Empty;
            WishInfo = new IdentityJobSeekerWish();
        }
        public int process_lastest { get; set; }
        public int type_job_seeker { get; set; }
        public int company_id { get; set; }
        public int japanese_level { get; set; }
        public bool account_active { get; set; }
        public int cv_id { get; set; }

        public string metadata { get; set; }
        public int nationality_id { get; set; }
        public int visa_id { get; set; }
        public DateTime? duration_visa { get; set; }
        public bool religion { get; set; }
        public string religion_detail { get; set; }
        public string qualification { get; set; }
    }    

    public class IdentityJobSeekerMetaItem
    {
        public string name { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string type { get; set; }        
    }
    public class IdentityJobSeekerDevice : IdentityDevice
    {
        public int job_seeker_id { get; set; }
    }

    public class IdentityJobSeekerConfig
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public int working_status { get; set; }
        public string working_detail { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int salary_type { get; set; }
        public string field_ids { get; set; }
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
    }

    public class IdentityJobSeekerCounter
    {
        public int Interviewing { get; set; }
        public int Recruitmented { get; set; }

        public int GetTotal()
        {
            return Interviewing + Recruitmented;
        }
    }
}
