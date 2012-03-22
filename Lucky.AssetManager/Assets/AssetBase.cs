using System;
using System.Collections.Generic;
using System.Web;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Configuration;

namespace Lucky.AssetManager.Assets {
    [Serializable]
    internal abstract class AssetBase : IAsset {

        protected HttpContextBase _context;
        protected IAssetManagerSettings _settings;

        protected AssetBase(HttpContextBase context, IAssetManagerSettings settings) {
            _context = context;
            _settings = settings;
            AlternatePaths = new Dictionary<string, string>();
        }

        public string Path { get; set; }
        public IE.Equality ConditionalEquality { get; set; }
        public IE.Version ConditionalBrowser { get; set; }
        public IAssetReader Reader { get; set; }
        public bool IgnoreProcessing { get; set; }

        public bool IsConditional {
            get { return ConditionalBrowser != IE.Version.All; }
        }

        public IDictionary<string, string> AlternatePaths { get; set; }
        public bool OnLayoutPage { get; set; }

        public bool IsInMemory {
            get { return Reader is MemoryAssetReader; }
        }

        public abstract IAssetKey Key { get; }

        public string CurrentPath { 
            get {
                if (!string.IsNullOrWhiteSpace(CurrentAlternateName) && AlternatePaths.ContainsKey(CurrentAlternateName)) {
                    return AlternatePaths[CurrentAlternateName];
                }
                return Path;
            }
        }

        public abstract string CurrentAlternateName { get; }

        public string CurrentFilePath {
            get { return CurrentPathIsExternal ? null : _context.Request.MapPath(CurrentPath); }
        }

        public bool CurrentPathIsExternal {
            get { return CurrentPath.Contains("://"); }
        }

        public bool IsProcessable {
            get { return CurrentPathIsExternal == false && IgnoreProcessing == false; }
        }
    }
}
