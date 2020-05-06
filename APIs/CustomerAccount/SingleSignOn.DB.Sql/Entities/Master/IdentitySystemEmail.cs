using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOn.DB.Sql.Entities
{
    public class IdentitySystemEmail : IdentityCommon
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Action { get; set; }

        public int ReceiverId { get; set; }

        public bool IsSent { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}
