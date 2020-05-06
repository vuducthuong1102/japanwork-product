using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileReading.Helpers
{
    public static class EnumCommonCode
    {
        public static int Success = 1;
        public static int Error = -1;
        public static int Error_Info_NotFound = -2;
    }

    public static class ScreenPosition
    {
        public static string TopLeft = "top-left";
        public static string TopCenter = "top-center";
        public static string TopRight = "top-right";
        public static string MiddleLeft = "middle-left";
        public static string MiddleCenter = "middle-center";
        public static string MiddleRight = "middle-right";
        public static string BottomLeft = "bottom-left";
        public static string BottomCenter = "botton-center";
        public static string BottomRight = "botton-right";
    }

    public enum EnumEmailTargetType
    {
        JobSeeker = 0,
        Company = 1
    }
}