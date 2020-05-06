using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class HomeModel
    {
        public List<IdentityProjectCategory> ProjectCategories { get; set; }
    }
}