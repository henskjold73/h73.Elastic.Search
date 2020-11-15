using System;
using System.Threading.Tasks;
using h73.Elastic.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class ExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExceptionOnMissingHost()
        {
            var client = new ElasticClient(node: "MissingNode");
            var q = new Query<object>();
            var r = Task.Run(() => new DocumentSearch<object>().SearchAsync(client, q)).Result;
        }
    }
}