using eSmart.Elastic.Core.Search.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace eSmart.Elastic.Search.Tests
{
    [TestClass]
    public class InnerHitsTests
    {
        [TestMethod]
        public void InnerHitsTests_Deserialize()
        {
            var result = "{\"took\":1,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":{\"total\":895,\"max_score\":1," +
                         "\"hits\":[{\"_index\":\"esmartdev_esmart_groundfault_component\",\"_type\":\"eSmart.GroundFault.Component\",\"_id\":\"9740e6f9-07fe-4624-9736-fff265751a8f\"," +
                         "\"_score\":1,\"_source\":{},\"inner_hits\":{\"Events\":{\"hits\":{\"total\":2,\"max_score\":1,\"hits\":[{\"_index\":\"esmartdev_esmart_groundfault_component\"," +
                         "\"_type\":\"eSmart.GroundFault.Component\",\"_id\":\"9740e6f9-07fe-4624-9736-fff265751a8f\",\"_nested\":{\"field\":\"Events\",\"offset\":7},\"_score\":1,\"_source" +
                         "\":{\"Type\":\"GroundFault\",\"Start\":\"2017-10-08T09:29:56.9326293+02:00\",\"End\":\"2017-10-09T16:37:56.9326293+02:00\",\"CaseCreated\":false,\"PowerCurrent\":79.34553667360244," +
                         "\"Voltage\":86.8306860112728,\"Duration\":1868}},{\"_index\":\"esmartdev_esmart_groundfault_component\",\"_type\":\"eSmart.GroundFault.Component\"," +
                         "\"_id\":\"9740e6f9-07fe-4624-9736-fff265751a8f\",\"_nested\":{\"field\":\"Events\",\"offset\":0},\"_score\":1,\"_source\":{\"Type\":\"GroundFault\"," +
                         "\"Start\":\"2018-08-05T09:29:56.9322674+02:00\",\"End\":\"2018-08-06T17:52:56.9322674+02:00\",\"CaseCreated\":false,\"PowerCurrent\":52.268288879314575," +
                         "\"Voltage\":111.51511493302654,\"Duration\":1943}}]}}}}]}}";

            var jsonObject = JsonConvert.DeserializeObject<SearchResult<IndexedClass>>(result);
            Assert.AreEqual("9740e6f9-07fe-4624-9736-fff265751a8f",jsonObject.Hits.HitsList[0].InnerHits["Events"].Hit.HitsList[0].Id);
        }
    }
}

