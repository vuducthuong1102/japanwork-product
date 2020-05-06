using SingleSignOn.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SingleSignOn.Models
{
    public class ApiInsertCommentReplyModel : UserItemModel
    {
        public long CommentPostId { get; set; }

        public string Content { get; set; }
    }

    public class ApiUpdateCommentReplyModel : UserItemModel
    {
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NUMBER_INT))]
        public long Id { get; set; }

        public string Content { get; set; }

    }
    public class ApiDeleteCommentReplyModel : UserItemModel
    {
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NUMBER_INT))]
        public long Id { get; set; }
    }

    public class ApiCommentReplyFilterModel : ApiPageModel
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }
    }
}