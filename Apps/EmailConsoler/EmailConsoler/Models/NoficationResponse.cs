using System;

namespace EmailConsoler.Models
{
    public class NotificationSeviceResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class NotificationInfo
    {
        public string RegistrationID { get; set; }
        public string DeviceID { get; set; }
        public bool iosDevice { get; set; }
        public dynamic Data { get; set; }
    }
}
