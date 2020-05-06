using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ApiSalaryFilterEditModel : ApiSalaryFilterModel
    {
        public int id { get; set; }
    }
    public class ApiSalaryFilterSearchModel : ApiCommonModel
    {

    }
    public class ApiSalaryFilterInsertModel : ApiSalaryFilterModel
    {
    }
    public class ApiSalaryFilterModel
    {
        public int type { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public int order { get; set; }
    }
}