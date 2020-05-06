using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityCompanyShort : IdentityCommon
    {
        public int id { get; set; }
        public string company_name { get; set; }        
        public string logo_full_path { get; set; }      
        public int agency_id { get; set; }
        public string company_code { get; set; }
    }

    public class IdentityCompany : IdentityCompanyShort
    {
        public string description { get; set; }
        public int company_size_id { get; set; }
        public string logo_path { get; set; }
        public int sub_industry_id { get; set; }
        public int establish_year { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public int branch { get; set; }
        public int desciption_translation_id { get; set; }
        public int headquater_id { get; set; }
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string map { get; set; }
        public string address_detail { get; set; }
        public string address_furigana { get; set; }
        public int status { get; set; }
        public int invitation_limit { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int pic_id { get; set; }
        public string company_code { get; set; }

        //Extends
        public int job_count { get; set; }
        public int application_count { get; set; }

        public int total { get; set; }
        public int total_public { get; set; }

        public IdentityAddress address_info { get; set; }               
        public List<IdentityCompanyLang> LangList { get; set; }

        public IdentityCompany()
        {
            LangList = new List<IdentityCompanyLang>();
        }
    }
    public class IdentityAgencyCompany
    {
        public int id { get; set; }
        public int agency_id { get; set; }
        public int company_id { get; set; }
    }    
    public class IdentityCompanyLang
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string company_name { get; set; }
        public string description { get; set; }
        public string language_code { get; set; }
    }

    public class IdentityCompanyCounter
    {
        public IdentityJobCounter JobCounter { get; set; }

        public IdentityCompanyCounter()
        {
            JobCounter = new IdentityJobCounter();
        }
    }
}
