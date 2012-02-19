using System;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets.Html {

    /// <summary>
    /// Builds the html strings to link to css files/handler.
    /// </summary>
    internal class CssHtmlBuilder : HtmlBuilderBase {
        protected override AssetType AssetType {
            get { return AssetType.Css; }
        }

        protected override string CreatLink(string contentUrl, IAsset asset) {
            var cssAsset = asset as CssAsset;
            if (cssAsset == null) {
                throw new ArgumentException("The param 'asset' must be of type CssAsset and not null.");
            }
            return String.Format(Constants.CssTemplate, contentUrl, cssAsset.Media);
        }
    }

}
