﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiJobMarket.Models
{
    public class ApiCompanyModel: ApiCommonModel
    {
        public int agency_id { get; set; }
        public string langcode { get; set; }
    }

    public class ApiCompanyDeleteModel
    {
        public string ids { get; set; }
        public int staff_id { get; set; }
        public int agency_id { get; set; }
        public int company_id { get; set; }
    }
    public class ApiCompanyUpdateModel
    {
        public int? id { get; set; }
        public int? agency_id { get; set; }
        public string company_name { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string address_detail { get; set; }
        public string address_furigana { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int? company_size_id { get; set; }
        public int? sub_industry_id { get; set; }
        public int? establish_year { get; set; }
        public string logo_path { get; set; }

        public int pic_id { get; set; }
        public int staff_id { get; set; }

        public ApiAddressInputModel Address { get; set; }
    }

    public class ApiCompanyUpdateAgencyModel
    {
        public int company_id { get; set; }
        public List<int> ListAgencyId { get; set; }
        public int agency_parent_id { get; set; }
    }

    public class ApiCompanyUpdateCompanyModel
    {
        public int agency_id { get; set; }
        public List<int> ListCompanyId { get; set; }
        public int agency_parent_id { get; set; }
    }

    public class ApiCompanyInsertModel
    {
        public string company_name { get; set; }
        public string description { get; set; }
        public int agency_id { get; set; }
        public int company_size_id { get; set; }
        public string logo_path { get; set; }
        public int sub_industry_id { get; set; }
        public int establish_year { get; set; }
        public string website { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public int branch { get; set; }
        public int headquater_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string langcode { get; set; }

        public ApiAddressInputModel Address { get; set; }
    }
}