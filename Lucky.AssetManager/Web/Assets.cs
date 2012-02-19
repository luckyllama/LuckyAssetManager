using System;
using System.Web;
using Lucky.AssetManager.Assets.Fluent;

namespace Lucky.AssetManager.Web {
    public static class Assets {

        public static ICssAssetBuilder MasterCss(string path, string assetGroup = null) {
            return Css(path, assetGroup, onLayoutPage: true);
        }

        public static ICssAssetBuilder Css(string path, string assetGroup = null, bool onLayoutPage = false) {
            return new CssAssetBuilder(path, GetAssetManager(assetGroup), assetGroup, onLayoutPage);
        }

        public static IHtmlString RenderCss(string assetGroup = null) {
            return RenderAsset(AssetType.Css, assetGroup);
        }

        public static IJavascriptAssetBuilder MasterJavascript(string path, string assetGroup = null) {
            return Javascript(path, assetGroup, onLayoutPage: true);
        }

        public static IJavascriptAssetBuilder Javascript(string path, string assetGroup = null, bool onLayoutPage = false) {
            return new JavascriptAssetBuilder(path, GetAssetManager(assetGroup), assetGroup, onLayoutPage);
        }

        public static IHtmlString RenderJavascript(string assetGroup = null) {
            return RenderAsset(AssetType.Javascript, assetGroup);
        }

        private static IHtmlString RenderAsset(AssetType type, string assetGroup = null) {
            return new HtmlString(GetAssetManager(assetGroup).Render(type, assetGroup));
        }

        internal static IAssetManager GetAssetManager(string assetGroup) {
            string instanceName = "AssetManagerInstance";
            if (!String.IsNullOrWhiteSpace(assetGroup)) {
                instanceName += assetGroup;
            }
            var context = HttpContext.Current;
            if (context.Items[instanceName] == null) {
                context.Items[instanceName] = new AssetManager();
            }
            return (IAssetManager)context.Items[instanceName];
        }
    }
}
