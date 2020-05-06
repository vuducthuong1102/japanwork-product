using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageTypeSuggestModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityTypeSuggest> SearchResults { get; set; }


        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
        public int? Type { get; set; }
    }

    public class TypeSuggestDetailsModel
    {
        public IdentityTypeSuggest TypeSuggestInfo { get; set; }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
    }
    public class TypeSuggestEditModel
    {
        public int id { get; set; }

        public string type { get; set; }

        public string description { get; set; }

        public string icon { get; set; }
             
        public List<IdentityEmploymentType> EmploymentTypes { get; set; }

        public string key { get; set; }

        public int form_id { get; set; }
    }
}