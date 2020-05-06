using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
{
    public class ApiAgencyCreateModel : ApiAgencyModel
    {

    }

    public class ApiAgencyModel
    {
        public int? agency_id { get; set; }
        public string agency { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public string address { get; set; }
        public string logo_path { get; set; }
    }

    public class ApiMarkIsReadAgencyNotificationModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? staff_id { get; set; }

        public bool is_read { get; set; }
    }

    public class ApiAgencyGetListNotificationModel : ApiGetListByPageModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? staff_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? agency_id { get; set; }

        public int is_update_view { get; set; }
    }

    public class ApiAgencyStaffModel 
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.ERROR_FIELD_NOT_NULL_REQUIRED))]
        public int? staff_id { get; set; }
    }
}