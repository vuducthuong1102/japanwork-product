using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Models
{
    public class ManageJobSeekerModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityJobSeeker> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class JobSeekerDetailsModel
    {
        public IdentityJobSeeker JobSeekerInfo { get; set; }
    }
    public class JobSeekerEditModel
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string logo_full_path { get; set; }

        public HttpPostedFileBase image_file_upload { get; set; }       

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_DESCRIPTION))]
        public string description { get; set; }

        public string email { get; set; }
        public string website { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string address_detail { get; set; }
        public string address_furigana { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int agency_id { get; set; }
        public int JobSeeker_size_id { get; set; }
        public int sub_industry_id { get; set; }
        public int establish_year { get; set; }

        public JobSeekerAddressModel Address { get; set; }

        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityIndustry> Industries { get; set; }

        public JobSeekerEditModel()
        {
            Address = new JobSeekerAddressModel();
        }
    }

    public class JobSeekerAddressModel
    {        
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
    }

    public class JobSeekerEduHistoryModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_NULL_VALUE))]
        public string school { get; set; }

        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }
        public int qualification_id { get; set; }
        public int major_id { get; set; }
        public string major_custom { get; set; }

        //Extensions
        public string start_date_str { get; set; }
        public string end_date_str { get; set; }
        public string qualification_label { get; set; }
        public string major_label { get; set; }
        public List<IdentityQualification> Qualifications { get; set; }
        public List<IdentityMajor> Majors { get; set; }
        public bool isDefault { get; set; }
    }

    public class JobSeekerWorkHistoryModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_NULL_VALUE))]
        public string company { get; set; }

        public string content_work { get; set; }
        public int form { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string address { get; set; }

        //Extensions
        public string start_date_str { get; set; }
        public string end_date_str { get; set; }
        public bool isDefault { get; set; }
    }

    public class JobSeekerCertificateModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_NULL_VALUE))]
        public string name { get; set; }

        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int status { get; set; }
        public string point { get; set; }
        public int pass { get; set; }

        //Extensions
        public string start_date_str { get; set; }
        public string end_date_str { get; set; }
        public bool isDefault { get; set; }
    }
}