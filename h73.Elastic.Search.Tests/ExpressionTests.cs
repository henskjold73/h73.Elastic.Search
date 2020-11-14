﻿using eSmart.Elastic.Core.Helpers;
using eSmart.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eSmart.Elastic.Search.Tests
{
    [TestClass]
    public class ExpressionTests
    {
        [TestMethod]
        public void Validate_DateTime_NotNullable()
        {
            new Query<IndexedClass>().Validate(ic => ic.DateTimeNotNullable, ic => ic.DateTimeNotNullable);
        }

        [TestMethod]
        public void Validate_DateTime_Nullable()
        {
            new Query<IndexedClass>().Validate(ic => ic.DateTimeNullable, ic => ic.DateTimeNullable);
        }

        [TestMethod]
        public void DateHistogram_Expression()
        {
            var json = new Query<IndexedClass>().DateHistogramAggregation(ic=>ic.MetaData.MetaPropertyName(i=>i.Mock4),"week").ToJson();
            Assert.AreEqual(json,"{\"query\":{\"bool\":{\"should\":[{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.IndexedClass\"}}," +
                                 "{\"type\":{\"value\":\"eSmart.Elastic.Search.Tests.InheritedGenericIndexedClass`1\"}},{\"type\":" +
                                 "{\"value\":\"eSmart.Elastic.Search.Tests.InheritedIndexedClass\"}}],\"minimum_should_match\":1}},\"aggs\":" +
                                 "{\"datehistogram_MetaData.Mock4\":{\"date_histogram\":{\"interval\":\"week\",\"field\":\"MetaData.Mock4\"}}}}");
        }
    }
}