using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EmailConsoler.Configuration
{
    public class LogFolderConfigSection : ConfigurationSection
    {
        public LogFolderConfigSection()
        {

        }

        [ConfigurationProperty("LogFolders")]
        public LogFolderConfigElementCollection LogFolders
        {
            get
            {
                return this["LogFolders"] as LogFolderConfigElementCollection;
            }
        }

        //public static LogFolderConfigSection GetConfigSection()
        //{
        //    return ConfigurationSettings.GetConfig("LogFolderSection") as LogFolderConfigSection;
        //}
    }
}
