using System.Collections.Generic;
using System.Linq;
using eSmart.Elastic.Core.Const;
using eSmart.Elastic.Core.Enums;
using eSmart.Elastic.Core.Helpers;
using eSmart.Elastic.Core.Search.Interfaces;
using eSmart.Elastic.Core.Search.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eSmart.Elastic.Search.Tests
{
    [TestClass]
    public class AssetMultipleSearchTests
    {
        [TestMethod]
        public void AddMultipleAssetSearches_4In_5out()
        {
           
            var text = "ABC";
            var queries = new List<IQuery>
            {
                new MatchQuery<IndexedClass>(text, t => t.SomeNumber).Name("SomeNumber"),
                new MatchQuery<IndexedClass>(text, t => t.ObjectId).Name("Id"),
                new MatchQuery<IndexedClass>(text, t => t.AString).Name("A"),
                new MatchQuery<IndexedClass>(text, t => t.Child.AString).Name("Kid")
            };


            var jsonQuery = new Query<IndexedClass>().AddQueries(queries, BooleanQueryType.Should).SetSize(3)
                .ToMultipleQuery();
            Assert.AreEqual(5, jsonQuery.Count);
        }

        [TestMethod]
        public void AddMultipleFreeTextAssetSearches_4In_5out()
        {

            var text = "ABC";
            var queries = new List<IQuery>
            {
                new MatchQuery<IndexedClass>(text, t => t.SomeNumber).Name("SomeNumber"),
                new MatchQuery<IndexedClass>(text, t => t.ObjectId).Name("Id"),
                new MatchQuery<IndexedClass>(text, t => t.AString).Name("A"),
                new MatchQuery<IndexedClass>(text, t => t.Child.AString).Name("Kid")
            };


            var jsonQuery = new Query<IndexedClass>().AddQueries(queries, BooleanQueryType.Should).SetSize(3)
                .ToMultipleQueryFreeText(text);
            var allQuery = ((BooleanQuery) jsonQuery["_all"].QueryItem[Strings.Bool]).Must.FirstOrDefault() as QueryStringQuery;
            Assert.AreEqual(text, allQuery?.QueryString.Query);
        }

        [TestMethod]
        public void AddMultipleAssetQueries_4In_4out()
        {

            var text = "ABC";
            var queries = new List<Query<IndexedClass>>
            {
                new Query<IndexedClass>().Match(text, t => t.SomeNumber).Name("SomeNumber"),
                new Query<IndexedClass>().Match(text, t => t.ObjectId).Name("Id"),
                new Query<IndexedClass>().Match(text, t => t.AString).Name("A"),
                new Query<IndexedClass>().Match(text, t => t.Child.AString).Name("Kid")
            };


            var mQuery = new MultipleQuery<IndexedClass>(queries);
            Assert.AreEqual(4, mQuery.Count);
        }
    }
}