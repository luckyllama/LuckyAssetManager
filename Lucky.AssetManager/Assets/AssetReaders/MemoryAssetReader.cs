using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Lucky.AssetManager.Assets.AssetReaders {
    [Serializable]
    public class MemoryAssetReader : IAssetReader {
        private readonly string _content;
        public IList<string> AssociatedFilePaths { get; private set; }

        public MemoryAssetReader(IList<string> associatedFilePaths, string content) {
            AssociatedFilePaths = associatedFilePaths;
            _content = content;
        }

        public string Content {
            get { return _content; }
        }

        public CacheItemPolicy CacheItemPolicy {
            get {
                var policy = new CacheItemPolicy();
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(AssociatedFilePaths));
                return policy;
            }
        }

        public override int GetHashCode() {
            int hash = AssociatedFilePaths.Aggregate(0, (current, path) => current ^ path.GetHashCode());
            hash ^= _content.GetHashCode();
            return hash;
        }
    }
}