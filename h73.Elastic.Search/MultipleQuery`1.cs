using System.Collections.Generic;
using System.Linq;
using h73.Elastic.Core.Const;
using h73.Elastic.Core.Search.Interfaces;
using Newtonsoft.Json;

namespace h73.Elastic.Search
{
    /// <summary>
    /// MultipleQuery wraps a list of queries
    /// </summary>
    /// <typeparam name="T">Type of T</typeparam>
    /// <seealso cref="Query{T}" />
    public class MultipleQuery<T> : Dictionary<string, Query<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleQuery{T}"/> class.
        /// </summary>
        public MultipleQuery()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleQuery{T}"/> class.
        /// </summary>
        /// <param name="queries">The queries.</param>
        public MultipleQuery(IList<IQuery> queries)
        {
            foreach (var query in queries)
            {
                this[query._Name] = new Query<T>(query);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleQuery{T}"/> class.
        /// </summary>
        /// <param name="queries">The queries.</param>
        public MultipleQuery(List<Query<T>> queries)
        {
            foreach (var query in queries)
            {
                this[query.QueryItem[Strings.Bool]._Name] = query;
            }
        }

        /// <summary>
        /// To json.
        /// </summary>
        /// <returns>Json</returns>
        public string ToJson()
        {
            const string n = "\n";
            var output = this.Select(aq =>
                JsonConvert.SerializeObject(new { }) + n +
                JsonConvert.SerializeObject(aq.Value));
            return string.Join(n, output) + n;
        }
    }
}
