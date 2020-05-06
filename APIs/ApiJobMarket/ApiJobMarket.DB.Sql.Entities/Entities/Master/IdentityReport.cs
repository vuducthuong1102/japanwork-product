using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityReport : IdentityCommon
    {

    }

    public class IdentityReportFilter
    {
        public int AgencyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ListDays { get; set; }
    }

    public class IdentityReportByYear
    {
        public int CurrentMonth { get; set; }
        public int ProcessingCount { get; set; }
        public int PublicCount { get; set; }
        public int UnProcessedCount { get; set; }
    }

    public class IdentityReportByWeek
    {
        public int Idx { get; set; }
        public int CurrentDate { get; set; }
        public int ProcessingCount { get; set; }
        public int PublicCount { get; set; }
        public int UnProcessedCount { get; set; }
    }

    public class IdentityReportApplicationByWeek
    {
        public int Idx { get; set; }
        public int CurrentDate { get; set; }
        public int ApprovedCount { get; set; }
        public int WaitingCount { get; set; }
        public int IgnoredCount { get; set; }
    }
}
