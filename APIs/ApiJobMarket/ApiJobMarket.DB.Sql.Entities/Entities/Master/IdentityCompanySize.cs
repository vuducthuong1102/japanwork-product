using Newtonsoft.Json;
using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCompanySize : IdentityCommon
    {
        public int id { get; set; }
        public string size { get; set; }
        public int status { get; set; }
    }
}
