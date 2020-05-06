using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityPage : CommonIdentity
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int PageTemplateId { get; set; }
        public dynamic CustomTemplate { get; set; }        
        public int SortOrder { get; set; }

        public IdentityPageTemplate TemplateInfo { get; set; }
        public List<IdentityWidget> Widgets { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public int Status { get; set; }

        

        //Extends
        public string Title { get; set; }
        public string Description { get; set; }
        public string BodyContent { get; set; }
        public string UrlFriendly { get; set; }
        public string LangCode { get; set; }
        public List<IdentityPageLang> MyLanguages { get; set; }

        public IdentityPage()
        {
            MyLanguages = new List<IdentityPageLang>();
            Widgets = new List<IdentityWidget>();
        }
    }

    public class IdentityPageLang
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BodyContent { get; set; }
        public string UrlFriendly { get; set; }
        public string LangCode { get; set; }
    }

    public class IdentityPageTemplate : CommonIdentity
    {
        public int Id { get; set; }
        public string Widgets { get; set; }
        public bool IsDefault { get; set; }
        public int Status { get; set; }
    }
}
