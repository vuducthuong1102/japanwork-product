using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ApiJobMarket.Models
{
    public class ApiCsModel
    {
        public int? id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? job_seeker_id { get; set; }
        public string cs_title { get; set; }
        public string date { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public int? gender { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string phone { get; set; }       
        public int? highest_edu { get; set; }       
        public string image { get; set; }      
        public int? station_id { get; set; }
        public int? train_line_id { get; set; }
    }

    public class ApiCsWorkHistoryModel
    {
        public int? id { get; set; }
        public int? cs_id { get; set; }
        public string company { get; set; }
        public int? sub_field_id { get; set; }
        public int? sub_industry_id { get; set; }
        public int? employment_type_id { get; set; }
        public int? employees_number { get; set; }

        [AllowHtml]
        public string resign_reason { get; set; }

        public string address { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? status { get; set; }

        public List<ApiCsWorkHistoryDetailModel> Details { get; set; }
    }

    public class ApiCsWorkHistoryDetailModel
    {
        public int? id { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public int? salary { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string content_work { get; set; }
    }

    public class ApiCsEduHistoryModel
    {
        public int? id { get; set; }
        public int? cs_id { get; set; }
        public int? qualification_id { get; set; }
        public int? major_id { get; set; }
        public string school { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? status { get; set; }
        public string address { get; set; }
        public string major_custom { get; set; }
    }

    public class ApiCsCertificateModel
    {
        public int? id { get; set; }
        public int? cs_id { get; set; }
        public string name { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int? pass { get; set; }
        public string point { get; set; }
        public int? status { get; set; }
    }

    public class ApiCsAddressModel
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

        public int? cs_id { get; set; }
        public int? job_seeker_id { get; set; }
        public bool is_contact_address { get; set; }
        public int? train_line_id { get; set; }
        public int? station_id { get; set; }
    }

    public class ApiCsUpdateModel
    {
        public ApiCsModel cs { get; set; }
        public List<ApiCsWorkHistoryModel> work_history { get; set; }
        public List<ApiCsEduHistoryModel> edu_history { get; set; }
        public List<ApiCsCertificateModel> certification { get; set; }
        public ApiCsAddressModel address { get; set; }
        //public ApiCsAddressModel address_contact { get; set; }
    }

    public class ApiCsDeleteModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cs_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? job_seeker_id { get; set; }
    }

    public class ApiCsSavePrintCodeModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cs_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public string cs_code_id { get; set; }
    }

    public class ApiCsDeletePrintCodeModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cs_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public string cs_code_id { get; set; }
    }

    public class ApiCsUploadImageModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public string image { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? cs_id { get; set; }

        public int? job_seeker_id { get; set; }
    }
}