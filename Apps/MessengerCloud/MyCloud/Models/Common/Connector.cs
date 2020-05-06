using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCloud.Models
{
    public class Connector : Member
    {
        //public int agency_id { get; set; }

        public List<ConnectionInfo> Connections { get; set; }

        public bool has_logout { get { return false; } set { } }

        public Connector()
        {
            Connections = new List<ConnectionInfo>();
        }
    }

    public class Member
    {
        public int id { get; set; }
        public string user_name { get; set; }

        //public string DisplayName { get; set; }
        //public string Avatar { get; set; }

        public string ConnectionId { get; set; }
        public DateTime last_connected { get; set; }
        public string last_ip_connected { get; set; }

        public string device_name { get; set; }
        public string device_id { get; set; }
        public int device_type { get; set; }
        public string registration_id { get; set; }
        public string language_code { get; set; }

        /// <summary>
        /// Type of user
        /// </summary>
        public int t { get; set; }
    }

    public class ConnectionInfo
    {
        public string ConnectionId { get; set; }
        public DateTime connected_time { get; set; }
        public string ip { get; set; }

        //public string device_name { get; set; }
        public string device_id { get; set; }
        public string registration_id { get; set; }
        public int device_type { get; set; }
    }

    public class Device
    {
        public int id { get; set; }
        public string device_name { get; set; }
        public string device_id { get; set; }
        public string registration_id { get; set; }
        public int device_type { get; set; }
        public string language_code { get; set; }
        public DateTime? created_date { get; set; }
        public int status { get; set; }
    }

    public class JobSeekerDevice : Device
    {
        public int job_seeker_id { get; set; }
    }
}