using System;
using eSmart.Common.DataModel.BusinessObject;
using eSmart.Common.DataModel.BusinessObject.Asset;
using eSmart.Common.DataModel.BusinessObject.Asset.AggregationPoint;
using eSmart.Common.DataModel.BusinessObject.Resource;
using eSmart.Elastic.Core.Json;
using eSmart.Elastic.Core.Search.Results;
using eSmart.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace eSmart.Elastic.Search.Bugs.Tests
{
    [TestClass]
    public class TypedTests
    {
        [TestMethod]
        public void Typed_Serialize()
        {

            var q = new Query<Asset>().Match("aaaa-bbbbbb-cccc-dddd", asset => asset.AssetGuid);
            var json1 = q.ToJson();
            var json2 = q.ToJson(jsonSerializerSettings: JsonHelpers.CreateSerializerSettings(typeNameHandling: TypeNameHandling.Objects));

            var t = new AggregationPoint
            {
                Address = new Address
                {
                    City = "Halden",
                    Id = 175,
                    ValidDate = DateTime.Parse("2010-01-01T00:00:00")
                },
                Resource = new SiteResource
                {
                    AssetId = 185
                }
            };

            var nn = new DocumentSearchResult<AggregationPoint>
            {
                Hits = new Hits<AggregationPoint>
                {
                    HitsList = new[]
                    {
                        new Hit<AggregationPoint>
                        {
                            Source = t
                        }
                    }
                }
            };



            var json = JsonConvert.SerializeObject(t,
                JsonHelpers.CreateSerializerSettings(typeNameHandling: TypeNameHandling.Objects));

            var obj = JsonConvert.DeserializeObject<Asset>(json,
                JsonHelpers.CreateSerializerSettings(typeNameHandling: TypeNameHandling.Objects));
        }
    }
}