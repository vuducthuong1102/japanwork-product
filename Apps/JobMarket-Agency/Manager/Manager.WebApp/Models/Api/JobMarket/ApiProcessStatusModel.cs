using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models.Api
{
    public class ApiProcessStatusEditModel : ApiProcessStatusModel
    {
        public int id { get; set; }
    }
    public class ApiProcessStatusInsertModel : ApiProcessStatusModel
    {
    }
    public class ApiProcessStatusModel
    {
        public string status_name { get; set; }
        public int status { get; set; }
        public int order { get; set; }
        public string description { get; set; }
        public int agency_id { get; set; }
        public int id { get; set; }
    }

    public class ApiProcessStatusSearchModel : ApiCommonModel
    {

    }
    public class ApiProcessStatusDeleteModel
    {
        public int id { get; set; }
    }
}