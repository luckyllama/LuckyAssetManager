using System.Collections.Generic;
using Lucky.AssetManager.Assets.AssetReaders;

namespace Lucky.AssetManager.Assets {
    public interface IAsset {
        string Path { get; set; }
        IE.Equality ConditionalEquality { get; set; }
        IE.Version ConditionalBrowser { get; set; }
        bool IgnoreProcessing { get; set; }
        IAssetReader Reader { get; set; }
        bool IsConditional { get; }
        IDictionary<string, string> AlternatePaths { get; set; }
        bool OnLayoutPage { get; set; }
        bool IsInMemory { get; }

        IAssetKey Key { get; }
        string CurrentPath { get; }
        string CurrentFilePath { get; }
        bool CurrentPathIsExternal { get; }

        bool IsProcessable { get; }
    }
}