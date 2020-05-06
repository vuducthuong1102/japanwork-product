using SingleSignOn.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SingleSignOn.Models
{
    public class UserItemModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string TokenKey { get; set; }
    }
}