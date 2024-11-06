

namespace Northwind.Utils.Caching
{
    public class ImageCachingOptions
    {
        public string CacheDirectory { get; set; }
        public int MaxCacheCount { get; set; }
        public TimeSpan CacheExpirationTime { get; set; }
    }

}
