using MsSql.AspNet.Identity.Entities;
using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    public class ArticleModel
    {
        public IdentityPost PostInfo { get; set; }
    }

    public class RelatedArticlesModel
    {
        public List<IdentityPost> Articles { get; set; }
        public string LangCode { get; set; }
    }

    public class GroupArticlesModel
    {
        public List<IdentityPost> Articles { get; set; }
        public string LangCode { get; set; }
    }

    public class ArticleSearchModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";

        public List<IdentityPost> SearchResults { get; set; }
    }
}