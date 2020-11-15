using h73.Elastic.Core.Json;
using h73.Elastic.Search.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace h73.Elastic.Search.Tests.ResultTests
{
    [TestClass]
    public class VoltageQualityTests
    {
        [TestMethod]
        public void VqResult_01()
        {
            var result = "{\"took\":31,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"skipped\":0,\"failed\":0},\"hits\":{\"total\":50888,\"max_score\":0,\"hits\":[]},\"aggregations\":" +
                         "{\"agg_filter_1\":{\"meta\":{},\"doc_count\":100,\"EventTypesSub\":{\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"UnderVoltage\"," +
                         "\"doc_count\":96,\"Cardinality\":{\"value\":65}},{\"key\":\"OverVoltage\",\"doc_count\":4,\"Cardinality\":{\"value\":3}}]},\"WorkOrderStateSub\":{\"doc_count_error_upper_bound\"" +
                         ":0,\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"New\",\"doc_count\":100}]}},\"cardinality_Asset.AssetGuid\":{\"value\":23062},\"agg_filter_0\":{\"meta\":{},\"doc_count\":547," +
                         "\"EventTypesAmi\":{\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"UnderVoltage\",\"doc_count\":506,\"Cardinality\":{\"value\":506}},{\"key\":" +
                         "\"OverVoltage\",\"doc_count\":41,\"Cardinality\":{\"value\":41}}]},\"WorkOrderStateAmi\":{\"doc_count_error_upper_bound\":0,\"sum_other_doc_count\":0,\"buckets\":[{\"key\":\"New\"," +
                         "\"doc_count\":547}]}}}}";

            var obj = JsonConvert.DeserializeObject<DocumentSearchResult<VoltageQualityEvent>>(result, JsonHelpers.CreateSerializerSettings());

        }
    }
}