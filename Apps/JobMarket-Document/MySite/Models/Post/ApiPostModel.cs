//using Newtonsoft.Json;
//using MySite.Resources;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Web;

//namespace MySite.Models
//{
//    public class ApiInputPostDataModel : CommonInputModel
//    {
//        public int PostId { get; set; }

//        public int Days { get; set; }

//        public string Title { get; set; }

//        public string ShortDescription { get; set; }

//        public string Description { get; set; }

//        public int? CategoryId { get; set; }

//        public List<int> Locations { get; set; }

//        //To remove image when updating
//        public string RemovingImages { get; set; }
//    }
//    public class ApiUpdatePostModel : CommonInputModel
//    {
//        public int Id { get; set; }

//        public int Days { get; set; }

//        public string Title { get; set; }

//        public string ShortDescription { get; set; }

//        public string Description { get; set; }
//    }

//    public class ApiDeletePostModel : CommonInputModel
//    {
//        public int Id { get; set; }

//        public int Status { get; set; }
//    }

//    public class ApiPostActionModel : CommonInputModel
//    {
//        public int PostId { get; set; }

//        public string ActionType { get; set; }

//        public int RatingScore { get; set; }
//    }

//    public class ApiPostDetailModel : CommonInputModel
//    {
//        public int PostId { get; set; }

//        public long CommentId { get; set; }

//        public long CommentReplyId { get; set; }

//        public int PageIndex { get; set; }

//        public int PageSize { get; set; }

//        public int ReplyPageSize { get; set; }
//    }

//    public class ApiGetTopPostByPlaceModel : CommonInputModel
//    {
//        public int PlaceId { get; set; }
//        public int PageSize { get; set; }
//    }
//}