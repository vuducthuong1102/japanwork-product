using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models
{
   
    public class ApiApplicationSendCvModel
    {
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int cv_id { get; set; }
    }
}