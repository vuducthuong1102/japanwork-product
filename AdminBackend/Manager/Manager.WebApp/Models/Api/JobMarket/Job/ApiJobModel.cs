using System;
using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    public class ApiJobModel : ApiGetListByPageModel
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int agency_id { get; set; }

        public int translate_status { get; set; }
    }
    public class ApiJobUpdateModel
    {
        public ApiJobUpdateInfoModel job { get; set; }
    }

    public class ApiJobUpdateInfoModel
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int quantity { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int salary_type_id { get; set; }
        public string work_start_time { get; set; }
        public string work_end_time { get; set; }
        public int probation_duration { get; set; }
        public int employment_type_id { get; set; }
        public bool flexible_time { get; set; }
        public string language_level { get; set; }
        public bool work_experience_doc_required { get; set; }
        public int duration { get; set; }
        public bool view_company { get; set; }
        public int qualification_id { get; set; }
        public int japanese_level_number { get; set; }
        public int sub_field_id { get; set; }
        public int sub_industry_id { get; set; }
        public DateTime? closed_time { get; set; }

        public List<ApiJobUpdateAddressModel> Addresses { get; set; }
        public List<ApiJobUpdateTranslationModel> Job_translations { get; set; }
        //public List<ApiJobUpdateSubFieldModel> Sub_fields { get; set; }
        public List<ApiJobUpdateTagModel> Tags { get; set; }
    }

    public class ApiJobUpdateAddressModel
    {
        public int id { get; set; }
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string furigana { get; set; }
        public string detail { get; set; }

        public List<ApiJobUpdateStationModel> Stations { get; set; }
    }

    public class ApiJobUpdateTranslationModel
    {
        public string title { get; set; }
        public string subsidy { get; set; }
        public string paid_holiday { get; set; }
        public string bonus { get; set; }
        public string certificate { get; set; }
        public string work_content { get; set; }
        public string requirement { get; set; }
        public string plus { get; set; }
        public string welfare { get; set; }
        public string training { get; set; }
        public string recruitment_procedure { get; set; }
        public string remark { get; set; }
        public int job_id { get; set; }
        public string language_code { get; set; }
        public int translate_status { get; set; }
    }

    public class ApiJobUpdateStationModel
    {
        public int id { get; set; }
    }

    public class ApiJobUpdateSubFieldModel
    {
        public int id { get; set; }
    }

    public class ApiJobUpdateTagModel
    {
        public int id { get; set; }
        public string tag { get; set; }
    }

    public class ApiJobDeleteModel
    {
        public int id { get; set; }
        public int agency_id { get; set; }
    }

    public class ApiJobUpdateStatusModel
    {
        public int id { get; set; }
        public int status { get; set; }
    }
}