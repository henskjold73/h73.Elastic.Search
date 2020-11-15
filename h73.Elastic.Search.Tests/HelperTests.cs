using System.Collections.Generic;
using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void ValueByKey_object()
        {
            var d = new Dictionary<string, object> {{"key", new {Id = "Key"}}};
            Assert.IsNull(d.ValueByKey("noKey"));
            Assert.IsNotNull(d.ValueByKey("key"));
        }

        [TestMethod]
        public void ValueByKey_doubleNullable()
        {
            var d = new Dictionary<string, double?> {{"key", 5.0}};
            Assert.IsNull(d.ValueByKey("noKey"));
            Assert.IsNotNull(d.ValueByKey("key"));
        }

        [TestMethod]
        public void ValueByKey_double()
        {
            var d = new Dictionary<string, double> {{"key", 5.0}};
            Assert.AreEqual(0.0, d.ValueByKey("noKey"));
            Assert.AreEqual(5.0, d.ValueByKey("key"));
        }
    }
}