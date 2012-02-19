using System;
using System.Text;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets.Html {
    internal abstract class HtmlBuilderBase : IHtmlBuilder {

        public string BuildLink(string contentUrl, IAsset asset) {
            if (string.IsNullOrWhiteSpace(contentUrl)) {
                throw new ArgumentException("The param 'contentUrl' must be a non-empty string.");
            }
            var link = CreatLink(contentUrl, asset);
            StringBuilder result = new StringBuilder();
            if (asset.IsConditional) {
                AppendConditionalCommentOpen(result, asset);
            }
            result.AppendLine(link);
            if (asset.IsConditional) {
                AppendConditionalCommentClose(result);
            }
            return result.ToString();
        }

        public string BuildHandlerLink(string key, IAsset asset) {
            if (string.IsNullOrWhiteSpace(key)) {
                throw new ArgumentException("The param 'key' must be a non-empty string.");
            }
            var contentUrl = String.Format(Constants.HttpHandlerUrl, AssetType, key);
            return BuildLink(contentUrl, asset);
        }

        protected abstract string CreatLink(string contentUrl, IAsset asset);

        protected abstract AssetType AssetType { get; }

        private static void AppendConditionalCommentOpen(StringBuilder builder, IAsset asset) {
            var comment = String.Format(Constants.ConditionalCommentOpen, asset.ConditionalEquality.AsString(), asset.ConditionalBrowser.AsString());
            comment.Replace("  ", " "); // get rid of double spacing in the case of Equality.EqualTo and Version.*
            builder.AppendLine(comment);
        }

        private static void AppendConditionalCommentClose(StringBuilder builder) {
            builder.AppendLine(Constants.ConditionalCommentClose);
        }

    }
}
