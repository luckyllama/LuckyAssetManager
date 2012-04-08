using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Configuration;
using Lucky.AssetManager.Processors;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Processors {

    [TestFixture]
    public class CssRelativePathProcessorTests {


        private Mock<IAbsoluteUrlManager> _urlManager;

        private HttpContextBase _context;
        private IAssetManagerSettings _settings;
        private string _fakeAbsoluteUrlPrefix = "http://FAKE-O.com/";

        [TestFixtureSetUp]
        public void FixtureInit() {
            var cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns("test-alt-path");
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
        }

        private static IAssetReader GetReader(string content) {
            return new MemoryAssetReader(new[] { "filePath1", "filePath2" }, content);
        }

        [SetUp]
        public void Setup() {
            _urlManager = new Mock<IAbsoluteUrlManager>();
            _urlManager.Setup(man => man.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string path, string relativeUrl) => _fakeAbsoluteUrlPrefix + relativeUrl);
        }

        #region IgnoredAssets

        [Test]
        public void IgnoredProcessing_DoesNotProcess() {
            var reader = GetReader("body { background: url(a/url.png); }");
            var asset = new CssAsset(_context, _settings) { Path = "", IgnoreProcessing = true, Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            var results = processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.That(ReferenceEquals(results.Single().Reader, reader));
            Assert.That(asset.Reader.Content, Is.Not.ContainsSubstring(_fakeAbsoluteUrlPrefix));
        }

        [Test]
        public void JavascriptAsset_DoesNotProcess() {
            var reader = GetReader("body { background: url(a/url.png); }");
            var asset = new JavascriptAsset(_context, _settings) { Path = "", IgnoreProcessing = true, Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            var results = processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.That(ReferenceEquals(results.Single().Reader, reader));
            Assert.That(asset.Reader.Content, Is.Not.ContainsSubstring(_fakeAbsoluteUrlPrefix));
        }

        #endregion IgnoredAssets

        [Test]
        public void RelativePath_IsProcessed() {
            var reader = GetReader("body { background: url(a/url.png); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.That(asset.Reader.Content, Contains.Substring(_fakeAbsoluteUrlPrefix));
        }

        [Test]
        public void RootPath_IsNotProcessed() {
            var reader = GetReader("body { background: url(/a/url.png); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.That(asset.Reader.Content, Is.Not.ContainsSubstring(_fakeAbsoluteUrlPrefix));
        }

        [Test]
        public void AbsolutePath_IsNotProcessed() {
            var reader = GetReader("body { background: url(http://domain.com/a/url.png); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.That(asset.Reader.Content, Is.Not.ContainsSubstring(_fakeAbsoluteUrlPrefix));
        }

        [Test]
        public void MultiplePaths_AreProcessed() {
            var reader = GetReader("body { background: url(a/url.png); background: url(b/url.png); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            Assert.That(asset.Reader.Content, Contains.Substring(_fakeAbsoluteUrlPrefix));
        }

        [Test]
        public void ProcessedWithSingleQuotes_KeepQuotes() {
            var reader = GetReader("body { background: url('a/url.png'); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.That(asset.Reader.Content, Contains.Substring("url('"));
            Assert.That(asset.Reader.Content, Contains.Substring("')"));
        }

        [Test]
        public void ProcessedWithDoubleQuotes_KeepQuotes() {
            var reader = GetReader("body { background: url(\"a/url.png\"); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.That(asset.Reader.Content, Contains.Substring("url(\""));
            Assert.That(asset.Reader.Content, Contains.Substring("\")"));
        }

        [Test]
        public void ProcessedWithMixedQuotes_KeepFirstQuoteStyle() {
            var reader = GetReader("body { background: url('a/url.png\"); }");
            var asset = new CssAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new CssRelativePathProcessor(_urlManager.Object);

            processer.Process(new[] { asset });

            _urlManager.Verify(manager => manager.GetAbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.That(asset.Reader.Content, Contains.Substring("url('"));
            Assert.That(asset.Reader.Content, Contains.Substring("')"));
        }
    }
}
