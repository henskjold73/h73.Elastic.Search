using System.Linq;
using eSmart.Core;
using eSmart.EarthFault;
using eSmart.Elastic.Core.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace eSmart.Elastic.Search.Tests.ResultTests
{
    [TestClass]
    [TestCategory("Result")]
    public class EarthFaultTests
    {
        [TestMethod]
        public void Result_with_filtered_aggregation()
        {
            var result = "{\"took\":1,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0}," +
                         "\"hits\":{\"total\":1501,\"max_score\":0,\"hits\":[]},\"aggregations\":{\"terms_Type\":" +
                         "{\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"EarthFault\",\"doc_count\":" +
                         "1431,\"Cardinality\":{\"value\":1295}},{\"key\":\"EarthFaultRen1\",\"doc_count\":66,\"Cardinality\":{\"value\":66}}" +
                         ",{\"key\":\"EarthFaultRen3\",\"doc_count\":4,\"Cardinality\":{\"value\":4}}]},\"agg_filter\":{\"doc_count\":70," +
                         "\"EarthFaultRen1234\":{\"value\":66}}}}";

            var js = JsonHelpers.CreateSerializerSettings();
            var resultObject = JsonConvert.DeserializeObject<DocumentSearchResult<Event>>(result, js);
            
        } 
        
        [TestMethod]
        public void Result_without_filtered_aggregation()
        {
            var result = "{\"took\":0,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":" +
                         "{\"total\":1501,\"max_score\":0,\"hits\":[]},\"aggregations\":{\"terms_Type\":{\"doc_count_error_upper_bound\":0," +
                         "\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"EarthFault\",\"doc_count\":1431,\"Cardinality\":{\"value\":1295}}," +
                         "{\"key\":\"EarthFaultRen1\",\"doc_count\":66,\"Cardinality\":{\"value\":66}},{\"key\":\"EarthFaultRen3\",\"doc_count\":" +
                         "4,\"Cardinality\":{\"value\":4}}]}}}";

            var js = JsonHelpers.CreateSerializerSettings();
            var resultObject = JsonConvert.DeserializeObject<DocumentSearchResult<Event>>(result, js);
            
        }

        [TestMethod]
        public void Result_without_filtered_and_unfiltered_aggregation()
        {
            var result = "{\"took\":1,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":" +
                         "{\"total\":1501,\"max_score\":0,\"hits\":[]},\"aggregations\":{\"terms_Type\":{\"doc_count_error_upper_bound\":0," +
                         "\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"EarthFault\",\"doc_count\":1431,\"Cardinality\":{\"value\":1295}}," +
                         "{\"key\":\"EarthFaultRen1\",\"doc_count\":66,\"Cardinality\":{\"value\":66}},{\"key\":\"EarthFaultRen3\"," +
                         "\"doc_count\":4,\"Cardinality\":{\"value\":4}}]},\"agg_filter\":{\"doc_count\":70,\"EarthFaultRen1234\":{\"value\":66}}}}";

            var js = JsonHelpers.CreateSerializerSettings();
            var resultObject = JsonConvert.DeserializeObject<DocumentSearchResult<Event>>(result, js);
            
            Assert.AreEqual(66, resultObject.Aggregations["EarthFaultRen1234"].Value);
            Assert.AreEqual(1295, resultObject.Aggregations["terms_Type"].Buckets.Single(b=>b.Key == "EarthFault").Cardinality.Value);
            Assert.AreEqual(66, resultObject.Aggregations["terms_Type"].Buckets.Single(b=>b.Key == "EarthFaultRen1").Cardinality.Value);
            Assert.AreEqual(4, resultObject.Aggregations["terms_Type"].Buckets.Single(b=>b.Key == "EarthFaultRen3").Cardinality.Value);
        }

        [TestMethod]
        public void Result_nested_aggs_with_tophits()
        {
            var result = "{\"took\":5,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":" +
                       "{\"total\":399,\"max_score\":0,\"hits\":[]},\"aggregations\":{\"terms_Asset.Name\":{\"doc_count_error_upper_bound\":" +
                       "6,\"sum_other_doc_count\":396,\"buckets\":[{\"key\":\"A063\",\"doc_count\":3,\"nested\":{\"doc_count_error_upper_bound\":" +
                       "0,\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"EarthFaultRen1\",\"doc_count\":1,\"TopHits\":{\"hits\":" +
                       "{\"total\":1,\"max_score\":null,\"hits\":[{\"_index\":\"esmartdev_esmart_earthfault_earthfaultevent\",\"_type\":" +
                       "\"eSmart.EarthFault.EarthFaultEvent\",\"_id\":\"323e3b4b-bbef-42be-b4e4-e0768f2302f9_EarthFaultRen1_636777437920937727\"," +
                       "\"_score\":null,\"_source\":{\"Type\":\"EarthFaultRen1\",\"Start\":\"2018-11-13T22:09:52.0937727+01:00\",\"Asset\":" +
                       "{\"ParticipantId\":1,\"Description\":\"Berghaug\",\"ValidTo\":\"2115-01-07T19:17:13.867\",\"StSrid\":4326,\"Long\":" +
                       "\"10.05398\",\"ValidFrom\":\"1915-01-07T19:17:13.867\",\"Id\":44866,\"AssetGuid\":\"323e3b4b-bbef-42be-b4e4-e0768f2302f9\"" +
                       ",\"GeoLocation\":{\"lon\":10.05398,\"lat\":60.20509},\"Lat\":\"60.20509\",\"Name\":\"A063\"},\"EventIdGuid\":" +
                       "\"323e3b4b-bbef-42be-b4e4-e0768f2302f9_EarthFaultRen1_636777437920937727\",\"End\":\"2018-11-15T02:28:30.637983+01:00\"," +
                       "\"Duration\":1698.642403505},\"sort\":[1542143392093]}]}}}]}}]}}}";

            var js = JsonHelpers.CreateSerializerSettings();
            var resultObject = JsonConvert.DeserializeObject<DocumentSearchResult<EarthFaultEvent>>(result, js);
        }

    }
}