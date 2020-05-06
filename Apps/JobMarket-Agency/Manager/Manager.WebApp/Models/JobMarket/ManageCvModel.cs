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
    public class ManageCvPreviewModel
    {
        public IdentityCv CvInfo { get; set; }

        //Extensions
        public string qualification_label { get; set; }
        public int type_job_seeker { get; set; }
        public int show_more { get; set; }
        public ManageCvPreviewAddressModel address { get; set; }
        public ManageCvPreviewAddressModel address_contact { get; set; }        
        public MsSql.AspNet.Identity.IdentityUser staff_info { get; set; }

        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityVisa> Visas { get; set; }
        public List<IdentityCv> Cvs { get; set; }
        public List<IdentityJobSeekerWish> WishList { get; set; }
        public List<IdentityPrefecture> Prefectures { get; set; }

        public List<IdentityEmploymentType> EmploymentTypes { get; set; }
        public List<IdentitySubField> SubFields { get; set; }
        public IdentityJobSeekerWish WishInfo { get; set; }
        public DateTime? created_at { get; set; }
        public int hide_pf { get; set; }
    }   
    
    public class ManageCvPreviewAddressModel
    {
        public IdentityAddress AddressInfo { get; set; }
        public string country_name { get; set; }
        public string address_full { get; set; }
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

    public class CvInviteModel
    {
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_SELECT_APPLICANT))]
        public string applicant { get; set; }

        public int job_id { get; set; }
        public List<CvInviteInfoModel> CvList { get; set; }
        public string note { get; set; }

        //Extends
        public string job_name { get; set; }
        public int invitation_limit { get; set; }
        public int invited_count { get; set; }
        public IdentityCompany CompanyInfo { get; set; }
    }

    public class CvInviteInfoModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string full_name { get; set; }
    }

    public class CvChoosenModel : CommonPagingModel
    {
        public int job_id { get; set; }
        public List<IdentityCv> SearchResults { get; set; }
        public List<IdentityJapaneseLevel> JapanseLevels { get; set; }
        public List<IdentityCountry> Countries { get; set; }
        public List<IdentityMajor> Majors { get; set; }
        public List<IdentityQualification> Qualifications { get; set; }
        public string CallbackFunction { get; set; }
    }

    public class CvItemInDropdownListModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public string fullname { get; set; }
        public string fullname_furigana { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public bool is_invited { get; set; }
    }
}