using SingleSignOn.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SingleSignOn.Models
{
    public class ApiFilterModel
    {        
        public string Keyword { get; set; }

        [Range(1, 50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LIMIT_PAGE_INDEX))]
        public int PageIndex { get; set; }

        [Range(1, 50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LIMIT_PAGE_SIZE))]
        public int PageSize { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_USERID_NULL))]
        public int UserId { get; set; }

        //[Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public string TokenKey { get; set; }

        public int OwnerId { get; set; }
    }

    public class ApiPageModel
    {
        [Range(1, 50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LIMIT_PAGE_INDEX))]
        public int PageIndex { get; set; }

        [Range(1, 50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LIMIT_PAGE_SIZE))]
        public int PageSize { get; set; }
    }
}