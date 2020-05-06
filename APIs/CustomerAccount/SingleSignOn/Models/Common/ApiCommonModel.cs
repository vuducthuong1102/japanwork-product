namespace SingleSignOn.Models
{
    public class ApiResponseCommonModel
    {
        public int status { get; set; }
        public dynamic value { get; set; }
        public ApiResponseErrorModel error { get; set; }

        public ApiResponseCommonModel()
        {
            error = new ApiResponseErrorModel();
        }
    }

    public class ApiResponseErrorModel
    {
        public string error_code { get; set; }
        public string message { get; set; }
        public string field { get; set; }
    }
}