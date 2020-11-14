using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using h73.Elastic.Core;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Search.Results;
using h73.Elastic.Search.Interfaces;
using Newtonsoft.Json;

namespace h73.Elastic.Search
{
    /// <summary>
    /// Document search result
    /// </summary>
    /// <typeparam name="T">Type of T</typeparam>
    /// <seealso cref="h73.Elastic.Core.Search.Results.SearchResult{T}" />
    public class DocumentSearchResult<T> : SearchResult<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        [JsonIgnore]
        public IFullQuery Query { get; set; }

        /// <summary>
        /// Scrolls.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>DocumentSearchResult of T with all hits</returns>
        public DocumentSearchResult<T> Scroll(ElasticClient client)
        {
            return new DocumentSearch<T>().Scroll(client, this);
        }

        /// <summary>
        /// Scrolls asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>DocumentSearchResult of T with all hits</returns>
        public async Task<DocumentSearchResult<T>> ScrollAsync(ElasticClient client)
        {
            return await new DocumentSearch<T>().ScrollAsync(client, this);
        }

        /// <summary>
        /// Returns the matching Aggregation of type Terms
        /// </summary>
        /// <param name="fieldExpression">The same expression used in the query.</param>
        /// <returns>Aggregation</returns>
        public Aggregation TermsAggregation(Expression<Func<T, object>> fieldExpression)
        {
            return GetAggregation("terms", fieldExpression);
        }

        /// <summary>
        /// Returns the matching Aggregation of type Terms
        /// </summary>
        /// <param name="fieldExpression">The same expression used in the query.</param>
        /// <returns>Aggregation</returns>
        public Aggregation FilterAggregation()
        {
            return Aggregations["agg_filter"];
        }

        /// <summary>
        /// Returns the matching Aggregation of type Nested
        /// </summary>
        /// <param name="fieldExpression">The same expression used in the query.</param>
        /// <param name="limit">How many levels to check (default: 100)</param>
        /// <returns>Aggregation</returns>
        public Aggregation NestedAggregation(Expression<Func<T, object>> fieldExpression, bool filtered = false, int limit = 100)
        {
            var exists = false;
            var c = 0;
            var aggName = $"nested_{ExpressionHelper.GetPropertyName(fieldExpression)}_{c}";
            
            while (!exists && c < limit)
            {
                if (Aggregations.ContainsKey(aggName))
                {
                    exists = true;
                    continue;
                }

                c++;
                aggName = $"nested_{ExpressionHelper.GetPropertyName(fieldExpression)}_{c}";
            }
            return Aggregations[aggName];
        }

        private Aggregation GetAggregation(string type, Expression<Func<T, object>> fieldExpression)
        {
            return Aggregations[$"{type}_{ExpressionHelper.GetPropertyName(fieldExpression)}"];
        }
    }
}
