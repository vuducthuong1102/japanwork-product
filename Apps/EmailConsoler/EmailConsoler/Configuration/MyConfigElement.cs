using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;


namespace EmailConsoler.Configuration
{
    public class MyConfigElement : System.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("folder", IsRequired = true)]
        public string folder
        {
            get
            {
                return this["folder"] as string;
            }
        }

        [ConfigurationProperty("speedms", IsRequired = true)]
        public string speedms
        {
            get
            {
                return this["speedms"] as string;
            }
        }
    }
}
