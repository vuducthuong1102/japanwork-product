using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageCandidateModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityCandidate> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_COMPANY))]
        public int company_id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SOURCE_CANDIDATE))]
        public int type_job_seeker { get; set; }

        public string company_name { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
        public List<IdentityCompany> Companies { get; set; }
        public List<IdentityCv> CVs { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> ListStaffs { get; set; }
        public List<SelectListItem> ListCompanies { get; set; }
    }
    public class ApiCandidateInsertModel
    {
        public int cv_id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int type { get; set; }
        public int company_id { get; set; }
        public int agency_parent_id { get; set; }
        public int agency_id { get; set; }
        public string list_job_seeker_ids { get; set; }

        public int staff_id { get; set; }

        public int pic_id { get; set; }
        public string list_job_ids { get; set; }
    }
    public class ApiCandidateDeleteModel
    {
        public int job_id { get; set; }
        public int agency_id { get; set; }
    }
}   