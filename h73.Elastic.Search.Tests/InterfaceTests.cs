using System.Linq;
using eSmart.Elastic.Core.Search.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eSmart.Elastic.Search.Tests
{
    [TestClass]
    public class InterfaceTests
    {
        [TestMethod]
        public void Interface_Use_Interface()
        {
            var query = new Query<IIndexedClass>().Match(string.Empty, iic => iic.Child);
            Assert.AreNotEqual(null, query.QueryItem);
        }

        [TestMethod]
        public void Interface_Use_Class()
        {
            Query<IInheritedIndexedClass> query =
                new Query<IInheritedIndexedClass>().Match(string.Empty, nameof(InheritedIndexedClass.InheritedText));
            var key = ((MatchQuery<IInheritedIndexedClass>) ((BooleanQuery) query.QueryItem["bool"]).Must[0]).Match.Keys
                .FirstOrDefault();
            Assert.AreEqual("InheritedText", key);
        }
    }
}