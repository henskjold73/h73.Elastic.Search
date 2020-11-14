using System;
using eSmart.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eSmart.Elastic.Search.Tests
{
    [TestClass]
    public class ValidateTests
    {
        [TestMethod]
        public void Validate_OnlyFrom()
        {
            const string expected = "{\"query\":{\"bool\":{\"must\":[{\"range\":{\"DateTimeNotNullable\":{\"lte\":\"2018-01-01T00:00:00\"}}}],\"should\":[{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}";
            var query = new Query<IndexedClass>().Validate(new DateTime(2018, 01, 01), x => x.DateTimeNotNullable);
            var json = query.ToJson();

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void Validate_OnlyTo()
        {
            const string expected = "{\"query\":{\"bool\":{\"must\":[{\"range\":{\"DateTimeNullable\":{\"gte\":\"2018-01-01T23:59:59.9999999\"}}}],\"should\":[{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}";
            var query = new Query<IndexedClass>().Validate(new DateTime(2018, 01, 01), null, x => x.DateTimeNullable);
            var json = query.ToJson();

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void Validate_None()
        {
            const string expected = "{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}";
            var query = new Query<IndexedClass>().Validate(new DateTime(2018, 01, 01));
            var json = query.ToJson();

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void Validate_Both()
        {
            const string expected = "{\"query\":{\"bool\":{\"must\":[{\"range\":{\"DateTimeNotNullable\":{\"lte\":\"2018-01-01T00:00:00\"}}},{\"range\":{\"DateTimeNullable\":{\"gte\":\"2018-01-01T23:59:59.9999999\"}}}],\"should\":[{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}";
            var query = new Query<IndexedClass>().Validate(new DateTime(2018, 01, 01), x => x.DateTimeNotNullable,
                x => x.DateTimeNullable);
            var json = query.ToJson();

            Assert.AreEqual(expected, json);
        }
    }
}