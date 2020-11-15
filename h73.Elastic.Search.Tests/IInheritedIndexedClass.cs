namespace h73.Elastic.Search.Tests
{
    public interface IInheritedIndexedClass : IIndexedClass
    {
        string InheritedText { get; set; }
    }
}