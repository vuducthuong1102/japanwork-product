using ApiJobMarket.DB.Sql.Entities;
using System;
using System.Collections.Generic;
using System.Net;

namespace ApiJobMarket.Models
{
    public class FileUploadResponseModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string Path { get; set; }
        public string CoverPath { get; set; }
        public string CoverFullPath { get; set; }
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

    public class ApiResponseAgencyItemModel
    {
        public int id { get; set; }
        public int agency_id { get; set; }
        public int constract_id { get; set; }
        public int company_id { get; set; }
    }

    public class ApiResponseCompanyItemModel
    {
        public int id { get; set; }
        public string company_name { get; set; }
        public string description { get; set; }
        public int company_size_id { get; set; }
        public string logo_path { get; set; }
        public int sub_industry_id { get; set; }
        public int establish_year { get; set; }
        public string website { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public int branch { get; set; }
        public int agency_id { get; set; }
        public int desciption_translation_id { get; set; }
        public int headquater_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string map { get; set; }

        //Extends
        public int job_count { get; set; }
        public int application_count { get; set; }
    }

    public class ApiResponseApplicationItemModel
    {
        public int id { get; set; }
        public DateTime? interview_accept_time { get; set; }
        public DateTime? cancelled_time { get; set; }
        public int cv_id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //Extends
        public string status_label { get; set; }

        public ApiResponseApplicationCandidateModel Cv { get; set; }
    }

    public class ApiResponseCandidateItemModel
    {
        public int id { get; set; }
        public DateTime? request_time { get; set; }
        public DateTime? applied_time { get; set; }
        public int cv_id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //Extends
        public string status_label { get; set; }

        public ApiResponseApplicationCandidateModel Cv { get; set; }
    }

    public class ApiResponseApplicationCandidateModel
    {
        public string fullname { get; set; }
        public DateTime? birthday { get; set; }
        public string image { get; set; }
    }
}