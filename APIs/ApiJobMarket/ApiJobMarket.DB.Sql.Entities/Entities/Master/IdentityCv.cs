using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCv : IdentityCommon
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string cv_title { get; set; }
        public DateTime? date { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public int gender { get; set; }
        public DateTime? birthday { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool marriage { get; set; }
        public int dependent_num { get; set; }
        public int highest_edu { get; set; }
        public string pr { get; set; }
        public string hobby_skills { get; set; }
        public string reason { get; set; }
        public string time_work { get; set; }
        public string aspiration { get; set; }
        public int form { get; set; }
        public string image { get; set; }
        public string pdf { get; set; }
        public string reason_pr { get; set; }
        public string contact_phone { get; set; }
        public bool check_address { get; set; }
        public bool check_work { get; set; }
        public bool check_ceti { get; set; }
        public bool check_timework { get; set; }
        public bool check_aspiration { get; set; }
        public string address_detail { get; set; }
        public int region_id { get; set; }
        public int perfecture_id { get; set; }
        public int city_id { get; set; }
        public string contact_address { get; set; }
        public int main_cv { get; set; }
        public int station_id { get; set; }
        public int train_line_id { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //Extends
        public string status_label { get; set; }
        public string image_full_path { get; set; }

        public int nationality_id { get; set; }
        public int visa_id { get; set; }
        public DateTime? duration_visa { get; set; }
        public string religion_detail { get; set; }
        public IdentityJobSeekerAddress address { get; set; }

        public IdentityJobSeekerAddress address_contact { get; set; }

        public List<IdentityJobSeekerCertificate> certification { get; set; }

        public List<IdentityJobSeekerEduHistory> edu_history { get; set; }

        public List<IdentityJobSeekerWorkHistory> work_history { get; set; }

        public dynamic Extensions { get; set; }

        public int agency_parent_id { get; set; }

        public int agency_id { get; set; }

        public int qualification_id { get; set; }
        public int company_count { get; set; }
        public int japanese_level_number { get; set; }
        public IdentityJobSeeker jobseeker { get; set; }
        public IdentityCv()
        {
            Extensions = new ExpandoObject();
            Extensions.is_invited = false;
        }
    }

    public class IdentityCvAddress : IdentityAddress
    {
        public int cv_id { get; set; }
        public int job_seeker_id { get; set; }
        public bool is_contact_address { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }
    }

    public class IdentityCvCertificate : IdentityCertificate
    {
        public int cv_id { get; set; }
        public int job_seeker_id { get; set; }
    }

    public class IdentityCvEduHistory
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public string school { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }

        public int qualification_id { get; set; }
        public int major_id { get; set; }
        public string major_custom { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }

    public class IdentityCvWorkHistory
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public string company { get; set; }
        public string content_work { get; set; }
        public int form { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }

    public class IdentityCvPdfCode
    {
        public int id { get; set; }
        public string code_id { get; set; }
        public int user_id { get; set; }
        public int form { get; set; }
        public string title { get; set; }
        public int cv_id { get; set; }
        public int work_background_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? expired_at_utc { get; set; }
    }
}
