using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;


namespace EmailConsoler.Configuration
{
    public class LogFolderConfigElement : System.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("item", IsRequired = true)]
        public string item
        {
            get
            {
                return this["item"] as string;
            }
        }

        [ConfigurationProperty("folder", IsRequired = true)]
        public string folder
        {
            get
            {
                return this["folder"] as string;
            }
        }

        [ConfigurationProperty("machine", IsRequired = true)]
        public string machine
        {
            get
            {
                return this["machine"] as string;
            }
        }


        [ConfigurationProperty("serverip", IsRequired = true)]
        public string serverip
        {
            get
            {
                return this["serverip"] as string;
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


        [ConfigurationProperty("app", IsRequired = true)]
        public string app
        {
            get
            {
                return this["app"] as string;
            }
        }
    }
}
