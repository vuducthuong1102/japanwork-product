using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
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
        public int agency_id { get; set; }
        public string description { get; set; }

    }

    public class ApiProcessStatusSearchModel : ApiCommonModel
    {

    }
}