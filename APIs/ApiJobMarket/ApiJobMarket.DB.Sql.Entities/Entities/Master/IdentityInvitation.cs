using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityInvitationBase : IdentityCommon
    {        
        public int job_seeker_id { get; set; }
        public int cv_id { get; set; }
    }

    public class IdentityInvitation : IdentityInvitationMaster
    {
        public int id { get; set; }
        public DateTime? accept_time { get; set; }       
        public int status { get; set; }
        public int invite_id { get; set; }
    }

    public class IdentityInvitationMaster : IdentityInvitationBase
    {
        public int master_id { get; set; }
        public int agency_id { get; set; }
        public int company_id { get; set; }
        public int job_id { get; set; }

        public int job_seeker_id { get; set; }
        public string note { get; set; }
        public DateTime? request_time { get; set; }

        //Extends
        public List<IdentityInvitationBase> Invitations { get; set; }
        public string status_label { get; set; }
        public int invitation_count { get; set; }
        public IdentityJob job_info { get; set; }

        public string job_ids { get; set; }
        public IdentityJobSeeker JobSeeker { get; set; }
             
    }

    public class IdentityFriendInvitationMaster : IdentityCommon
    {
        public int master_id { get; set; }
        public int sender_id { get; set; }
        public int job_id { get; set; }
        public string note { get; set; }
        public DateTime? request_time { get; set; }

        //Extends
        public List<IdentityFriendInvitation> Invitations { get; set; }
        public string status_label { get; set; }
        public int invitation_count { get; set; }
        public IdentityJob job_info { get; set; }
    }

    public class IdentityFriendInvitation : IdentityFriendInvitationMaster
    {
        public int id { get; set; }
        public string receiver_email { get; set; }
        public DateTime? accept_time { get; set; }
        public bool visited { get; set; }
        public DateTime? visit_time { get; set; }
        public int sending_count { get; set; }
        public int status { get; set; }
        public int invite_id { get; set; }
    }    
}
