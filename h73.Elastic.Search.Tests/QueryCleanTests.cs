using h73.Elastic.Search.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class QueryCleanTests
    {
        [TestMethod]
        public void QueryClean_Complex()
        {
            var query = new Query<IndexedClass>(true);
            query.And(new Query<IndexedClass>(true).Match(1, o => o.AString));
            
            query.Or(new Query<IndexedClass>(true).Match(21, o => o.AString));

            var json = query.Clean().ToJson();
        }
    }
}