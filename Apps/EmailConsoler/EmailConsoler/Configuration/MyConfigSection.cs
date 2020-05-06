using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EmailConsoler.Configuration
{
    public class MyConfigSection : ConfigurationSection
    {
        public MyConfigSection()
        {

        }

        [ConfigurationProperty("MyConfigs")]
        public MyConfigElementCollection LogFolders
        {
            get
            {
                return this["MyConfigs"] as MyConfigElementCollection;
            }
        }

        //public static MyConfigSection GetConfigSection()
        //{
        //    return ConfigurationSettings.GetConfig("MySection") as MyConfigSection;
        //}
    }
}
