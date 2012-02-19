namespace Lucky.AssetManager.Assets {
    public interface IAssetKey {
        bool IsExternal { get; set; }
        IE.Version Browser { get; set; }
        IE.Equality Equality { get; set; }
        AssetType AssetType { get; }
        // these methods are placed here to help indicate they should be overridden
        bool Equals(object obj);
        int GetHashCode();
    }
}