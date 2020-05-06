using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageProjectModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityProject> SearchResults { get; set; }

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

        public List<IdentityProjectCategory> Categories { get; set; }        
    }

    public class ProjectDetailsModel
    {
        public IdentityProject ProjectInfo { get; set; }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
    }

    public class ProjectEditModel
    {
        public int Id { get; set; }
        public string CurrentCover { get; set; }
        public string CurrentCoverFullPath { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_DESCRIPTION))]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_COVER_IMAGE))]
        public List<HttpPostedFileBase> Cover { get; set; }

        public int CategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_POST_HIGHLIGHTS))]
        public bool IsHighlights { get; set; }

        [AllowHtml]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONTENT))]
        public string BodyContent { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int Status { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_URL_FRIENDLY))]
        public string UrlFriendly { get; set; }

        public List<IdentityProjectImage> Images { get; set; }

        public List<IdentityProjectCategory> Categories { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_IN_CHARGE))]
        public string PersonInCharge { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_BEGIN))]
        public string BeginDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FINISH))]
        public string FinishDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FRAMEWORK))]
        public string FrameWork { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CUSTOMER))]
        public string Customer { get; set; }
    }
}