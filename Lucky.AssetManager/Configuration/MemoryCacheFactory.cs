using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Lucky.AssetManager.Configuration {
    public class MemoryCacheFactory : ICacheFactory {
        public ObjectCache GetCache() {
            return MemoryCache.Default;
        }
    }
}
