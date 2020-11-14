using eSmart.Elastic.Core.Enums;
using eSmart.Elastic.Core.Json;
using eSmart.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eSmart.Elastic.Search.Tests
{
    [TestClass]
    public class AssetQuestTests
    {
        [TestMethod]
        public void Query_AddToBoolean()
        {
            var q1 = new Query<IndexedClass>(true).Match("TEST1", ic => ic.AString);
            var q2 = new Query<IndexedClass>(true).Match("TEST2", ic => ic.AString);
            var q12 = q1.AddToBoolean(q2, BooleanQueryType.MustNot);

            var json = q12.ToJson();

            Assert.AreEqual("{\"query\":{\"bool\":{\"must\":[{\"match\":{\"AString\":\"TEST1\"}}],\"must_not\":[{\"bool\":{\"must\":[{\"match\":{\"AString\":\"TEST2\"}}]}}]}}}",json);
        }
    }
}