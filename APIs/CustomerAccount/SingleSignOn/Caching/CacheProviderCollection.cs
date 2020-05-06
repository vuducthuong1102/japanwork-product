using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Configuration.Provider;

using SingleSignOn.Caching.Providers;

namespace SingleSignOn.Caching
{
    public class CacheProviderCollection : ProviderCollection
    {
        new public CacheProviderBase this[string name]
        {
            get { return (CacheProviderBase)base[name]; }
        }
    }
}
