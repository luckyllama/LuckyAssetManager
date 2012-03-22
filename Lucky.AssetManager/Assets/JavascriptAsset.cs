using System;
using System.Web;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets {
    [Serializable]
    internal class JavascriptAsset : AssetBase {

        public JavascriptAsset() : base(new HttpContextWrapper(HttpContext.Current), AssetManager.Settings) { }

        public JavascriptAsset(HttpContextBase context, IAssetManagerSettings settings) : base(context, settings) { }

        public override IAssetKey Key {
            get {
                return new JavascriptAssetKey {
                    Equality = ConditionalEquality,
                    Browser = ConditionalBrowser,
                    IsExternal = CurrentPathIsExternal
                };
            }
        }

        public override string CurrentAlternateName {
            get { return _settings.Javascript.AlternateName; }
        }

        #region Standard Object Overrides

        public override bool Equals(Object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != typeof(JavascriptAsset)) {
                return false;
            }
            return Equals((JavascriptAsset)obj);
        }

        public bool Equals(JavascriptAsset other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Equals(other.Path, Path) && Equals(other.ConditionalEquality, ConditionalEquality) && Equals(other.ConditionalBrowser, ConditionalBrowser);
        }

        public override int GetHashCode() {
            unchecked {
                int result = (Path != null ? Path.GetHashCode() : 0);
                result = (result * 71) ^ Key.GetHashCode();
                result ^= Reader.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(JavascriptAsset a, JavascriptAsset b) {
            if (ReferenceEquals(a, null)) {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }

        public static bool operator !=(JavascriptAsset a, JavascriptAsset b) {
            return !(a == b);
        }

        #endregion Standard Object Overrides
    }

    internal class JavascriptAssetKey : IAssetKey {
        public IE.Version Browser { get; set; }
        public IE.Equality Equality { get; set; }
        public bool IsExternal { get; set; }
        public AssetType AssetType { get { return AssetType.Javascript; } }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            var otherKey = obj as JavascriptAssetKey;
            if (otherKey == null) {
                return false;
            }
            return AssetType.Equals(otherKey.AssetType) 
                   && Browser == otherKey.Browser
                   && Equality == otherKey.Equality
                   && IsExternal == otherKey.IsExternal;
        }

        public override int GetHashCode() {
            unchecked {
                int result = 79 ^ AssetType.GetHashCode();
                result = (result * 83) ^ Equality.GetHashCode();
                result = (result * 89) ^ Browser.GetHashCode();
                result = (result * 97) ^ IsExternal.GetHashCode();
                return result;
            }
        }
    }
}
