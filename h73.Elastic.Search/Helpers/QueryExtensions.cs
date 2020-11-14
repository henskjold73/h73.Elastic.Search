using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using h73.Elastic.Core.Const;
using h73.Elastic.Core.Search.Queries;

namespace h73.Elastic.Search.Helpers
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Ors the specified or query function.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="orQueryFunc">The or query function.</param>
        /// <returns>Query of T</returns>
        /// <exception cref="ArgumentNullException">query</exception>
        public static Query<T> Or<T>(this Query<T> query, Func<Query<T>, Query<T>> orQueryFunc)
            where T : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var query2 = orQueryFunc(new Query<T>());
            return query.Or(query2);
        }

        /// <summary>
        /// Ands the specified and query function.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="andQueryFunc">The and query function.</param>
        /// <returns>Query of T</returns>
        /// <exception cref="ArgumentNullException">query</exception>
        public static Query<T> And<T>(this Query<T> query, Func<Query<T>, Query<T>> andQueryFunc)
            where T : class
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var query2 = andQueryFunc(new Query<T>());
            return query.And(query2);
        }

        /// <summary>
        /// To the query.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="queryValue">The query value.</param>
        /// <param name="searchFilters">The search filters.</param>
        /// <returns>Query of T</returns>
        public static Query<T> ToQuery<T>(object queryValue, List<Expression<Func<T, string>>> searchFilters)
            where T : class
        {
            var query = new Query<T>();

            foreach (var searchFilter in searchFilters)
            {
            }

            return query;
        }

        public static BooleanQueryRoot CreateBooleanQueryRoot<T>(this Query<T> q)
        {
            return new BooleanQueryRoot { [Strings.Bool] = q.QueryItem[Strings.Bool] };
        }
    }
}
