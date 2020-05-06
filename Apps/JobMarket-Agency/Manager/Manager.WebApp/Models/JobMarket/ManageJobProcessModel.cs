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
    public class ManageJobProcessModel : CommonPagingModel
    {

        public int status_id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SOURCE_CANDIDATE))]
        public int type_job_seeker { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_JOB_FIELD))]
        public int sub_id { get; set; }
        public int agency_id { get; set; }
        public int staff_id { get; set; }
        public List<SelectListItem> ListCompanies { get; set; }
        public List<IdentitySubField> SubFields { get; set; }

        public List<IdentityProcessStatus> ProcessStatuses { get; set; }
        public int type { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }

    }
}