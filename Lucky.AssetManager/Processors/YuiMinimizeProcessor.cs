using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;

namespace Lucky.AssetManager.Processors {
    public class YuiMinimizeProcessor : IProcessor {
        public IEnumerable<IAsset> Process(IEnumerable<IAsset> assets) {

            var results = assets.Where(a => !a.IsProcessable).ToList();

            foreach (IAsset asset in assets.Where(a => a.IsProcessable)) {
                string newContent = asset.Reader.Content;
                if (asset is CssAsset && string.IsNullOrEmpty(newContent) == false) {
                    newContent = Yahoo.Yui.Compressor.CssCompressor.Compress(newContent);
                } else {
                    if (CultureInfo != null) {
                        newContent = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(newContent, true, true, false, false, -1, Encoding.Default, CultureInfo);
                    } else {
                        newContent = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(newContent);
                    }
                }

                asset.Reader = new MemoryAssetReader(asset.Reader.AssociatedFilePaths, newContent);
                results.Add(asset);
            }

            return results;
        }

        public CultureInfo CultureInfo { get; set; }
    }
}
