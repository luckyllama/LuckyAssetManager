using System.Collections.Generic;
using System.Linq;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;

namespace Lucky.AssetManager.Processors {
    public class YuiMinimizeProcessor : IProcessor {
        public IEnumerable<IAsset> Process(IEnumerable<IAsset> assets) {

            var results = assets.Where(a => a.CurrentPathIsExternal || a.IgnoreProcessing).ToList();
            foreach (IAsset asset in assets.Where(a => a.CurrentPathIsExternal == false && a.IgnoreProcessing == false)) {
                string newContent = asset.Reader.Content;
                if (asset is CssAsset && string.IsNullOrEmpty(newContent) == false) {
                    newContent = Yahoo.Yui.Compressor.CssCompressor.Compress(newContent);
                } else {
                    newContent = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(newContent);
                }

                asset.Reader = new MemoryAssetReader(asset.Reader.AssociatedFilePaths, newContent);
                results.Add(asset);
            }

            return results;
        }
    }
}
