using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models.Api.JobMarket.Agency
{
    public class ApiAgencyReportModel
    {
        public int agency_id { get; set; }
    }

    public class ApiAgencyReportInYearModel : ApiAgencyReportModel
    {
        public int year { get; set; }
    }    
}