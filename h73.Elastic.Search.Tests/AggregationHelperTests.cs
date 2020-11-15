using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Json;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;
using h73.Elastic.Search.Helpers;
using h73.Elastic.Search.Tests.Support;
using h73.Elastic.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class AggregationHelperTests
    {
        [TestMethod]
        public void HistogramAggregation_Nested_FromHelper()
        {
            var q = new Query<IndexedClass>(true).NestedAggregation(x => x.Children,
                    AggregationsHelper.HistogramAggregation<IndexedClass, int>(
                        ic => ic.Children.PropertyName(c => c.SomeNumber), 5))
                .SetSize(0);

            var json = q.ToJson();
            Assert.AreEqual(
                "{\"query\":{\"bool\":{}},\"aggs\":{\"nested_Children_0\":" +
                "{\"nested\":{\"path\":\"Children\"},\"aggs\":{\"nested\":" +
                "{\"histogram\":{\"interval\":5,\"field\":\"Children.SomeNumber\"}}}}},\"size\":0}",
                json);
        }

        [TestMethod]
        public void CardinalityAggregation_FromHelper()
        {
            var q = new Query<IndexedClass>(true).NestedAggregation(x => x.Children,
                    AggregationsHelper.CardinalityAggregation<IndexedClass>(ic => ic.Children.PropertyName(c => c.SomeNumber)))
                .SetSize(0);

            var json = q.ToJson();
            Assert.AreEqual(
                "{\"query\":{\"bool\":{}},\"aggs\":{\"nested_Children_0\":{\"nested\":{\"path\":\"Children\"}," +
                "\"aggs\":{\"nested\":{\"cardinality\":{\"field\":\"Children.SomeNumber\"}}}}},\"size\":0}",
                json);
        }

        [TestMethod]
        public void TermsAggregation_RangeAggregation_FromHelper()
        {
            var agg = AggregationsHelper.RangeAggregation<Event, double?>(x => x.Duration,
                new RangeAggrValue<double?> {From = 2 * 60, Key = "Above120"});
            agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.CardinalityAggregation<Event>(ci => ci.AssetGuid).TopHitsAggregation( ParameterHelper.Sort(
                new KeyValuePair<Expression<Func<Event, object>>, AggsOrderDirection>(ic => ic.Start,
                    AggsOrderDirection.Desc)), ParameterHelper.Excludes<Event>(ic => ic.StSrid));
            var termsAgg = AggregationsHelper.TermsAggregation<Event>(x => x.AssetGuid, 1000, null, agg,
                AggregationsHelper.BucketSelector("count", "nested._bucket_count", "params.count > 0"));


            var q2 = new Query<Event>().Match("EarthFault", e => e.Type)
                .DateHistogramAggregation(x=>x.Start, "week", 1, termsAgg)
                .SetSize(0);

            var nested = q2.Aggregations.FirstOrDefault().Value.Aggregations.FirstOrDefault().Value.Aggregations.FirstOrDefault().Key;
            var n = q2.Aggregations.FirstOrDefault().Value.Aggregations.FirstOrDefault().Key;
            Assert.AreEqual("nested", nested);
            Assert.AreEqual("nested", n);
        }

        [TestMethod]
        public void AddCardinalityAggregation_FromHelper()
        {
            var term = AggregationsHelper.TermsAggregation<IndexedClass>(ic => ic.AString);
            Assert.IsNull(term.Aggregations);
            term.AddCardinalityAggregation<IndexedClass>(ic => ic.AString);
            Assert.IsNotNull(term.Aggregations);
        }

        [TestMethod]
        public void FilterAggregation_FromHelper()
        {
            var term = AggregationsHelper.TermsAggregation<Event>(ic => ic.Type);
            var filter = new Query<Event>(true).Match("EarthFault", e => e.Type).CreateBooleanQueryRoot();
            var filter1 = new Query<Event>(true).Match("EarthFault1", e => e.Type).CreateBooleanQueryRoot();
            
            Assert.IsNull(term.Aggregations);
            
            term.FilterAggregation<Event>(filter);
            term.FilterAggregation<Event>(filter1);

            var json =
                "{\"terms\":{\"field\":\"Type\"},\"aggs\":{\"agg_filter_0\":{\"filter\":{\"bool\":{\"must\":" +
                "[{\"match\":{\"Type\":\"EarthFault\"}}]}}},\"agg_filter_1\":{\"filter\":{\"bool\":{\"must\":[{\"match\":{\"Type\":\"EarthFault1\"}}]}}}}}";

            Assert.AreEqual(json, term.ToJson());
        }

    }
}