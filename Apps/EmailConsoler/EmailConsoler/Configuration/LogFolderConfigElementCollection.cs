using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EmailConsoler.Configuration
{
    public class LogFolderConfigElementCollection : ConfigurationElementCollection
    {
        public LogFolderConfigElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as LogFolderConfigElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LogFolderConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogFolderConfigElement)(element)).folder;
        }
    }

}
