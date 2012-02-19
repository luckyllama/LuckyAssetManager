using System;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Web;

namespace Lucky.AssetManager.Web {
    public class AssetsHandler : IHttpHandler {
        private readonly ObjectCache _cache;

        public AssetsHandler() : this(MemoryCache.Default) {}
        public AssetsHandler(ObjectCache cache) {
            _cache = cache;
        }

        public void ProcessRequest(HttpContext context) {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context) {
            var key = GetKey(context.Request.Params);

            if (_cache.Contains(key) == false) {
                throw new ArgumentException("Cache does not contain key '" + key + "'");
            }
            var assetType = GetAssetType(context.Request.Params);

            switch (assetType) {
                case AssetType.Css:
                    context.Response.ContentType = @"text/css";
                    break;
                case AssetType.Javascript:
                    context.Response.ContentType = @"application/javascript";
                    break;
            }

            TryGzipEncodePage(context);
            context.Response.Write(_cache.Get(key).ToString());
        }

        private static string GetKey(NameValueCollection contextParams) {
            var key = contextParams["key"];
            if (string.IsNullOrWhiteSpace(key)) {
                throw new ArgumentException("The request parameter 'key' must be a non-empty string.");
            }
            return key;
        }

        private AssetType GetAssetType(NameValueCollection contextParams) {
            var type = contextParams["type"];
            if (string.IsNullOrWhiteSpace(type)) {
                throw new ArgumentException("The request parameter 'type' must be a non-empty string.");
            }
            AssetType assetType;
            if (!Enum.TryParse(type, true, out assetType)) {
                throw new ArgumentException("The request parameter 'type' must be a valid asset type.");
            }
            return assetType;
        }

        public bool IsReusable {
            get { return false; }
        }

        public void TryGzipEncodePage(HttpContextBase context) {
            string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
            if (acceptEncoding.Contains("gzip")) {
                context.Response.Filter = new System.IO.Compression.GZipStream(context.Response.Filter, System.IO.Compression.CompressionMode.Compress);
                context.Response.AppendHeader("Content-Encoding", "gzip");
            } else if (acceptEncoding.Contains("deflate")) {
                context.Response.Filter = new System.IO.Compression.DeflateStream(context.Response.Filter, System.IO.Compression.CompressionMode.Compress);
                context.Response.AppendHeader("Content-Encoding", "deflate");
            }
        }
    }
}
