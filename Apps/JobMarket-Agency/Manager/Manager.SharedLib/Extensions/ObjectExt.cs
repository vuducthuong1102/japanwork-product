using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace Manager.SharedLibs
{
    /// <summary>
    /// http://softwarebydefault.com/2013/02/10/deep-copy-generics/
    /// Deep copy objects using Binary Serialization 
    /// </summary>
    public static class ObjectExt
    {
        public static T DeepCopy<T>(this T objectToCopy)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, objectToCopy);

            memoryStream.Position = 0;
            T returnValue = (T)binaryFormatter.Deserialize(memoryStream);


            memoryStream.Close();
            memoryStream.Dispose();


            return returnValue;
        }
    }

    public static class ObjectExtensions
    {
        public static bool IsAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

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

        public static string DateTimeQuestToString(this DateTime? dateTime, string format = "HH:mm dd/MM/yyyy", bool isUtc = true)
        {
            if (dateTime != null)
            {
                if (isUtc)
                    dateTime = dateTime.Value.ToLocalTime();

                return dateTime.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string DateTimeQuestToLocaleString(this DateTime? dateTime, string currentLang, string format = "dd/MM/yyyy")
        {
            if (dateTime != null)
            {

                if (currentLang == "ja-JP")
                {
                    var jaCulture = new CultureInfo(currentLang);
                    return dateTime.Value.ToLocalTime().ToString(jaCulture.DateTimeFormat.LongDatePattern, jaCulture);
                }
                else
                {
                    return dateTime.Value.ToLocalTime().ToString(format);
                }
            }


            return string.Empty;
        }

        public static string DateTimeQuestToStringNow(this DateTime? dateTime, string format = "HH:mm dd/MM/yyyy", bool isUtc = true)
        {
            if (dateTime != null)
            {
                if (isUtc)
                    dateTime = dateTime.Value.ToLocalTime();

                if (dateTime.Value.Date == DateTime.Now.Date)
                    return dateTime.Value.ToString("HH:mm");
                else
                    return dateTime.Value.ToString(format);
            }

            return string.Empty;
        }

        public static string DateTimeQuestToLocaleStringNow(this DateTime? dateTime, string format = "HH:mm dd/MM/yyyy")
        {
            if (dateTime != null)
            {
                if (dateTime.Value.Date == DateTime.Now.Date)
                    return dateTime.Value.ToLocalTime().ToString("HH:mm");
                else
                    return dateTime.Value.ToLocalTime().ToString(format);
            }

            return string.Empty;
        }

        public static string TimeSpanQuestToString(this TimeSpan? timeSpan, string format = @"hh\:mm")
        {
            if (timeSpan != null)
                return timeSpan.Value.ToString(format);

            return string.Empty;
        }

        public static string FormatWithComma(this int value)
        {
            var retunStr = "0";
            if (value > 0)
            {
                retunStr = String.Format("{0:n0}", value);
            }

            return retunStr;
        }
    }

    public class MyObjectExtensions
    {
        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }
    }
}