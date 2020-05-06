using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Models.Master
{
    public class ApiCompanyNoteUpdateModel
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int staff_id { get; set; }
        public int agency_id { get; set; }
        public int type { get; set; }
        public string note { get; set; }
    }
    public class ApiCompanyNoteModel : ApiGetListByPageModel
    {
    }
}