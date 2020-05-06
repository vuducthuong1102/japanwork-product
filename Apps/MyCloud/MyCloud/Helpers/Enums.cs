using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCloud.Helpers
{
    public enum EnumPurposes
    {        
        Chat = 1,
        SendRequest = 2
    }

    public class DeviceType
    {
        public static string Android = "android";
        public static string IOS = "ios";
        public static string Desktop = "desktop";
    }
}