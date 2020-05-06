using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Entities
{
    [Serializable]
    public class IdentityNavigation : IdentityCommon
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Area { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public bool Visible { get; set; }
        public int Authenticate { get; set; }
        public string CssClass { get; set; }
        public int SortOrder { get; set; }
        public string AbsoluteUri { get; set; }
        public int Active { get; set; }
        public string IconCss { get; set; }

        public List<IdentityNavigation> SubNavigation { get; set; }

        public string FullDesc
        {
            get
            {
                string strFullDesc = Title;

                if (!string.IsNullOrEmpty(Desc))
                {
                    strFullDesc += " - " + Desc;
                }

                return strFullDesc;
            }
        }

        public bool HasChildren
        {
            get
            {
                return SubNavigation != null && SubNavigation.Any();
            }
        }

        public bool HasVisbleChildren
        {
            get
            {
                return SubNavigation != null && SubNavigation.Any(m => m.Visible);
            }
        }

        public List<IdentityNavigationLang> LangList { get; set; }

        public string CurrentTitleLang { get; set; }
        public string CurrentAbsoluteUriLang { get; set; }

        public IdentityNavigation()
        {
            LangList = new List<IdentityNavigationLang>();
        }
    }

    [Serializable]
    public class IdentityNavigationLang
    {
        public int Id { get; set; }
        public int NavigationId { get; set; }
        public string Title { get; set; }
        public string AbsoluteUri { get; set; }
        public string LangCode { get; set; }
    }
}
