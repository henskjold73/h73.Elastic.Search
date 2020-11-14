using System;

namespace eSmart.Elastic.Search.Tests
{
    public interface IIndexedClass
    {
        string AString { get; set; }
        IndexedClass Child { get; set; }
        DateTime DateTimeNotNullable { get; set; }
        DateTime? DateTimeNullable { get; set; }
        Guid ObjectId { get; set; }
        int SomeNumber { get; set; }
    }
}