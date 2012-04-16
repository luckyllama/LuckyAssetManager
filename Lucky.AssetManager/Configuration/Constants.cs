namespace Lucky.AssetManager.Configuration {
    internal class Constants {
        public const string DefaultCssMediaType = "all";
        public const string DefaultAssetGroup = "Default";
        public const string ConditionalCommentOpen = "<!--[if {0} {1}]>";
        public const string ConditionalCommentClose = "<![endif]-->";
        public const string HttpHandlerUrl = "assets.axd?type={0}&key={1}";
        public const string CssTemplate = "<link href=\"{0}\" media=\"{1}\" rel=\"stylesheet\" type=\"text/css\" />";
        public const string JavascriptTemplate = "<script src=\"{0}\" type=\"text/javascript\"></script>";
    }
}
