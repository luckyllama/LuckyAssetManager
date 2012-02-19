using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucky.AssetManager.Assets;
using Lucky.AssetManager.Configuration;
using Moq;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests {

    [TestFixture]
    public class AssetManagerTests {

        private IAssetOutputManager _cssOutputManager;
        private IAssetOutputManager _jsOutputManager;
        private readonly List<BuildHtmlCall> _buildHtmlCalls = new List<BuildHtmlCall>();
        private AssetManager _assetManager;

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

            var output = new Mock<IAssetOutputManager>();
            output.Setup(o => o.BuildHtml(It.IsAny<IEnumerable<IAsset>>()))
                .Callback<IEnumerable<IAsset>>(
                    (assets) => 
                    _buildHtmlCalls.Add(new BuildHtmlCall {
                        Assets = assets
                }));
            _cssOutputManager = output.Object;
            _jsOutputManager = output.Object;
        }

        [SetUp]
        public void Setup() {
            _buildHtmlCalls.Clear();
            _assetManager = new AssetManager(_cssOutputManager, _jsOutputManager);
        }

        private class BuildHtmlCall {
            public IEnumerable<IAsset> Assets { get; set; }
        }

        #region Add

        [Test]
        [ExpectedException(typeof (ArgumentNullException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void Add_NullAsset_ThrowsException() {
            _assetManager.Add(null, null);
        }

        [Test]
        public void Add_CssAsset_NullGroup_CorrectlyAddsAsset() {
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_JavascriptAsset_NullGroup_CorrectlyAddsAsset() {
            var newAsset = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, null);
            _assetManager.Render(AssetType.Javascript, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_CssAsset_WithGroup_CorrectlyAddsAsset() {
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_JavascriptAsset_WithGroup_CorrectlyAddsAsset() {
            var newAsset = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateCssAsset_SameGroup_OnlyAddsOneAsset() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path" };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateJavascriptAsset_SameGroup_OnlyAddsOneAsset() {
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateCssAssetWithDifferentAltPaths_SameGroup_OnlyAddsOneAsset() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path" };
            newAsset1.AlternatePaths.Add("test-alt-name", "alt-path");
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateJavascriptAssetWithDifferentAltPaths_SameGroup_OnlyAddsOneAsset() {
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            newAsset2.AlternatePaths.Add("test-alt-name", "alt-path");
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateCssAsset_DifferentGroups_AddsTwoAssets() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path" };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset1, "test-group1");
            _assetManager.Add(newAsset2, "test-group2");
            _assetManager.Render(AssetType.Css, "test-group1");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Css, "test-group2");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateJavascriptAsset_DifferentGroups_AddsTwoAssets() {
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset1, "test-group1");
            _assetManager.Add(newAsset2, "test-group2");
            _assetManager.Render(AssetType.Javascript, "test-group1");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Javascript, "test-group2");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Add_DuplicateCssAsset_DifferentOnLayoutPage_SameGroup_OverridesAsset_Order1() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path", OnLayoutPage = false };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path", OnLayoutPage = true };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
            Assert.That(asset.OnLayoutPage, Is.EqualTo(true));
        }

        [Test]
        public void Add_DuplicateCssAsset_DifferentOnLayoutPage_SameGroup_OverridesAsset_Order2() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path", OnLayoutPage = true };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path", OnLayoutPage = false };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
            Assert.That(asset.OnLayoutPage, Is.EqualTo(true));
        }

        [Test]
        public void Add_DuplicateJavascriptAsset_DifferentOnLayoutPage_SameGroup_OverridesAsset_Order1() {
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path", OnLayoutPage = false };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path", OnLayoutPage = true };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
            Assert.That(asset.OnLayoutPage, Is.EqualTo(true));
        }

        [Test]
        public void Add_DuplicateJavascriptAsset_DifferentOnLayoutPage_SameGroup_OverridesAsset_Order2() {
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path", OnLayoutPage = true };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path", OnLayoutPage = false };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
            Assert.That(asset.OnLayoutPage, Is.EqualTo(true));
        }

        [Test]
        public void Add_MultipleCssAsset_SameGroup_CorrectlyAddsAssets() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path1" };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path2" };
            var newAsset3 = new CssAsset(_context, _settings) { Path = "test-path3" };
            var newAsset4 = new CssAsset(_context, _settings) { Path = "test-path4" };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Add(newAsset3, "test-group");
            _assetManager.Add(newAsset4, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(4));
            Assert.That(call.Assets.Count(a => a.Path == "test-path1"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path1"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path2"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path2"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path3"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path3"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path4"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path4"));
        }

        [Test]
        public void Add_MultipleJavascriptAssets_SameGroup_CorrectlyAddsAssets() {
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path1" };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path2" };
            var newAsset3 = new JavascriptAsset(_context, _settings) { Path = "test-path3" };
            var newAsset4 = new JavascriptAsset(_context, _settings) { Path = "test-path4" };
            _assetManager.Add(newAsset1, "test-group");
            _assetManager.Add(newAsset2, "test-group");
            _assetManager.Add(newAsset3, "test-group");
            _assetManager.Add(newAsset4, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(4));
            Assert.That(call.Assets.Count(a => a.Path == "test-path1"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path1"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path2"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path2"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path3"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path3"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path4"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path4"));
        }

        [Test]
        public void Add_MultipleCssAssets_DifferentGroups_CorrectlyAddsAssets() {
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path1" };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path2" };
            var newAsset3 = new CssAsset(_context, _settings) { Path = "test-path3" };
            var newAsset4 = new CssAsset(_context, _settings) { Path = "test-path4" };
            _assetManager.Add(newAsset1, "test-group1");
            _assetManager.Add(newAsset2, "test-group3");
            _assetManager.Add(newAsset3, "test-group1");
            _assetManager.Add(newAsset4, "test-group2");
            _assetManager.Render(AssetType.Css, "test-group1");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(2));
            Assert.That(call.Assets.Count(a => a.Path == "test-path1"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path1"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path3"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path3"));

            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Css, "test-group2");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            Assert.That(call.Assets.Count(a => a.Path == "test-path4"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path4"));

            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Css, "test-group3");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            Assert.That(call.Assets.Count(a => a.Path == "test-path2"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path2"));
        }

        [Test]
        public void Add_MultipleJavascriptAssets_DifferentGroups_CorrectlyAddsAssets() {
            // subtly different
            var newAsset1 = new JavascriptAsset(_context, _settings) { Path = "test-path1" };
            var newAsset2 = new JavascriptAsset(_context, _settings) { Path = "test-path2" };
            var newAsset3 = new JavascriptAsset(_context, _settings) { Path = "test-path3" };
            var newAsset4 = new JavascriptAsset(_context, _settings) { Path = "test-path4" };
            _assetManager.Add(newAsset1, "test-group2");
            _assetManager.Add(newAsset2, "test-group3");
            _assetManager.Add(newAsset3, "test-group3");
            _assetManager.Add(newAsset4, "test-group1");
            _assetManager.Render(AssetType.Javascript, "test-group1");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            Assert.That(call.Assets.Count(a => a.Path == "test-path4"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path4"));

            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Javascript, "test-group2");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            Assert.That(call.Assets.Count(a => a.Path == "test-path1"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path1"));

            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Javascript, "test-group3");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(2));
            Assert.That(call.Assets.Count(a => a.Path == "test-path2"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path2"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path3"), Is.EqualTo(1));
            Assert.IsInstanceOf<JavascriptAsset>(call.Assets.Single(a => a.Path == "test-path3"));
        }

        #endregion Add

        #region Remove 

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "asset", MatchType = MessageMatch.Contains)]
        public void Remove_NullAsset_ThrowsException() {
            _assetManager.Remove(null, null);
        }

        [Test]
        public void Remove_ExisitingCssAsset_ByRef_NullGroup_RemovesOneAsset() {
            // given a css asset that's in the manager under a null asset group,
            // the remove() call (by CssAsset reference) correctly removes this asset
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, null);
            _assetManager.Remove(newAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ExistingCssAsset_ByValue_NullGroup_RemovesOneAsset() {
            // given a css asset that's in the manager under a null asset group,
            // the remove() call (by CssAsset value) removes this asset
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            var assetCopy = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, null);
            _assetManager.Remove(assetCopy, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ExisitingCssAsset_ByRef_OutOfOrder_NullGroup_RemovesOneAsset() {
            // given a css asset that's not yet (but will be) in the manager under a null asset group,
            // the remove() call (by CssAsset reference) correctly removes this asset
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Remove(newAsset, null);
            _assetManager.Add(newAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ExistingCssAsset_ByValue_OutOfOrder_NullGroup_RemovesOneAsset() {
            // given a css asset that's not yet (but will be) in the manager under a null asset group,
            // the remove() call (by CssAsset value) removes this asset
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            var assetCopy = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Remove(assetCopy, null);
            _assetManager.Add(newAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ExisitingCssAsset_WithGroup_RemovesOneAsset() {
            // given a css asset that's in the manager under an asset group,
            // the remove() call correctly removes this asset
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, "test-group");
            _assetManager.Remove(newAsset, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ExisitingCssAsset_InDifferentGroups_DoesNotRemoveAsset() {
            // given a css asset that's in the manager under a different asset group than
            // the remove() call, correctly removes this asset.
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, "test-group1");
            _assetManager.Remove(newAsset, "test-group2");
            _assetManager.Render(AssetType.Css, "test-group1");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            _buildHtmlCalls.Clear();
            _assetManager.Render(AssetType.Css, "test-group2");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_NonExistingCssAsset_NullGroup_DoesNotRemoveAsset() {
            // given a css asset that's NOT in the manager under a null asset group,
            // the remove() call does nothing
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Remove(newAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_NonExistingCssAsset_WithGroup_DoesNotRemoveAsset() {
            // given a css asset that's NOT in the manager under an asset group,
            // the remove() call does nothing
            var newAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Remove(newAsset, "test-group");
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }

        #endregion Remove

        #region Render

        [Test]
        public void Render_CssAssetType_RendersOnlyCorrectAssetType() {
            // given mixed assets, a render of AssetType.Css only returns Css assets
            var cssAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(cssAsset, null);
            var jsAsset = new JavascriptAsset(_context, _settings) { Path = "test-path2" };
            _assetManager.Add(jsAsset, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<CssAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path"));
        }

        [Test]
        public void Render_JavascriptAssetType_RendersOnlyCorrectAssetType() {
            // given mixed assets, a render of AssetType.Javascript only returns Javascript assets
            var cssAsset = new CssAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(cssAsset, null);
            var jsAsset = new JavascriptAsset(_context, _settings) { Path = "test-path2" };
            _assetManager.Add(jsAsset, null);
            _assetManager.Render(AssetType.Javascript, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(1));
            var asset = call.Assets.First();
            Assert.IsInstanceOf<JavascriptAsset>(asset);
            Assert.That(asset.Path, Is.EqualTo("test-path2"));
        }

        [Test]
        public void Render_ExistingAssetGroup_RendersGroup() {
            // given assets in a group, calling that group renders those assets
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path1" };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path2" };
            var newAsset3 = new CssAsset(_context, _settings) { Path = "test-path3" };
            var newAsset4 = new CssAsset(_context, _settings) { Path = "test-path4" };
            _assetManager.Add(newAsset1, null);
            _assetManager.Add(newAsset2, null);
            _assetManager.Add(newAsset3, null);
            _assetManager.Add(newAsset4, null);
            _assetManager.Render(AssetType.Css, null);
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            Assert.That(call.Assets.Count(), Is.EqualTo(4));
            Assert.That(call.Assets.Count(a => a.Path == "test-path1"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path1"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path2"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path2"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path3"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path3"));
            Assert.That(call.Assets.Count(a => a.Path == "test-path4"), Is.EqualTo(1));
            Assert.IsInstanceOf<CssAsset>(call.Assets.Single(a => a.Path == "test-path4"));

        }

        [Test]
        public void Render_NonExistingAssetGroup_RendersNothing() {
            // given assets in a group, calling a different group renders nothing
            var newAsset1 = new CssAsset(_context, _settings) { Path = "test-path1" };
            var newAsset2 = new CssAsset(_context, _settings) { Path = "test-path2" };
            var newAsset3 = new CssAsset(_context, _settings) { Path = "test-path3" };
            var newAsset4 = new CssAsset(_context, _settings) { Path = "test-path4" };
            _assetManager.Add(newAsset1, null);
            _assetManager.Add(newAsset2, null);
            _assetManager.Add(newAsset3, null);
            _assetManager.Add(newAsset4, null);
            _assetManager.Render(AssetType.Css, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Render_NoAlternateName_RendersAlternateFromSettings() {
            // if no alternate path name is passed, the path from settings is used.
            Assert.Inconclusive("Not Yet Implemented - Must Mock Settings");
            var newAsset = new JavascriptAsset(_context, _settings) { Path = "test-path" };
            _assetManager.Add(newAsset, "test-group");
            _assetManager.Render(AssetType.Javascript, "test-group");
            Assert.That(_buildHtmlCalls.Count, Is.EqualTo(1));
            var call = _buildHtmlCalls.First();
            //Assert.That(call.AlternateName, Is.EqualTo("an-alt-path"));
        }

        #endregion Render
    }
}
