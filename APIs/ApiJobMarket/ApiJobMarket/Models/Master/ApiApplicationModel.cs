namespace ApiJobMarket.Models
{
    public class ApiApplicationModel : ApiGetListByPageModel
    {
        public int id { get; set; }
        public int employment_type_id { get; set; }
        public int has_process { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public int qualification_id { get; set; }
        public int sub_field_id { get; set; }
        public int japanese_level_number { get; set; }
        public int prefecture_id { get; set; }
        public int visa_id { get; set; }
    }
    public class ApiApplicationByPageModel : ApiGetListByPageModel
    {
        public int? agency_id { get; set; }
    }

    public class ApiApplicationAcceptModel
    {
        public int? id { get; set; }
        public int? agency_id { get; set; }
        public int? cv_id { get; set; }
        public int? staff_id { get; set; }
        public int? pic_id { get; set; }
        public int? is_invitation { get; set; }
    }
    public class ApiApplicationSendCvModel
    {
        public int? job_id { get; set; }
        public int? job_seeker_id { get; set; }
        public int? cv_id { get; set; }
    }
    public class ApiApplicationPicModel
    {
        public int job_seeker_id { get; set; }
        public int agency_id { get; set; }
        public int pic_id { get; set; }
    }
    public class ApiApplicationIgnoreModel
    {
        public int? id { get; set; }
        public int? agency_id { get; set; }
        public int? cv_id { get; set; }
        public int? staff_id { get; set; }
    }
}