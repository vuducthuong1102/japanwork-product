using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ApiSuggestEditModel : ApiSuggestModel
    {
        public int id { get; set; }
    }

    public class ApiSuggestSearchModel : ApiCommonModel
    {

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
    }
}