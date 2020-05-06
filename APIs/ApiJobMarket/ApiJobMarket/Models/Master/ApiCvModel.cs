using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ApiJobMarket.Models
{
    public class ApiCvModel
    {
        public int? id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? job_seeker_id { get; set; }
        public string cv_title { get; set; }
        public string date { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public int? gender { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string phone { get; set; }

        public int japanese_level_number { get; set; }
        public int? marriage { get; set; }
        public int? dependent_num { get; set; }
        public int? highest_edu { get; set; }

        [AllowHtml]
        public string pr { get; set; }

        [AllowHtml]
        public string hobby_skills { get; set; }

        [AllowHtml]
        public string reason { get; set; }

        [AllowHtml]
        public string time_work { get; set; }

        [AllowHtml]
        public string aspiration { get; set; }

        public int? form { get; set; }
        public string image { get; set; }

        [AllowHtml]
        public string reason_pr { get; set; }
        public int? check_address { get; set; }
        public int? check_work { get; set; }
        public int? check_ceti { get; set; }
        public int? check_timework { get; set; }
        public int? check_aspiration { get; set; }
        public int? station_id { get; set; }
        public int? train_line_id { get; set; }
    }

    public class ApiCvWorkHistoryModel
    {
        public int? id { get; set; }
        public int? cv_id { get; set; }
        public string company { get; set; }

        [AllowHtml]
        public string content_work { get; set; }

        public string address { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? form { get; set; }
        public int? status { get; set; }
    }

    public class ApiCvEduHistoryModel
    {
        public int? id { get; set; }
        public int? cv_id { get; set; }
        public int? qualification_id { get; set; }
        public int? major_id { get; set; }
        public string school { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? status { get; set; }
        public string address { get; set; }
        public string major_custom { get; set; }
    }

    public class ApiCvCertificateModel
    {
        public int? id { get; set; }
        public int? cv_id { get; set; }
        public string name { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? pass { get; set; }
        public string point { get; set; }
        public int? status { get; set; }
    }

    public class ApiCvAddressModel
    {
        public int? id { get; set; }
        public int country_id { get; set; }
        public int? region_id { get; set; }
        public int? prefecture_id { get; set; }
        public int? city_id { get; set; }
        public string detail { get; set; }
        public string furigana { get; set; }
        public string note { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string postal_code { get; set; }

        public int? cv_id { get; set; }
        public int? job_seeker_id { get; set; }
        public bool is_contact_address { get; set; }
        public int? train_line_id { get; set; }
        public int? station_id { get; set; }
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
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cv_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? job_seeker_id { get; set; }
    }

    public class ApiCvSavePrintCodeModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cv_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public string cv_code_id { get; set; }
    }

    public class ApiCvDeletePrintCodeModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cv_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public string cv_code_id { get; set; }
    }

    public class ApiCvUploadImageModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public string image { get; set; }

        public string access_token { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cv_id { get; set; }

        public int? job_seeker_id { get; set; }
    }

    public class ApiCvByPageModel : ApiGetListByPageModel
    {
        
    }
}