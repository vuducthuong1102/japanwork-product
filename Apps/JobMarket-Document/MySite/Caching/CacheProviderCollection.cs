using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Configuration.Provider;

using MySite.Caching.Providers;

namespace MySite.Caching
{
    public class CacheProviderCollection : ProviderCollection
    {
        new public CacheProviderBase this[string name]
        {
            get { return (CacheProviderBase)base[name]; }
        }
    }
}
