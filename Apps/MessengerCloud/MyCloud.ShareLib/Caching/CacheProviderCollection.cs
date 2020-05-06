using System.Configuration.Provider;

using MyCloud.SharedLib.Caching.Providers;

namespace MyCloud.SharedLib.Caching
{
    public class CacheProviderCollection : ProviderCollection
    {
        new public CacheProviderBase this[string name]
        {
            get { return (CacheProviderBase)base[name]; }
        }
    }
}
