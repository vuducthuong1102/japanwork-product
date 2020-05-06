using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ApiTypeSuggestEditModel : ApiTypeSuggestModel
    {
        public int id { get; set; }
    }
    public class ApiTypeSuggestSearchModel : ApiCommonModel
    {

    }
    public class ApiTypeSuggestInsertModel : ApiTypeSuggestModel
    {
    }
    public class ApiTypeSuggestModel
    {
        public string type { get; set; }
        public int form_id { get; set; }

        public string description { get; set; }
        public string icon { get; set; }
    }
}