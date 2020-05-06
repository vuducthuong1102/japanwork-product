using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MySite.ShareLibs
{
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

        public static string TimeSpanQuestToString(this TimeSpan? timeSpan, string format = @"hh\:mm")
        {
            if (timeSpan != null)
                return timeSpan.Value.ToString(format);

            return string.Empty;
        }

        public static string FormatWithComma(this int value)
        {
            var retunStr = string.Empty;
            if (value > 0)
            {
                retunStr = String.Format("{0:n0}", value);
            }

            return retunStr;
        }
    }

}
