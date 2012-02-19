using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.Html;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager {
    public interface IAssetManager {
        void Add(IAsset asset, string assetGroup = null);
        void Remove(IAsset asset, string assetGroup = null);
        string Render(AssetType assetType, string assetGroup = null);
    }

    /// <summary>
    /// The AssetManager class manages the list of assets being added or removed as well as returns the rendered html asset string.
    /// </summary>
    internal class AssetManager : IAssetManager {

        private static AssetManagerSettings settings = ConfigurationManager.GetSection("lucky/assetManager") as AssetManagerSettings;
        public static AssetManagerSettings Settings {
            get { return settings; }
        }

        private readonly IAssetOutputManager _cssOutputManager;
        private readonly IAssetOutputManager _jsOutputManager;

        public AssetManager()
            : this(new AssetOutputManager(new CssHtmlBuilder(), Settings.Css.Processors, Settings),
                new AssetOutputManager(new JavascriptHtmlBuilder(), Settings.Javascript.Processors, Settings)) {
        }

        public AssetManager(IAssetOutputManager cssOutputManager, IAssetOutputManager jsOutputManager) {
            _cssOutputManager = cssOutputManager;
            _jsOutputManager = jsOutputManager;
        }

        private IDictionary<string, IList<IAsset>> _cssAssets = new Dictionary<string, IList<IAsset>>();
        private IDictionary<string, IList<IAsset>> _jsAssets = new Dictionary<string, IList<IAsset>>();
        private IDictionary<string, IList<IAsset>> _removedAssets = new Dictionary<string, IList<IAsset>>();

        public void Add(IAsset asset, string assetGroup) {
            if (asset == null) {
                throw new ArgumentNullException("asset");
            }

            if (string.IsNullOrWhiteSpace(assetGroup)) {
                assetGroup = Constants.DefaultAssetGroup;
            }

            var assets = GetAssetDictionary(asset);
            if (assets.ContainsKey(assetGroup)) {
                if (_removedAssets.ContainsKey(assetGroup) && _removedAssets[assetGroup].Contains(asset)) {
                    // because we can't assume correct order of adding/removing assets
                    // if the asset has been marked to remove, don't do anything here.
                } else if (!assets[assetGroup].Contains(asset)) {
                    // if we don't already have this asset, add it
                    assets[assetGroup].Add(asset);
                } else if (asset.OnLayoutPage && assets[assetGroup].Single(a => a.Equals(asset)).OnLayoutPage == false) {
                    // if we do already have this asset but the version we have isn't marked "OnLayoutPage" and the one being added is, keep only the one being added
                    assets[assetGroup][assets[assetGroup].IndexOf(asset)] = asset;
                }
            } else {
                if (_removedAssets.ContainsKey(assetGroup) && _removedAssets[assetGroup].Contains(asset)) {
                    // ignore this asset
                } else {
                    assets.Add(assetGroup, new List<IAsset> { asset });   
                }
            }
        }

        public void Remove(IAsset asset, string assetGroup) {
            if (asset == null) {
                throw new ArgumentNullException("asset");
            }

            if (string.IsNullOrWhiteSpace(assetGroup)) {
                assetGroup = Constants.DefaultAssetGroup;
            }
            var assets = GetAssetDictionary(asset);
            if (assets.ContainsKey(assetGroup) && assets[assetGroup].Contains(asset)) {
                assets[assetGroup].Remove(asset);
            }
            // because we can't assume correct order of adding/removing assets
            // remember that we removed this asset
            if (!_removedAssets.ContainsKey(assetGroup)) {
                _removedAssets.Add(assetGroup, new List<IAsset> { asset });
            } else {
                _removedAssets[assetGroup].Add(asset);
            }
        }

        public string Render(AssetType assetType, string assetGroup) {
            if (string.IsNullOrWhiteSpace(assetGroup)) {
                assetGroup = Constants.DefaultAssetGroup;
            }
            string result = string.Empty;
            if (assetType == AssetType.Css) {
                if (_cssAssets.ContainsKey(assetGroup) && _cssAssets[assetGroup].Any()) {
                    result = _cssOutputManager.BuildHtml(_cssAssets[assetGroup]);
                }
            } else if (assetType == AssetType.Javascript) {
                if (_jsAssets.ContainsKey(assetGroup) && _jsAssets[assetGroup].Any()) {
                    result = _jsOutputManager.BuildHtml(_jsAssets[assetGroup]);
                }
            }
            return result;
        }

        private IDictionary<string, IList<IAsset>> GetAssetDictionary(IAsset asset) {
            if (asset is CssAsset) {
                return _cssAssets;
            }
            if (asset is JavascriptAsset) {
                return _jsAssets;
            }
            throw new ArgumentException("Unknown implementation of IAsset", "asset");
        }

    }

    public enum AssetType {
        Css,
        Javascript
    }

}
