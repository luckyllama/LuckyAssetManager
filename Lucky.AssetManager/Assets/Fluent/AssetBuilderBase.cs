using System;

namespace Lucky.AssetManager.Assets.Fluent {

    public interface IAssetBuilder<T> where T : IAssetBuilder<T> {
        T WithAlternatePath(string name, string path);
        T ForIE(IE.Equality equality, IE.Version browser);
        T IgnoreProcessing();
        void Add();
        void Remove();
    }

    public interface IJavascriptAssetBuilder : IAssetBuilder<IJavascriptAssetBuilder> { }
    public interface ICssAssetBuilder : IAssetBuilder<ICssAssetBuilder> {
        ICssAssetBuilder ForMediaType(string mediaType);
    }

    public abstract class AssetBuilderBase<T> : IAssetBuilder<T> where T : IAssetBuilder<T> {

        protected IAsset Asset;
        protected IAssetManager AssetManager;
        protected string AssetGroup;

        public abstract T WithAlternatePath(string name, string path);

        protected void AddAlternatePath(string name, string path) {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(path)) {
                if (Asset.AlternatePaths.ContainsKey(name)) {
                    Asset.AlternatePaths[name] = path;
                } else {
                    Asset.AlternatePaths.Add(name, path);
                }
            }
        }

        public abstract T ForIE(IE.Equality equality, IE.Version browser);

        protected void AddIEConditionals(IE.Equality equality, IE.Version browser) {
            Asset.ConditionalEquality = equality;
            Asset.ConditionalBrowser = browser;
        }

        public abstract T IgnoreProcessing();

        protected void SetIgnoreProcessing() {
            Asset.IgnoreProcessing = true;
        }

        public void Add() {
            AssetManager.Add(Asset, AssetGroup);
        }

        public void Remove() {
            AssetManager.Remove(Asset, AssetGroup);
        }
    }
}
