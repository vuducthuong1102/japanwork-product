using ApiJobMarket.DB.Sql.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ApiJobSeekerModel : ApiCommonFilterModel
    {
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

        public List<IdentityJobSeekerAddress> Addresses { get; set; }
    }    

    public class ApiJobSeekerGetDetailModel {
        public int id { get; set; }
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
    }

    public class ApiJobSeekerByPageModel : ApiGetListByPageModel
    {
        public int? agency_id { get; set; }
        public int type_job_seeker { get; set; }
        public int staff_id { get; set; }
    }
}