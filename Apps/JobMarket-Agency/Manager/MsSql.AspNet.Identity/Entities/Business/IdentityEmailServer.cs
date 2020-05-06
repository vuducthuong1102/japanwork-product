using System;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityEmailServer : IdentityCommon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AgencyId { get; set; }
        public int StaffId { get; set; }

        //SMTP config
        public string SMTPConfig { get; set; }

        //POP config
        public string POPConfig { get; set; }

        public bool TestingSuccessed { get; set; }

        public int Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public IdentityEmailServerSMTPConfig SendingConfig { get; set; }
        public IdentityEmailServerPOPConfig ReceivingConfig { get; set; }
    }

    public class IdentityEmailServerSMTPConfig
    {
        public string SMTPServer { get; set; }
        public int Port { get; set; }
        public bool SSLRequired { get; set; }
    }

    public class IdentityEmailServerPOPConfig
    {
        public string POPServer { get; set; }
        public int Port { get; set; }
        public bool SSLRequired { get; set; }
    }
}
