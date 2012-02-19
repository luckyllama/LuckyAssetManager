using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Lucky.AssetManager.Tests {

    [TestFixture]
    public class ConditionalCommentTests {

        #region AsString(IE.Version)

        [Test]
        public void AsStringVersion_All_ReturnsCorrectString() {
            Assert.That(IE.Version.All.AsString(), Is.EqualTo(""));
        }

        [Test]
        public void AsStringVersion_IE_ReturnsCorrectString() {
            Assert.That(IE.Version.IE.AsString(), Is.EqualTo("IE"));
        }

        [Test]
        public void AsStringVersion_IE5_ReturnsCorrectString() {
            Assert.That(IE.Version.IE5.AsString(), Is.EqualTo("IE 5"));
        }

        [Test]
        public void AsStringVersion_IE50_ReturnsCorrectString() {
            Assert.That(IE.Version.IE50.AsString(), Is.EqualTo("IE 5.0"));
        }

        [Test]
        public void AsStringVersion_IE55_ReturnsCorrectString() {
            Assert.That(IE.Version.IE55.AsString(), Is.EqualTo("IE 5.5"));
        }

        [Test]
        public void AsStringVersion_IE6_ReturnsCorrectString() {
            Assert.That(IE.Version.IE6.AsString(), Is.EqualTo("IE 6"));
        }

        [Test]
        public void AsStringVersion_IE7_ReturnsCorrectString() {
            Assert.That(IE.Version.IE7.AsString(), Is.EqualTo("IE 7"));
        }

        [Test]
        public void AsStringVersion_IE8_ReturnsCorrectString() {
            Assert.That(IE.Version.IE8.AsString(), Is.EqualTo("IE 8"));
        }

        [Test]
        public void AsStringVersion_IE9_ReturnsCorrectString() {
            Assert.That(IE.Version.IE9.AsString(), Is.EqualTo("IE 9"));
        }

        [Test]
        public void AsStringVersion_IE10_ReturnsCorrectString() {
            Assert.That(IE.Version.IE10.AsString(), Is.EqualTo("IE 10"));
        }

        #endregion AsString(IE.Version)

        #region AsString(IE.Equality)

        [Test]
        public void AsStringEquality_EqualTo_ReturnsCorrectString() {
            Assert.That(IE.Equality.EqualTo.AsString(), Is.EqualTo(""));
        }

        [Test]
        public void AsStringEquality_LessThan_ReturnsCorrectString() {
            Assert.That(IE.Equality.LessThan.AsString(), Is.EqualTo("lt"));
        }

        [Test]
        public void AsStringEquality_LessThanOrEqualTo_ReturnsCorrectString() {
            Assert.That(IE.Equality.LessThanOrEqualTo.AsString(), Is.EqualTo("lte"));
        }

        [Test]
        public void AsStringEquality_GreaterThan_ReturnsCorrectString() {
            Assert.That(IE.Equality.GreaterThan.AsString(), Is.EqualTo("gt"));
        }

        [Test]
        public void AsStringEquality_GreaterThanOrEqualTo_ReturnsCorrectString() {
            Assert.That(IE.Equality.GreaterThanOrEqualTo.AsString(), Is.EqualTo("gte"));
        }

        #endregion AsString(IE.Equality)
    }
}
