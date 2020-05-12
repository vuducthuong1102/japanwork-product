using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityStatisticsFilter
    {
        public int agency_id { get; set; }
        public int year { get; set; }
    }

    public class IdentityStatisticsCommon
    {
        public int agency_id { get; set; }        
    }

    public class IdentityStatisticsJobByYear
    {
        public int id { get; set; }
        public int agency_id { get; set; }
        public int year { get; set; }
        public int month_1 { get; set; }
        public int month_2 { get; set; }
        public int month_3 { get; set; }
        public int month_4 { get; set; }
        public int month_5 { get; set; }
        public int month_6 { get; set; }
        public int month_7 { get; set; }
        public int month_8 { get; set; }
        public int month_9 { get; set; }
        public int month_10 { get; set; }
        public int month_11 { get; set; }
        public int month_12 { get; set; }
        public DateTime? last_cal_time { get; set; }
    }

    public class IdentityStatisticsJobByMonth
    {

    }  
}
