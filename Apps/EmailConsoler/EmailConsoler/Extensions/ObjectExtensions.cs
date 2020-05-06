using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;


namespace EmailConsoler.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>Serializes the object to a JSON string.</summary>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object value, bool isIndented = false, bool isCamelCase = true)
        {
            if (isCamelCase)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                };

                if (isIndented)
                    return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
                else
                    return JsonConvert.SerializeObject(value, settings);
            }
            else
            {

                if (isIndented)
                    return JsonConvert.SerializeObject(value, Formatting.Indented);
                else
                    return JsonConvert.SerializeObject(value);
            }
        }
    }
}
