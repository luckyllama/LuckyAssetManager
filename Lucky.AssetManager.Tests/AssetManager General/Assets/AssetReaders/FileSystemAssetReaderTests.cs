using System;
using System.Linq;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.AssetReaders;
using Lucky.AssetManager.Configuration;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Assets.AssetReaders {
    /// <summary>
    ///This is a test class for FileSystemAssetReader and is intended
    ///to contain all FileSystemAssetReader Unit Tests
    ///</summary>
    [TestFixture]
    public class FileSystemAssetReaderTests {

        private CssAsset _cssAsset;
        private JavascriptAsset _jsAsset;
        private HttpContextBase _context;
        private IAssetManagerSettings _setitngs;

        [TestFixtureSetUp]
        public void FixtureInit() {
            var cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns(String.Empty);
            var jsConfig = new Mock<AssetConfiguration>();
            jsConfig.Setup(c => c.AlternateName).Returns(String.Empty);

            var settings = new Mock<IAssetManagerSettings>();
            settings.Setup(s => s.Css).Returns(cssConfig.Object);
            settings.Setup(s => s.Javascript).Returns(jsConfig.Object);

            _setitngs = settings.Object;
            
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.MapPath(It.IsAny<string>()))
                .Returns<string>(s => @"c:\full\path\" + s);
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request).Returns(request.Object);

            _context = context.Object;
        }

        [SetUp]
        public void Setup() {
            _cssAsset = new CssAsset(_context, _setitngs);
            _jsAsset = new JavascriptAsset(_context, _setitngs);
        }

        #region Constructor

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void Constructor_NullAsset_ThrowsException() {
            new FileSystemAssetReader(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Constructor_CssAssetWithoutPath_ThrowsException() {
            new FileSystemAssetReader(_cssAsset);
        }

        [Test]
        public void Constructor_CssAssetWithPath_SetsAssociatedFilePaths() {
            _cssAsset.Path = "a-path.css";
            var reader = new FileSystemAssetReader(_cssAsset);
            Assert.That(reader.AssociatedFilePaths.Any());
            Assert.That(reader.AssociatedFilePaths.Count(), Is.EqualTo(1));
            Assert.That(reader.AssociatedFilePaths.First(), Contains.Substring("a-path.css"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Constructor_JsAssetWithoutPath_ThrowsException() {
            new FileSystemAssetReader(_jsAsset);
        }

        [Test]
        public void Constructor_JsAssetWithPath_SetsAssociatedFilePaths() {
            _jsAsset.Path = "a-path.js";
            var reader = new FileSystemAssetReader(_jsAsset);
            Assert.That(reader.AssociatedFilePaths.Any());
            Assert.That(reader.AssociatedFilePaths.Count(), Is.EqualTo(1));
            Assert.That(reader.AssociatedFilePaths.First(), Contains.Substring("a-path.js"));
        }

        #endregion Constructor

        #region Content

        [Test]
        public void Content_ReturnsContent() {
            Assert.Inconclusive("need to mock stream reader");
        }

        #endregion Content

        #region CacheItemPolicy

        [Test]
        public void CacheItemPolicy_CssAsset_ReturnsPolicyWithAssociatedFilePaths() {
            //_cssAsset.Path = "a-path.css";
            //var reader = new FileSystemAssetReader(_cssAsset);
            //Assert.That(reader.CacheItemPolicy.ChangeMonitors.Any());
            //Assert.That(reader.CacheItemPolicy.ChangeMonitors.Count, Is.EqualTo(1));
            //var monitor = reader.CacheItemPolicy.ChangeMonitors.First();
            //Assert.That(monitor is HostFileChangeMonitor);
            //Assert.That((monitor as HostFileChangeMonitor).FilePaths.Any());
            //Assert.That((monitor as HostFileChangeMonitor).FilePaths.Count(), Is.EqualTo(1));
            //Assert.That((monitor as HostFileChangeMonitor).FilePaths.First(), Contains.Substring("a-path.css"));
            Assert.Inconclusive("need to mock HostFileChangeMonitor");
        }

        #endregion CacheItemPolicy
    }
}
