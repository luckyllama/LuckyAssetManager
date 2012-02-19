using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace Lucky.AssetManager.Assets.AssetReaders {
    [Serializable]
    public class FileSystemAssetReader : IAssetReader {
        private readonly IAsset _asset;
        public IList<string> AssociatedFilePaths { get; private set; }

        public FileSystemAssetReader(IAsset asset) {
            if (asset == null) {
                throw new ArgumentNullException("asset");
            }
            if (string.IsNullOrWhiteSpace(asset.Path)) {
                throw new ArgumentException("The param 'asset' must have a non-empty string for the property 'path'.");
            }
            _asset = asset;
            AssociatedFilePaths = new List<string>{ asset.CurrentFilePath };
        }

        public string Content {
            get {
                using (var file = new StreamReader(_asset.CurrentFilePath)) {
                    return file.ReadToEnd();
                }
            }
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
            hash ^= _asset.Key.GetHashCode();
            return hash;
        }
    }
}
