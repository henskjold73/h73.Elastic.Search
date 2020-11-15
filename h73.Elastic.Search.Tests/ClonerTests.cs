using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace h73.Elastic.Search.Tests
{
    [TestClass]
    public class ClonerTests
    {
        [TestMethod]
        public void Clone_Query_NoClone_SameValue()
        {
            var q1 = new Query<IndexedClass>();
            var q2 = q1;

            q1.SetSize(100);

            Assert.AreEqual(q1.Size, q2.Size);
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void Clone_Query_Clone_DiffValue()
        {
            var q1 = new Query<IndexedClass>();
            var q2 = q1.Clone();

            Assert.AreNotEqual(q1, q2);

            q1.SetSize(100);

            Assert.AreEqual(100, q1.Size);
            Assert.AreEqual(10, q2.Size);

            
        }
    }
}