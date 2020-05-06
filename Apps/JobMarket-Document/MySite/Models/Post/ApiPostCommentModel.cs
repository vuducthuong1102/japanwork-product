using MySite.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class  ApiPostCommentModel
    {
        public int PostId { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public string TokenKey { get; set; }
    }
}