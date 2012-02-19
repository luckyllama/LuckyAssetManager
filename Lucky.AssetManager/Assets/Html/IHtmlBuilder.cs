namespace Lucky.AssetManager.Assets.Html {
    public interface IHtmlBuilder {
        string BuildLink(string contentUrl, IAsset asset);
        string BuildHandlerLink(string key, IAsset asset);
    }
}