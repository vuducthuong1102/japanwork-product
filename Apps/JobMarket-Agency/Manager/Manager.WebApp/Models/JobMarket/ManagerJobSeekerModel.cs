using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ManageJobSeekerModel : CommonPagingModel
    {
        public List<IdentityJobSeeker> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SOURCE_CANDIDATE))]
        public int type_job_seeker { get; set; }

        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }
        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityQualification> Qualifications { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public int gender { get; set; }
        public int major_id { get; set; }
        public int employment_type_id { get; set; }
        public bool has_process { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int qualification_id { get; set; }
        public int staff_id { get; set; }
        public int sub_field_id { get; set; }
        public int japanese_level_number { get; set; }
        public int prefecture_id { get; set; }
        public int visa_id { get; set; }
        public List<IdentityMajor> Majors { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }
        public List<IdentityField> Fields { get; set; }
        public List<IdentityRegion> Regions { get; set; }
        public List<IdentityVisa> Visas { get; set; }
    }
    public class ManageJobSeekerNoteModel : CommonPagingModel
    {
        public int job_seeker_id { get; set; }
        public int type { get; set; }
        public int type_job_seeker { get; set; }
        public string note { get; set; }
        public int staff_id { get; set; }
        public List<IdentityJobSeekerNote> SearchResults { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> ListStaffs { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public string tk { get; set; }
    }
    public class ApiJobSeekerNoteModel : ApiGetListByPageModel
    {
    }
        public class JobSeekerNoteDeleteModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public int type_job_seeker { get; set; }
    }

    public class ApiJobSeekerNoteUpdateModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public int staff_id { get; set; }
        public int type { get; set; }
        public string note { get; set; }
        public int type_job_seeker { get; set; }
        public int agency_id { get; set; }
    }
    public class JobSeekerUpdateProfileModel : CommonPagingModel
    {
        public int job_seeker_id { get; set; }
        [MaxLength(30, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_EMAIL_MAX_LENGTH))]
        public string email { get; set; }
        [MaxLength(20, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_PHONE_MAX_LENGTH))]
        public string phone { get; set; }
        public int marriage { get; set; }
        public int dependent_num { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_JOBSEEKER_NAME_NULL))]
        [MaxLength(30, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_FULL_NAME_MAX_LENGTH))]
        public string fullname { get; set; }

        public string fullname_furigana { get; set; }

        public string display_name { get; set; }
        public string image { get; set; }
        public string image_full_path { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_NULL_VALUE))]
        public string birthday { get; set; }

        public int gender { get; set; }
        public string id_card { get; set; }
        public string note { get; set; }
        public string video_path { get; set; }
        public string expected_job_title { get; set; }
        public int expected_salary_min { get; set; }
        public int expected_salary_max { get; set; }
        public int work_status { get; set; }
        public string google_id { get; set; }
        public string facebook_id { get; set; }
        public int job_seeking_status_id { get; set; }
        public int qualification_id { get; set; }
        public int salary_type_id { get; set; }
        public int japanese_level_number { get; set; }

        public int pic_id { get; set; }
        public int agency_id { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }

        public HttpPostedFileBase image_file_upload { get; set; }
        public dynamic Extensions { get; set; }

        public List<IdentityQualification> Qualifications { get; set; }
        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityRegion> Regions { get; set; }

        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityField> Fields { get; set; }
        public List<IdentityCity> Cities { get; set; }

        public IdentityJobSeekerAddress address { get; set; }
        public IdentityJobSeekerAddress address_contact { get; set; }

        public IdentityTrainLine train_line_info { get; set; }
        public IdentityStation station_info { get; set; }

        public List<IdentityVisa> Visas { get; set; }

        public int exclude_ct_add { get; set; }
        public int hide_pf { get; set; }
        public int nationality_id { get; set; }
        public int visa_id { get; set; }
        public string duration_visa { get; set; }
        public bool religion { get; set; }
        public string religion_detail { get; set; }
        public int staff_id { get; set; }
        public List<CommonCustomFieldModel> metadata { get; set; }

        public List<JobSeekerWishModel> WishModel { get; set; }
        public string SelectedWishModel { get; set; }
        
        public int next_step { get; set; }

        public JobSeekerUpdateProfileModel()
        {
            Extensions = new ExpandoObject();
            address = new IdentityJobSeekerAddress();
            address_contact = new IdentityJobSeekerAddress();
        }
    }
    public class JobSeekerDeleteModel
    {
        public string ids { get; set; }
        public int type { get; set; }

        public int is_ignore { get; set; }
        public string tk { get; set; }

        public int id { get; set; }

        public int job_seeker_id { get; set; }
        public IdentityJobSeekerCounter Counter{ get; set; }
    }
    public class JobSeekerWishModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public List<int> employment_type_ids { get; set; }
        public int employment_type_id { get; set; }
        public List<int> prefecture_ids { get; set; }
        public List<int> sub_field_ids { get; set; }

        [MaxLength(8, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_SALARY_MUST_NUMBERIC))]
        [RegularExpression("^[0-9,]*$", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_SALARY_MUST_NUMBERIC))]
        public string salary_min { get; set; }

        [MaxLength(8, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_SALARY_MUST_NUMBERIC))]
        [RegularExpression("^[0-9,]*$", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_SALARY_MUST_NUMBERIC))]
        public string salary_max { get; set; }
        public string start_date { get; set; }
        public int exclude_ct_add { get; set; }

        public List<IdentityRegion> Regions { get; set; }
    }
    public class JobSeekerEduHistoryModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_SCHOOL_NAME_NULL))]
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
        public bool is_detail { get; set; }
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

        public bool is_detail { get; set; }
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
        public bool is_detail { get; set; }
    }

    public class JobSeekerInviteModel
    {
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_SELECT_APPLICANT))]
        public string applicant { get; set; }

        public int job_id { get; set; }
        public List<JobSeekerInviteInfoModel> JobSeekerList { get; set; }
        public string note { get; set; }

        //Extends
        public string job_name { get; set; }
        public int invitation_limit { get; set; }
        public int invited_count { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
    }

    public class JobSeekerInviteInfoModel
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public string full_name { get; set; }
    }

    public class JobSeekerChoosenModel : CommonPagingModel
    {
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public List<IdentityJobSeeker> SearchResults { get; set; }
        public string CallbackFunction { get; set; }
        public int gender { get; set; }
        public int major_id { get; set; }
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SOURCE_CANDIDATE))]
        public int type_job_seeker { get; set; }
        public int country_id { get; set; }
        public int company_id { get; set; }

        public int pic_id { get; set; }

        public List<IdentityMajor> Majors { get; set; }
        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> ListStaffs { get; set; }
        public List<IdentityJapaneseLevel> JapanseLevels { get; set; }
        public List<IdentityQualification> Qualifications { get; set; }

        public int status { get; set; }

    }

    public class JobSeekerItemInDropdownListModel
    {
        public int id { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public bool is_invited { get; set; }
    }
}