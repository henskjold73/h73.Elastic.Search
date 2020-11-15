using System;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Json;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Queries;
using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class RenQueryTests
    {
        [TestMethod]
        public void Ren1_gt24h()
        {
            var nestedQuery24H = new Query<IndexedClass>(true).RangeGreaterThan<object>(24 * 60,
                c => c.Children.PropertyName(e => e.DateTimeNotNullable));
            var q24H = new Query<IndexedClass>(true)
                .Nested(c => c.Child, nestedQuery24H.QueryItem, matchingInnerHits: true).Excludes(ic => ic.Child)
                .SetSize(25);

            var json = q24H.ToJson();
            Assert.AreEqual(
                "{\"query\":{\"bool\":{\"must\":[{\"nested\":{\"path\":\"Child\",\"query\":{\"bool\":{\"must\":" +
                "[{\"range\":{\"Children.DateTimeNotNullable\":{\"gte\":1440}}}]}},\"inner_hits\":{}}}]}},\"_source\":" +
                "{\"excludes\":[\"Child\"]},\"size\":25}", json);
        }

        [TestMethod]
        public void Ren2_gt2hx5()
        {
            var nestedQgt2Hx5 = new Query<IndexedClass>(true)
                .RangeGreaterThan<object>(2 * 60, c => c.Children.PropertyName(e => e.SomeNumber))
                .RangeGreaterThan<object>(new DateTime(2018, 05, 23),
                    c => c.Children.PropertyName(e => e.DateTimeNotNullable), boost: 0);

            var qgt2Hx5 = new Query<IndexedClass>(true)
                .Nested(c => c.Child, nestedQgt2Hx5.QueryItem,
                    customInnerHits: new InnerHits {Size = 100},
                    scoreMode: ScoreMode.Sum)
                .Excludes(ic => ic.Child)
                .SetSize(25)
                .SetMinScore(5);

            var json = qgt2Hx5.ToJson();
            Assert.AreEqual(
                "{\"query\":{\"bool\":{\"must\":[{\"nested\":{\"path\":\"Child\",\"score_mode\":\"Sum\"," +
                "\"query\":{\"bool\":{\"must\":[{\"range\":{\"Children.SomeNumber\":{\"gte\":120}}},{\"range\":" +
                "{\"Children.DateTimeNotNullable\":{\"gte\":\"2018-05-23T00:00:00\",\"boost\":0.0}}}]}},\"inner_hits\":" +
                "{\"size\":100}}}]}},\"_source\":{\"excludes\":[\"Child\"]},\"size\":25,\"min_score\":5.0}", json);
        }

        [TestMethod]
        public void Ren3_gt25percent()
        {
            var gt25percent = new Query<IndexedClass>(true).SetSize(0);
            var aggr = AggregationsHelper
                .DateHistogram<IndexedClass>(ic => ic.Children.PropertyName(c => c.DateTimeNotNullable), "week",
                    order: new AggsOrder(AggsOrderBy.Key, AggsOrderDirection.Desc))
                .TermsAggregation("_id", order: new AggsOrder("sum", AggsOrderDirection.Desc));

            aggr.Aggregations["nested"].Add("sum",
                AggregationsHelper.SumAggregation<IndexedClass>(ic =>
                    ic.Children.PropertyName(c => c.DateTimeNotNullable)));
            aggr.Aggregations["nested"].Add("above25percent_filter",
                AggregationsHelper.BucketSelector("sum","sum", "params.sum > 2520"));

            gt25percent.NestedAggregation(ic => ic.Children, aggr);
            Assert.AreEqual(
                "{\"query\":{\"bool\":{}},\"aggs\":{\"nested_Children_0\":{\"nested\":{\"path\":\"Children\"},\"aggs\":{\"nested\":{\"date_histogram\":" +
                "{\"interval\":\"week\",\"field\":\"Children.DateTimeNotNullable\",\"order\":{\"_key\":\"desc\"}},\"aggs\":{\"nested\":{\"terms\":" +
                "{\"field\":\"_id\",\"order\":{\"sum\":\"desc\"}},\"aggs\":{\"sum\":{\"sum\":{\"field\":\"Children.DateTimeNotNullable\"}},\"above25percent_filter\":" +
                "{\"bucket_selector\":{\"buckets_path\":{\"sum\":\"sum\"},\"script\":\"params.sum > 2520\"}}}}}}}}},\"size\":0}",
                gt25percent.ToJson());
        }
    }
}