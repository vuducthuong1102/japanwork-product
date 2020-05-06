
namespace Manager.WebApp.Models
{
    public class ApiAgencyCreateModel : ApiAgencyModel
    {
        
    }

    public class ApiAgencyModel
    {
        public int agency_id { get; set; }
        public string agency { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public string address { get; set; }
        public string logo_path { get; set; }
    }

    public class ApiMarkIsReadAgencyNotificationModel
    {        
        public int id { get; set; }        
        public int staff_id { get; set; }
        public bool is_read { get; set; }
    }

    public class ApiAgencyGetListNotificationModel : ApiGetListByPageModel
    {        
        public int staff_id { get; set; }        
        public int is_update_view { get; set; }
    }

    public class ApiAgencyStaffModel
    {        
        public int staff_id { get; set; }
    }

    public class ApiAgencyReportModel
    {
        public int agency_id { get; set; }
    }

    public class ApiAgencyReportByYearModel : ApiAgencyReportModel
    {
        public int year { get; set; }
    }
    
    public class ApiAgencyReportByWeekModel : ApiAgencyReportModel
    {
       
    }
}