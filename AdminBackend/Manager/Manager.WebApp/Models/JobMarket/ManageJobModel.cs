using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;
using static Manager.WebApp.LanguagesProvider;

namespace Manager.WebApp.Models
{
    public class ManageJobModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityJob> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TRANSLATE_STATUS))]
        public int? TranslateStatus { get; set; }
        public int company_id { get; set; }
        public string company_name { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
        public List<IdentityCompany> Companies { get; set; }
        public List<IdentityIndustry> Industries { get; set; }
    }

    public class JobDetailsModel
    {
        public IdentityJob JobInfo { get; set; }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
    }
    public class JobEditModel
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int quantity { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int salary_type_id { get; set; }
        public string work_start_time { get; set; }
        public string work_end_time { get; set; }
        public int probation_duration { get; set; }
        public int employment_type_id { get; set; }
        public bool flexible_time { get; set; }
        public string language_level { get; set; }
        public bool work_experience_doc_required { get; set; }
        public int duration { get; set; }
        public bool view_company { get; set; }
        public int qualification_id { get; set; }
        public int japanese_level_number { get; set; }
        public int sub_field_id { get; set; }
        public int sub_industry_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string closed_time { get; set; }
        [AllowHtml]
        public string subsidy { get; set; }

        [AllowHtml]
        public string paid_holiday { get; set; }

        [AllowHtml]
        public string bonus { get; set; }

        [AllowHtml]
        public string certificate { get; set; }

        [AllowHtml]
        public string work_content { get; set; }

        [AllowHtml]
        public string requirement { get; set; }

        [AllowHtml]
        public string plus { get; set; }

        [AllowHtml]
        public string welfare { get; set; }

        [AllowHtml]
        public string training { get; set; }

        [AllowHtml]
        public string recruitment_procedure { get; set; }

        [AllowHtml]
        public string remark { get; set; }

        public List<IdentityJobTag> SelectedTags { get; set; }
        public List<int> TagIds { get; set; }

        public List<JobUpdateAddressModel> Addresses { get; set; }
        public List<JobUpdateTranslationModel> Job_translations { get; set; }
        public List<JobUpdateTagModel> Tags { get; set; }

        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityRegion> Regions { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public List<IdentityCity> Cities { get; set; }
        public List<IdentityStation> Stations { get; set; }

        public List<IdentityJobAddress> SelectedAddresses { get; set; }
        public IdentityCompany CompanyInfo { get; set; }

        public List<IdentityIndustry> Industries { get; set; }
        public List<IdentityField> Fields { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }
        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityQualification> Qualifications { get; set; }
    }

    public class JobUpdateAddressModel
    {
        public int id { get; set; }
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string furigana { get; set; }
        public string detail { get; set; }

        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityRegion> Regions { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public List<IdentityCity> Cities { get; set; }

        public List<int> StationsIds { get; set; }
        public List<JobUpdateStationModel> Stations { get; set; }
    }
    public class JobEditLanguageModel
    {
        public int id { get; set; }
        public IdentityJob JobInfo { get; set; }
        public string language_translate { get; set; }
        public int company_id { get; set; }
        public int quantity { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int salary_type_id { get; set; }
        public string work_start_time { get; set; }
        public string work_end_time { get; set; }
        public int probation_duration { get; set; }
        public int employment_type_id { get; set; }
        public bool flexible_time { get; set; }
        public string language_level { get; set; }
        public bool work_experience_doc_required { get; set; }
        public int duration { get; set; }
        public bool view_company { get; set; }
        public int qualification_id { get; set; }
        public int japanese_level_number { get; set; }
        public int sub_field_id { get; set; }
        public int sub_industry_id { get; set; }

        public string to_language { get; set; }

        public string from_language { get; set; }
        public Languages Language { get; set; }

        public int translate_status { get; set; }
        public int filter_translate_status { get; set; }
        public int status { get; set; }


        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string title { get; set; }

        [AllowHtml]
        public string subsidy { get; set; }

        [AllowHtml]
        public string paid_holiday { get; set; }

        [AllowHtml]
        public string bonus { get; set; }

        [AllowHtml]
        public string certificate { get; set; }

        [AllowHtml]
        public string work_content { get; set; }

        [AllowHtml]
        public string requirement { get; set; }

        [AllowHtml]
        public string plus { get; set; }

        [AllowHtml]
        public string welfare { get; set; }

        [AllowHtml]
        public string training { get; set; }

        [AllowHtml]
        public string recruitment_procedure { get; set; }

        [AllowHtml]
        public string remark { get; set; }

        public List<IdentityJobTag> SelectedTags { get; set; }
        public List<int> TagIds { get; set; }

        public List<JobUpdateAddressModel> Addresses { get; set; }
        //public List<JobUpdateTranslationModel> Job_translations { get; set; }

        public List<IdentityJobTranslation> Job_translations { get; set; }
        public List<JobUpdateTagModel> Tags { get; set; }

        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityRegion> Regions { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public List<IdentityCity> Cities { get; set; }
        public List<IdentityStation> Stations { get; set; }

        public List<IdentityJobAddress> SelectedAddresses { get; set; }
        public IdentityCompany CompanyInfo { get; set; }

        public List<IdentityIndustry> Industries { get; set; }
        public List<IdentityField> Fields { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }
        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentityQualification> Qualifications { get; set; }
    }

    public class JobUpdateTranslationModel
    {
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string title { get; set; }

        public string subsidy { get; set; }

        public string paid_holiday { get; set; }

        public string bonus { get; set; }

        public string certificate { get; set; }

        public string work_content { get; set; }

        public string requirement { get; set; }

        public string plus { get; set; }

        public string welfare { get; set; }

        public string training { get; set; }

        public string recruitment_procedure { get; set; }

        public string remark { get; set; }

        public string language_code { get; set; }
    }

    public class JobUpdateStationModel
    {
        public int id { get; set; }
    }

    public class JobUpdateSubFieldModel
    {
        public int id { get; set; }
    }

    public class JobUpdateTagModel
    {
        public int id { get; set; }
        public string tag { get; set; }
    }
    public class JobUpdateStatusModel
    {
        public int id { get; set; }
        public int status { get; set; }
    }
}