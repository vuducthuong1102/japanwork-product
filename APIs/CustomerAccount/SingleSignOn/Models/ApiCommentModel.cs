using SingleSignOn.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SingleSignOn.Models
{
    public class ApiInsertCommentModel : UserItemModel
    {
        public int PostId { get; set; }

        public string Content { get; set; }

        //public string Images { get; set; }
    }

    public class ApiUpdateCommentModel: UserItemModel
    {
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NUMBER_INT))]
        public long Id { get; set; }

        public string Content { get; set; }

        //public string Images { get; set; }
    }
    public class ApiDeleteCommentModel : UserItemModel
    {
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NUMBER_INT))]
        public long Id { get; set; }
    }

    public class ApiCommentFilterModel:ApiPageModel
    {
        public int PostId { get; set; }

        public int UserId { get; set; }
    }
}