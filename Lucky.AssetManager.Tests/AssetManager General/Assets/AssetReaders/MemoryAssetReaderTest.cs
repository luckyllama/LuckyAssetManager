using System.Linq;
using Lucky.AssetManager.Assets.AssetReaders;
using System.Collections.Generic;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Assets.AssetReaders {
    
    /// <summary>
    ///This is a test class for MemoryAssetReader and is intended
    ///to contain all MemoryAssetReader Unit Tests
    ///</summary>
    [TestFixture]
    public class MemoryAssetReaderTest {

        [TestFixtureSetUp]
        public void FixtureInit() {}

        [SetUp]
        public void Setup() {}

        #region Constructor

        [Test]
        public void Constructor_FilePaths_SetsAssociatedFilePaths() {
            var paths = new List<string> { "a-path.css" };
            var reader = new MemoryAssetReader(paths, "content");
            Assert.That(reader.AssociatedFilePaths.Any());
            Assert.That(reader.AssociatedFilePaths.Count(), Is.EqualTo(1));
            Assert.That(reader.AssociatedFilePaths.First(), Is.EqualTo("a-path.css"));
        }

        #endregion Constructor

        #region Content

        [Test]
        public void Content_ReturnsContent() {
            var paths = new List<string> { "a-path.css" };
            var reader = new MemoryAssetReader(paths, "test-content");
            Assert.That(reader.Content, Is.EqualTo("test-content"));
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
