using System.Linq;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Json;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Results;
using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class ComplexAggregationsTests
    {
        [TestMethod]
        public void Aggregations_Nested_Get()
        {
            var a1 = new DocumentSearchResult<IndexedClass>
                {Aggregations = new Aggregations {["nested_Children_1"] = new Aggregation()}};
            var a3 = new DocumentSearchResult<IndexedClass>
                {Aggregations = new Aggregations {["nested_Children_3"] = new Aggregation()}};
            var nested1 = a1.NestedAggregation(ic => ic.Children);
            var nested3 = a3.NestedAggregation(ic => ic.Children);
            Assert.IsNotNull(nested1);
            Assert.IsNotNull(nested3);
        }

        [TestMethod]
        public void Aggregations_Nested_Get2()
        {
            var jsonData =
                "{\"took\":32,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":{\"total\":841,\"max_score\":0,\"hits\":[]}" +
                ",\"aggregations\":{\"nested_Children_0\":{\"doc_count\":33549,\"nested\":{\"buckets\":[{\"key_as_string\":\"2018-10-22T00:00:00.000Z\",\"key\":1540166400000," +
                "\"doc_count\":38,\"nested\":{\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":26,\"buckets\":[]}},{\"key_as_string\":\"2018-10-15T00:00:00.000Z\"," +
                "\"key\":1539561600000,\"doc_count\":149,\"nested\":{\"doc_count_error_upper_bound\":-1,\"sum_other_doc_count\":129,\"buckets\":[{\"key\":\"505ad21c-9cb7-4c17-99a8-a193f248986c\"," +
                "\"doc_count\":2,\"sum\":{\"value\":3798}},{\"key\":\"cc3dfb17-b653-4627-b8c1-98afa752ae92\",\"doc_count\":2,\"sum\":{\"value\":3044}},{\"key\":\"63475d9a-3b48-4196-a287-1d1788b311ad\"," +
                "\"doc_count\":2,\"sum\":{\"value\":2735}},{\"key\":\"35bea27a-f44c-4c43-86f4-ce1a97104d1d\",\"doc_count\":2,\"sum\":{\"value\":2676}}]}}]}}}}";

            var searchResult =
                JsonConvert.DeserializeObject<DocumentSearchResult<IndexedClass>>(jsonData,
                    JsonHelpers.CreateSerializerSettings());

            var agg = searchResult.NestedAggregation(asset => asset.Children);
            var agg2 = agg.Nested.Buckets.SelectMany(b => b.Nested.Buckets).Select(nb => new {nb.Key});
            var dist = agg2.Distinct();
            var count = dist.Count();

            Assert.AreEqual(33549, agg.DocCount);
            Assert.AreEqual("505ad21c-9cb7-4c17-99a8-a193f248986c", agg2.First().Key);
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void Aggregations_Nested_Terms_Range()
        {
            var q1 = new Query<IndexedClass>().NestedAggregation(
                ic => ic.Children,
                AggregationsHelper.TermsAggregation<IndexedClass>(x => x.Children.PropertyName(p => p.AString), 10,
                    new AggsOrder(AggsOrderBy.Key, AggsOrderDirection.Desc),
                    AggregationsHelper.SumAggregation<IndexedClass>(sum => sum.SomeNumber)),
                AggregationsHelper.RangeAggregation<IndexedClass, int?>(
                    x => x.Children.PropertyName(p => p.SomeNumber), new RangeAggrValue<int?>
                    {
                        Key = "SomeKey",
                        To = 24
                    }),
                AggregationsHelper.RangeAggregation<IndexedClass, int?>(
                    x => x.Children.PropertyName(p => p.SomeNumber),
                    new RangeAggrValue<int?>
                    {
                        Key = "SomeKey1",
                        To = 12,
                    },
                    new RangeAggrValue<int?>
                    {
                        Key = "SomeKey2",
                        From = 31,
                    })
            );

            var json = q1.ToJson();
            Assert.AreEqual(
                "{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}},\"aggs\":{\"nested_Children_0\":{\"nested\":{\"path\":\"Children\"},\"aggs\":{\"nested\":{\"terms\":{\"field\":\"Children.AString\",\"size\":10,\"order\":{\"_key\":\"desc\"}},\"aggs\":{\"Sum\":{\"sum\":{\"field\":\"SomeNumber\"}},\"nested\":{\"range\":{\"ranges\":[{\"key\":\"SomeKey\",\"to\":24}],\"field\":\"Children.SomeNumber\"},\"aggs\":{\"nested\":{\"range\":{\"ranges\":[{\"key\":\"SomeKey1\",\"to\":12},{\"key\":\"SomeKey2\",\"from\":31}],\"field\":\"Children.SomeNumber\"}}}}}}}}}}",
                json);
        }

        [TestMethod]
        public void Aggregations_Sum()
        {
            var q1 = new Query<IndexedClass>().TermsAggregation(x => x.AString, 5,
                AggregationsHelper.SumAggregation<IndexedClass>(sum => sum.SomeNumber));
            var json = q1.ToJson();
            const string r1 =
                "{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.IndexedClass\"}}," +
                "{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}}," +
                "{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}," +
                "\"aggs\":{\"terms_AString\":{\"terms\":{\"field\":\"AString\",\"size\":5},\"aggs\":{\"Sum\":{\"sum\":{\"field\":\"SomeNumber\"}}}}}}";
            Assert.AreEqual(r1, json);
        }

        [TestMethod]
        public void MultipleNested()
        {
            var q1 = new Query<IndexedClass>().NestedAggregation(ic => ic.Children)
                .NestedAggregation(ic => ic.Children);
            var json = q1.ToJson();
            const string r1 =
                "{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.IndexedClass\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}},\"aggs\":{\"nested_Children_0\":{\"nested\":{\"path\":\"Children\"}},\"nested_Children_1\":{\"nested\":{\"path\":\"Children\"}}}}";
            Assert.AreEqual(r1, json);
        }

        [TestMethod]
        public void FilterAggregation_With_Nested()
        {
            var queryAsFilter = new Query<IndexedClass>(true).Term("TEST", ic => ic.AString, BooleanQueryType.MustNot)
                .CreateBooleanQueryRoot();
            var queryagg = new Query<IndexedClass>(true).TermsAggregation(x => x.AString, 5,
                AggregationsHelper.SumAggregation<IndexedClass>(sum => sum.SomeNumber));
            var q = new Query<IndexedClass>(true).FilterAggregation(queryAsFilter, queryagg.Aggregations);

            var json = q.ToJson();
            Assert.AreEqual(
                "{\"query\":{\"bool\":{}},\"aggs\":{\"agg_filter_0\":{\"aggs\":{\"terms_AString\":{\"terms\":" +
                "{\"field\":\"AString\",\"size\":5},\"aggs\":{\"Sum\":{\"sum\":{\"field\":\"SomeNumber\"}}}}}," +
                "\"filter\":{\"bool\":{\"must_not\":[{\"term\":{\"AString\":\"TEST\"}}]}}}}}", json);
        }

        [TestMethod]
        public void FilteredAggregationResult()
        {
            var json =
                "{\"took\":1,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":{\"total\":841,\"max_score\":0,\"hits\":[]},\"aggregations\":" +
                "{\"agg_filter\":{\"doc_count\":3414,\"nested_Events_0\":{\"doc_count\":42,\"nested\":{\"buckets\":[{\"key_as_string\":\"2018-08-27T00:00:00.000Z\",\"key\":1535328000000,\"doc_count\":1," +
                "\"nested\":{\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":0,\"buckets\":[]}},{\"key_as_string\":\"2018-08-20T00:00:00.000Z\",\"key\":1534723200000,\"doc_count\":0,\"nested\":{" +
                "\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":0,\"buckets\":[]}}]}}}}}";

            var result =
                JsonConvert.DeserializeObject<DocumentSearchResult<object>>(json,
                    JsonHelpers.CreateSerializerSettings());
            Assert.AreEqual(42, result.Aggregations["nested_Events_0"].DocCount);
        }

        [TestMethod]
        public void RangeAggregation_Query()
        {
            var q = new Query<IndexedClass>(true)
                .CardinalityAggregation(ic => ic.ObjectId)
                .RangeAggregation(ic => ic.SomeNumber,
                    new[]
                    {
                        new RangeAggrValue<int> {Key = "123", From = 1, To = 4},
                        new RangeAggrValue<int> {Key = "4", From = 4, To = 5}
                    },
                    AggregationsHelper.CardinalityAggregation<IndexedClass>(ic => ic.ObjectId)
                );
            var json = q.ToJson();
            Assert.AreEqual("{\"query\":{\"bool\":{}},\"aggs\":{\"cardinality_ObjectId\":{\"cardinality\":" +
                            "{\"field\":\"ObjectId\"}},\"range_SomeNumber\":{\"range\":{\"ranges\":[{\"key\":" +
                            "\"123\",\"from\":1,\"to\":4},{\"key\":\"4\",\"from\":4,\"to\":5}],\"field\":\"SomeNumber\"}" +
                            ",\"aggs\":{\"Cardinality\":{\"cardinality\":{\"field\":\"ObjectId\"}}}}}}", json);
        }
    }
}