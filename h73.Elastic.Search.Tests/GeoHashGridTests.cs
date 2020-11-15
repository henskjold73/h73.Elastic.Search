using System.Collections.Generic;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;
using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class GeoHashGridTests
    {
        [TestMethod]
        public void GeoHashGrid()
        {
            const string expected = "{\"query\":{\"bool\":{}},\"aggs\":{\"GeoHashGridCluster\":{\"geohash_grid\":{\"precision\":6,\"field\":\"Location.Point\"},\"aggs\":{\"center_lat\":{\"avg\":{\"script\":\"doc[\'Location.Point\'].lat\"}},\"center_lng\":{\"avg\":{\"script\":\"doc[\'Location.Point\'].lon\"}}}}},\"size\":0}";
            var q = new Query<dynamic>
            {
                Aggregations =
                {
                    ["GeoHashGridCluster"] = new GeoHashGridAggregation("Location.Point", 6)
                    {
                        Aggregations = new Dictionary<string, IAggregation>
                        {
                            ["center_lat"] =
                            new ScriptAggregation {Avg = new ScriptAggr {Script = "doc['Location.Point'].lat"}},
                            ["center_lng"] =
                            new ScriptAggregation {Avg = new ScriptAggr {Script = "doc['Location.Point'].lon"}}
                        }
                    }
                }
            };
            q.SetSize(0);
            
            Assert.AreEqual(expected, q.ToJson());
        }
    }
}