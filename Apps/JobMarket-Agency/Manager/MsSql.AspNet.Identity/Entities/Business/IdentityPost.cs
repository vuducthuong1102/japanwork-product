using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityPost : CommonIdentity
    {
        public int Id { get; set; }       

        public bool IsHighlights { get; set; }

        public string Cover { get; set; }

        public int CategoryId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int Status { get; set; }

        //Extends
        public string UrlFriendly { get; set; }
        public string CreatedDateLabel { get; set; }
        public string CategoryLabel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BodyContent { get; set; }
        public string LangCode { get; set; }

        public List<IdentityPostLang> MyLanguages { get; set; }

        public IdentityPost()
        {
            MyLanguages = new List<IdentityPostLang>();
        }
    }    

    public class IdentityComment : IdentityMemberInfo
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

    public class IdentityCommentReply : IdentityMemberInfo
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

    public class IdentityPostAction: IdentityMemberInfo
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


    public class IdentityPostLang
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BodyContent { get; set; }
        public string LangCode { get; set; }
        public string UrlFriendly { get; set; }
    }
}
