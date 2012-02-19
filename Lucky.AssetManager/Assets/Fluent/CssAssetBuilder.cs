using System;
using System.Web;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets.Fluent {

    /// <summary>
    /// A fluent interface to build a CssAsset.
    /// </summary>
    public class CssAssetBuilder : AssetBuilderBase<ICssAssetBuilder>, ICssAssetBuilder {

        internal CssAssetBuilder(string path, IAssetManager assetManager, string assetGroup, bool onLayoutPage)
            : this(new HttpContextWrapper(HttpContext.Current), Lucky.AssetManager.AssetManager.Settings,
                path, assetManager, assetGroup, onLayoutPage) { }

        internal CssAssetBuilder(HttpContextBase context, IAssetManagerSettings settings,
                string path, IAssetManager assetManager, string assetGroup, bool onLayoutPage) {
            if (string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("The param 'path' must be a non-empty string.");
            }
            if (assetManager == null) {
                throw new ArgumentNullException("assetManager");
            }
            Asset = new CssAsset(context, settings) { Path = path, Media = Constants.DefaultCssMediaType, OnLayoutPage = onLayoutPage };
            Asset.Reader = new FileSystemAssetReader(Asset);
            AssetManager = assetManager;
            AssetGroup = assetGroup;
        }

        public override ICssAssetBuilder WithAlternatePath(string name, string path) {
            AddAlternatePath(name, path);
            return this;
        }

        public override ICssAssetBuilder ForIE(IE.Equality equality, IE.Version browser) {
            AddIEConditionals(equality, browser);
            return this;
        }

        public override ICssAssetBuilder IgnoreProcessing() {
            SetIgnoreProcessing();
            return this;
        }

        public ICssAssetBuilder ForMediaType(string mediaType) {
            if (!string.IsNullOrWhiteSpace(mediaType)) {
                ((CssAsset)Asset).Media = mediaType;
            }
            return this;
        }
    }
}
