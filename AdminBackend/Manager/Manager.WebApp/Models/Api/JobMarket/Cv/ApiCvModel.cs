using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ApiCvModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string cv_title { get; set; }
        public string date { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public int gender { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int marriage { get; set; }
        public int dependent_num { get; set; }
        public int highest_edu { get; set; }
        public string pr { get; set; }
        public string hobby_skills { get; set; }
        public string reason { get; set; }
        public string time_work { get; set; }
        public string aspiration { get; set; }
        public int form { get; set; }
        public string image { get; set; }
        public string reason_pr { get; set; }
        public int check_address { get; set; }
        public int check_work { get; set; }
        public int check_ceti { get; set; }
        public int check_timework { get; set; }
        public int check_aspiration { get; set; }
        public int station_id { get; set; }
        public int train_line_id { get; set; }

        public HttpPostedFileBase image_file_upload { get; set; }
    }

    public class ApiCvWorkHistoryModel : ApiJobSeekerWorkHistoryModel
    {
        public int cv_id { get; set; }
    }

    public class ApiCvEduHistoryModel : ApiJobSeekerEduHistoryModel
    {
        public int cv_id { get; set; }
    }

    public class ApiCvCertificateModel : ApiJobSeekerCertificateModel
    {
        public int cv_id { get; set; }
    }

    public class ApiCvAddressModel
    {
        public int id { get; set; }
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string detail { get; set; }
        public string furigana { get; set; }
        public string note { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string postal_code { get; set; }

        public int cv_id { get; set; }
        public int job_seeker_id { get; set; }
        public bool is_contact_address { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }
    }

    public class ApiCvUpdateModel
    {
        public ApiCvModel cv { get; set; }
        public List<ApiCvWorkHistoryModel> work_history { get; set; }
        public List<ApiCvEduHistoryModel> edu_history { get; set; }
        public List<ApiCvCertificateModel> certification { get; set; }
        public ApiCvAddressModel address { get; set; }
        public ApiCvAddressModel address_contact { get; set; }

        public string access_token { get; set; }
    }

    public class ApiCvDeleteModel
    {
        public int cv_id { get; set; }
        public int job_seeker_id { get; set; }
    }

    public class ApiCvSavePrintCodeModel
    {        
        public int cv_id { get; set; }        
        public string cv_code_id { get; set; }
    }

    public class ApiCvDeletePrintCodeModel
    {        
        public int cv_id { get; set; }        
        public string cv_code_id { get; set; }
    }

    public class ApiCvUploadImageModel
    {        
        public string image { get; set; }
        public string access_token { get; set; }        
        public int cv_id { get; set; }
        public int job_seeker_id { get; set; }
    }

    public class ApiCvByPageModel : ApiGetListByPageModel
    {

    }
}