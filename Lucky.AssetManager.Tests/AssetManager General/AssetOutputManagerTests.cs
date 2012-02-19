using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Assets.Html;
using Lucky.AssetManager.Configuration;
using Lucky.AssetManager.Processors;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests {

    [TestFixture]
    public class AssetOutputManagerTests {

        private IAssetOutputManager _outputManager;
        private List<HtmlBuilderCall> _htmlBuilderCalls = new List<HtmlBuilderCall>();
        private List<CompressorCall> _compressorCalls = new List<CompressorCall>();
        private HttpContextBase _context;
        private IAssetManagerSettings _settings;

        [TestFixtureSetUp]
        public void FixtureInit() {
            var cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns("test-alt-name");
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

            var builder = new Mock<IHtmlBuilder>();
            builder.Setup(b => b.BuildLink(It.IsAny<string>(), It.IsAny<IAsset>()))
                .Callback<string, IAsset>((url, asset) =>
                    _htmlBuilderCalls.Add(new HtmlBuilderCall {
                        Asset = asset,
                        Url = url,
                        IsHandler = false
                    }))
                    .Returns<string,IAsset>((url, asset) => url);
            builder.Setup(b => b.BuildHandlerLink(It.IsAny<string>(), It.IsAny<IAsset>()))
                .Callback<string, IAsset>((url, asset) =>
                    _htmlBuilderCalls.Add(new HtmlBuilderCall {
                        Asset = asset,
                        Url = url,
                        IsHandler = true
                    }))
                    .Returns<string, IAsset>((url, asset) => url);

            //var compressor = new Mock<ICompressor>();
            //compressor.Setup(c => c.Compress(It.IsAny<string>()))
            //    .Callback<string>(content => _compressorCalls.Add(new CompressorCall {Content = content}))
            //    .Returns<string>(content => content.Replace(" ", ""));

            //var reader = new Mock<IAssetReader>();
            //reader.Setup(r => r.ReadToEnd(It.IsAny<string>()))
            //    .Returns("this is a mock file's contents");

            //var processor = new Mock<IProcessor>();
            //processor.Setup(p => p.Process(It.IsAny<IAsset>(), It.IsAny<string>(), It.IsAny<string>()))
            //    .Returns<IAsset,string,string>((asset, alt, content) => content);

            _outputManager = new AssetOutputManager(builder.Object, null, null);
        }

        [SetUp]
        public void Setup() {
            _htmlBuilderCalls.Clear();
            _compressorCalls.Clear();
        }

        #region Helper Objects
        
        private class HtmlBuilderCall {
            public string Url { get; set; }
            public IAsset Asset { get; set; }
            public bool IsHandler { get; set; }
        }

        private class CompressorCall {
            public string Content { get; set; }
        }

        #endregion Helper Objects

        #region BuildHtml General

        [Test]
        public void BuildHtml_EmptyAssets_ReturnsEmptyString() {
            //var result = _outputManager.BuildHtml(null);
            //Assert.That(result, Is.Empty);
        }

        [Test]
        public void BuildHtml_NeitherMinimizedNorCombined_BuildsNormalLink() {
            var asset = new CssAsset(_context, _settings) { Path = "://test-path" };
            var assets = new List<IAsset> { asset };
            var result = _outputManager.BuildHtml(assets);
            Assert.That(_htmlBuilderCalls.Count, Is.EqualTo(1));
            var call = _htmlBuilderCalls.First();
            Assert.IsFalse(call.IsHandler);
            Assert.That(call.Asset, Is.EqualTo(asset));
            Assert.That(result.Contains(asset.Path));
        }

        #endregion BuildHtml General

        #region BuildHtml Ordering

        [Test]
        public void BuildHtml_OnAndOffLayoutPage_RendersInCorrectOrder() {
            var onLayoutAsset1 = new CssAsset(_context, _settings) { Path = "://onLayout1" };
            var onLayoutAsset2 = new CssAsset(_context, _settings) { Path = "://onLayout2" };
            var notOnLayoutAsset1 = new CssAsset(_context, _settings) { Path = "://notOnLayout1" };
            var notOnLayoutAsset2 = new CssAsset(_context, _settings) { Path = "://notOnLayout2" };
            var assets = new List<IAsset> { notOnLayoutAsset1, onLayoutAsset1, notOnLayoutAsset2, onLayoutAsset2 };

            var result = _outputManager.BuildHtml(assets);
            Assert.That(_htmlBuilderCalls.Count, Is.EqualTo(4));
            var call = _htmlBuilderCalls[0];
            Assert.That(call.Asset, Is.EqualTo(notOnLayoutAsset1));
            call = _htmlBuilderCalls[1];
            Assert.That(call.Asset, Is.EqualTo(onLayoutAsset1));
            call = _htmlBuilderCalls[2];
            Assert.That(call.Asset, Is.EqualTo(notOnLayoutAsset2));
            call = _htmlBuilderCalls[3];
            Assert.That(call.Asset, Is.EqualTo(onLayoutAsset2));
            Assert.That(result.IndexOf(notOnLayoutAsset1.Path), Is.LessThan(result.IndexOf(onLayoutAsset1.Path)));
            Assert.That(result.IndexOf(onLayoutAsset1.Path), Is.LessThan(result.IndexOf(notOnLayoutAsset2.Path)));
            Assert.That(result.IndexOf(notOnLayoutAsset2.Path), Is.LessThan(result.IndexOf(onLayoutAsset2.Path)));
        }

        #endregion BuildHtml Ordering

    }
}
