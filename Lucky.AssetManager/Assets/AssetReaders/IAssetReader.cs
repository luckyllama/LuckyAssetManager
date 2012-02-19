using System.Collections.Generic;
using System.Runtime.Caching;

namespace Lucky.AssetManager.Assets.AssetReaders {
    public interface IAssetReader {
        IList<string> AssociatedFilePaths { get; }
        string Content { get; }
        CacheItemPolicy CacheItemPolicy { get; }
    }
}