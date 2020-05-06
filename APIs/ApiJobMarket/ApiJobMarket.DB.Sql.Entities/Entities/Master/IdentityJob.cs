using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityJob : IdentityCommon
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int quantity { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int salary_min { get; set; }
        public int salary_max { get; set; }
        public int salary_type_id { get; set; }
        public TimeSpan? work_start_time { get; set; }
        public TimeSpan? work_end_time { get; set; }
        public int probation_duration { get; set; }
        public int status { get; set; }
        public DateTime? closed_time { get; set; }
        public int employment_type_id { get; set; }
        public bool flexible_time { get; set; }
        public string language_level { get; set; }
        public bool work_experience_doc_required { get; set; }
        public int view_count { get; set; }
        public int duration { get; set; }
        public bool view_company { get; set; }
        public int qualification_id { get; set; }
        public int station_id { get; set; }
        public int japanese_level_number { get; set; }
        public int sub_field_id { get; set; }
        public int sub_industry_id { get; set; }
        public int translate_status { get; set; }
        public string job_code { get; set; }
        public int pic_id { get; set; }

        public bool japanese_only { get; set; }
        public DateTime? created_at { get; set; }

        public int company_pic_id { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        //Extends
        public dynamic Extensions { get; set; }

        //public string status_label { get; set; }
        //public int application_count { get; set; }
        //public bool is_saved { get; set; }
        //public bool is_applied { get; set; }

        public List<IdentityJobAddress> Addresses { get; set; }

        //public List<IdentityJobSubField> Sub_fields { get; set; }

        public List<IdentityJobTag> Tags { get; set; }

        public List<IdentityJobTranslation> Job_translations { get; set; }

        public IdentityCompanyShort company_info { get; set; }

        [JsonIgnore]
        public int job_seeker_id { get; set; }

        public IdentityJob()
        {
            Extensions = new ExpandoObject();
            Extensions.application_count = 0;
            Extensions.candidate_count = 0;
            Extensions.is_saved = false;
            Extensions.is_applied = false;
            Extensions.is_refuse = false;
            Extensions.status_label = "";
            Extensions.process_lastest = 0;
        }
    }

    public class IdentityJobTranslation
    {
        public int id { get; set; }
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
        public string language_code { get; set; }
        public string remark { get; set; }
        public int job_id { get; set; }
        public string friendly_url { get; set; }
        public int translate_status { get; set; }

        public int staff_id { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }
    }

    public class IdentityJobCounter
    {
        public int Draft { get; set; }
        public int Published { get; set; }
        public int Closed { get; set; }
        public int Saved { get; set; }
        public int Expired { get; set; }

        public int GetTotal()
        {
            return Draft + Published + Closed + Saved + Expired;
        }
    }

}
