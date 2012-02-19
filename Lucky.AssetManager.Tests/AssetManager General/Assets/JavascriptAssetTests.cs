using System;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Configuration;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Assets {
    [TestFixture]
    public class JavascriptAssetTests {

        private HttpContextBase _context;
        private IAssetManagerSettings _settings;

        [TestFixtureSetUp]
        public void FixtureInit() {
            var cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns(String.Empty);
            var jsConfig = new Mock<AssetConfiguration>();
            jsConfig.Setup(c => c.AlternateName).Returns("test-alt-name");

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

        #region CurrentAlternateName

        [Test]
        public void CurrentAlternateName_ReturnsFromSettings() {
            var asset = new JavascriptAsset(_context, _settings);
            Assert.That(asset.CurrentAlternateName, Is.EqualTo("test-alt-name"));
        }

        #endregion CurrentAlternateName

        #region CurrentPath

        [Test]
        public void CurrentPath_WithAlternatePath_ReturnsCorrectPath() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "original-path" };
            asset.AlternatePaths.Add("test-alt-name", "test-value");
            Assert.That(asset.CurrentAlternateName, Is.EqualTo("test-alt-name"));
            Assert.That(asset.CurrentPath, Is.EqualTo("test-value"));
        }

        [Test]
        public void CurrentPath_WithoutAlternatePath_ReturnsCorrectPath() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "original-path" };
            Assert.That(asset.CurrentAlternateName, Is.EqualTo("test-alt-name"));
            Assert.That(asset.CurrentPath, Is.EqualTo("original-path"));
        }

        [Test]
        public void CurrentPath_WithWrongAlternatePath_ReturnsCorrectPath() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "original-path" };
            asset.AlternatePaths.Add("test-alt-name2", "test-value");
            Assert.That(asset.CurrentAlternateName, Is.EqualTo("test-alt-name"));
            Assert.That(asset.CurrentPath, Is.EqualTo("original-path"));
        }

        #endregion CurrentPath

        #region CurrentFilePath

        [Test]
        public void CurrentFilePath_RelativePath_CallsMapPath() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "original-path" };
            Assert.That(asset.CurrentPath, Is.EqualTo("original-path"));
            Assert.That(asset.CurrentFilePath.StartsWith(@"c:\full\path\"));
        }

        [Test]
        public void CurrentFilePath_ExternalPath_ReturnsUnmodifiedPath() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "http://original-path" };
            Assert.That(asset.CurrentPath, Is.EqualTo("http://original-path"));
            Assert.That(asset.CurrentFilePath, Is.EqualTo(null));
        }

        #endregion CurrentFilePath

        #region CurrentPathIsExternal

        [Test]
        public void CurrentPathIsExternal_ExternalPath_ReturnsTrue() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "http://original-path" };
            Assert.That(asset.CurrentPath, Is.EqualTo("http://original-path"));
            Assert.That(asset.CurrentPathIsExternal, Is.True);
        }

        [Test]
        public void CurrentPathIsExternal_RelativePath_ReturnsFalse() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "original-path" };
            Assert.That(asset.CurrentPath, Is.EqualTo("original-path"));
            Assert.That(asset.CurrentPathIsExternal, Is.False);
        }

        #endregion CurrentPathIsExternal

        #region Equals(object)

        [Test]
        public void Equals_Null_IsNotEqual() {
            var asset = new JavascriptAsset(_context, _settings);
            Assert.IsFalse(asset.Equals(null));
        }

        [Test]
        public void Equals_SameAsset_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings);
            Assert.IsTrue(asset.Equals(asset));
        }

        [Test]
        public void Equals_NonJavascriptAsset_IsNotEqual() {
            var asset = new JavascriptAsset(_context, _settings);
            Assert.IsFalse(asset.Equals("test"));
        }

        #endregion Equals(object)

        #region Equals(JavascriptAsset)

        [Test]
        public void Equals_AssetWithSamePath_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            var equalAsset = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            Assert.IsTrue(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithDifferentPath_IsNotEqual() {
            var asset = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            var equalAsset = new JavascriptAsset(_context, _settings) { Path = "test-path2" };
            Assert.IsFalse(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithSameEquality_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings) { ConditionalEquality = IE.Equality.GreaterThan };
            var equalAsset = new JavascriptAsset(_context, _settings) { ConditionalEquality = IE.Equality.GreaterThan };
            Assert.IsTrue(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithDifferentEquality_IsNotEqual() {
            var asset = new JavascriptAsset(_context, _settings) { ConditionalEquality = IE.Equality.LessThan };
            var equalAsset = new JavascriptAsset(_context, _settings) { ConditionalEquality = IE.Equality.GreaterThan };
            Assert.IsFalse(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithSameBrowser_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            var equalAsset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            Assert.IsTrue(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithDifferentBrowser_IsNotEqual() {
            var asset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            var equalAsset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE6 };
            Assert.IsFalse(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithSelectedAltPath_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            asset.AlternatePaths.Add("test-alt-name", "/other-path.css");
            var equalAsset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            Assert.IsTrue(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithUnselectedAltPath_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            asset.AlternatePaths.Add("unselected-alt-name", "/other-path.css");
            var equalAsset = new JavascriptAsset(_context, _settings) { ConditionalBrowser = IE.Version.IE5 };
            Assert.IsTrue(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithSameProperties_IsEqual() {
            var asset = new JavascriptAsset(_context, _settings) {
                Path = "test-path",
                ConditionalEquality = IE.Equality.GreaterThan,
                ConditionalBrowser = IE.Version.IE5
            };
            var equalAsset = new JavascriptAsset(_context, _settings) {
                Path = "test-path",
                ConditionalEquality = IE.Equality.GreaterThan,
                ConditionalBrowser = IE.Version.IE5
            };
            Assert.IsTrue(asset.Equals(equalAsset));
        }

        [Test]
        public void Equals_AssetWithDifferentProperties_IsNotEqual() {
            var asset = new JavascriptAsset(_context, _settings) {
                Path = "test-path",
                ConditionalEquality = IE.Equality.LessThan,
                ConditionalBrowser = IE.Version.IE5
            };
            var equalAsset = new JavascriptAsset(_context, _settings) {
                Path = "test-path",
                ConditionalEquality = IE.Equality.GreaterThan,
                ConditionalBrowser = IE.Version.IE5
            };
            Assert.IsFalse(asset.Equals(equalAsset));
        }

        #endregion Equals(JavascriptAsset)

        #region Equals Operator Override (==)
            
        [Test]
        public void EqualsOperator_Nulls_AreNotEqual() {
            var asset = new JavascriptAsset(_context, _settings);
            Assert.IsFalse(null == asset);
            Assert.IsFalse(asset == null);
        }

        #endregion Equals Operator Override (==)

        #region Not Equals Operator Override (==)

        [Test]
        public void NotEqualsOperator_Nulls_AreNotEqual() {
            var asset = new JavascriptAsset(_context, _settings);
            Assert.IsTrue(null != asset);
            Assert.IsTrue(asset != null);
        }

        #endregion Not Equals Operator Override (==)
    }
}
