using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
{
    public class ApiTypeSuggestEditModel : ApiTypeSuggestItem
    {
        public int id { get; set; }
    }
    public class ApiTypeSuggestInsertModel : ApiTypeSuggestItem
    {
    }
    public class ApiTypeSuggestItem
    {
        public string type { get; set; }
        public int form_id { get; set; }

        public string description { get; set; }
        public string icon { get; set; }
    }
}