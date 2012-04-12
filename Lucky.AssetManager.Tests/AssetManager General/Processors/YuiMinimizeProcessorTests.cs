using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Configuration;
using Lucky.AssetManager.Processors;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Processors {

    [TestFixture]
    public class YuiMinimizeProcessorTests {

        private HttpContextBase _context;
        private IAssetManagerSettings _settings;

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

        #region Javascript Compression

        [Test]
        public void JavascriptAsset_ForeignCultureInfo_RespectsCultureInfo() {
            var reader = GetReader("var stuff = {foo:0.9, faa:3};");
            const string expected = "var stuff={foo:0,9,faa:3};";

            var asset = new JavascriptAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new YuiMinimizeProcessor { CultureInfo = CultureInfo.CreateSpecificCulture("it-IT") };

            var results = processer.Process(new[] { asset });

            Assert.That(results.Single().Reader.Content, Is.EqualTo(expected));
        }

        [Test]
        public void JavascriptAsset_DefaultCultureInfo_RespectsCultureInfo() {
            var reader = GetReader("var stuff = {foo:0.9, faa:3};");
            const string expected = "var stuff={foo:0.9,faa:3};";

            var asset = new JavascriptAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new YuiMinimizeProcessor { CultureInfo = CultureInfo.CreateSpecificCulture("en-US") };

            var results = processer.Process(new[] { asset });

            Assert.That(results.Single().Reader.Content, Is.EqualTo(expected));
        }

        [Test]
        public void JavascriptAsset_WithCultureInfoSpecialChars_ShowsSpecialChars() {
            var reader = GetReader("var aäßáéíóú = 5;");
            const string expected = "var aäßáéíóú=5;";

            var asset = new JavascriptAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new YuiMinimizeProcessor { CultureInfo = CultureInfo.CreateSpecificCulture("en-US") };

            var results = processer.Process(new[] { asset });

            Assert.That(results.Single().Reader.Content, Is.EqualTo(expected));
        }

        [Test]
        public void JavascriptAsset_NoCultureInfoSpecialChars_ShowsSpecialChars() {
            var reader = GetReader("var aäßáéíóú = 5;");
            const string expected = "var aäßáéíóú=5;";

            var asset = new JavascriptAsset(_context, _settings) { Path = "", Reader = reader };

            var processer = new YuiMinimizeProcessor();

            var results = processer.Process(new[] { asset });

            Assert.That(results.Single().Reader.Content, Is.EqualTo(expected));
        }

        #endregion Javascript Compression
    }
}
