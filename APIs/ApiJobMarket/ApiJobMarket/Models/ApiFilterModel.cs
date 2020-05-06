using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Models
{
    public class ApiFilterModel : ApiCommonModel
    {        
        public dynamic Extensions { get; set; }

        public ApiFilterModel()
        {
            Extensions = new ExpandoObject();
        }
    }    

    public class ApiPageModel
    {
        [Range(1, 50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LIMIT_PAGE_INDEX))]
        public int PageIndex { get; set; }

        [Range(1, 50, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_LIMIT_PAGE_SIZE))]
        public int PageSize { get; set; }
    }
}