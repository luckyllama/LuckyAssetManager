using System.Runtime.Caching;

namespace Lucky.AssetManager.Configuration {
    public class MemoryCacheFactory : ICacheFactory {
        public ObjectCache GetCache() {
            return MemoryCache.Default;
        }
    }
}
