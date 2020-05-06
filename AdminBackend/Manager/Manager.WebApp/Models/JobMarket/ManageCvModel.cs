using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;

namespace Manager.WebApp.Models
{
    public class ManageJobSeekerCvModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityCv> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public int JobSeekerId { get; set; }
    }

    public class ManageCvPreviewModel
    {
        public IdentityCv CvInfo { get; set; }

        //Extensions
        public string qualification_label { get; set; }

        public ManageCvPreviewAddressModel address { get; set; }
        public ManageCvPreviewAddressModel address_contact { get; set; }
    }

    public class ManageCvPreviewAddressModel
    {
        public IdentityAddress AddressInfo { get; set; }
        public string country_name { get; set; }
        public List<IdentityStation> Stations { get; set; }
    }

    public class CvEduHistoryModel : JobSeekerEduHistoryModel
    {
        public int cv_id { get; set; }
    }

    public class CvWorkHistoryModel : JobSeekerWorkHistoryModel
    {
        public int cv_id { get; set; }
    }

    public class CvCertificateModel : JobSeekerCertificateModel
    {
        public int cv_id { get; set; }
    }
}