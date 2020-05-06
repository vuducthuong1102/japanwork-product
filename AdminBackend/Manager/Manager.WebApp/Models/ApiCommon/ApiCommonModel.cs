using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ApiFilterModel : ApiCommonModel
    {
        public dynamic Extensions { get; set; }

        public ApiFilterModel()
        {
            Extensions = new ExpandoObject();
        }
    }
    public class ApiResponseCommonModel
    {
        public int status { get; set; }
        public dynamic value { get; set; }
        public string message { get; set; }
        public ApiResponseErrorModel error { get; set; }
        public int total { get; set; }

        public ApiResponseCommonModel()
        {
            status = (int)HttpStatusCode.OK;
            error = new ApiResponseErrorModel();
        }
    }
    public class ApiResponseErrorModel
    {
        public string error_code { get; set; }
        public string message { get; set; }
        public List<ApiResponseErrorFieldModel> field { get; set; }
    }

    public class ApiResponseErrorFieldModel
    {
        public string name { get; set; }
        public string message { get; set; }
    }
    public class ApiCommonModel
    {
        public int? page_size { get; set; }
        public int? page_index { get; set; }
        public string keyword { get; set; }

        public int? job_seeker_id { get; set; }
        public string token { get; set; }

        public int? status { get; set; }

        public string language_code { get; set; }
    }

    public class ApiUploadFileModel
    {
        public string ObjectId { get; set; }
        public string SubDir { get; set; }
        public bool InCludeDatePath { get; set; }

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

    }

    public class ApiGetListByIdsModel
    {
        public List<int> ListIds { get; set; }
    }

    public class ApiCommonFilterModel : ApiCommonModel
    {
        public dynamic Extensions { get; set; }

        public ApiCommonFilterModel()
        {
            Extensions = new ExpandoObject();
        }
    }

    public class ApiCommonOperatorInfoModel : ApiCommonModel
    {
        public int Id { get; set; }
    }

    public class ApiGetStationByPageModel : ApiGetListByPageModel
    {
        public int place_id { get; set; }
    }

    public class ApiGetTrainLineByPageModel : ApiGetListByPageModel
    {
        public int place_id { get; set; }
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

    public class ApiGetMessagesModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int OwnerId { get; set; }
        public int UserTwo { get; set; }
    }
}