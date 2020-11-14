using System.Collections.Generic;

namespace h73.Elastic.Search.Helpers
{
    public static class DictionaryHelper
    {
        public static T ValueByKey<T2,T>(this Dictionary<T2, T> d, T2 key)
        {
            return d.ContainsKey(key) ? d[key] : default(T);
        }

    }
}