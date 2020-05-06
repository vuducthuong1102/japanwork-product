﻿using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SelectItem = System.Web.Mvc.SelectListItem;

namespace Manager.WebApp.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
    
        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        //[RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NUMBER_INPUT_ONLY))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_USER_CODE))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FULL_NAME))]
        public string FullName { get; set; }

        //[Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PHONE))]
        //[RegularExpression(@"\d{9,13}", ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_PHONE_INVALID))]
        //public string PhoneNumber { get; set; }

        //[Required(AllowEmptyStrings = false)]
        //[Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_EMAIL))]
        //public string Email { get; set; }

        public IEnumerable<SelectItem> RolesList { get; set; }

        public List<SelectItem> allCompanies { get; set; }
        public List<int> selectCompany { get; set; }

        public string Role { get; set; }
        //Search history
        public string SEmail { get; set; }
        public string SRoleId { get; set; }
        public string SearchExec { get; set; }
        public string Page { get; set; }
        public int SIsLocked { get; set; }

        public ApplicationUser User { get; set; }
        public LockoutViewModel Lockout { get; set; }

        public bool IsActived { get; set; }

        public List<IdentityCompany> Companies { get; set; }

        //[Display(Name = "Đại lý")]
        //public int ProviderId { get; set; }
        //public List<IdentityProvider> ListProvider { get; set; }
    }

    public class UpdateUserProfileModel
    {
        public string UserId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}