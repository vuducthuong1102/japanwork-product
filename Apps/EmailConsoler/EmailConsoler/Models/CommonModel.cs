using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoler.Models
{
    public class EmailModel
    {
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Receiver { get; set; }
        public List<string> Receivers { get; set; }
        public string SenderPwd { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ActiveLink { get; set; }
        public string Note { get; set; }
        public dynamic Data { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> AttachmentLinks { get; set; }
    }

    public class CustomerEmailAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
    }
}
