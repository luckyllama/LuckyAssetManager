using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.Html;
using Lucky.AssetManager.Configuration;
using Lucky.AssetManager.Processors;

namespace Lucky.AssetManager {
    public interface IAssetOutputManager {
        string BuildHtml(IEnumerable<IAsset> assets);
    }

    /// <summary>
    /// Manages the building, combining, and minimizing, of an enumeration of assets.
    /// </summary>
    internal class AssetOutputManager : IAssetOutputManager {
        private readonly IHtmlBuilder _htmlBuilder;
        private readonly IEnumerable<IProcessor> _processors;
        private readonly IAssetManagerSettings _settings;

        public AssetOutputManager(IHtmlBuilder htmlBuilder, IEnumerable<IProcessor> processors, IAssetManagerSettings settings) {
            _htmlBuilder = htmlBuilder;
            _settings = settings;
            _processors = processors;
            if (_processors == null) {
                _processors = new List<IProcessor>();
            }
        }

        public string BuildHtml(IEnumerable<IAsset> assets) {
            var processedAssets = assets;
            if (_processors != null && _processors.Any()) {
                processedAssets = _processors.Aggregate(assets, (current, processor) => processor.Process(current));
            }
            return BuildLinks(processedAssets);
        }

        private string BuildLinks(IEnumerable<IAsset> assets) {
            var result = new StringBuilder();
            foreach (IAsset asset in assets.OrderByDescending(a => a.OnLayoutPage)) {
                if (asset.IsInMemory) {
                    var key = CacheContent(asset);
                    if (_settings.Debug) {
                        BuildDebugHtmlComment(result, asset);
                    }
                    result.AppendLine(_htmlBuilder.BuildHandlerLink(key, asset));
                } else {
                    result.AppendLine(_htmlBuilder.BuildLink(GetAssetUrl(asset), asset));
                }
            }
            return result.ToString();
        }

        private static string CacheContent(IAsset asset) {
            var key = asset.GetHashCode().ToString(CultureInfo.InvariantCulture);
            if (MemoryCache.Default.Contains(key) == false) {
                MemoryCache.Default.Add(new CacheItem(key, asset.Reader.Content), asset.Reader.CacheItemPolicy);
            }
            return key;
        }

        private static string GetAssetUrl(IAsset asset) {
            if (asset.CurrentPath.Contains("://")) {
                return asset.CurrentPath;
            }
            return VirtualPathUtility.ToAbsolute(asset.CurrentPath);
        }

        private static void BuildDebugHtmlComment(StringBuilder builder, IEnumerable<IAsset> assets) {
            builder.AppendLine();
            builder.AppendLine("<!-- Assets:");
            foreach (string filePath in assets.SelectMany(asset => asset.Reader.AssociatedFilePaths)) {
                builder.AppendLine(filePath);
            }
            builder.AppendLine(" -->");
        }

        private static void BuildDebugHtmlComment(StringBuilder builder, IAsset assets) {
            BuildDebugHtmlComment(builder, new List<IAsset> { assets });
        }
    }
}
