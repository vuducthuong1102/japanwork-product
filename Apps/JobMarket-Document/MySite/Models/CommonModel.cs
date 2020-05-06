using System.Collections.Generic;
using System.Web;

namespace MySite.Models
{
    public class CommonPagingModel
    {
        public string keyword { get; set; }

        public string SearchExec { get; set; }

        //For paging
        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int StartCount()
        {
            var index = this.CurrentPage * PageSize - PageSize + 1;
            return index;
        }

        public int EndCount(int searchResultsCount)
        {
            var index = StartCount() + searchResultsCount - 1;
            return index;
        }

        public bool WasOverRecordsInPage()
        {
            return (this.TotalCount > PageSize);
        }
    }

    public class ApiCommonModel
    {
        public int page_size { get; set; }
        public int page_index { get; set; }
        public string keyword { get; set; }

        public int job_seeker_id { get; set; }
        public string token { get; set; }
    }

    public class CommonJsonReturnModel
    {

    }

    public class ApiSearchModel
    {
        public string Keyword { get; set; }
    }

    public class JavascriptRedirectModel
    {
        public JavascriptRedirectModel(string location)
        {
            Location = location;
        }

        public string Location { get; set; }
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

    public class ApiGetListByPageModel : ApiCommonFilterModel
    {

    }

    public class ApiGetTrainLineByPageModel : ApiGetListByPageModel
    {
        public int place_id { get; set; }
    }

    public class ApiGetStationByPageModel : ApiGetListByPageModel
    {
        public int place_id { get; set; }
    }

    public class ApiSearchPlaceByPageModel : ApiGetListByPageModel
    {
        public List<int> regions { get; set; }
        public List<int> prefectures { get; set; }
    }

    public class ApiGetListByIdsModel
    {
        public List<int> ListIds { get; set; }
    }

    public class TreeViewItemModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public int population { get; set; }
        public string flagUrl { get; set; }
        public bool @checked { get; set; }
        public bool hasChildren { get; set; }
        public List<TreeViewItemModel> children { get; set; }

        public TreeViewItemModel()
        {
            children = new List<TreeViewItemModel>();
        }
    }

    public class ApiUploadFileModel
    {
        public string ObjectId { get; set; }
        public string SubDir { get; set; }
        public bool InCludeDatePath { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public ApiUploadFileModel()
        {
            Files = new List<HttpPostedFileBase>();
        }
    }

    public class FileUploadResponseModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string Path { get; set; }
    }
}