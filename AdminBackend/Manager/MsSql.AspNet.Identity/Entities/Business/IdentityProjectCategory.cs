using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityProjectCategory : IdentityCommonCategory
    {
        //Extends
        public string Cover { get; set; }
        public int ParentId { get; set; }
        public string UrlFriendly { get; set; }
        public string Description { get; set; }       
        public string LangCode { get; set; }

        public bool HasProject { get; set; }

        public List<IdentityProjectCategoryLang> MyLanguages { get; set; }

        public IdentityProjectCategory()
        {
            MyLanguages = new List<IdentityProjectCategoryLang>();
        }
    }

    public class IdentityProjectCategoryLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlFriendly { get; set; }
        public int ProjectCategoryId { get; set; }
    }
}
