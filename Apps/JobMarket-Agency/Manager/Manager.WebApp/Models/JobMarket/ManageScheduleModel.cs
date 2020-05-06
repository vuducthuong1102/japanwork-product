using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ScheduleCalendarModel
    {
        public bool isAgency { get; set; }
    }

    public class ScheduleUpdateModel
    {
        public string sd_id { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TITLE))]
        public string sd_title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_FROM_DATE))]
        public string sd_start_time { get; set; }

        [Required(ErrorMessageResourceType = typeof(ManagerResource), ErrorMessageResourceName = nameof(ManagerResource.ERROR_NOT_NULL_REQUIRED))]
        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_TO_DATE))]
        public string sd_end_time { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_SCHEDULE_CAT))]
        public int sd_schedule_cat { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.LB_PERSON_IN_CHARGE))]
        public int sd_pic_id { get; set; }

        [Display(ResourceType = typeof(Resources.ManagerResource), Name = nameof(ManagerResource.SCHEDULE_CONTENT))]
        public string sd_content { get; set; }

        public bool sd_approved { get; set; }

        public List<MsSql.AspNet.Identity.IdentityUser> StaffList { get; set; }

        public bool isReadOnly { get; set; }
    }

    public class ScheduleJsonModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public bool editable { get; set; }
        public bool approved { get; set; }
        public string color { get; set; }

        public int pic_id { get; set; }
        public string content { get; set; }
    }

    public class ScheduleWidgetModel
    {
        public string title { get; set; }       
        public IdentitySchedule data { get; set; }
    }
}