using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Json;
using h73.Elastic.Core.Response;
using h73.Elastic.Core.Search.Results;
using h73.Elastic.Search.Helpers;
using h73.Elastic.Search.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class TopHitsAggsTests
    {
        [TestMethod]
        public void TopHits_Json()
        {
            var top = AggregationsHelper.TopHitsAggregation(
                ParameterHelper.Sort(
                    new KeyValuePair<Expression<Func<IndexedClass, object>>, AggsOrderDirection>(ic => ic.AString,
                        AggsOrderDirection.Desc),
                    new KeyValuePair<Expression<Func<IndexedClass, object>>, AggsOrderDirection>(ic => ic.SomeNumber,
                        AggsOrderDirection.Asc)
                ),
                ParameterHelper.Include<IndexedClass>(ic => ic.ObjectId, ic => ic.MockEnum)
            );
            var json = JsonConvert.SerializeObject(top, JsonHelpers.CreateSerializerSettings());

            Assert.AreEqual("{\"top_hits\":{\"_source\":{\"includes\":[\"ObjectId\",\"MockEnum\"]},\"sort\":[{\"AString\":\"Desc\"},{\"SomeNumber\":\"Asc\"}]}}", json);
        }

        [TestMethod]
        public void TopHitsResult_Json()
        {
            string json;
            using (StreamReader r = new StreamReader("Support/tophitsresult.json"))
            {
                json = r.ReadToEnd();
            }

            var obj = JsonConvert.DeserializeObject<SearchResult<Event>>(json, JsonHelpers.CreateSerializerSettings());
        }
    }
}