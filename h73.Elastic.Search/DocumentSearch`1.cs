using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using h73.Elastic.Core;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Json;
using h73.Elastic.Core.Search.Results;
using h73.Elastic.Search.Exceptions;
using h73.Elastic.Search.Helpers;
using Newtonsoft.Json;
using Strings = h73.Elastic.Search.Const.Strings;

namespace h73.Elastic.Search
{
    /// <summary>
    /// Class for performing actions through an ElasticClient
    /// </summary>
    /// <typeparam name="T">Type of T</typeparam>
    public class DocumentSearch<T>
        where T : class
    {
        public DocumentSearch()
        {
            UseTypeIndex = true;
        }

        public bool UseTypeIndex { get; set; }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQuery">if set to <c>true</c> [return query].</param>
        /// <returns>DocumentSearchResult of T</returns>
        public DocumentSearchResult<T> Search(ElasticClient client, Query<T> query, JsonSerializerSettings jsonSerializerSettings = null, bool returnQuery = false)
        {
            return Do(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQuery);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQuery">if set to <c>true</c> [return query].</param>
        /// <returns>DocumentSearchResult of T</returns>
        public DocumentSearchResult<T> Search<T2>(ElasticClient client, Query<T2> query, JsonSerializerSettings jsonSerializerSettings = null, bool returnQuery = false)
            where T2 : class
        {
            return Do(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQuery);
        }

        /// <summary>
        /// Searches asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQuery">if set to <c>true</c> [return query].</param>
        /// <returns>DocumentSearchResult of T</returns>
        public async Task<DocumentSearchResult<T>> SearchAsync(ElasticClient client, Query<T> query, JsonSerializerSettings jsonSerializerSettings = null, bool returnQuery = false)
        {
            return await DoAsync(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQuery);
        }

        /// <summary>
        /// Searches asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="indexPattern">Index pattern</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQuery">if set to <c>true</c> [return query].</param>
        /// <returns>DocumentSearchResult of T</returns>
        public async Task<DocumentSearchResult<T>> SearchAsync(ElasticClient client, Query<T> query, string indexPattern ,JsonSerializerSettings jsonSerializerSettings = null, bool returnQuery = false)
        {
            return await DoAsync(client, query, indexPattern, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQuery);
        }

        /// <summary>
        /// Searches asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQuery">if set to <c>true</c> [return query].</param>
        /// <returns>DocumentSearchResult of T</returns>
        public async Task<DocumentSearchResult<T>> SearchAsync<T2>(ElasticClient client, Query<T> query, JsonSerializerSettings jsonSerializerSettings = null, bool returnQuery = false) where T2 : class
        {
            return await DoAsync<T2>(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQuery);
        }
        
        private async Task<DocumentSearchResult<T>> DoAsync<T2>(ElasticClient client, Query<T> query, string searchPath, JsonSerializerSettings jsonSerializerSettings, HttpMethod httpMethod, bool returnQuery) where T2 : class
        {
            var indexPattern = ServerHelpers.CreateIndexName<T2>(client.TenantId);

            var rq = string.IsNullOrEmpty(query.Scroll) ? null : new SearchRequestParameters { Scroll = TimeSpan.FromMinutes(5) };
            var response = await client.Client.DoRequestAsync<StringResponse>(httpMethod, $"{indexPattern}/{searchPath}", new CancellationToken(false), query.ToJson(), rq);
            CheckResponse(response);
            var result = JsonConvert.DeserializeObject<DocumentSearchResult<T>>(response.Body, jsonSerializerSettings);
            if (returnQuery)
            {
                result.Query = query;
            }

            return result;
        }

        /// <summary>
        /// Multiple searches.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="queries">The queries.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQueries">if set to <c>true</c> [return queries].</param>
        /// <returns>List of DocumentSearchResult of T</returns>
        public List<DocumentSearchResult<T>> MultipleSearch(ElasticClient client, MultipleQuery<T> queries, JsonSerializerSettings jsonSerializerSettings = null, bool returnQueries = false)
        {
            return Do(client, queries, Core.Const.Strings.MultiSearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQueries);
        }

        /// <summary>
        /// Multiple searches asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="queries">The queries.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQueries">if set to <c>true</c> [return queries].</param>
        /// <returns>List of DocumentSearchResult of T</returns>
        public async Task<List<DocumentSearchResult<T>>> MultipleSearchAsync(ElasticClient client, MultipleQuery<T> queries, JsonSerializerSettings jsonSerializerSettings = null, bool returnQueries = false)
        {
            return await DoAync(client, queries, Core.Const.Strings.MultiSearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQueries);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>SearchResult of T2</returns>
        public SearchResult<T2> Search<T2>(ElasticClient client, Query<T> query, JsonSerializerSettings jsonSerializerSettings = null)
            where T2 : class
        {
            return Do<T2>(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="type">Doc type</param>
        /// <returns>ElasticsearchResponse of string</returns>
        public ElasticsearchResponse<string> Search(ElasticClient client, string query, string type)
        {
            return Do(client, query, type, Core.Const.Strings.SearchPath, HttpMethod.POST);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="returnQuery">if set to <c>true</c> [return query].</param>
        /// <returns>SearchResult of T2</returns>
        public SearchResult<T2> Search<T2>(ElasticClient client, Query<T> query, Func<T, T2> selector, JsonSerializerSettings jsonSerializerSettings = null, bool returnQuery = false)
            where T2 : class
        {
            var result = Do(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, returnQuery);
            var output = result?.SelectResult(selector);
            return output;
        }

        /// <summary>
        /// Counts.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>SearchResult of T</returns>
        public SearchResult<T> Count(ElasticClient client, Query<T> query, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return Do(client, query, Core.Const.Strings.CountPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, false);
        }

        /// <summary>
        /// Searches asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="token">The token.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>SearchResult of T</returns>
        public async Task<SearchResult<T>> SearchAsync(ElasticClient client, Query<T> query, CancellationToken token, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return await DoAsync(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, token);
        }

        /// <summary>
        /// Searches asynchronous.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="token">The token.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>SearchResult of T2</returns>
        public async Task<SearchResult<T2>> SearchAsync<T2>(ElasticClient client, Query<T> query, CancellationToken token, JsonSerializerSettings jsonSerializerSettings = null)
            where T2 : class
        {
            return await DoAsync<T2>(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, token);
        }

        /// <summary>
        /// Searches asynchronous.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="token">The token.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>SearchResult of T2</returns>
        public async Task<SearchResult<T2>> SearchAsync<T2>(ElasticClient client, Query<T> query, Func<T, T2> selector, CancellationToken token, JsonSerializerSettings jsonSerializerSettings = null)
            where T2 : class
        {
            var result = await DoAsync(client, query, Core.Const.Strings.SearchPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, token);
            var output = result?.SelectResult(selector);
            return output;
        }

        /// <summary>
        /// Counts asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="query">The query.</param>
        /// <param name="token">The token.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>SearchResult of T</returns>
        public async Task<SearchResult<T>> CountAsync(ElasticClient client, Query<T> query, CancellationToken token, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return await DoAsync(client, query, Core.Const.Strings.CountPath, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings(), HttpMethod.POST, token);
        }

        /// <summary>
        /// Scrolls.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="assetSearchResult">The asset search result.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>DocumentSearchResult of T</returns>
        /// <exception cref="MissingValuesException">
        /// Search result missing ScrollId
        /// or
        /// Search result missing _Query. When searching set returnQuery to true.
        /// or
        /// Search result missing _Query's Scroll. When searching set returnQuery to true and remember to activate scrolling by using .SetScroll().
        /// </exception>
        public DocumentSearchResult<T> Scroll(ElasticClient client, DocumentSearchResult<T> assetSearchResult, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrEmpty(assetSearchResult.ScrollId))
            {
                throw new MissingValuesException("Search result missing ScrollId");
            }

            if (assetSearchResult.Query == null)
            {
                throw new MissingValuesException(
                    "Search result missing _Query. When searching set returnQuery to true.");
            }

            if (assetSearchResult.Query.Scroll == null)
            {
                throw new MissingValuesException(
                    "Search result missing _Query's Scroll. When searching set returnQuery to true and remember to activate scrolling by using .SetScroll().");
            }

            var body = new { scroll = assetSearchResult.Query.Scroll, scroll_id = assetSearchResult.ScrollId };
            var response = client.Client.DoRequest<StringResponse>(HttpMethod.POST, $"{Core.Const.Strings.ScrollPath}", body.ToJson(jsonSerializerSettings: jsonSerializerSettings));
            CheckResponse(response);
            var result =
                JsonConvert.DeserializeObject<DocumentSearchResult<T>>(response.Body, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings());
            result.Query = assetSearchResult.Query;

            return result;
        }

        /// <summary>
        /// Scrolls asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="assetSearchResult">The asset search result.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>DocumentSearchResult of T</returns>
        /// <exception cref="MissingValuesException">
        /// Search result missing ScrollId
        /// or
        /// Search result missing _Query. When searching set returnQuery to true.
        /// or
        /// Search result missing _Query's Scroll. When searching set returnQuery to true and remember to activate scrolling by using .SetScroll().
        /// </exception>
        public async Task<DocumentSearchResult<T>> ScrollAsync(ElasticClient client, DocumentSearchResult<T> assetSearchResult, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrEmpty(assetSearchResult.ScrollId))
            {
                throw new MissingValuesException("Search result missing ScrollId");
            }

            if (assetSearchResult.Query == null)
            {
                throw new MissingValuesException(
                    "Search result missing _Query. When searching set returnQuery to true.");
            }

            if (assetSearchResult.Query.Scroll == null)
            {
                throw new MissingValuesException(
                    "Search result missing _Query's Scroll. When searching set returnQuery to true and remember to activate scrolling by using .SetScroll().");
            }

            var body = new { scroll = assetSearchResult.Query.Scroll, scroll_id = assetSearchResult.ScrollId };
            var response = await client.Client.DoRequestAsync<StringResponse>(HttpMethod.POST, $"{Core.Const.Strings.ScrollPath}", new CancellationToken(false), body.ToJson(jsonSerializerSettings: jsonSerializerSettings));
            CheckResponse(response);
            var result =
                JsonConvert.DeserializeObject<DocumentSearchResult<T>>(response.Body, jsonSerializerSettings ?? JsonHelpers.CreateSerializerSettings());
            result.Query = assetSearchResult.Query;

            return result;
        }

        private static void CheckResponse(ElasticsearchResponse<string> response)
        {
            if (response?.Body == null || string.IsNullOrEmpty(response.Body))
            {
                if (!string.IsNullOrEmpty(response?.DebugInformation))
                {
                    throw QueryException.MissingBody(Strings.MissingBody, new Exception(response.DebugInformation, response.OriginalException));
                }

                throw QueryException.MissingBody(Strings.MissingBody, response?.OriginalException);
            }
        }

        private static SearchResult<T2> Do<T2>(ElasticClient client, Query<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod)
            where T2 : class
        {
            var response = client.Client.DoRequest<StringResponse>(httpMethod, $"{ServerHelpers.CreateIndexName<T2>(client.TenantId)}/{path}", query.ToJson());
            if (typeof(T).IsCastableTo(typeof(T2), true))
            {
                JsonConvert.DeserializeObject<SearchResult<T2>>(response.Body, jsonSettings);
            }

            return null;
        }

        private static ElasticsearchResponse<string> Do(ElasticClient client, string query, string type, string path, HttpMethod httpMethod)
        {
            var response = client.Client.DoRequest<StringResponse>(httpMethod, $"{ServerHelpers.CreateIndexName(client.TenantId, type)}/{path}", query);
            return response;
        }

        private static async Task<SearchResult<T>> DoAsync(ElasticClient client, Query<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, CancellationToken token)
        {
            var response = await client.Client.DoRequestAsync<StringResponse>(httpMethod, $"{ServerHelpers.CreateIndexName<T>(client.TenantId)}/{path}", token, query.ToJson());
            CheckResponse(response);
            return JsonConvert.DeserializeObject<SearchResult<T>>(response.Body, jsonSettings);
        }

        private static async Task<SearchResult<T2>> DoAsync<T2>(ElasticClient client, Query<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, CancellationToken token)
            where T2 : class
        {
            var response = await client.Client.DoRequestAsync<StringResponse>(httpMethod, $"{ServerHelpers.CreateIndexName<T>(client.TenantId)}/{path}", token, query.ToJson());
            CheckResponse(response);
            if (typeof(T).IsCastableTo(typeof(T2), true))
            {
                JsonConvert.DeserializeObject<SearchResult<T2>>(response.Body, jsonSettings);
            }

            return null;
        }

        private static string GetIndexPattern<T2>(ElasticClient client)
            where T2 : class
        {
            return string.IsNullOrEmpty(client.TenantId) ? "_all" : $"{ServerHelpers.CreateIndexName<T2>(client.TenantId)}";
        }

        private List<DocumentSearchResult<T>> Do(ElasticClient client, MultipleQuery<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, bool returnQueries)
        {
            var indexPattern = $"{client.TenantId}_*";
            var response = client.Client.DoRequest<StringResponse>(httpMethod, $"{indexPattern}/{path}", query.ToJson());
            CheckResponse(response);
            var responses = JsonConvert.DeserializeObject<MultipleDocumentSearchResult<T>>(response.Body, jsonSettings);
            var c = 0;
            var qs = query.ToArray();
            responses.Resonses.ForEach(result =>
            {
                result._Name = qs[c].Key;
                if (returnQueries)
                {
                    result.Query = qs[c].Value;
                }

                c++;
            });

            return responses.Resonses;
        }

        private async Task<List<DocumentSearchResult<T>>> DoAync(ElasticClient client, MultipleQuery<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, bool returnQueries)
        {
            var indexPattern = $"{client.TenantId}_*";
            var response = await client.Client.DoRequestAsync<StringResponse>(httpMethod, $"{indexPattern}/{path}", CancellationToken.None, query.ToJson());
            CheckResponse(response);
            var responses = JsonConvert.DeserializeObject<MultipleDocumentSearchResult<T>>(response.Body, jsonSettings);
            var c = 0;
            var qs = query.ToArray();
            responses.Resonses.ForEach(result =>
            {
                result._Name = qs[c].Key;
                if (returnQueries)
                {
                    result.Query = qs[c].Value;
                }

                c++;
            });

            return responses.Resonses;
        }
        
        private async Task<DocumentSearchResult<T>> DoAsync(ElasticClient client, Query<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, bool returnQuery)
        {
            var indexPattern = GetIndexPattern(client);
            return await DoAsync(client, query, indexPattern, path, jsonSettings, httpMethod, returnQuery);
        }

        private async Task<DocumentSearchResult<T>> DoAsync(ElasticClient client, Query<T> query, string indexPattern, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, bool returnQuery)
        {
            var rq = string.IsNullOrEmpty(query.Scroll) ? null : new SearchRequestParameters { Scroll = TimeSpan.FromMinutes(5) };
            var response = await client.Client.DoRequestAsync<StringResponse>(httpMethod, $"{indexPattern}/{path}", new CancellationToken(false), query.ToJson(), rq);
            CheckResponse(response);
            var result = JsonConvert.DeserializeObject<DocumentSearchResult<T>>(response.Body, jsonSettings);
            if (returnQuery)
            {
                result.Query = query;
            }

            return result;
        }

        private string GetIndexPattern(ElasticClient client, Type type = null)
        {
            if (UseTypeIndex)
            {
                return type.IsNotNull() ? ServerHelpers.CreateIndexName(client.TenantId, type.FullName) : ServerHelpers.CreateIndexName<T>(client.TenantId) ;
            }
            return string.IsNullOrEmpty(client.TenantId) ? "_all" : $"{client.TenantId}_*";
        }

        private DocumentSearchResult<T> Do(ElasticClient client, Query<T> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, bool returnQuery)
        {
            if (typeof(T).IsInterface)
            {
                throw new Exception("DocumentSearchResult<T> can not use an interface as a generic argument.");
            }

            var indexPattern = GetIndexPattern(client);
            var rq = string.IsNullOrEmpty(query.Scroll) ? null : new SearchRequestParameters { Scroll = TimeSpan.FromMinutes(5) };
            var response =
                client.Client.DoRequest<StringResponse>(httpMethod, $"{indexPattern}/{path}", query.ToJson(), rq);
            CheckResponse(response);
            var result = JsonConvert.DeserializeObject<DocumentSearchResult<T>>(response.Body, jsonSettings);
            if (returnQuery)
            {
                result.Query = query;
            }

            return result;
        }

        private DocumentSearchResult<T> Do<T2>(ElasticClient client, Query<T2> query, string path, JsonSerializerSettings jsonSettings, HttpMethod httpMethod, bool returnQuery)
            where T2 : class
        {
            if (typeof(T).IsInterface)
            {
                throw new Exception("DocumentSearchResult<T> can not use an interface as a generic argument.");
            }

            var indexPattern = GetIndexPattern<T2>(client);
            var rq = string.IsNullOrEmpty(query.Scroll) ? null : new SearchRequestParameters { Scroll = TimeSpan.FromMinutes(5) };
            var response =
                client.Client.DoRequest<StringResponse>(httpMethod, $"{indexPattern}/{path}", query.ToJson(), rq);
            CheckResponse(response);
            var result = JsonConvert.DeserializeObject<DocumentSearchResult<T>>(response.Body, jsonSettings);
            if (returnQuery)
            {
                result.Query = query;
            }

            return result;
        }
    }
}
