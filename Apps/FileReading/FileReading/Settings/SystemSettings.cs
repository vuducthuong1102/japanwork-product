using FileReading.Helpers;
using System;
using System.Configuration;

namespace FileReading.Settings
{
    public class SystemSettings
    {
        public static bool ReadFileDirectly
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ReadFileDirectly"]);
            }
        }

        public static string MediaFileUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaFileUrl"];
            }
        }

        public static string GenerateFileUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["GenerateFileUrl"];
            }
        }

        public static bool WatermarkEnabled
        {
            get
            {
                return Utils.ConvertToBoolean(ConfigurationManager.AppSettings["WatermarkEnabled"]);
            }
        }

        public static string WatermarkPosition
        {
            get
            {
                return ConfigurationManager.AppSettings["WatermarkPosition"];
            }
        }
    }

    public class ImageSettings
    {     
        public static int AvatarWidth
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Img:AvatarWidth"]);
            }
        }

        public static int AvatarHeight
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Img:AvatarHeight"]);
            }
        }

        public static string AvatarFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["Img:AvatarFolder"];
            }
        }
    }
}