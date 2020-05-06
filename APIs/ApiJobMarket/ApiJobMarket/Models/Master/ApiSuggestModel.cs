using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
{
    public class ApiSuggestEditModel : ApiSuggestModel
    {
        public int id { get; set; }
    }
    public class ApiSuggestInsertModel : ApiSuggestModel
    {
    }
    public class ApiSuggestModel
    {
        public string title { get; set; }
        public string content { get; set; }
        public int type { get; set; }
        public int form { get; set; }

        //public string language_code { get; set; }
    }

    public class ApiSuggestSearchModel : ApiCommonModel
    {

    }
}