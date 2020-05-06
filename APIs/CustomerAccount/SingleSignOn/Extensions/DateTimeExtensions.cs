using System;


using SingleSignOn.Helpers;

namespace SingleSignOn.Extensions
{
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