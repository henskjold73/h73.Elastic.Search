using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Json;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;
using h73.Elastic.Core.Search.Queries;
using h73.Elastic.Core.Search.Results;
using h73.Elastic.Search.Helpers;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class JsonTests
    {
        IList<IndexedClass> _objects;

        [TestInitialize]
        public void Init()
        {
            _objects = Builder<IndexedClass>.CreateListOfSize(500).Build();
        }

        [TestMethod]
        public void Serialize_Deserialize_IndexedClass()
        {
            var serialized = JsonConvert.SerializeObject(_objects, JsonHelpers.CreateSerializerSettings());
            var deserialized =
                JsonConvert.DeserializeObject<List<IndexedClass>>(serialized, JsonHelpers.CreateSerializerSettings());
            Assert.AreEqual(_objects.Count, deserialized.Count);
        }

        [TestMethod]
        public void Serialize_Deserialize_IndexedClass_TypeMissing()
        {
            var settings = JsonHelpers.CreateSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.None;
            var serialized = JsonConvert.SerializeObject(_objects, settings);
            var deserialized =
                JsonConvert.DeserializeObject<List<IndexedClass>>(serialized, JsonHelpers.CreateSerializerSettings());
            Assert.AreEqual(_objects.Count, deserialized.Count);
        }

        [TestMethod]
        public void Serialize_Query_With_Enum()
        {

            // Arrange
            var test0 = MockEnum.Test0;
            var test1_3 = MockEnum.Test1 | MockEnum.Test3;
            var expectedSingle =
                "{\"query\":{\"bool\":{\"must\":[{\"match\":{\"MockEnum\":\"Test0\"}}],\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.IndexedClass\"}}," +
                "{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}";
            var expectedMultiple =
                "{\"query\":{\"bool\":{\"must\":[{\"match\":{\"MockEnum\":\"Test1, Test3\"}}],\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.IndexedClass\"}}," +
                "{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}}}";

            // Act
            var querySingleEnum = new Query<IndexedClass>()
                .Match(test0, i => i.MockEnum);

            var queryMultipleEnum = new Query<IndexedClass>()
                .Match(test1_3, i => i.MockEnum);

            // Json
            var json1 = querySingleEnum.ToJson();
            var json2 = queryMultipleEnum.ToJson();

            // Assert
            Assert.AreEqual(
                expectedSingle,
                json1);

            Assert.AreEqual(
                expectedMultiple,
                json2);
        }

        [TestMethod]
        public void BucketSelector_Terms()
        {
            var agg = AggregationsHelper.RangeAggregation<IndexedClass, double?>(x => x.SomeNumber,
                new RangeAggrValue<double?> {From = 2 * 60, Key = "Above120"});
            agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.CardinalityAggregation<IndexedClass>(ci => ci.ObjectId).TopHitsAggregation( ParameterHelper.Sort(
                new KeyValuePair<Expression<Func<IndexedClass, object>>, AggsOrderDirection>(ic => ic.DateTimeNotNullable,
                    AggsOrderDirection.Desc)), ParameterHelper.Excludes<IndexedClass>(ic => ic.SomeNumber));

            

            var q2 = new Query<IndexedClass>(true).Match("EarthFault", e => e.AString)
                .TermsAggregation(x=>x.ObjectId,100, agg, AggregationsHelper.BucketSelector("count","nested._bucket_count", "params.count > 0"))
                .SetSize(0);
            Assert.AreEqual("nested", q2.Aggregations.FirstOrDefault().Value.Aggregations.Keys.FirstOrDefault());
        }


        [TestMethod]
        public void TopHits_Result()
        {
            var json = "{\"hits\":{\"total\":1,\"max_score\":null,\"hits\":[{\"_index\":\"h73dev_h73_core_event\",\"_type\":\"h73.Elastic.Search.Tests.IndexedClass\",\"_id\":\"9af4387b-cecb-42e6-9e4b-a8bbca4cc662_EarthFault_636630941494510178\",\"_score\":null,\"_source\":{\"SomeNumber\":1,\"ObjectId\":\"00000000-0000-0000-0000-000000000001\",\"AString\":\"AString1\",\"Child\":null,\"Children\":null,\"DateTimeNotNullable\":\"2018-11-07T00:00:00\",\"DateTimeNullable\":\"2018-11-07T00:00:00\",\"ListStrings\":null,\"MockEnum\":0,\"MetaData\":null},\"sort\":[1527497349451]}]}}";
            
            var deserialized = JsonConvert.DeserializeObject<TopHitsResult<IndexedClass>>(json);

        }

        [TestMethod]
        public void BucketSelector_Terms_TopHits_Result()
        {
            var json = "{\"took\":9,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0}" +
                       ",\"hits\":{\"total\":1330,\"max_score\":0,\"hits\":[]},\"aggregations\":" +
                       "{\"terms_AssetGuid\":{\"doc_count_error_upper_bound\":3,\"sum_other_doc_count\":1203,\"buckets\":[" +
                       "{\"key\":\"9af4387b-cecb-42e6-9e4b-a8bbca4cc662\",\"doc_count\":5,\"nested\":" +
                       "{\"buckets\":[{\"key\":\"Above120\",\"from\":120,\"doc_count\":1,\"tophits\":{\"hits\":{\"total\":1," +
                       "\"max_score\":null,\"hits\":[{\"_index\":\"h73dev_h73_core_event\",\"_type\":\"h73.Elastic.Search.Tests.IndexedClass\"," +
                       "\"_id\":\"9af4387b-cecb-42e6-9e4b-a8bbca4cc662_EarthFault_636630941494510178\",\"_score\":null,\"_source\":{\"SomeNumber\":1," +
                       "\"ObjectId\":\"00000000-0000-0000-0000-000000000001\",\"AString\":\"AString1\",\"Child\":null,\"Children\":null," +
                       "\"DateTimeNotNullable\":\"2018-11-07T00:00:00\",\"DateTimeNullable\":\"2018-11-07T00:00:00\",\"ListStrings\":null," +
                       "\"MockEnum\":0,\"MetaData\":null},\"sort\":[1527497349451]}]}},\"cardinality\":{\"value\":1}}]}}]}}}";

            var deserialized = JsonConvert.DeserializeObject<SearchResult<IndexedClass>>(json, JsonHelpers.CreateSerializerSettings());
            var tophit = deserialized.Aggregations.FirstOrDefault().Value.Buckets.FirstOrDefault().Nested.Buckets.FirstOrDefault().TopHits<IndexedClass>().Hits.HitsList.FirstOrDefault().Source;
            
            Assert.IsTrue(tophit is IndexedClass);
        }
    }
}
