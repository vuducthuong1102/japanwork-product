using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using System.Web.Mvc;
using MySite.Resources;

namespace MySite.Helpers
{
    public static class TimestampedContentExtensions
    {
        public static string VersionedContent(this UrlHelper helper, string contentPath)
        {
            var context = helper.RequestContext.HttpContext;

            if (context.Cache[contentPath] == null)
            {
                var physicalPath = context.Server.MapPath(contentPath);
                var version = @"v=" + new FileInfo(physicalPath).LastWriteTime.ToString(@"yyyyMMddHHmmss");

                var translatedContentPath = helper.Content(contentPath);

                var versionedContentPath =
                    contentPath.Contains(@"?")
                        ? translatedContentPath + @"&" + version
                        : translatedContentPath + @"?" + version;

                context.Cache.Add(physicalPath, version, null, DateTime.Now.AddMinutes(1), TimeSpan.Zero,
                    CacheItemPriority.Normal, null);

                context.Cache[contentPath] = versionedContentPath;
                return versionedContentPath;
            }
            else
            {
                return context.Cache[contentPath] as string;
            }
        }
    }

    public class NumberHelpers
    {
        public static string NumberReformat(int number)
        {
            return string.Format("{0:n0}", number);
        }
    }

    public class DateHelpers
    {
        public static string FormatToString(DateTime datetime)
        {
            var minutes = (int)(DateTime.Now - datetime).TotalMinutes;
            if (minutes < 60)
            {
                if (minutes < 1)
                {
                    return 1.ToString() + UserWebResource.LB_Minute_m;
                }
                else
                {
                    return minutes.ToString() + UserWebResource.LB_Minute_m;
                }
            }
            var hour = (int)(DateTime.Now - datetime).TotalHours;
            if (hour < 24)
            {
                return hour.ToString() + UserWebResource.LB_Hour_m;
            }

            var day = (int)(DateTime.Now - datetime).TotalDays;
            if (day < 7)
            {
                return day.ToString() + UserWebResource.LB_Day_m;
            }

            var week = (int)(day / 7);
            if (week <= 52)
            {
                return week.ToString() + UserWebResource.LB_Week_m;
            }

            var year = (int)(day / 365);

            return year.ToString() + UserWebResource.LB_Year_m;
        }

        public static string FormatToStringTitle(DateTime datetime)
        {
            var minutes = (int)(DateTime.Now - datetime).TotalMinutes;
            if (minutes < 60)
            {
                if (minutes < 1)
                {
                    return UserWebResource.LB_JustNow;
                }
                else
                {
                    return minutes.ToString() + " " + UserWebResource.LB_Minute + GetManyNumber(minutes);
                }
            }
            var hour = (int)(DateTime.Now - datetime).TotalHours;
            if (hour < 24)
            {
                return hour.ToString() + " " + UserWebResource.LB_Hour + GetManyNumber(hour);
            }
            var day = (int)(DateTime.Now - datetime).TotalDays;
            if (day == 1)
            {
                return UserWebResource.LB_Yesterday + " " + UserWebResource.LB_At + " " + datetime.ToString("HH:mm");
            }
            else if (day < 365)
            {

                return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + UserWebResource.LB_At + " " + datetime.ToString("HH:mm");
            }
            else 
            {
                return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + datetime.Month.ToString() + datetime.Year.ToString();
            }
        }

        public static string FormatToStringTitle(DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            var minutes = (int)(DateTime.Now - datetime.Value).TotalMinutes;
            if (minutes < 60)
            {
                if (minutes < 1)
                {
                    return UserWebResource.LB_JustNow;
                }
                else
                {
                    return minutes.ToString() + " " + UserWebResource.LB_Minute + GetManyNumber(minutes);
                }
            }
            var hour = (int)(DateTime.Now - datetime.Value).TotalHours;
            if (hour < 24)
            {
                return hour.ToString() + " " + UserWebResource.LB_Hour + GetManyNumber(hour);
            }
            var day = (int)(DateTime.Now - datetime.Value).TotalDays;
            if (day == 1)
            {
                return UserWebResource.LB_Yesterday + " " + UserWebResource.LB_At + " " + datetime.Value.ToString("HH:mm");
            }
            else if (day < 365)
            {

                return datetime.Value.Day.ToString() + " " + GetMonth(datetime.Value.Month) + " " + UserWebResource.LB_At + " " + datetime.Value.ToString("HH:mm");
            }
            else
            {
                return datetime.Value.Day.ToString() + " " + GetMonth(datetime.Value.Month) + " " + datetime.Value.Month.ToString() + datetime.Value.Year.ToString();
            }
        }

