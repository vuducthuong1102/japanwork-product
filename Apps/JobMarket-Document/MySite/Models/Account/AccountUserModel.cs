using MySite.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySite.Models.Account
{
    public class AccountUserModel
    {

    }

    public class ExternalLoginConfirmationViewModel
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Phone { get; set; }

        public string SocialProvider { get; set; }
        public string SocialToken { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class ApiAuthLoginWithModel
    {
        public string UserName { get; set; }
        /// <summary>
        /// Social is string
        /// </summary>
        /// 
        public string Email { get; set; }

        public string SocialProvider { get; set; }

        public string DisplayName { get; set; }        
    }
}