using System;
using System.Collections.Generic;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Assets.Fluent;
using Lucky.AssetManager.Configuration;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Assets.Fluent {
    [TestFixture]
    public class JavascriptAssetBuilderTests {

        private Mock<IAssetManager> _manager;
        private HttpContextBase _context;
        private IAssetManagerSettings _settings;

        private IAsset _lastAddedAsset;
        private string _lastAddedAssetGroup;

        private IAsset _lastRemovedAsset;
        private string _lastRemovedAssetGroup;

        [TestFixtureSetUp]
        public void FixtureInit() {
            _manager = new Mock<IAssetManager>();
            _manager.Setup(am => am.Add(It.IsAny<IAsset>(), It.IsAny<string>()))
                .Callback((IAsset asset, string group) => {
                    _lastAddedAsset = asset;
                    _lastAddedAssetGroup = group;
                });
            _manager.Setup(am => am.Remove(It.IsAny<IAsset>(), It.IsAny<string>()))
                .Callback((IAsset asset, string group) => {
                    _lastRemovedAsset = asset;
                    _lastRemovedAssetGroup = group;
                });

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
        }

        #region Constructor

        [Test] 
        public void Constructor_ValidParams_ThrowsNoErrors() {
            var builder = new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true);
            Assert.That(builder, Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Constructor_NullPath_ThrowsException() {
            new JavascriptAssetBuilder(_context, _settings, null, _manager.Object, "group", true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Constructor_EmptyPath_ThrowsException() {
            new JavascriptAssetBuilder(_context, _settings, "", _manager.Object, "group", true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "path", MatchType = MessageMatch.Contains)]
        public void Constructor_WhiteSpacePath_ThrowsException() {
            new JavascriptAssetBuilder(_context, _settings, "   ", _manager.Object, "group", true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "assetManager", MatchType = MessageMatch.Contains)]
        public void Constructor_NullAssetManager_ThrowsException() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", null, "group", true);
        }

        [Test]
        public void Constructor_AnyBoolean_SetsOnLayoutPage() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true).Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.OnLayoutPage, Is.EqualTo(true));

            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", false).Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.OnLayoutPage, Is.EqualTo(false));
        }

        #endregion Constructor

        #region Add
            
        [Test]
        public void Add_ValidParams_AddsAsset() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true).Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.Path, Is.EqualTo("valid-path"));
            Assert.That(_lastAddedAsset, Is.InstanceOf(typeof(JavascriptAsset)));
        }

        [Test]
        public void Add_GivenAssetGroup_AddsAssetGroup() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true).Add();
            Assert.That(_lastAddedAssetGroup, Is.Not.Null);
            Assert.That(_lastAddedAssetGroup, Is.EqualTo("group"));
        }

        [Test]
        public void Add_NoAssetGroup_DoesNotAddAssetGroup() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, null, true).Add();
            Assert.That(_lastAddedAssetGroup, Is.Null);
        }

        #endregion Add

        #region Remove

        [Test]
        public void Remove_ValidParams_RemovesAsset() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true).Remove();
            Assert.That(_lastRemovedAsset, Is.Not.Null);
            Assert.That(_lastRemovedAsset.Path, Is.EqualTo("valid-path"));
            Assert.That(_lastRemovedAsset, Is.InstanceOf(typeof(JavascriptAsset)));
        }

        [Test]
        public void Remove_GivenAssetGroup_RemovesAssetGroup() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true).Remove();
            Assert.That(_lastRemovedAssetGroup, Is.Not.Null);
            Assert.That(_lastRemovedAssetGroup, Is.EqualTo("group"));
        }

        [Test]
        public void Remove_NoAssetGroup_DoesNotRemoveAssetGroup() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, null, true).Remove();
            Assert.That(_lastRemovedAssetGroup, Is.Null);
        }

        #endregion Remove

        #region WithAlternatePath

        [Test]
        public void WithAlternatePath_ValidParams_AddsAlternatePath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("name", "path")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            var expectedValue = new KeyValuePair<string, string>("name", "path");
            Assert.That(_lastAddedAsset.AlternatePaths, Contains.Item(expectedValue));
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(1));
        }

        [Test]
        public void WithAlternatePath_NullName_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath(null, "path")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_EmptyName_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("", "path")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_WhitespaceName_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("   ", "path")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_NullPath_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("name", null)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_EmptyPath_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("name", "")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_WhitespacePath_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("name", "   ")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_NotCalled_DoesNotAddPath() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(0));
        }

        [Test]
        public void WithAlternatePath_CalledMultipleTimes_AddsAllAlternatePaths() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .WithAlternatePath("name", "path")
                .WithAlternatePath("name1", "path2")
                .WithAlternatePath("name3", "path4")
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            var expectedValue1 = new KeyValuePair<string, string>("name", "path");
            var expectedValue2 = new KeyValuePair<string, string>("name1", "path2");
            var expectedValue3 = new KeyValuePair<string, string>("name3", "path4");
            Assert.That(_lastAddedAsset.AlternatePaths, Contains.Item(expectedValue1));
            Assert.That(_lastAddedAsset.AlternatePaths, Contains.Item(expectedValue2));
            Assert.That(_lastAddedAsset.AlternatePaths, Contains.Item(expectedValue3));
            Assert.That(_lastAddedAsset.AlternatePaths.Count, Is.EqualTo(3));
        }

        #endregion WithAlternatePath

        #region ForIE

        [Test]
        public void ForIE_ValidParams_AddsConditional() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .ForIE(IE.Equality.GreaterThan, IE.Version.IE5)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.ConditionalEquality, Is.EqualTo(IE.Equality.GreaterThan));
            Assert.That(_lastAddedAsset.ConditionalBrowser, Is.EqualTo(IE.Version.IE5));
        }

        [Test]
        public void ForIE_ValidParams_MakesAssetConditional() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .ForIE(IE.Equality.GreaterThan, IE.Version.IE5)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.IsConditional, Is.EqualTo(true));
        }

        [Test]
        public void ForIE_NotCalled_DoesNotMakeAssetConditional() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.IsConditional, Is.EqualTo(false));
        }

        [Test]
        public void ForIE_CalledMultipleTimes_AddsOnlyLastConditional() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .ForIE(IE.Equality.GreaterThan, IE.Version.IE5)
                .ForIE(IE.Equality.EqualTo, IE.Version.IE6)
                .ForIE(IE.Equality.LessThan, IE.Version.IE7)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.That(_lastAddedAsset.ConditionalEquality, Is.EqualTo(IE.Equality.LessThan));
            Assert.That(_lastAddedAsset.ConditionalBrowser, Is.EqualTo(IE.Version.IE7));
        }

        #endregion ForIE

        #region IgnoreProcessing

        [Test]
        public void IgnoreProcessing_Called_SetsIgnoreProcessingToTrue() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .IgnoreProcessing()
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.IsTrue(_lastAddedAsset.IgnoreProcessing);
        }

        [Test]
        public void IgnoreProcessing_NotCalled_IgnoreProcessingIsFalse() {
            new JavascriptAssetBuilder(_context, _settings, "valid-path", _manager.Object, "group", true)
                .Add();
            Assert.That(_lastAddedAsset, Is.Not.Null);
            Assert.IsFalse(_lastAddedAsset.IgnoreProcessing);
        }

        #endregion IgnoreProcessing
    }
}
