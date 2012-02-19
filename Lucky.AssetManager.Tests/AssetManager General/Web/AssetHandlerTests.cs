using System;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Web;
using Lucky.AssetManager.Web;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Web {

    [TestFixture]
    public class AssetHandlerTests {
        
        private string _lastContentTypeSet;
        private string _lastResponseWritten = string.Empty;

        [TestFixtureSetUp]
        public void FixtureInit() {}

        private HttpContextBase CreateContext(string key = "1234", string type = "Css") {
            var @params = new NameValueCollection();
            if (key != null) {
                @params.Add("key", key);
            }
            if (type != null) {
                @params.Add("type", type);
            }

            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(r => r.Params)
                .Returns(@params);

            var mockResponse = new Mock<HttpResponseBase>();
            mockResponse.SetupSet(r => r.ContentType)
                .Callback(s => _lastContentTypeSet = s);
            mockResponse.Setup(r => r.Write(It.IsAny<string>()))
                .Callback((string s) => _lastResponseWritten = s);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request).Returns(mockRequest.Object);
            context.Setup(c => c.Response).Returns(mockResponse.Object);

            return context.Object;
        }

        private static ObjectCache CreateCache(string key = "1234", string content = "test-content") {

            var cache = new Mock<ObjectCache>();
            cache.Setup(c => c.Contains(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((cacheKey, x) => cacheKey == key);
            cache.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((cacheKey, x) => cacheKey == key ? content : "");

            return cache.Object;
        }

        #region ProcessRequest

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "'key'", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_NoParamKey_ThrowsException() {
            var context = CreateContext(key: null);
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "'key'", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_EmptyParamKey_ThrowsException() {
            var context = CreateContext(key: "");
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "'key'", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_WhiteSpaceParamKey_ThrowsException() {
            var context = CreateContext(key: "   ");
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "1234", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_NoCachedKey_ThrowsException() {
            var context = CreateContext();
            var emptyCache = CreateCache(key: null, content: null);

            var handler = new AssetsHandler(emptyCache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "'type'", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_NoParamType_ThrowsException() {
            var context = CreateContext(type: null);
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "'type'", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_EmptyParamType_ThrowsException() {
            var context = CreateContext(type: "");
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "'type'", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_WhiteSpaceParamType_ThrowsException() {
            var context = CreateContext(type: "   ");
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "valid asset type", MatchType = MessageMatch.Contains)]
        public void ProcessRequest_IncorrectParamType_ThrowsException() {
            var context = CreateContext(type: "zzzz");
            var cache = CreateCache();
            
            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);
        }

        [Test]
        public void ProcessRequest_ValidSetup_SetsCssContentType() {
            var context = CreateContext();
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);

            Assert.That(_lastContentTypeSet, Is.EqualTo("text/css"));
        }

        [Test]
        public void ProcessRequest_ValidSetup_SetsJavascriptContentType() {
            var context = CreateContext(type: "Javascript");
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);

            Assert.That(_lastContentTypeSet, Is.EqualTo("application/javascript"));
        }

        [Test]
        public void ProcessRequest_ValidSetup_WritesContent() {
            var context = CreateContext();
            var cache = CreateCache();

            var handler = new AssetsHandler(cache);
            handler.ProcessRequest(context);

            Assert.That(_lastResponseWritten, Is.EqualTo("test-content"));
        }

        #endregion ProcessRequest
    }
}
