using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCloud.Models
{
    public class ApiUserModel
    {
        public int UserId { get; set; }
    }

    public class CheckUserOnlineModel
    {
        public string ListUsers { get; set; }
    }

    public class UserOnlineCheckListModel
    {
        public List<UserOnlineInfoModel> UsersList { get; set; }

        public UserOnlineCheckListModel()
        {
            UsersList = new List<UserOnlineInfoModel>();
        }
    }

    public class UserOnlineInfoModel
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
    }

    public class ApiJobSeekerDeviceModel
    {
        public int job_seeker_id { get; set; }
        public string device_name { get; set; }
        public string device_id { get; set; }
        public string registration_id { get; set; }
        public int device_type { get; set; }
        public string language_code { get; set; }
    }
}