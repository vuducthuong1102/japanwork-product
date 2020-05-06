using Newtonsoft.Json;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SingleSignOn.Models
{
    public class ApiInsertPostModel : UserItemModel
    {
        public int Id { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NUMBER_INT))]
        public int Days { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public int? CategoryId { get; set; }

        public List<IdentityLocation> Locations { get; set; }

        public List<IdentityImage> Images { get; set; }

        public ICollection<HttpPostedFileBase> PostedFiles { get; set; }
    }
    public class ApiUpdatePostModel : UserItemModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public int Id { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NUMBER_INT))]
        public int Days { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public List<IdentityLocation> Locations { get; set; }

        public List<IdentityImage> Images { get; set; }
    }

    public class ApiDeletePostModel : UserItemModel
    {
        [Required(ErrorMessageResourceType = typeof(UserApiResource), ErrorMessageResourceName = nameof(UserApiResource.COMMON_ERROR_NULL_VALUE))]
        public int Id { get; set; }

        public int Status { get; set; }
    }

    public class ApiPostActionModel : UserItemModel
    {
        public int PostId { get; set; }

        public string ActionType { get; set; }

        public int RatingScore { get; set; }
    }

    public class ApiPostDetailModel
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}