using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityReport : CommonIdentity
    {
        
    }

    public class ReportFilter
    {
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

    public class IdentityReportInWeek
    {
        public int Idx { get; set; }
        public int CurrentDate { get; set; }
        public int ProcessingCount { get; set; }
        public int PublicCount { get; set; }
        public int UnProcessedCount { get; set; }
    }
}
