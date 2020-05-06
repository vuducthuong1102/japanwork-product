using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageApplicationModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityApplication> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_COMPANY))]
        public int company_id { get; set; }

        public string company_name { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
        public List<IdentityCompany> Companies { get; set; }
        //public List<SelectListItem> ListCompanies { get; set; }
        public List<IdentityIndustry> Industries { get; set; }
        public List<IdentityCv> CVs { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }

        public int job_id { get; set; }
        public int cv_id { get; set; }
        public int agency_id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SOURCE_CANDIDATE))]
        public int type_job_seeker { get; set; }
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

    public class ApplicationIgnoreModel
    {
        public int id { get; set; }
        public int cv_id { get; set; }
    }

    public class ApplicationAcceptModel
    {
        public int id { get; set; }
        public int cv_id { get; set; }
        public int pic_id { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> ListStaffs { get; set; }
    }
}