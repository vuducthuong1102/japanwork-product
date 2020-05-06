﻿using Manager.WebApp.Resources;
using MsSql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Manager.WebApp.LanguagesProvider;

namespace Manager.WebApp.Models.System
{
    public class ManageAccessLangModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        public string Description { get; set; }

        public string LangCode { get; set; }

        public string AccessId { get; set; }

        public bool IsUpdate { get; set; }
        public List<Languages> Languages { get; set; }

        public IdentityAccess AccessInfo { get; set; }

        public ManageAccessLangModel()
        {
            Languages = new List<Languages>();
            AccessInfo = new IdentityAccess();
        }
    }
}