using System.Collections.Generic;
using h73.Elastic.Core.Search.Results;
using Newtonsoft.Json;

namespace h73.Elastic.Search
{
    /// <summary>
    /// MultipleDocumentSearchResult of Type T
    /// </summary>
    /// <typeparam name="T">Type of T</typeparam>
    /// <seealso cref="h73.Elastic.Core.Search.Results.MultipleSearchResult{T}" />
    public class MultipleDocumentSearchResult<T> : MultipleSearchResult<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets the resonses.
        /// </summary>
        /// <value>
        /// The resonses.
        /// </value>
        [JsonProperty("responses")]
        public new List<DocumentSearchResult<T>> Resonses { get; set; }
    }
}
