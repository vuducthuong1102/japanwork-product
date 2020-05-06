using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class ManagePageModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityPage> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }
    }

    public class PageCommonInfoModel
    {
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_DESCRIPTION))]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONTENT))]
        [AllowHtml]
        public string BodyContent { get; set; }

        public string UrlFriendly { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }
    }

    public class PageCreateModel : PageCommonInfoModel
    {

    }

    public class PageEditModel : PageCommonInfoModel
    {
        public int Id { get; set; }
    }
}