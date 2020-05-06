using System.ComponentModel.DataAnnotations;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;
using System.Web.Mvc;

namespace Manager.WebApp.Models
{
    public class ManageFooterModel
    {
        public IdentityFooter FooterInfo { get; set; }

        //For filtering
        [AllowHtml]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_CONTENT))]
        public string BodyContent { get; set; }
    }    
}