using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiJobMarket.Models
{
    public class ApiSalaryFilterEditModel : ApiSalaryFilterItem
    {
        public int id { get; set; }
    }
    public class ApiSalaryFilterInsertModel : ApiSalaryFilterItem
    {
    }
    public class ApiSalaryFilterItem
    {
        public int type { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public int order { get; set; }
    }
}