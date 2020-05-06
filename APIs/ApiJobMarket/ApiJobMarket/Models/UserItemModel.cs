using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Models
{
    public class UserItemModel
    {
        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string TokenKey { get; set; }
    }

    public class ApiFollowUserModel
    {
        public int OwnerId { get; set; }

        public int UserId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}