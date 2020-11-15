using System;
using h73.Elastic.Core.Const;
using h73.Elastic.Core.Search.Queries;
using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class BooleanRotCastTests
    {
        [TestMethod]
        public void Bq_Or()
        {
            var q1 = new Query<object>().Type(typeof(IndexedClass));
            var q2 = new Query<object>().For("Test");
            var q = q1.Or(q2);

            Assert.AreEqual(2, ((BooleanQuery)q.QueryItem[Strings.Bool]).Should.Count);
        }

        [TestMethod]
        public void Bq_Or_Func()
        {
            var q1 = new Query<object>().Type(typeof(IndexedClass));
            var q = q1.Or(q2 => q2.For("Test"));

            Assert.AreEqual(2, ((BooleanQuery)q.QueryItem[Strings.Bool]).Should.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Bq_Or_Func_NullCheck()
        {
            var q = ((Query<object>) null).Or(q2 => q2.For("Test"));
        }

        [TestMethod]
        public void Bq_Or_KeepTypeMatching()
        {
            var q1 = new Query<IndexedClass>().Type(typeof(InheritedIndexedClass)).Validate(query => query.DateTimeNotNullable, query => query.DateTimeNullable);
            var q2 = new Query<IndexedClass>().For("Test");
            var q = q1.Or(q2);

            var bq = (BooleanQuery)q.QueryItem[Strings.Bool];
            var shouldCount = ((BooleanQuery)((BooleanQueryRoot) bq.Should[0])[Strings.Bool]).Should.Count;

            Assert.AreEqual(3, shouldCount);
        }

        [TestMethod]
        public void Bq_And()
        {
            var q1 = new Query<object>().Type(typeof(IndexedClass));
            var q2 = new Query<object>().For("Test");
            var q = q1.And(q2);

            Assert.AreEqual(2, ((BooleanQuery)q.QueryItem[Strings.Bool]).Must.Count);
        }

        [TestMethod]
        public void Bq_And_Func()
        {
            var q1 = new Query<object>().Type(typeof(IndexedClass));
            var q = q1.And(q2 => q2.For("Test"));

            Assert.AreEqual(2, ((BooleanQuery)q.QueryItem[Strings.Bool]).Must.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Bq_And_Func_NullCheck()
        {
            var q = ((Query<object>) null).And(q2 => q2.For("Test"));
        }

        [TestMethod]
        public void Bq_And_KeepTypeMatching()
        {
            var q1 = new Query<IndexedClass>().Type(typeof(InheritedIndexedClass)).Validate(query => query.DateTimeNotNullable, query => query.DateTimeNullable);
            var q2 = new Query<IndexedClass>().For("Test");
            var q = q1.And(q2);

            var bq = (BooleanQuery)q.QueryItem[Strings.Bool];
            var shouldCount = ((BooleanQuery)((BooleanQueryRoot)bq.Must[0])[Strings.Bool]).Should.Count;

            Assert.AreEqual(3, shouldCount);
        }

        [TestMethod]
        public void Bqr_And_PostFilter()
        {
            var q1 = new Query<IndexedClass>(true).Type(typeof(InheritedIndexedClass)).Term("TERM", query => query.AString);
            var q2 = new Query<IndexedClass>(true).TermsAggregation(x=>x.AString);
            var q = q2.AddToPostFilter(q1);

            var json = q.ToJson();

            Assert.AreEqual("{\"query\":{\"bool\":{}},\"post_filter\":{\"bool\":{\"must\":" +
                            "[{\"type\":{\"value\":\"h73.Elastic.Search.Tests.InheritedIndexedClass\"}}," +
                            "{\"term\":{\"AString\":\"TERM\"}}]}},\"aggs\":{\"terms_AString\":{\"terms\":" +
                            "{\"field\":\"AString\"}}}}", json);
        }
    }
}