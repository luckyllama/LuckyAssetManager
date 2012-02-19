using System;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Configuration;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests.Assets {
    [TestFixture]
    public class JavascriptAssetKeyTests {

        private HttpContextBase _context;
        private IAssetManagerSettings _settings;
        private IAssetManagerSettings _notExternalAltSettings;
        private IAssetManagerSettings _externalAltSettings;

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

            cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns("external");
            jsConfig = new Mock<AssetConfiguration>();
            jsConfig.Setup(c => c.AlternateName).Returns("external");

            settings = new Mock<IAssetManagerSettings>();
            settings.Setup(s => s.Css).Returns(cssConfig.Object);
            settings.Setup(s => s.Javascript).Returns(jsConfig.Object);

            _externalAltSettings = settings.Object;

            cssConfig = new Mock<AssetConfiguration>();
            cssConfig.Setup(c => c.AlternateName).Returns("notExternal");
            jsConfig = new Mock<AssetConfiguration>();
            jsConfig.Setup(c => c.AlternateName).Returns("notExternal");

            settings = new Mock<IAssetManagerSettings>();
            settings.Setup(s => s.Css).Returns(cssConfig.Object);
            settings.Setup(s => s.Javascript).Returns(jsConfig.Object);

            _notExternalAltSettings = settings.Object;
        }

        #region JavascriptAsset.GetKey

        [Test]
        public void JavascriptAsset_GetKey_ThrowsNoErrors() {
            var asset = new JavascriptAsset(_context, _settings) {
                                         Path = "valid-path"
                                     };
            Assert.That(asset.Key, Is.Not.Null);
        }

        [Test]
        public void JavascriptAsset_GetKey_ReturnsCssAssetKey() {
            var asset = new JavascriptAsset(_context, _settings) {
                Path = "valid-path"
            };
            Assert.That(asset.Key as JavascriptAssetKey, Is.Not.Null);
        }

        [Test]
        public void JavascriptAsset_GetKey_GetsCorrectEquality() {
            var asset = new JavascriptAsset(_context, _settings) {
                Path = "valid-path",
                ConditionalEquality = IE.Equality.GreaterThan
            };
            var key = asset.Key;
            Assert.That(key, Is.Not.Null);
            Assert.That(key.Equality, Is.EqualTo(IE.Equality.GreaterThan));
        }

        [Test]
        public void JavascriptAsset_GetKey_GetsCorrectBrowser() {
            var asset = new JavascriptAsset(_context, _settings) {
                Path = "valid-path",
                ConditionalBrowser = IE.Version.IE7
            };
            var key = asset.Key;
            Assert.That(key, Is.Not.Null);
            Assert.That(key.Browser, Is.EqualTo(IE.Version.IE7));
        }

        [Test]
        public void JavascriptAsset_GetKey_GetsCorrectExternalFlag() {
            var asset = new JavascriptAsset(_context, _notExternalAltSettings) {
                Path = "valid-path"
            };
            asset.AlternatePaths.Add("notExternal", "/relative-path.css");
            asset.AlternatePaths.Add("external", "http://www.fullpath.com/file.css");

            var notExternalKey = asset.Key;
            Assert.That(notExternalKey, Is.Not.Null);
            Assert.That(notExternalKey.IsExternal, Is.False);

            asset = new JavascriptAsset(_context, _externalAltSettings) {
                Path = "valid-path"
            };
            asset.AlternatePaths.Add("notExternal", "/relative-path.css");
            asset.AlternatePaths.Add("external", "http://www.fullpath.com/file.css");

            var externalKey = asset.Key;
            Assert.That(externalKey, Is.Not.Null);
            Assert.That(externalKey.IsExternal, Is.True);
        }

        [Test]
        public void JavascriptAsset_GetKey_NonExistantAlternateName_ThrowsNoErrors() {
            var asset = new JavascriptAsset(_context, _notExternalAltSettings) {
                Path = "valid-path"
            };
            var key = asset.Key;
            Assert.That(key, Is.Not.Null);
        }

        #endregion JavascriptAsset.GetKey

        #region GetHashCode

        [Test]
        public void GetHashCode_IdenticalKeys_HaveSameHashCode() {
            var key = new JavascriptAssetKey {
                Browser = IE.Version.IE5,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            var key2 = new JavascriptAssetKey {
                Browser = IE.Version.IE5,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            Assert.AreEqual(key.GetHashCode(), key2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentKeys_HaveDifferentHashCode() {
            var key = new JavascriptAssetKey {
                Browser = IE.Version.IE6,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            var key2 = new JavascriptAssetKey {
                Browser = IE.Version.IE5,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            Assert.AreNotEqual(key.GetHashCode(), key2.GetHashCode());
        }

        [Test]
        public void GetHashCode_AllPropertiesAreTakenIntoAccount() {
            var key = new JavascriptAssetKey();
            var hash1 = key.GetHashCode();
            key.Browser = IE.Version.IE6;
            var hash2 = key.GetHashCode();
            Assert.That(hash1, Is.Not.EqualTo(hash2));
            key.Equality = IE.Equality.LessThan;
            var hash3 = key.GetHashCode();
            Assert.That(hash1, Is.Not.EqualTo(hash3));
            Assert.That(hash2, Is.Not.EqualTo(hash3));
            key.IsExternal = true;
            var hash4 = key.GetHashCode();
            Assert.That(hash1, Is.Not.EqualTo(hash4));
            Assert.That(hash2, Is.Not.EqualTo(hash4));
            Assert.That(hash3, Is.Not.EqualTo(hash4));
        }

        [Test]
        public void GetHashCode_CssAssetKey_HasDifferentHashCodeThanCssAsset() {
            var key = new JavascriptAssetKey { IsExternal = true };
            var cssKey = new CssAssetKey { IsExternal = true };
            Assert.That(key.GetHashCode(), Is.Not.EqualTo(cssKey.GetHashCode()));
        }

        [Test]
        public void GetHashCode_SimilarKeys_HaveDifferentHashCodes() {
            var key1 = new JavascriptAssetKey { Browser = (IE.Version) 2 };
            var key2 = new JavascriptAssetKey { Equality = (IE.Equality) 2};
            Assert.That(key1.GetHashCode(), Is.Not.EqualTo(key2.GetHashCode()));
        }

        #endregion GetHashCode

        #region Equals

        [Test]
        public void Equals_IdenticalKeys_AreEqual() {
            var key1 = new JavascriptAssetKey {
                Browser = IE.Version.IE5,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            var key2 = new JavascriptAssetKey {
                Browser = IE.Version.IE5,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            Assert.IsTrue(key1.Equals(key2));
        }

        [Test]
        public void Equals_DifferentKeys_AreNotEqual() {
            var key1 = new JavascriptAssetKey {
                Browser = IE.Version.IE6,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            var key2 = new JavascriptAssetKey {
                Browser = IE.Version.IE5,
                Equality = IE.Equality.LessThan,
                IsExternal = true
            };
            Assert.IsFalse(key1.Equals(key2));
        }

        [Test]
        public void Equals_AllPropertiesAreTakenIntoAccount() {
            var key1 = new JavascriptAssetKey();
            var key2 = new JavascriptAssetKey();
            Assert.IsTrue(key1.Equals(key2));

            key1.Browser = IE.Version.IE6;
            Assert.IsFalse(key1.Equals(key2));
            key2.Browser = IE.Version.IE6;
            Assert.IsTrue(key1.Equals(key2));

            key1.Equality = IE.Equality.GreaterThan;
            Assert.IsFalse(key1.Equals(key2));
            key2.Equality = IE.Equality.GreaterThan;
            Assert.IsTrue(key1.Equals(key2));

            key1.IsExternal = true;
            Assert.IsFalse(key1.Equals(key2));
            key2.IsExternal = true;
            Assert.IsTrue(key1.Equals(key2));
        }

        [Test]
        public void Equals_NullKey_DoesNotEqualNonNullKey() {
            var key1 = new JavascriptAssetKey();
            Assert.IsFalse(key1.Equals(null));
        }

        [Test]
        public void Equals_CssKey_DoesNotEqualJavascriptKey() {
            var key1 = new JavascriptAssetKey { IsExternal = true };
            var key2 = new CssAssetKey { IsExternal = true };
            Assert.IsFalse(key1.Equals(key2));
        }

        #endregion Equals
    }
}
