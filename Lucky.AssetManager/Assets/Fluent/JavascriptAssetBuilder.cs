using System;
using System.Web;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets.Fluent {

    /// <summary>
    /// A fluent interface to build a JavascriptAsset.
    /// </summary>
    public class JavascriptAssetBuilder : AssetBuilderBase<IJavascriptAssetBuilder>, IJavascriptAssetBuilder {

        internal JavascriptAssetBuilder(string path, IAssetManager assetManager, string assetGroup, bool onLayoutPage)
            : this(new HttpContextWrapper(HttpContext.Current), Lucky.AssetManager.AssetManager.Settings,
                path, assetManager, assetGroup, onLayoutPage) { }

        internal JavascriptAssetBuilder(HttpContextBase context, IAssetManagerSettings settings, 
                string path, IAssetManager assetManager, string assetGroup, bool onLayoutPage) {
            if (string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("The param 'path' must be a non-empty string.");
            }
            if (assetManager == null) {
                throw new ArgumentNullException("assetManager");
            }
            Asset = new JavascriptAsset(context, settings) { Path = path, OnLayoutPage = onLayoutPage };
            Asset.Reader = new FileSystemAssetReader(Asset);
            AssetManager = assetManager;
            AssetGroup = assetGroup;
        }

        public override IJavascriptAssetBuilder WithAlternatePath(string name, string path) {
            AddAlternatePath(name, path);
            return this;
        }

        public override IJavascriptAssetBuilder IgnoreProcessing() {
            SetIgnoreProcessing();
            return this;
        }

        public override IJavascriptAssetBuilder ForIE(IE.Equality equality, IE.Version browser) {
            AddIEConditionals(equality, browser);
            return this;
        }
    }
}
