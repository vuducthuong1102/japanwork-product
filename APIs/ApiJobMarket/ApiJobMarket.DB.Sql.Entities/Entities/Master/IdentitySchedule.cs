using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentitySchedule : IdentityCommon
    {
        public int id { get; set; }
        public int agency_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int pic_id { get; set; }
        public int schedule_cat { get; set; }
        //Extends
    }
}
