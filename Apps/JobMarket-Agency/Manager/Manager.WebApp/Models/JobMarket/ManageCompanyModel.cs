using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;

namespace Manager.WebApp.Models
{
    public class ManageCompanyModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityCompany> SearchResults { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string ToDate { get; set; }

        //For filtering
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_NAME))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_STATUS))]
        public int? Status { get; set; }

        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }
        public List<IdentityIndustry> Industries { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }
        public List<IdentityCity> Citys { get; set; }
        public List<IdentityRegion> Regions { get; set; }
        public int prefecture_id { get; set; }
        public int sub_industry_id { get; set; }
        public int agency_id { get; set; }

        public int staff_id { get; set; }

     
        public bool ishiring { get; set; }
    }
    public class CompanyDetailsModel
    {
        public IdentityCompany CompanyInfo { get; set; }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
    }

    public class ManageCompanyNoteModel : CommonPagingModel
    {
        public int company_id { get; set; }
        public int type { get; set; }
        public string note { get; set; }
        public int staff_id { get; set; }
        public List<IdentityCompanyNote> SearchResults { get; set; }
        public List<MsSql.AspNet.Identity.IdentityUser> ListStaffs { get; set; }
    }

    public class CompanyNoteDeleteModel
    {
        public int company_id { get; set; }
        public int id { get; set; }
    }
    public class ApiCompanyNoteUpdateModel
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int staff_id { get; set; }
        public int agency_id { get; set; }
        public int type { get; set; }
        public string note { get; set; }
    }

    public class ApiCompanyNoteModel : ApiGetListByPageModel
    {
    }
    public class CompanyEditModel
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string logo_full_path { get; set; }

        public HttpPostedFileBase image_file_upload { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_COMPANY_NAME))]
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string company_name { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_DESCRIPTION))]
        public string description { get; set; }

        [MaxLength(30, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_EMAIL_MAX_LENGTH))]
        public string email { get; set; }
        public string website { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_PHONE_MAX_LENGTH))]
        public string phone { get; set; }
        public string fax { get; set; }
        public string address_detail { get; set; }
        public string address_furigana { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int staff_id { get; set; }
        public int agency_id { get; set; }
        public int company_size_id { get; set; }
        public int sub_industry_id { get; set; }
        public DateTime? created_at { get; set; }

        [MinLength(4, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_ESTABLISH_YEAR_MUST_NUMBERIC))]
        [MaxLength(4, ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_ESTABLISH_YEAR_MUST_NUMBERIC))]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.COMMON_ERROR_ESTABLISH_YEAR_MUST_NUMBERIC))]
        public string establish_year { get; set; }
        public int pic_id { get; set; }

        public string address_full { get; set; }
        public CompanyAddressModel Address { get; set; }

        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityIndustry> Industries { get; set; }
        public List<IdentityCompanySize> CompanySizes { get; set; }
        
        public CompanyEditModel()
        {
            Address = new CompanyAddressModel();
        }
    }

    public class CompanyAddressModel
    {        
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
    }

    public class CompanyDeleteModel
    {
        public string Ids { get; set; }
        public string tk { get; set; }
    }

    public class CompanyClearDataModel : CommonPagingModel
    {
        public string tk { get; set; }
        public int company_id { get; set; }
        public IdentityCompany CompanyIno { get; set; }
        public IdentityCompanyCounter Counter { get; set; }

        public List<IdentityJob> Jobs { get; set; }
        public List<IdentityCompany> Companies { get; set; }

        public List<MsSql.AspNet.Identity.IdentityUser> Staffs { get; set; }
        public List<IdentitySubField> SubFields { get; set; }
        public int agency_id { get; set; }
        public CompanyClearDataModel()
        {
            Counter = new IdentityCompanyCounter();
        }
    }

    public class AssignStaffToCompanyModel
    {
        public int staff_id { get; set; }
        public List<SelectListItem> allCompanies { get; set; }
        public List<int> selectCompany { get; set; }
    }
}