using System;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityDevice : IdentityCommon
    {
        public int id { get; set; }
        public string device_name { get; set; }
        public string device_id { get; set; }
        public string registration_id { get; set; }
        public int device_type { get; set; }
        public DateTime? created_at { get; set; }
        public int status { get; set; }
        public DateTime? last_connected { get; set; }        
    }
}