        public static string FormatToOnlineStringTitle(DateTime datetime)
        {
            var minutes = (int)(DateTime.Now - datetime).TotalMinutes;
            if (minutes < 60)
            {
                if (minutes < 1)
                {
                    return string.Empty;
                }
                else
                {
                    return minutes.ToString() + " " + UserWebResource.LB_Minute + GetManyNumber(minutes);
                }
            }
            var hour = (int)(DateTime.Now - datetime).TotalHours;
            if (hour < 24)
            {
                return hour.ToString() + " " + UserWebResource.LB_Hour + GetManyNumber(hour);
            }
            var day = (int)(DateTime.Now - datetime).TotalDays;
            if (day == 1)
            {
                return 1 + " " + UserWebResource.LB_Day;
            }
            else if (day < 365)
            {
                return string.Empty;
                //return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + UserWebResource.LB_At + " " + datetime.ToString("HH:mm");
            }
            else
            {
                return string.Empty;
                //return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + datetime.Month.ToString() + datetime.Year.ToString();
            }
        }

        public static string FormatToOnlineStringTitle(DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            var minutes = (int)(DateTime.Now - datetime.Value).TotalMinutes;
            if (minutes < 60)
            {
                if (minutes < 1)
                {
                    return string.Empty;
                }
                else
                {
                    return minutes.ToString() + " " + UserWebResource.LB_Minute + GetManyNumber(minutes);
                }
            }
            var hour = (int)(DateTime.Now - datetime.Value).TotalHours;
            if (hour < 24)
            {
                return hour.ToString() + " " + UserWebResource.LB_Hour + GetManyNumber(hour);
            }
            var day = (int)(DateTime.Now - datetime.Value).TotalDays;
            if (day == 1)
            {
                return 1 + " " + UserWebResource.LB_Day;
            }
            else if (day < 365)
            {
                return string.Empty;
                //return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + UserWebResource.LB_At + " " + datetime.ToString("HH:mm");
            }
            else
            {
                return string.Empty;
                //return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + datetime.Month.ToString() + datetime.Year.ToString();
            }
        }

        public static string GetManyNumber(int number)
        {
            if (number > 1)
            {
                return UserWebResource.LB_Number_Many;
            }
            else
            {
                return String.Empty;
            }
        }

        public static string FormatToString(DateTime datetime, string Format)
        {
            return datetime.ToString(UserWebResource.LB_Format_Date);
        }
        public static string GetMonth(int month)
        {
            switch (month)
            {
                case 1: return UserWebResource.LB_January;
                case 2: return UserWebResource.LB_February;
                case 3: return UserWebResource.LB_March;
                case 4: return UserWebResource.LB_April;
                case 5: return UserWebResource.LB_May;
                case 6: return UserWebResource.LB_June;
                case 7: return UserWebResource.LB_July;
                case 8: return UserWebResource.LB_August;
                case 9: return UserWebResource.LB_September;
                case 10: return UserWebResource.LB_October;
                case 11: return UserWebResource.LB_November;
                default: return UserWebResource.LB_December;
            }
        }
    }

    public static class MyExtensions
    {
        public static bool HasData<T>(this List<T> myList)
        {
            if (myList != null && myList.Count > 0)
                return true;
            return false;
        }

        public static bool HasData<T>(this T[] myArr)
        {
            if (myArr != null && myArr.Length > 0)
                return true;
            return false;
        }

        public static string DateTimeQuestToString(this DateTime? dateTime, string format = "HH:mm dd/MM/yyyy")
        {
            if (dateTime != null)
                return dateTime.Value.ToString(format);

            return string.Empty;
        }

        public static string DateTimeQuestToStringNow(this DateTime? dateTime, string format = "HH:mm dd/MM/yyyy")
        {
            if (dateTime != null)
            {
                if (dateTime.Value.Date == DateTime.Now.Date)
                    return dateTime.Value.ToString("HH:mm");
                else
                    return dateTime.Value.ToString(format);
            }

            return string.Empty;
        }
    }
}