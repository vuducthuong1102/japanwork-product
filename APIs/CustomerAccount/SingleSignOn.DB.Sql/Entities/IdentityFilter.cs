using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOn.DB.Sql.Entities
{
   public class IdentityFilter: IdentityUserInfo
    {
        public string Keyword { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

    }
}
