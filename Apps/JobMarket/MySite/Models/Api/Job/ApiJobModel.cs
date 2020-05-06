using System.Collections.Generic;

namespace MySite.Models
{
    public class ApiJobSearchModel : ApiCommonFilterModel
    {
        public string title { get; set; }
        public int employment_type_id { get; set; }
        public int japanese_level_number { get; set; }
        public int field_id { get; set; }
        public int language_id { get; set; }
        public string language_code { get; set; }
        public int salary_max { get; set; }
        public int salary_min { get; set; }

        public List<int> city_ids { get; set; }
        public List<int> sub_industry_ids { get; set; }
        public List<int> sub_field_ids { get; set; }
        public List<int> station_ids { get; set; }
        public List<int> prefecture_ids { get; set; }
        public List<int> japanese_levels { get; set; }

        public string sorting_date { get; set; }
    }

    public class ApiJobGetRecentModel : ApiCommonModel
    {
        public int company_id { get; set; }
        public string ignore_ids { get; set; }
    }

    public class ApiSaveJobModel
    {
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
    }

    public class ApiJobActionApplyModel
    {
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int cv_id { get; set; }
    }

    public class ApiInvitationActionModel
    {
        public int id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
    }

    public class ApiApplicationCancelModel
    {
        public int id { get; set; }
        public int job_id { get; set; }
        public int job_seeker_id { get; set; }
        public int cv_id { get; set; }
    }

    public class ApiFriendInvitationModel
    {
        public int invite_id { get; set; }
        public List<string> Emails { get; set; }
        public int job_id { get; set; }
        public string note { get; set; }
        public int job_seeker_id { get; set; }
    }

    public class ApiJobGetDetailModel : ApiCommonModel
    {
        public int id { get; set; }
    }

    public class ApiJobCheckInviteModel 
    {
        public int id { get; set; }
        public int job_seeker_id { get; set; }
    }
}