using System.Linq;
using h73.Elastic.Core.Search.Results;

namespace h73.Elastic.Search.Helpers
{
    public static class ResultExtensions
    {
        public static Bucket ByKey(this Bucket[] buckets, string key)
        {
            return buckets.SingleOrDefault(b => b.Key == key);
        }
    }
}