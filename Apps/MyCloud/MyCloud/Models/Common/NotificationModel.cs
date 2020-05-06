namespace MyCloud.Models
{
    public class NotificationInputModel
    {
        public string registration_id { get; set; } // registrationID FCM được user gửi lên qua signal
        public string device_id { get; set; } // deviceID được user gửi lên qua signal
        public dynamic Data { get; set; } // data notification
        public string language_code { get; set; }
    }
}