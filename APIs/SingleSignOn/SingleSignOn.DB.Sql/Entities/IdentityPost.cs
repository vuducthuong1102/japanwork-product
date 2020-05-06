using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOn.DB.Sql.Entities
{
    public class IdentityPost: IdentityUserInfo
    {
        public int Id { get; set; }

        public int Days { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public int Like_Count { get; set; }

        public int Share_Count { get; set; }

        public int Comment_Count { get; set; }

        public int Report_Count { get; set; }

        public int Rating_Count { get; set; }

        public int Saved_Count { get; set; }

        public decimal RatingScore { get; set; }

        public int? CategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public string Locations { get; set; }

        public string Images { get; set; }

        //Extends
        public string CategoryName { get; set; }

        public bool IsLike { get; set; }

        public decimal UserRating { get; set; }

        public int TotalImages { get; set; }

        public int TotalLocations { get; set; }
    }

    public class IdentityImage
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsCover { get; set; }
    }

    public class IdentityComment : IdentityUserInfo
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public int PostId { get; set; }

        public string Images { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public int Like_Count { get; set; }

        public int Comment_Count { get; set; }
    }

    public class IdentityCommentFilter : IdentityFilter
    {
        public int PostId { get; set; }
    }

    public class IdentityCommentReply : IdentityUserInfo
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public long PostCommentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public int Like_Count { get; set; }

    }

    public class IdentityCommentReplyFilter : IdentityFilter
    {
        public int CommnentId { get; set; }

    }

    public class IdentityPostAction: IdentityUserInfo
    {
        public long Id { get; set; }

        public string ActionType { get; set; }

        public int RatingScore { get; set; }

        public int PostId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

    }

    public class IdentityPostData
    {

        public List<IdentityImage> Images { get; set; }

        public List<IdentityLocation> Locations { get; set; }

        public int PostId { get; set; }
    }

    public class IdentityPostDetail 
    {
        public IdentityPost Post { get; set; }

        public IdentityPostData PostData { get; set; }

        public List<IdentityComment> PostComment { get; set; }

        //public List<IdentityCommentReply> CommentReply { get; set; }
    }

}
