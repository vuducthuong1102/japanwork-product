using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Models
{
    public class ManageAgencyModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityAgency> SearchResults { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CODE))]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }        
    }

    public class AgencyCommonInfoModel
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

    public class AgencyCreateModel : AgencyCommonInfoModel
    {

    }

    public class AgencyEditModel : AgencyCommonInfoModel
    {
        public int Id { get; set; }
    }
}