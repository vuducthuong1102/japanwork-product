using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageInvitationModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityInvitation> SearchResults { get; set; }

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

        public int job_id { get; set; }
        public int invite_id { get; set; }

        public string company_name { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
        public List<IdentityCompany> Companies { get; set; }
        public List<IdentityCv> CVs { get; set; }
        public List<IdentityJobSeeker> JobSeekers { get; set; }
    }    
}