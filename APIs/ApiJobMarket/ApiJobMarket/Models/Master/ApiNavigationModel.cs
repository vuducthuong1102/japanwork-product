using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
{
    public class ApiNavigationEditModel : ApiNavigationModel
    {
        public int Id { get; set; }
    }
    public class ApiNavigationInsertModel : ApiNavigationModel
    {
    }
    public class ApiNavigationModel
    {
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

        //public string language_code { get; set; }
    }

    public class ApiNavigationSearchModel : ApiCommonModel
    {

    }

    public class ApiNavigationLangEditModel : ApiNavigationLangModel
    {
        public int Id { get; set; }
    }
    public class ApiNavigationLangInsertModel : ApiNavigationLangModel
    {
    }
    public class ApiNavigationLangModel
    {
        public int NavigationId { get; set; }
        public string Title { get; set; }
        public string AbsoluteUri { get; set; }
        public string LangCode { get; set; }
    }
}