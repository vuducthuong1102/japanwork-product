using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;


namespace ApiJobMarket.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>Serializes the object to a JSON string.</summary>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object value, bool isCamelCase = true)
        {
            if (isCamelCase)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                };

                return JsonConvert.SerializeObject(value, settings);
            }
            else
            {
                return JsonConvert.SerializeObject(value);
            }
        }

        public static List<int> AddUniqueNumberToList(this List<int> list, int newNumber)
        {
            if(list != null && list.Count > 0)
            {
                var currentNum = list.Where(x => x == newNumber).FirstOrDefault();
                if(currentNum == 0)
                {
                    list.Add(newNumber);
                }
            }
            else
            {
                list = new List<int>();
                list.Add(newNumber);
            }

            return list;
        }
    }
}