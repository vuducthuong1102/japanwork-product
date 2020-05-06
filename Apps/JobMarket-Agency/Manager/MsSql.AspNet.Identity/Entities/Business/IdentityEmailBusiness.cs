using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityEmailBusiness : CommonIdentity
    {
        public int Id { get; set; }        
        public int AgencyId { get; set; }
        public int StaffId { get; set; }
        public int JobSeekerId { get; set; }
        public int CompanyId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Attachments { get; set; }
        public bool IsOnlineUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Status { get; set; }

        public List<IdentityEmailAddObject> Receivers { get; set; }
    }

    public class IdentityEmailAddObject
    {
        public int object_id { get; set; }
        public string email { get; set; }        
    }
}
