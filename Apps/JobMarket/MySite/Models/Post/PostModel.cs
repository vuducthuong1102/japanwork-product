using MySite.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MySite.Models
{
    public class PostModel
    {
        public string Title { get; set; }

        public int Days { get; set; }
        public int PostId { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public int UserId { get; set; }

        public string TokenKey { get; set; }

        public string Locations { get; set; }


        //To remove image when updating
        public string RemovingImages { get; set; }
    }

    public class PostActionModel
    {
        public int PostId { get; set; }

        public string ActionType { get; set; }

        public int RatingScore { get; set; }

        public int UserId { get; set; }

        public string TokenKey { get; set; }
    }
}