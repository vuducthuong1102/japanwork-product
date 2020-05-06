
using System.Collections.Generic;
using System.Net;

namespace MyCloud.Models
{
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
}