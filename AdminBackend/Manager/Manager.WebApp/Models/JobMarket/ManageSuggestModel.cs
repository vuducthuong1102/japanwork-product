using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageSuggestModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentitySuggest> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PROJECT_CATEGORY))]
        public int? CategoryId { get; set; }
    }

    public class SuggestDetailsModel
    {
        public IdentitySuggest SuggestInfo { get; set; }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
    }
    public class SuggestEditModel
    {
        public int id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string title { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONTENT))]
        public string content { get; set; }

        public List<IdentityTypeSuggest> ListTypeSuggests { get; set; }

        public int type { get; set; }

        public int form { get; set; }

        public int Status { get; set; }
    }
}