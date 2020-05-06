using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SingleSignOn.Models
{
    //public class SendSMSModel
    //{
    //     //{"msisdn":"số điện thoại nhận tin nhắn","msg":"nội dung tin nhắn","sendtime":"thời gian gửi tin nhắn","tranid":"Mã giao dịch đối tác"}
    //    public string msisdn { get; set; }
    //    public string msg { get; set; }

    //    /// <summary>
    //    ///Sendtime format ddMMyyyyHHmmssss
    //    /// </summary>
    //    public string sendtime { get; set; }
    //    public string tranid { get; set; }
    //}

    public class EmailModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string SenderPwd { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}