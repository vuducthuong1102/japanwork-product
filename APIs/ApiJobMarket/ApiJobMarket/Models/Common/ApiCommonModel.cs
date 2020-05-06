using System.Collections.Generic;
using System.Web;

namespace ApiJobMarket.Models
{   
    public class ApiCommonModel
    {
        public int? page_size { get; set; }
        public int? page_index { get; set; }
        public string keyword { get; set; }
        public int? status { get; set; }

        public int? agency_id { get; set; }

        public int? job_seeker_id { get; set; }
        public string token { get; set; }
        public string language_code { get; set; }
    }

    public class ApiUploadFileModel
    {
        public string ObjectId { get; set; }
        public string SubDir { get; set; }
        public bool InCludeDatePath { get; set; }
        public bool GenerateThumb { get; set; }

        public List<HttpPostedFile> Files { get; set; }

        public List<string> FilesInString { get; set; }

        public ApiUploadFileModel()
        {
            FilesInString = new List<string>();
            Files = new List<HttpPostedFile>();
        }
    }   

    public class ApiAddressInputModel
    {
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
    }

    public class ApiGetListByPageModel : ApiCommonModel
    {
        public int agency_id { get; set; }
        public int status { get; set; }

        public int cv_id { get; set; }
        public int staff_id { get; set; }
        public int type_job_seeker { get; set; }
        public int company_id { get; set; }
        public int gender { get; set; }
        public int major_id { get; set; }
        public int country_id { get; set; }

        public int japanese_level { get; set; }
        public int job_id { get; set; }
    }

    public class ApiJobGetDetailModel : ApiCommonModel
    {
        public int id { get; set; }
    }

    public class ApiGetListByIdsModel
    {
        public List<int> ListIds { get; set; }
    }
    public class ApiJobCheckInviteModel
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
    }
    public class ApiConversationModel
    {
        public int Id { get; set; }

        public int UserOne { get; set; }

        public int UserTwo { get; set; }

        public string Ip { get; set; }

        public int Status { get; set; }

        public int OwnerId { get; set; }

    }

    public class ApiConversationReplyModel
    {
        public long Id { get; set; }

        public int ConversationId { get; set; }

        public int Type { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public string Ip { get; set; }

        public int Status { get; set; }
    }

    public class ApiConversationFilterModel : ApiFilterModel
    {
        public int OwnerId { get; set; }

        public int UserTwo { get; set; }
    }
}