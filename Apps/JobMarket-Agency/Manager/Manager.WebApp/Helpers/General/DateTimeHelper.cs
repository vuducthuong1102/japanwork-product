using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics.CodeAnalysis;
using Manager.WebApp.Resources;

namespace Manager.WebApp
{
    public static class DateTimeHelper
    {
        private const int SECOND = 1;
        private const int MINUTE = 60 * SECOND;
        private const int HOUR = 60 * MINUTE;
        private const int DAY = 24 * HOUR;
        private const int MONTH = 30 * DAY;

        /// <summary>
        /// Returns a friendly version of the provided DateTime, relative to now. E.g.: "2 days ago", or "in 6 months".
        /// </summary>
        /// <param name="dateTime">The DateTime to compare to Now</param>
        /// <returns>A friendly string</returns>
        public static string GetFriendlyRelativeTime(DateTime dateTime)
        {
            if (DateTime.UtcNow.Ticks == dateTime.Ticks)
            {
                return "Right now!";
            }

            bool isFuture = (DateTime.UtcNow.Ticks < dateTime.Ticks);
            var ts = DateTime.UtcNow.Ticks < dateTime.Ticks ? new TimeSpan(dateTime.Ticks - DateTime.UtcNow.Ticks) : new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks);

            double delta = ts.TotalSeconds;

