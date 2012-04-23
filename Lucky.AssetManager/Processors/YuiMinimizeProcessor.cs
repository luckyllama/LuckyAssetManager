using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;

namespace Lucky.AssetManager.Processors {
    public class YuiMinimizeProcessor : IProcessor {

        private readonly ObjectCache _cache; 

        public YuiMinimizeProcessor() {
            // defaults
            CultureInfo = new CultureInfo("en-US");
            Encoding = Encoding.UTF8;
            _cache = AssetManager.Settings.CacheFactory.GetCache();
        }

        public IEnumerable<IAsset> Process(IEnumerable<IAsset> assets) {

            var key = string.Join("_", assets.Select(a => a.Key.GetHashCode().ToString(CultureInfo.InvariantCulture)));
            if (_cache.Contains(key)) {
                return _cache[key] as IEnumerable<IAsset>;
            }

            var results = assets.Where(a => !a.IsProcessable).ToList();

            foreach (IAsset asset in assets.Where(a => a.IsProcessable)) {
                string newContent = asset.Reader.Content;
                if (asset is CssAsset && string.IsNullOrEmpty(newContent) == false) {
                    newContent = Yahoo.Yui.Compressor.CssCompressor.Compress(newContent);
                } else {
                    newContent = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(newContent, true, true, false, false, -1, Encoding, CultureInfo);
                }

                asset.Reader = new MemoryAssetReader(asset.Reader.AssociatedFilePaths, newContent);
                results.Add(asset);
            }

            _cache.Add(key, results, new DateTimeOffset(DateTime.Now.AddDays(1)));

            return results;
        }

        public CultureInfo CultureInfo { get; set; }
        public Encoding Encoding { get; set; }
    }
}
