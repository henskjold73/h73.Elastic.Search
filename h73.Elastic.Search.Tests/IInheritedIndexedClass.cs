namespace eSmart.Elastic.Search.Tests
{
    public interface IInheritedIndexedClass : IIndexedClass
    {
        string InheritedText { get; set; }
    }
}