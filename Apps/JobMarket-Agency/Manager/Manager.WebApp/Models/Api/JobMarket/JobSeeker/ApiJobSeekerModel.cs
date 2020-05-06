using ApiJobMarket.DB.Sql.Entities;
using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    public class ApiJobSeekerByPageModel : ApiGetListByPageModel
    {
        public int employment_type_id { get; set; }
        public int has_process { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int qualification_id { get; set; }
        public int sub_field_id { get; set; }
        public int japanese_level_number { get; set; }
        public int prefecture_id { get; set; }
        public int visa_id { get; set; }
    }

    public class ApiJobSeekerModel : ApiCommonFilterModel
    {
        public int id { get; set; }
        public int agency_id { get; set; }
        public int staff_id { get; set; }
        public int user_id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int? marriage { get; set; }
        public int? dependent_num { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public string display_name { get; set; }
        public string image { get; set; }
        public string birthday { get; set; }
        public int? gender { get; set; }
        public string id_card { get; set; }
        public string note { get; set; }
        public string video_path { get; set; }
        public string expected_job_title { get; set; }
        public int? expected_salary_min { get; set; }
        public int? expected_salary_max { get; set; }
        public int? work_status { get; set; }
        public string google_id { get; set; }
        public string facebook_id { get; set; }
        public int qualification_id { get; set; }
        public int job_seeking_status_id { get; set; }
        public int salary_type_id { get; set; }
        public int japanese_level_number { get; set; }
        public int pic_id { get; set; }
        public int country_id { get; set; }
        public int major_id { get; set; }
        public int nationality_id { get; set; }
        public int visa_id { get; set; }
        public string duration_visa { get; set; }
        public int religion { get; set; }
        public string religion_detail { get; set; }
        public List<IdentityJobSeekerAddress> Addresses { get; set; }

        public List<IdentityJobSeekerMetaItem> metadata { get; set; }
    }    

    public class ApiJobSeekerGetDetailModel {
        public int id { get; set; }
    }
    public class ApiJobSeekerDeleteModel
    {
        public string ids { get; set; }
        public int type { get; set; }
        public int id { get; set; }
        public int? agency_id { get; set; }
    }
    public class ApiJobSeekerEduHistoryModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string school { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }
        public int qualification_id { get; set; }
        public int major_id { get; set; }
        public string major_custom { get; set; }
    }

    public class ApiJobSeekerWorkHistoryModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string company { get; set; }
        public string content_work { get; set; }
        public int form { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }
    }

    public class ApiJobSeekerCertificateModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string name { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int status { get; set; }
        public int pass { get; set; }
        public string point { get; set; }
        public int is_detail { get; set; }
    }

    public class ApiJobSeekerSuggestionModel :ApiGetListByPageModel
    {
        public int job_id { get; set; }
    }
    public class ApiJobSeekerWishModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public int employment_type_id { get; set; }
        public string employment_type_ids { get; set; }
        public string prefecture_ids { get; set; }
        public string sub_field_ids { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public string start_date { get; set; }
    }
}