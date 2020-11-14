using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Bogus;
using eSmart.Elastic.Core;
using eSmart.Elastic.Core.Enums;
using eSmart.Elastic.Core.Helpers;
using eSmart.Elastic.Core.Search.Aggregations;
using eSmart.Elastic.Core.Search.Interfaces;
using eSmart.Elastic.Search.Helpers;
using eSmart.Elastic.TypeMapping;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using Newtonsoft.Json;

namespace eSmart.Elastic.Search.IntegrationTests
{
    [TestClass]
    public class UnitTests
    {
        ElasticClient _client;

        [TestInitialize]
        public void TestInitialize1()
        {
            var faker = new Faker();
            var itemCountGenerator = new RandomGenerator();
            
            var customers = Builder<Customer>.CreateListOfSize(1000)
                .All()
                .With(c => c.FirstName = faker.Name.FirstName())
                .With(c => c.LastName = faker.Name.LastName())
                .With(c => c.TelephoneNumber = faker.Phone.PhoneNumber())
                .With(c => c.Id = Guid.NewGuid().ToString())
                .With(c => c.Created = faker.Date.Past())
                .With(c => c.Age = itemCountGenerator.Next(1, 200))
                .Random(70)
                .With(c => c.EmailAddress = faker.Internet.Email())
                .Build();
            
            customers.ForEach(c =>
            {
                c.Products = Builder<Product>.CreateListOfSize(itemCountGenerator.Next(1, 200))
                    .All()
                    .With(oi => oi.Description = faker.Lorem.Sentence(itemCountGenerator.Next(3, 10)))
                    .With(oi => oi.Id = Guid.NewGuid().ToString())
                    .With(oi => oi.Name = faker.Hacker.Noun())
                    .With(oi => oi.Price = faker.Finance.Amount())
                    .Build().ToList();
            });
            
            var configuration =
                IntegrationTestHelper.GetApplicationConfiguration();

            _client = new ElasticClient(configuration.Protocol, configuration.Host, configuration.Port,
                true, configuration.Username, configuration.Password, "test");

            var indexer = new ElasticIndexer<Customer>(_client);

            var tmap = new TypeMapping<Customer>();
            tmap.AddMapping(
                new KeyValuePair<Expression<Func<Customer, object>>, FieldTypes>(customer => customer.Products,
                    FieldTypes.Nested),
                new KeyValuePair<Expression<Func<Customer, object>>, FieldTypes>(customer => customer.FirstName,
                    FieldTypes.Keyword),
                new KeyValuePair<Expression<Func<Customer, object>>, FieldTypes>(customer => customer.LastName,
                    FieldTypes.Keyword));

            indexer.DeleteIndex(ServerHelpers.CreateIndexName<Customer>("test"));
            indexer.CreateIndex(tmap);
            indexer.IndexBulk(customers, customer => customer.Id, 100);

            Thread.Sleep(3000);
        }

        [TestMethod]
        public void Nested_Avg()
        {
            var q = new Query<Customer>(true).NestedAggregation(customer => customer.Products,
                    AggregationsHelper.AvgAggregation<Customer>(
                        customer => customer.Products.PropertyName(p => p.Price)))
                .SetSize(0);

            var result = new DocumentSearch<Customer>().Search(_client, q);

            Assert.AreEqual(1000, result.Hits.Total);

            var nested = result.NestedAggregation(customer => customer.Products);
            Assert.IsNotNull(nested.Nested.Value);
        }

        [TestMethod]
        public void AddAggregation_Helper()
        {
            var q = new Query<Customer>(true).SetSize(0);

            var aggr = AggregationsHelper
                .DateHistogram<Customer>(ic => ic.Created, "week",
                    order: new AggsOrder(AggsOrderBy.Key, AggsOrderDirection.Desc))
                .TermsAggregation("_id", order: new AggsOrder("sum", AggsOrderDirection.Desc));

            aggr.Aggregations["nested"].Add("sum",
                AggregationsHelper.SumAggregation<Customer>(ic =>
                    ic.Age));
            aggr.Aggregations["nested"].Add("above25percent_filter",
                AggregationsHelper.BucketSelector("sum", "sum", "params.sum > 180"));

            q.AddAggregation(aggr);

            var result = new DocumentSearch<Customer>().Search(_client, q);
            var b = result.Aggregations["agg_DateHistogram"].Buckets.Where(x=>x.Nested.Buckets.Any());
        }
    }
}