            if (delta < 1 * MINUTE)
            {
                return isFuture ? "in " + (ts.Seconds == 1 ? "one second" : ts.Seconds + " seconds") : ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE)
            {
                return isFuture ? "in a minute" : "a minute ago";
            }
            if (delta < 45 * MINUTE)
            {
                return isFuture ? "in " + ts.Minutes + " minutes" : ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE)
            {
                return isFuture ? "in an hour" : "an hour ago";
            }
            if (delta < 24 * HOUR)
            {
                return isFuture ? "in " + ts.Hours + " hours" : ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR)
            {
                return isFuture ? "tomorrow" : "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return isFuture ? "in " + ts.Days + " days" : ts.Days + " days ago";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return isFuture ? "in " + (months <= 1 ? "one month" : months + " months") : months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return isFuture ? "in " + (years <= 1 ? "one year" : years + " years") : years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }

    //public class DateHelpers
    //{
    //    public static string FormatToString(DateTime datetime)
    //    {
    //        var minutes = (int)(DateTime.Now - datetime).TotalMinutes;
    //        if (minutes < 60)
    //        {
    //            if (minutes < 1)
    //            {
    //                return 1.ToString() + ManagerResource.LB_Minute_m;
    //            }
    //            else
    //            {
    //                return minutes.ToString() + ManagerResource.LB_Minute_m;
    //            }
    //        }
    //        var hour = (int)(DateTime.Now - datetime).TotalHours;
    //        if (hour < 24)
    //        {
    //            return hour.ToString() + ManagerResource.LB_Hour_m;
    //        }

    //        var day = (int)(DateTime.Now - datetime).TotalDays;
    //        if (day < 7)
    //        {
    //            return day.ToString() + ManagerResource.LB_Day_m;
    //        }

    //        var week = (int)(day / 7);
    //        if (week <= 52)
    //        {
    //            return week.ToString() + ManagerResource.LB_Week_m;
    //        }

    //        var year = (int)(day / 365);

    //        return year.ToString() + ManagerResource.LB_Year_m;
    //    }

    //    public static string FormatToStringTitle(DateTime datetime, bool isUtc = true)
    //    {
    //        if (isUtc)
    //        {
    //            datetime = datetime.ToLocalTime();
    //        }

    //        var minutes = (int)(DateTime.Now - datetime).TotalMinutes;
    //        if (minutes < 60)
    //        {
    //            if (minutes < 1)
    //            {
    //                return ManagerResource.LB_JustNow;
    //            }
    //            else
    //            {
    //                return minutes.ToString() + " " + ManagerResource.LB_Minute + GetManyNumber(minutes);
    //            }
    //        }
    //        var hour = (int)(DateTime.Now - datetime).TotalHours;
    //        if (hour < 24)
    //        {
    //            return hour.ToString() + " " + ManagerResource.LB_Hour + GetManyNumber(hour);
    //        }
    //        var day = (int)(DateTime.Now - datetime).TotalDays;
    //        if (day == 1)
    //        {
    //            return ManagerResource.LB_Yesterday + " " + ManagerResource.LB_At + " " + datetime.ToString("HH:mm");
    //        }
    //        else if (day < 365)
    //        {

    //            return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + ManagerResource.LB_At + " " + datetime.ToString("HH:mm");
    //        }
    //        else
    //        {
    //            return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + datetime.Month.ToString() + datetime.Year.ToString();
    //        }
    //    }

    //    public static string FormatToStringTitle(DateTime? datetime, bool isUtc = true)
    //    {
    //        if (datetime == null)
    //            return string.Empty;
    //        if (isUtc)
    //        {
    //            datetime = datetime.Value.ToLocalTime();
    //        }

    //        var minutes = (int)(DateTime.Now - datetime.Value).TotalMinutes;
    //        if (minutes < 60)
    //        {
    //            if (minutes < 1)
    //            {
    //                return ManagerResource.LB_JustNow;
    //            }
    //            else
    //            {
    //                return minutes.ToString() + " " + ManagerResource.LB_Minute + GetManyNumber(minutes);
    //            }
    //        }
    //        var hour = (int)(DateTime.Now - datetime.Value).TotalHours;
    //        if (hour < 24)
    //        {
    //            return hour.ToString() + " " + ManagerResource.LB_Hour + GetManyNumber(hour);
    //        }
    //        var day = (int)(DateTime.Now - datetime.Value).TotalDays;
    //        if (day == 1)
    //        {
    //            return ManagerResource.LB_Yesterday + " " + ManagerResource.LB_At + " " + datetime.Value.ToString("HH:mm");
    //        }
    //        else if (day < 365)
    //        {

    //            return datetime.Value.Day.ToString() + " " + GetMonth(datetime.Value.Month) + " " + ManagerResource.LB_At + " " + datetime.Value.ToString("HH:mm");
    //        }
    //        else
    //        {
    //            return datetime.Value.Day.ToString() + " " + GetMonth(datetime.Value.Month) + " " + datetime.Value.Month.ToString() + datetime.Value.Year.ToString();
    //        }
    //    }

    //    public static string FormatToOnlineStringTitle(DateTime datetime, bool isUtc = true)
    //    {
    //        if (isUtc)
    //            datetime = datetime.ToLocalTime();

    //        var minutes = (int)(DateTime.Now - datetime).TotalMinutes;
    //        if (minutes < 60)
    //        {
    //            if (minutes < 1)
    //            {
    //                return ManagerResource.LB_JustNow;
    //            }
    //            else
    //            {
    //                return minutes.ToString() + " " + ManagerResource.LB_Minute + GetManyNumber(minutes);
    //            }
    //        }
    //        var hour = (int)(DateTime.Now - datetime).TotalHours;
    //        if (hour < 24)
    //        {
    //            return hour.ToString() + " " + ManagerResource.LB_Hour + GetManyNumber(hour);
    //        }
    //        var day = (int)(DateTime.Now - datetime).TotalDays;
    //        if (day == 1)
    //        {
    //            return 1 + " " + ManagerResource.LB_Day;
    //        }
    //        else if (day < 365)
    //        {
    //            //return string.Empty;
    //            return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + ManagerResource.LB_At + " " + datetime.ToString("HH:mm");
    //        }
    //        else
    //        {
    //            //return string.Empty;
    //            return datetime.Day.ToString() + " " + GetMonth(datetime.Month) + " " + datetime.Month.ToString() + datetime.Year.ToString();
    //        }
    //    }

    //    public static string FormatToOnlineStringTitle(DateTime? datetime, bool isUtc = true)
    //    {
    //        if (datetime == null)
    //            return string.Empty;

    //        if (isUtc)
    //            datetime = datetime.Value.ToLocalTime();

    //        var minutes = (int)(DateTime.Now - datetime.Value).TotalMinutes;
    //        if (minutes < 60)
    //        {
    //            if (minutes < 1)
    //            {
    //                return string.Empty;
    //            }
    //            else
    //            {
    //                return minutes.ToString() + " " + ManagerResource.LB_Minute + GetManyNumber(minutes);
    //            }
    //        }
    //        var hour = (int)(DateTime.Now - datetime.Value).TotalHours;
    //        if (hour < 24)
    //        {
    //            return hour.ToString() + " " + ManagerResource.LB_Hour + GetManyNumber(hour);
    //        }
    //        var day = (int)(DateTime.Now - datetime.Value).TotalDays;
    //        if (day == 1)
    //        {
    //            return 1 + " " + ManagerResource.LB_Day;
    //        }
    //        else if (day < 365)
    //        {
    //            //return string.Empty;
    //            return datetime.Value.Day.ToString() + " " + GetMonth(datetime.Value.Month) + " " + ManagerResource.LB_At + " " + datetime.Value.ToString("HH:mm");
    //        }
    //        else
    //        {
    //            //return string.Empty;
    //            return datetime.Value.Day.ToString() + " " + GetMonth(datetime.Value.Month) + " " + datetime.Value.Month.ToString() + datetime.Value.Year.ToString();
    //        }
    //    }

    //    public static string GetManyNumber(int number)
    //    {
    //        if (number > 1)
    //        {
    //            return ManagerResource.LB_Number_Many;
    //        }
    //        else
    //        {
    //            return String.Empty;
    //        }
    //    }

    //    public static string FormatToString(DateTime datetime, string Format)
    //    {
    //        return datetime.ToString(ManagerResource.LB_Format_Date);
    //    }
    //    public static string GetMonth(int month)
    //    {
    //        switch (month)
    //        {
    //            case 1: return ManagerResource.LB_January;
    //            case 2: return ManagerResource.LB_February;
    //            case 3: return ManagerResource.LB_March;
    //            case 4: return ManagerResource.LB_April;
    //            case 5: return ManagerResource.LB_May;
    //            case 6: return ManagerResource.LB_June;
    //            case 7: return ManagerResource.LB_July;
    //            case 8: return ManagerResource.LB_August;
    //            case 9: return ManagerResource.LB_September;
    //            case 10: return ManagerResource.LB_October;
    //            case 11: return ManagerResource.LB_November;
    //            default: return ManagerResource.LB_December;
    //        }
    //    }
    //}



    /// <summary>
    /// Returns the absolute DateTime or the Seconds since Unix Epoch, where Epoch is UTC 1970-01-01T0:0:0Z.
    /// </summary>
    public static class EpochTime
    {
        /// <summary>
        /// DateTime as UTV for UnixEpoch
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Per JWT spec:
        /// Gets the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the desired date/time.
        /// </summary>
        /// <param name="datetime">The DateTime to convert to seconds.</param>
        /// <remarks>if dateTimeUtc less than UnixEpoch, return 0</remarks>
        /// <returns>the number of seconds since Unix Epoch.</returns>
        public static long GetIntDate(DateTime datetime)
        {
            DateTime dateTimeUtc = datetime;
            if (datetime.Kind != DateTimeKind.Utc)
            {
                dateTimeUtc = datetime.ToUniversalTime();
            }

            if (dateTimeUtc.ToUniversalTime() <= UnixEpoch)
            {
                return 0;
            }

            return (long)(dateTimeUtc - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Creates a DateTime from epoch time.
        /// </summary>
        /// <param name="secondsSinceUnixEpoch">Number of seconds.</param>
        /// <returns>The DateTime in UTC.</returns>
        public static DateTime DateTime(long secondsSinceUnixEpoch)
        {
            if (secondsSinceUnixEpoch <= 0)
            {
                return UnixEpoch;
            }

            return DateTimeUtil.Add(UnixEpoch, TimeSpan.FromSeconds(secondsSinceUnixEpoch)).ToUniversalTime();
        }
    }



    /// <summary>
    /// Helper class for adding DateTimes and Timespans.
    /// </summary>
    internal static class DateTimeUtil
    {
        /// <summary>
        /// Add a DateTime and a TimeSpan.
        /// The maximum time is DateTime.MaxTime.  It is not an error if time + timespan > MaxTime.
        /// Just return MaxTime.
        /// </summary>
        /// <param name="time">Initial <see cref="DateTime"/> value.</param>
        /// <param name="timespan"><see cref="TimeSpan"/> to add.</param>
        /// <returns><see cref="DateTime"/> as the sum of time and timespan.</returns>
        public static DateTime Add(DateTime time, TimeSpan timespan)
        {
            if (timespan == TimeSpan.Zero)
            {
                return time;
            }

            if (timespan > TimeSpan.Zero && DateTime.MaxValue - time <= timespan)
            {
                return GetMaxValue(time.Kind);
            }

            if (timespan < TimeSpan.Zero && DateTime.MinValue - time >= timespan)
            {
                return GetMinValue(time.Kind);
            }

            return time + timespan;
        }

        /// <summary>
        /// Gets the Maximum value for a DateTime specifying kind.
        /// </summary>
        /// <param name="kind">DateTimeKind to use.</param>
        /// <returns>DateTime of specified kind.</returns>
        public static DateTime GetMaxValue(DateTimeKind kind)
        {
            if (kind == DateTimeKind.Unspecified)
                return new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);

            return new DateTime(DateTime.MaxValue.Ticks, kind);
        }

        /// <summary>
        /// Gets the Minimum value for a DateTime specifying kind.
        /// </summary>
        /// <param name="kind">DateTimeKind to use.</param>
        /// <returns>DateTime of specified kind.</returns>
        public static DateTime GetMinValue(DateTimeKind kind)
        {
            if (kind == DateTimeKind.Unspecified)
                return new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc);

            return new DateTime(DateTime.MinValue.Ticks, kind);
        }
    }



    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static long ToEpochTime(this DateTime dateTime)
        {
            var result = EpochTime.GetIntDate(dateTime);
            return result;
        }


        /// <summary>
        /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
        /// </summary>
        public static DateTime ToDateTimeFromEpoch(this long intDate)
        {
            var result = EpochTime.DateTime(intDate);
            return result;
        }
    }
}