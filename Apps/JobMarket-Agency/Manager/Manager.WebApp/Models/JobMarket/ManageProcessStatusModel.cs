using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.Entities;
using static Manager.WebApp.LanguagesProvider;

namespace Manager.WebApp.Models
{
    public class ManageProcessStatusModel : CommonPagingModel
    {

        public List<IdentityProcessStatus> SearchResults { get; set; }
    }
    public class ProcessStatusEditModel
    {
        public int id { get; set; }
        public string status_name { get; set; }
        public bool status { get; set; }
        public int agency_id { get; set; }
        public string description { get; set; }
        public int order { get; set; }
    }
}