using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace EmailConsoler.Configuration
{
    public class BaseSettings
    {
        protected Nullable<TValue> GetValue<TValue>(string key) where TValue : struct, IConvertible
        {
            Nullable<TValue> returnValue = null;
            string valueAsString = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(valueAsString) == false)
            {
                returnValue = (TValue)Convert.ChangeType(valueAsString, typeof(TValue));
            }

            return returnValue;
        }

        protected string GetValue(string key, string defaultValue)
        {
            string returnValue = ConfigurationManager.AppSettings[key];
            return returnValue != null ? returnValue : defaultValue;
        }

        protected TValue GetValue<TValue>(string key, TValue defaultValue) where TValue : struct, IConvertible
        {
            Nullable<TValue> returnValue = GetValue<TValue>(key);

            TValue resultValue = (returnValue != null) ? returnValue.Value : defaultValue;

            return resultValue;
        }
    }
}