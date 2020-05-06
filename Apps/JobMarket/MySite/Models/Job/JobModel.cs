using ApiJobMarket.DB.Sql.Entities;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Web;
using MySite.Resources;
using System.ComponentModel.DataAnnotations;

namespace MySite.Models
{
    public class JobSearchModel : CommonPagingModel
    {
        public string sorting_date { get; set; }
        public int employment_type_id { get; set; }
        public int field_id { get; set; }
        public int japanese_level_number { get; set; }
        public string sub_industry_ids { get; set; }
        public string sub_field_ids { get; set; }       
        public string city_ids { get; set; }
        public string station_ids { get; set; }

        //Selected area ids from city filter
        public string ct_region_ids { get; set; }
        public string ct_prefecture_ids { get; set; }

        //Selected area ids from station filter
        public string st_region_ids { get; set; }
        public string st_prefecture_ids { get; set; }

        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }

        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int calculate_by { get; set; }
        public int salary_id { get; set; }
        public List<IdentityField> FieldLists { get; set; }

        public List<IdentityApplication> JobLists { get; set; }
    }   

    public class JobSearchResultModel : CommonPagingModel
    {
        public List<JobInfoModel> JobsList { get; set; }
    }

    public class ApplicationSearchResultModel : CommonPagingModel
    {
        //public List<JobInfoModel> JobsList { get; set; }
        public List<ApplicationInfoModel> ApplicationList { get; set; }
    }

    public class ApplicationInfoModel : IdentityApplication
    {
        public string employment_type_label { get; set; }
        public bool employment_type_show_trains { get; set; }
    }

    public class InvitationSearchResultModel : CommonPagingModel
    {
        public List<InvitationInfoModel> InvitationList { get; set; }
    }

    public class InvitationInfoModel : IdentityInvitation
    {
        public string employment_type_label { get; set; }
        public bool employment_type_show_trains { get; set; }
    }

    public class JobInfoModel : IdentityJob
    {
        public string employment_type_label { get; set; }
        public bool employment_type_show_trains { get; set; }
        public string employment_type_calculate_label { get; set; }
        public string qualification_label { get; set; }
        public bool HasLoggedIn { get; set; }
    }

    public class JobViewDetailModel
    {
        public int id { get; set; }
        public JobInfoModel job_info { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }
        public int is_invitation { get; set; }
    }

    public class JobApplyModel
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public HttpPostedFileBase file_upload { get; set; }
        public string note { get; set; }
        public List<IdentityCv> Cvs { get; set; }
        public string token { get; set; }
        public int is_invitation { get; set; }
    }

    public class JobApplicationModel
    {
        public int id { get; set; }
        public int job_id { get; set; }
    }

    public class JobInviteFriendApplyModel
    {
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public string token { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_EMAIL_NULL))]
        [EmailAddress(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.ERROR_EMAIL_INVALID))]
        public string email { get; set; }
        public string note { get; set; }
    }

    public class InvitedFriendSearchResultModel : CommonPagingModel
    {
        public List<InvitedFriendInfoModel> InvitedFriends { get; set; }
    }

    public class InvitedFriendInfoModel : IdentityFriendInvitation
    {
        public string employment_type_label { get; set; }
        public bool employment_type_show_trains { get; set; }
    }
}