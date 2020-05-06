using Manager.WebApp.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Manager.WebApp.Settings
{
    public class GeneralFrontEndSettings : FrontEndSettingsBase
    {
        [Required]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE), Description = nameof(ManagerResource.LB_TITLE))]
        [AllowHtml]
        public string SiteName { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_LOGO), Description = nameof(ManagerResource.LB_LOGO_DES))]
        public string SiteLogo { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EMBED_SCRIPTS), Description = nameof(ManagerResource.LB_EMBED_SCRIPTS_DES))]
        [AllowHtml]
        public string EmbeddedScripts { get; set; }
    }
}
