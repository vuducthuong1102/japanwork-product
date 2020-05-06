using ApiJobMarket.DB.Sql.Entities;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Web;
using MySite.Resources;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System;

namespace MySite.Models
{
    public class JobSeekerUpdateProfileModel : CommonPagingModel
    {
        public string email { get; set; }
        public string phone { get; set; }
        public int marriage { get; set; }
        public int dependent_num { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
        public string fullname { get; set; }

        public string fullname_furigana { get; set; }

        public string display_name { get; set; }
        public string image { get; set; }
        public string image_full_path { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
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
        public int nationality_id { get; set; }
        public int visa_id { get; set; }
        public string duration_visa { get; set; }
        public bool religion { get; set; }
        public string religion_detail { get; set; }
        public HttpPostedFileBase image_file_upload { get; set; }
        public dynamic Extensions { get; set; }

        public List<IdentityQualification> Qualifications { get; set; }
        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityRegion> Regions { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public List<IdentityCity> Cities { get; set; }
        public List<IdentityVisa> Visas { get; set; }

        public IdentityJobSeekerAddress address { get; set; }
        public IdentityJobSeekerAddress address_contact { get; set; }

        public IdentityTrainLine train_line_info { get; set; }
        public List<IdentityTrainLine> train_lines { get; set; }
        public IdentityStation station_info { get; set; }

        public int exclude_ct_add { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }
        public int japanese_level_number { get; set; }

        public bool read_only { get; set; }
        public JobSeekerUpdateProfileModel()
        {
            Extensions = new ExpandoObject();
            address = new IdentityJobSeekerAddress();
            address_contact = new IdentityJobSeekerAddress();
            JapaneseLevels = new List<IdentityJapaneseLevel>();
        }
    }

    public class JobSeekerAddressModel
    {
        public string postal_code { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string detail { get; set; }
        public string furigana { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }
    }

    public class JobSeekerEduHistoryModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
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

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
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

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
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

    #region Cv

    public class CvCreationMethodModel
    {
        public int cv_id { get; set; }
        public int form { get; set; }
        public int creation_method { get; set; }
    }

    public class CvSearchResultModel : CommonPagingModel
    {
        public List<CvInfoModel> CvList { get; set; }
    }

    public class CvInfoModel : IdentityCv
    {

    }

    public class CvUpdateModel : JobSeekerUpdateProfileModel
    {
        public int id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
        public string title { get; set; }

        public int form { get; set; }
        public string hobby_skills { get; set; }
        public string pr { get; set; }
        public string reason { get; set; }
        public string time_work { get; set; }
        public string aspiration { get; set; }
        public string created_date { get; set; }
        public int japanese_level_number { get; set; }

        public List<JobSeekerEduHistoryModel> EduHistories { get; set; }
        public List<JobSeekerWorkHistoryModel> WorkHistories { get; set; }
        public List<JobSeekerCertificateModel> Certificates { get; set; }

        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }

    }

    public class CvEduHistoryModel : JobSeekerEduHistoryModel
    {
        public int cv_id { get; set; }
    }

    public class CvWorkHistoryModel : JobSeekerWorkHistoryModel
    {
        public int cv_id { get; set; }
    }

    public class CvCertificateModel : JobSeekerCertificateModel
    {
        public int cv_id { get; set; }
    }

    public class CvDropDownItemModel
    {
        public int id { get; set; }
        public string cv_title { get; set; }
    }

    #endregion

    #region Cs

    public class CsCreationMethodModel
    {
        public int cs_id { get; set; }
        public int form { get; set; }
        public int creation_method { get; set; }
    }

    public class CsSearchResultModel : CommonPagingModel
    {
        public List<CsInfoModel> CsList { get; set; }
    }

    public class CsInfoModel : IdentityCs
    {

    }

    public class CsUpdateModel : JobSeekerUpdateProfileModel
    {
        public int id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_NULL_VALUE))]
        public string title { get; set; }
        public string created_date { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }

        public List<JobSeekerEduHistoryModel> EduHistories { get; set; }
        public List<CsWorkHistoryModel> WorkHistories { get; set; }
        public List<JobSeekerCertificateModel> Certificates { get; set; }

    }

    public class CsEduHistoryModel : JobSeekerEduHistoryModel
    {
        public int cs_id { get; set; }
    }

    public class CsWorkHistoryModel : JobSeekerWorkHistoryModel
    {
        public int idx { get; set; }
        public int cs_id { get; set; }
        public int sub_field_id { get; set; }
        public int sub_industry_id { get; set; }
        public int employment_type_id { get; set; }
        public int employees_number { get; set; }
        public string resign_reason { get; set; }

        public List<CsWorkHistoryDetailModel> Details { get; set; }

        //Extensions
        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityField> Fields { get; set; }
        public List<IdentityIndustry> Industries { get; set; }
        public string json_obj { get; set; }

        public CsWorkHistoryModel()
        {
            Details = new List<CsWorkHistoryDetailModel>();
        }
    }

    public class CsWorkHistoryDetailModel
    {
        public int id { get; set; }
        public string department { get; set; }
        public string position { get; set; }
        public int salary { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string content_work { get; set; }
    }

    public class CsCertificateModel : JobSeekerCertificateModel
    {
        public int cs_id { get; set; }
    }

    public class CsDropDownItemModel
    {
        public int id { get; set; }
        public string cs_title { get; set; }
    }

    #endregion

    public class InvitationActionModel
    {
        public int id { get; set; }
        public int job_id { get; set; }
    }

    public class JobSeekerNotificationModel
    {
        public IdentityNotification NotifInfo { get; set; }
        public IdentityJob JobInfo { get; set; }
    }

    public class JobSeekerInviteFriendModel
    {
        public int job_seeker_id { get; set; }
        public string token { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_EMAIL_NULL))]
        [EmailAddress(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.ERROR_EMAIL_INVALID))]
        public string email { get; set; }
        public string note { get; set; }
    }

    public class JobSeekerInviteResendModel
    {
        public int invite_id { get; set; }
        public string token { get; set; }
        public int job_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.COMMON_ERROR_EMAIL_NULL))]
        [EmailAddress(ErrorMessageResourceType = typeof(UserWebResource), ErrorMessageResourceName = nameof(UserWebResource.ERROR_EMAIL_INVALID))]
        public string email { get; set; }
        public string note { get; set; }
    }

    public class JobSeekerWishModel
    {
        public int job_seeker_id { get; set; }
        public List<int> employment_type_ids { get; set; }
        public List<int> prefecture_ids { get; set; }
        public List<int> sub_field_ids { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public string start_date { get; set; }
        public int exclude_ct_add { get; set; }
        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityField> Fields { get; set; }
        public List<IdentityRegion> Regions { get; set; }
    }
}