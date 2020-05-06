
namespace Manager.WebApp.Models
{
    public class ApiScheduleModel
    {
        public int id { get; set; }
        public int agency_id { get; set; }
        public int staff_id { get; set; }
        public int schedule_cat { get; set; }
        public int pic_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }

        /// <summary>
        /// Start time: Format - yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string start_time { get; set; }

        /// <summary>
        /// End time: Format - yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string end_time { get; set; }
    }

    public class ApiScheduleByStaffModel
    {
        public int agency_id { get; set; }
        public int staff_id { get; set; }

        /// <summary>
        /// From date limitation: Format - yyyy-MM-dd
        /// </summary>
        public string start_time { get; set; }

        /// <summary>
        /// To date limitation: Format - yyyy-MM-dd
        /// </summary>
        public string end_time { get; set; }
    }
}