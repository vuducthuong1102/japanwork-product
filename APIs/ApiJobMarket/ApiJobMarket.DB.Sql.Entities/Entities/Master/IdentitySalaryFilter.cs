using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentitySalaryFilter
    {
        public int id { get; set; }
        public int type { get; set; }
        public int min { get; set; }
        public int max { get; set; }

        public int order { get; set; }

        public int total_count { get; set; }
    }
}
