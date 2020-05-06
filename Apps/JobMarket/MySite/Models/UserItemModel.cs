using MySite.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class UserItemModel
    {
        public int UserId { get; set; }

        public string TokenKey { get; set; }
    }

    public class ApiFollowUserModel
    {
        public int OwnerId { get; set; }

        public int UserId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}