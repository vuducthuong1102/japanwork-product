using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EmailConsoler.Configuration
{
    public class MyConfigElementCollection : ConfigurationElementCollection
    {
        public MyConfigElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as MyConfigElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MyConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MyConfigElement)(element)).folder;
        }
    }

}
