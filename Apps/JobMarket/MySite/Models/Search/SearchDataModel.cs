using SingleSignOn.DB.Sql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class SearchDataModel
    {
        public List<IdentityUser> ListUser { get; set; }

        public bool IsSearchAll { get; set; }
    }
}