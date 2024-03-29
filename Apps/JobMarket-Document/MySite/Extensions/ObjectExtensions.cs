﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;


namespace MySite
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

        public static string FormatWithComma(this int value)
        {
            var retunStr = string.Empty;
            if(value > 0)
            {
                retunStr = String.Format("{0:n0}", value);
            }

            return retunStr;
        }
    }
}