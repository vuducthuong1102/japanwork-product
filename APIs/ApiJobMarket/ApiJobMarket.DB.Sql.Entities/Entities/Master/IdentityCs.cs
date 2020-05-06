using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCs : IdentityCommon
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string cs_title { get; set; }
        public DateTime? date { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public int gender { get; set; }
        public DateTime? birthday { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int highest_edu { get; set; }
        public string image { get; set; }
        public string pdf { get; set; }            
        public int main_cs { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }
        public int status { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //Extends
        public string image_full_path { get; set; }

        public IdentityCsAddress address { get; set; }

        //public IdentityCsAddress address_contact { get; set; }

        public List<IdentityCsCertificate> certification { get; set; }

        public List<IdentityCsEduHistory> edu_history { get; set; }

        public List<IdentityCsWorkHistory> work_history { get; set; }

        public dynamic Extensions { get; set; }

        public IdentityCs()
        {
            Extensions = new ExpandoObject();
            Extensions.is_invited = false;
        }
    }

    public class IdentityCsAddress : IdentityAddress
    {
        public int cs_id { get; set; }
        public int job_seeker_id { get; set; }
        public bool is_contact_address { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }
    }

    public class IdentityCsCertificate : IdentityCertificate
    {
        public int cs_id { get; set; }
    }

    public class IdentityCsEduHistory
    {
        public int id { get; set; }
        public int cs_id { get; set; }
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

    public class IdentityCsWorkHistory
    {
        public int id { get; set; }
        public int cs_id { get; set; }
        public string company { get; set; }
        public int sub_field_id { get; set; }
        public int sub_industry_id { get; set; }
        public int employment_type_id { get; set; }
        public int employees_number { get; set; }
        public string resign_reason { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        public List<IdentityCsWorkHistoryDetail> Details { get; set; }

        public string RemoveDetailItems { get; set; }

        public IdentityCsWorkHistory()
        {
            Details = new List<IdentityCsWorkHistoryDetail>();
        }
    }

    public class IdentityCsWorkHistoryDetail
    {
        public int id { get; set; }
        public int cs_work_history_id { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public int salary { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string content_work { get; set; }
    }

    //public class IdentityCsPdfCode
    //{
    //    public int id { get; set; }
    //    public string code_id { get; set; }
    //    public int user_id { get; set; }
    //    public int form { get; set; }
    //    public string title { get; set; }
    //    public int cs_id { get; set; }
    //    public int work_background_id { get; set; }
    //    public DateTime? created_at { get; set; }
    //    public DateTime? updated_at { get; set; }
    //    public DateTime? expired_at_utc { get; set; }
    //}
}
