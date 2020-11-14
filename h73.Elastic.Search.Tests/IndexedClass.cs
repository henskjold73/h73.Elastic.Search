using System;
using System.Collections.Generic;
using eSmart.Elastic.Core.Data;

namespace eSmart.Elastic.Search.Tests
{
    public class IndexedClass : IIndexedClass
    {
        public int SomeNumber { get; set; }
        public Guid ObjectId { get; set; }
        public string AString { get; set; }
        public IndexedClass Child { get; set; }
        public List<IndexedClass> Children { get; set; }
        public DateTime DateTimeNotNullable { get; set; }
        public DateTime? DateTimeNullable { get; set; }
        public List<string> ListStrings { get; set; }
        public MockEnum MockEnum { get; set; }
        public MetaData<MockClass1> MetaData { get; set; }
    }

    public class InheritedGenericIndexedClass<T> : IndexedClass, IInheritedIndexedClass where T : IndexedClass
    {
        public string InheritedText { get; set; }
        public T GenericProperty { get; set; }
    }

    public class InheritedIndexedClass : IndexedClass, IInheritedIndexedClass
    {
        public string InheritedText { get; set; }
    }

    public class MockClass1
    {
        public string Mock1 { get; set; }
        public int Mock2 { get; set; }
        public float Mock3 { get; set; }
        public object Mock4 { get; set; }
        public string Mock5 { get; set; }
    }
}