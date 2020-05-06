using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class LocationModel
    {
        public string name { get; set; }

        public string formatted_address { get; set; }

        public string place_id { get; set; }

        public string url { get; set; }
    }
}