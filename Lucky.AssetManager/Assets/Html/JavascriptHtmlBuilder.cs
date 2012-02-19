using System;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets.Html {

    /// <summary>
    /// Builds the html strings to link to javascript files/handler.
    /// </summary>
    internal class JavascriptHtmlBuilder : HtmlBuilderBase {
        protected override AssetType AssetType {
            get { return AssetType.Javascript; }
        }

        protected override string CreatLink(string contentUrl, IAsset asset) {
            var jsAsset = asset as JavascriptAsset;
            if (jsAsset == null) {
                throw new ArgumentException("The param 'asset' must be of type CssAsset and not null.");
            }
            return String.Format(Constants.JavascriptTemplate, contentUrl);
        }

    }
}
