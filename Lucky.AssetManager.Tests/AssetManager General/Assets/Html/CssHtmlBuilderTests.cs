using System;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.Html;
using Lucky.AssetManager.Configuration;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Assets.Html {
    [TestFixture]
    public class CssHtmlBuilderTests {

        private CssHtmlBuilder _builder;
        private CssAsset _conditionalAsset;
        private CssAsset _conditionalAsset2;
        private CssAsset _asset;
        private HttpContextBase _context;
        private IAssetManagerSettings _settings;

        [TestFixtureSetUp]
        public void FixtureInit() {

            var cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns(String.Empty);
            var jsConfig = new Mock<AssetConfiguration>();
            jsConfig.Setup(c => c.AlternateName).Returns(String.Empty);

            var settings = new Mock<IAssetManagerSettings>();
            settings.Setup(s => s.Css).Returns(cssConfig.Object);
            settings.Setup(s => s.Javascript).Returns(jsConfig.Object);

            _settings = settings.Object;

            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.MapPath(It.IsAny<string>()))
                .Returns<string>(s => @"c:\full\path\" + s);
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request).Returns(request.Object);

            _context = context.Object;

            _builder = new CssHtmlBuilder();
            _conditionalAsset = new CssAsset(context.Object, settings.Object) {
                Path = "~/test-path.css",
                ConditionalBrowser = IE.Version.IE7,
                ConditionalEquality = IE.Equality.LessThan
            };
            _conditionalAsset2 = new CssAsset(context.Object, settings.Object) {
                Path = "~/test-path.css",
                ConditionalBrowser = IE.Version.IE8,
                ConditionalEquality = IE.Equality.EqualTo
            };
            _asset = new CssAsset(context.Object, settings.Object) {
                Path = "~/test-path2.css",
            };
        }

        #region BuildLink

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void BuildLink_NonCssAsset_ThrowsArgumentException() {
            _builder.BuildLink("test", new JavascriptAsset(_context, _settings));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void BuildLink_NullAsset_ThrowsArgumentException() {
            _builder.BuildLink("test", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "contentUrl", MatchType = MessageMatch.Contains)]
        public void BuildLink_NullContentUrl_ThrowsArgumentException() {
            _builder.BuildLink(null, new CssAsset(_context, _settings));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "contentUrl", MatchType = MessageMatch.Contains)]
        public void BuildLink_EmptyContentUrl_ThrowsArgumentException() {
            _builder.BuildLink(null, new CssAsset(_context, _settings));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "contentUrl", MatchType = MessageMatch.Contains)]
        public void BuildLink_WhitespaceContentUrl_ThrowsArgumentException() {
            _builder.BuildLink("    ", new CssAsset(_context, _settings));
        }

        [Test]
        public void BuildLink_ConditionalAsset_HasConditionalOpen() {
            var result = _builder.BuildLink("test", _conditionalAsset);

            const string expectedSubstring = "<!--[if lt IE 7]>";
            Assert.That(result, Contains.Substring(expectedSubstring));
        }

        [Test]
        public void BuildLink_ConditionalAsset_HasOnlyOneConditionalOpen() {
            var result = _builder.BuildLink("test", _conditionalAsset);

            const string expectedSubstring = "<!--[if lt IE 7]>";
            Assert.That(result.IndexOf(expectedSubstring) == result.LastIndexOf(expectedSubstring));
        }

        [Test]
        public void BuildLink_ConditionalAsset_EqualTo_HasConditionalOpen() {
            var result = _builder.BuildLink("test", _conditionalAsset2);

            const string expectedSubstring = "<!--[if IE 8]>";
            Assert.That(result, Contains.Substring(expectedSubstring));
        }

        [Test]
        public void BuildLink_ConditionalAsset_EqualTo_HasOnlyOneConditionalOpen() {
            var result = _builder.BuildLink("test", _conditionalAsset2);

            const string expectedSubstring = "<!--[if  IE 8]>";
            Assert.That(result.IndexOf(expectedSubstring) == result.LastIndexOf(expectedSubstring));
        }

        [Test]
        public void BuildLink_ConditionalAsset_HasConditionalClose() {
            var result = _builder.BuildLink("test", _conditionalAsset);

            const string expectedSubstring = "<![endif]-->";
            Assert.That(result, Contains.Substring(expectedSubstring));
        }

        [Test]
        public void BuildLink_ConditionalAsset_HasOnlyOneConditionalClose() {
            var result = _builder.BuildLink("test", _conditionalAsset);

            const string expectedSubstring = "<![endif]-->";
            Assert.That(result.IndexOf(expectedSubstring) == result.LastIndexOf(expectedSubstring));
        }

        [Test]
        public void BuildLink_Asset_HasCssLink() {
            var result = _builder.BuildLink("test-url", _asset);

            Assert.That(result, Contains.Substring("<link"));
            Assert.That(result, Contains.Substring("href=\"test-url\""));
        }

        #endregion BuildLink

        #region BuildHandlerLink

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void BuildHandlerLink_NonCssAsset_ThrowsArgumentException() {
            _builder.BuildHandlerLink("test", new JavascriptAsset(_context, _settings));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void BuildHandlerLink_NullAsset_ThrowsArgumentException() {
            _builder.BuildHandlerLink("test", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "key", MatchType = MessageMatch.Contains)]
        public void BuildHandlerLink_NullKey_ThrowsArgumentException() {
            _builder.BuildHandlerLink(null, new CssAsset(_context, _settings));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "key", MatchType = MessageMatch.Contains)]
        public void BuildHandlerLink_EmptyKey_ThrowsArgumentException() {
            _builder.BuildHandlerLink(null, new CssAsset(_context, _settings));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "key", MatchType = MessageMatch.Contains)]
        public void BuildHandlerLink_WhitespaceContentUrl_ThrowsArgumentException() {
            _builder.BuildHandlerLink("    ", new CssAsset(_context, _settings));
        }

        [Test]
        public void BuildHandlerLink_ConditionalAsset_HasConditionalOpen() {
            var result = _builder.BuildHandlerLink("test", _conditionalAsset);

            const string expectedSubstring = "<!--[if lt IE 7]>";
            Assert.That(result, Contains.Substring(expectedSubstring));
        }

        [Test]
        public void BuildHandlerLink_ConditionalAsset_HasOnlyOneConditionalOpen() {
            var result = _builder.BuildHandlerLink("test", _conditionalAsset);

            const string expectedSubstring = "<!--[if lt IE 7]>";
            Assert.That(result.IndexOf(expectedSubstring) == result.LastIndexOf(expectedSubstring));
        }

        [Test]
        public void BuildHandlerLink_ConditionalAsset_HasConditionalClose() {
            var result = _builder.BuildHandlerLink("test", _conditionalAsset);

            const string expectedSubstring = "<![endif]-->";
            Assert.That(result, Contains.Substring(expectedSubstring));
        }

        [Test]
        public void BuildHandlerLink_ConditionalAsset_HasOnlyOneConditionalClose() {
            var result = _builder.BuildHandlerLink("test", _conditionalAsset);

            const string expectedSubstring = "<![endif]-->";
            Assert.That(result.IndexOf(expectedSubstring) == result.LastIndexOf(expectedSubstring));
        }

        [Test]
        public void BuildHandlerLink_Asset_HasCssLink() {
            var result = _builder.BuildHandlerLink("test-url", _asset);

            Assert.That(result, Contains.Substring("<link"));
            Assert.That(result, Contains.Substring("href=\"/assets.axd"));
        }

        #endregion BuildHandlerLink
    }
}
