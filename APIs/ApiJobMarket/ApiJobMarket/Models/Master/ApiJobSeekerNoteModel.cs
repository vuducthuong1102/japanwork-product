using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Models.Master
{
    public class ApiJobSeekerNoteUpdateModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
        public int staff_id { get; set; }
        public int type { get; set; }
        public string note { get; set; }
        public int type_job_seeker { get; set; }
        public int agency_id { get; set; }
    }
    public class ApiJobSeekerNoteModel : ApiGetListByPageModel
    {
        public int type_job_seeker { get; set; }
        public int staff_id { get; set; }
        public int agency_id { get; set; }
    }
}