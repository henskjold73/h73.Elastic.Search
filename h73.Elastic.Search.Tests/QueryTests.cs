using System;
using System.Collections.Generic;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;
using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class QueryTests
    {
        private const int IntValue = 73;
        private readonly string _stringValue = "73";
        private readonly DateTime _datetimeValue = DateTime.Now;
        private readonly string[] _stringListValue = {"a", "b", "c"};
        private readonly int[] _intListValue = {12, 73, 3};
        private readonly double[] _doubleListValue = {12.0009, 73.01, 3.0};
        private readonly BooleanQueryType _enumValue = BooleanQueryType.Must;

        [TestMethod]
        public void Chained_Most_If_Not_All_Types()
        {
            const string propertyName = "Name";

            var query = new Query<object>();

            query = query.Type(typeof(IndexedClass));
            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.Match($"{_stringValue}", propertyName);
            query = query.WildcardMatch($"*{_stringValue}", propertyName);
            query = query.WildcardMatch($"*{_stringValue}*", propertyName);
            query = query.Match($"{_stringValue}", propertyName, BooleanQueryType.MustNot);
            query = query.Terms(_stringListValue, propertyName);
            query = query.RangeGreaterThan(IntValue, propertyName);
            query = query.RangeLesserThan(IntValue, propertyName);
            query = query.Match($"{IntValue}", propertyName);
            query = query.Match($"{IntValue}", propertyName, BooleanQueryType.MustNot);
            query = query.Terms(_intListValue, propertyName);
            query = query.Match(true, propertyName);
            query = query.Match(false, propertyName);
            query = query.RangeLesserThan(_datetimeValue, propertyName);
            query = query.RangeGreaterThan(_datetimeValue, propertyName);
            query = query.Match(_datetimeValue, propertyName);
            query = query.Match(_enumValue, propertyName);
            query = query.RangeGreaterThan(_datetimeValue.Date, propertyName);
            query = query.RangeLesserThan(_datetimeValue.Date, propertyName);
            query = query.Terms(_doubleListValue, propertyName);
            Assert.IsNotNull(query);
        }

        [TestMethod]
        public void CleanUp_object_5in_1out()
        {
            var propertyName = "Name";
            var query = new Query<object>();

            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.WildcardMatch($"{_stringValue}*", propertyName);

            query.CleanUp();

            Assert.AreEqual(1, query.BooleanQuery.Must.Count);
        }


        [TestMethod]
        public void CleanUp_object_5in_2out_x3()
        {
            var propertyName = "Name";
            var query = new Query<object>();

            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.Match($"{_stringValue}*", propertyName);
            query = query.WildcardMatch($"{_stringValue}*", propertyName);
            query = query.Match($"{_stringValue}*", propertyName);
            query = query.WildcardMatch($"{_stringValue}*", propertyName);

            query = query.WildcardMatch($"{_stringValue}*", propertyName, BooleanQueryType.Should);
            query = query.Match($"{_stringValue}*", propertyName, BooleanQueryType.Should);
            query = query.WildcardMatch($"{_stringValue}*", propertyName, BooleanQueryType.Should);
            query = query.Match($"{_stringValue}*", propertyName, BooleanQueryType.Should);
            query = query.WildcardMatch($"{_stringValue}*", propertyName, BooleanQueryType.Should);

            query = query.WildcardMatch($"{_stringValue}*", propertyName, BooleanQueryType.MustNot);
            query = query.Match($"{_stringValue}*", propertyName, BooleanQueryType.MustNot);
            query = query.WildcardMatch($"{_stringValue}*", propertyName, BooleanQueryType.MustNot);
            query = query.Match($"{_stringValue}*", propertyName, BooleanQueryType.MustNot);
            query = query.WildcardMatch($"{_stringValue}*", propertyName, BooleanQueryType.MustNot);

            query.CleanUp();

            Assert.AreEqual(2, query.BooleanQuery.Must.Count);
            Assert.AreEqual(2, query.BooleanQuery.MustNot.Count);
            Assert.AreEqual(2, query.BooleanQuery.Should.Count);
        }

        [TestMethod]
        public void TypeQuery_Updates_indexPattern()
        {
            var query = new Query<IndexedClass>();
            Assert.AreEqual(3, query.TypesList.Count);
        }

        [TestMethod]
        public void Sorting_Adding_Sorting()
        {
            var query1 = new Query<IndexedClass>().Sort("some", "asc");
            var query2 = new Query<IndexedClass>().Sort("some", "asc").Sort("another", "desc");
            Assert.AreEqual(1, query1.Sorting.Count);
            Assert.AreEqual(2, query2.Sorting.Count);
        }

        [TestMethod]
        public void Clone_RangedQuery()
        {
            var query1 = new Query<IndexedClass>().RangeGreaterThan(new DateTime(2018, 02, 13),
                x => x.DateTimeNotNullable);
            var json1 = query1.ToJson();
            var query2 = query1.Clone();
            var json2 = query2.ToJson();

            Assert.AreEqual(json1, json2);
        }

        [TestMethod]
        public void Clone_NestedBools_Clean()
        {
            var ints = new[] {1, 2, 3, 4, 5};
            var query = new Query<IndexedClass>().Range(new DateTime(2018, 02, 13), new DateTime(2018, 03, 13), x=>x.DateTimeNotNullable);

            query.BooleanQuery.Filter = new List<IQuery>
            {
                new GeoBoundingBoxFilter("Location.Point", $"{45}, {40}", $"{40}, {45}")
            };
            query.And(new Query<IndexedClass>().Range((int?)1, 2, x=>x.Child.SomeNumber));
            var queryInts = new Query<IndexedClass>();
            foreach (var i in ints)
            {
                queryInts.Match(i, q => q.SomeNumber);
            }

            query.Or(queryInts).And(new Query<IndexedClass>(false));

        }

        [TestMethod]
        public void NestedQuery_Serialize()
        {
            var query1 = new Query<IndexedClass>(true).Match(2, x=>x.Children.PropertyName(x2=>x2.SomeNumber));
            var json1 = query1.ToJson();

            var query2 = new Query<IndexedClass>().Nested(x => x.Children, query1.QueryItem);
            var json2 = query2.ToJson();

            Assert.AreEqual("{\"query\":{\"bool\":{\"must\":[{\"match\":{\"Children.SomeNumber\":2}}]}}}", json1);
            Assert.AreEqual("{\"query\":{\"bool\":{\"must\":[{\"nested\":{\"path\":\"Children\",\"query\":{\"bool\":{\"must\":[{\"match\":{\"Children.SomeNumber\":2}}]}}}}],\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}", json2);
        }

    }
}