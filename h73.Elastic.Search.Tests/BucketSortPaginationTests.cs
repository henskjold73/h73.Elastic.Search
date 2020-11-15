using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using h73.Elastic.Core.Enums;
using h73.Elastic.Search.Helpers;
using h73.Elastic.Search.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class BucketSortPaginationTests
    {
        [TestMethod]
        public void BucketSort_From_Size()
        {
            var q = new Query<EarthFaultEvent>()
                .TermsAggregation(x=>x.Type, null, AggregationsHelper.BucketSortAggregation("bucket_sort",null,1,23));
            var qJson = q.ToJson();
            Assert.AreEqual("{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.Support.EarthFaultEvent\"}}]," +
                            "\"minimum_should_match\":1}},\"aggs\":{\"terms_Type\":{\"terms\":{\"field\":\"Type\"},\"aggs\":{\"BucketSort\":{\"bucket_sort\":{\"size\":1,\"from\":23}}}}}}", qJson);
        }

        [TestMethod]
        public void BucketSelector_Multiple()
        {
            var q = new Query<EarthFaultEvent>()
                .TermsAggregation(x => x.Type, null,
                    AggregationsHelper.BucketSelector("max", "Max", $"params.max < 99"),
                    AggregationsHelper.BucketSelector("max", "Max", $"params.max > 0"));
            
            var qJson = q.ToJson();

            var matches = Regex.Matches(qJson, "BucketSelector_").Count;

            Assert.AreEqual(2, matches);
        }

        [TestMethod]
        public void BucketSort_MultipleSorts()
        {
            var q = new Query<EarthFaultEvent>()
                .TermsAggregation(x => x.Type, null,
                    AggregationsHelper.BucketSortAggregation(
                        10,0, 
                        new KeyValuePair<string, AggsOrderDirection>("terms_Type", AggsOrderDirection.Asc),
                        new KeyValuePair<string, AggsOrderDirection>("terms_Type", AggsOrderDirection.Desc)
                        ));
            
            var qJson = q.ToJson();

            Assert.AreEqual("{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.Support.EarthFaultEvent\"}}],\"minimum_should_match\":1}}," +
                            "\"aggs\":{\"terms_Type\":{\"terms\":{\"field\":\"Type\"},\"aggs\":{\"BucketSort\":{\"bucket_sort\":{\"sort\":[{\"terms_Type\":\"Asc\"}," +
                            "{\"terms_Type\":\"Desc\"}],\"size\":10,\"from\":0}}}}}}", qJson);
        }
    }
}