using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class ManageInterviewProcessModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityCandidate> SearchResults { get; set; }

        public List<IdentityCv> CVs { get; set; }

        public List<IdentityInterviewProcess> ListInterviewProcess { get; set; }

        public List<IdentityCompany> Companies { get; set; }

        public List<IdentitySubField> SubFields { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public List<IdentityProcessStatus> ProcessStatuses { get; set; }

        public int status_id { get; set; }

        public int job_id { get; set; }
        public int agency_id { get; set; }

        public int staff_id { get; set; }

        public int pic_id { get; set; }
        public int interview_status_id { get; set; }
        public int pic_job_id { get; set; }


        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_COMPANY))]
        public int company_id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SOURCE_CANDIDATE))]
        public int type_job_seeker { get; set; }
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_JAPANESE_LEVEL))]
        public int japanese_level { get; set; }

        public List<SelectListItem> ListCompanies { get; set; }
        public List<IdentityJapaneseLevel> JapaneseLevels { get; set; }

        public int type { get; set; }
        public int sub_id { get; set; }
    }

    public class InterviewProcessCommonInfoModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        public string Avatar { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }
    }

    public class InterviewProcessCreateModel : InterviewProcessInfoModel
    {

    }

    public class InterviewProcessEditModel : InterviewProcessInfoModel
    {
        public int Id { get; set; }
    }
    public class InterviewProcessInsertModel : InterviewProcessInfoModel
    {
        public InterviewProcessInsertModel()
        {
            ListInterviewProcess = new List<IdentityInterviewProcess>();
            JobSeekers = new List<IdentityJobSeeker>();
            Companies = new List<IdentityCompany>();
            ListCompanies = new List<SelectListItem>();
            SearchResults = new List<IdentityJob>();
        }
        public List<IdentityInterviewProcess> ListInterviewProcess { get; set; }
        public List<IdentityJobSeeker> JobSeekers { get; set; }
        public List<IdentityCompany> Companies { get; set; }

        public List<IdentitySubField> SubFields { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }

        public List<SelectListItem> ListCompanies { get; set; }
        public List<IdentityJob> SearchResults { get; set; }
        public string[] list_ids { get; set; }
    }
    public class ApiInterviewProcessDeleteModel
    {
        public int id { get; set; }
    }

    public class InterviewProcessInfoModel : CommonPagingModel
    {
        public string note { get; set; }
        public int candidate_id { get; set; }
        public int job_seeker_id { get; set; }
        public int job_id { get; set; }
        public int cv_id { get; set; }
        public int agency_id { get; set; }
        public int status_id { get; set; }
        public string created_at { get; set; }
        public string modified_at { get; set; }
        public DateTime? modified_at_time { get; set; }

        public string NameStatus { get; set; }
        public string full_name { get; set; }
        public string company_name { get; set; }

        public string job_name { get; set; }
        public IdentityJobSeeker job_seeker_info { get; set; }
        public int type_job_seeker { get; set; }
        public int company_id { get; set; }
        public int staff_id { get; set; }

        public string list_job_seeker_ids { get; set; }
        public List<IdentityProcessStatus> ProcessStatuses { get; set; }
    }
}