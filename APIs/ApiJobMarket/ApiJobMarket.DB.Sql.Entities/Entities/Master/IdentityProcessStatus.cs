using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityProcessStatus
    {
        public int id { get; set; }

        public string status_name { get; set; }

        public int status { get; set; }

        public int order { get; set; }
        public string description { get; set; }

        public int agency_id { get; set; }
    }
}
