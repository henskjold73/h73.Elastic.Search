using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using h73.Elastic.Core.Const;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Search;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;
using h73.Elastic.Core.Search.Queries;
using h73.Elastic.Search.Helpers;
using h73.Elastic.Search.Interfaces;
using MoreLinq;
using Newtonsoft.Json;

namespace h73.Elastic.Search
{
    /// <summary>
    /// Query object to be serialized and sent to Elasticsearch
    /// </summary>
    /// <typeparam name="T">Type of T</typeparam>
    /// <seealso cref="h73.Elastic.Search.Interfaces.IFullQuery" />
    [Serializable]
    public class Query<T> : IFullQuery
    {
        private int _size = -1;
        private SearchType _searchType = SearchType.Search;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{T}"/> class.
        /// </summary>
        public Query()
        {
            PrepareBooleanQuery();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{T}" /> class.
        /// </summary>
        /// <param name="noTypeFilter">if set to <c>true</c> [no type filter].</param>
        public Query(bool noTypeFilter)
        {
            PrepareBooleanQuery(noTypeFilter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{T}"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="boolQueryOperator">The bool query operator.</param>
        public Query(IQuery query, BooleanQueryType boolQueryOperator = BooleanQueryType.Must)
        {
            PrepareBooleanQuery();
            AddQuery(query, boolQueryOperator);
        }

        /// <summary>
        /// Gets or sets the query item.
        /// </summary>
        /// <value>
        /// The query item.
        /// </value>
        [JsonProperty("query")]
        public Dictionary<string, IQuery> QueryItem { get; set; }

        
        [JsonProperty("post_filter", NullValueHandling = NullValueHandling.Ignore)]
        public IQuery PostFilter { get; set; }

        /// <summary>
        /// Gets or sets the aggregations.
        /// </summary>
        /// <value>
        /// The aggregations.
        /// </value>
        [JsonProperty("aggs")]
        public Dictionary<string, IAggregation> Aggregations { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [JsonIgnore]
        public Source Source { get; set; }

        /// <summary>
        /// Gets the output source.
        /// </summary>
        /// <value>
        /// The output source.
        /// </value>
        [JsonProperty("_source", NullValueHandling = NullValueHandling.Ignore)]
        public object OutputSource => Source?.Output;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        [JsonProperty("size")]
        public int Size
        {
            get => _size == -1 ? 10 : _size;
            set => _size = value;
        }

        /// <summary>
        /// Gets or sets min_score.
        /// </summary>
        /// <value>
        /// min_score.
        /// </value>
        [JsonProperty("min_score", NullValueHandling = NullValueHandling.Ignore)]
        public double? MinScore { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        [JsonProperty("from")]
        public int From { get; set; }

        /// <summary>
        /// Gets or sets the scroll.
        /// </summary>
        /// <value>
        /// The scroll.
        /// </value>
        [JsonProperty("scroll")]
        public string Scroll { get; set; }

        /// <summary>
        /// Gets or sets the scroll identifier.
        /// </summary>
        /// <value>
        /// The scroll identifier.
        /// </value>
        [JsonProperty("scroll_id")]
        public string ScrollId { get; set; }

        /// <summary>
        /// Gets or sets the sorting.
        /// </summary>
        /// <value>
        /// The sorting.
        /// </value>
        [JsonProperty("sort", NullValueHandling = NullValueHandling.Ignore)]
        public Sort Sorting { get; set; }

        /// <summary>
        /// Gets or sets the types list.
        /// </summary>
        /// <value>
        /// The types list.
        /// </value>
        [JsonIgnore]
        public List<string> TypesList { get; set; }

        /// <summary>
        /// Gets the boolean query.
        /// </summary>
        /// <value>
        /// The boolean query.
        /// </value>
        [JsonIgnore]
        public BooleanQuery BooleanQuery => (BooleanQuery) QueryItem[Strings.Bool];

        /// <summary>
        /// Shoulds serialize query.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSerializeQuery()
        {
            if (QueryItem.ContainsKey(Strings.Bool))
            {
                return ((BooleanQuery) QueryItem[Strings.Bool]).Must.Any()
                       || ((BooleanQuery) QueryItem[Strings.Bool]).MustNot.Any()
                       || ((BooleanQuery) QueryItem[Strings.Bool]).Should.Any();
            }

            return false;
        }

        /// <summary>
        /// Shoulds serialize aggregations.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSerializeAggregations()
        {
            return _searchType == SearchType.Search && Aggregations.Any();
        }

        /// <summary>
        /// Shoulds size serialize.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSerializeSize()
        {
            return Size != 10;
        }

        /// <summary>
        /// Shoulds serialize from.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSerializeFrom()
        {
            return From > 0;
        }

        /// <summary>
        /// Shoulds serialize scroll.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSerializeScroll()
        {
            return !string.IsNullOrEmpty(Scroll) && !string.IsNullOrEmpty(ScrollId);
        }

        /// <summary>
        /// Shoulds serialize scroll identifier.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSerializeScrollId()
        {
            return !string.IsNullOrEmpty(ScrollId);
        }

        /// <summary>
        /// Sets the size of the result.
        /// </summary>
        /// <param name="size">Size of result</param>
        /// <returns>Self</returns>
        public Query<T> SetSize(int size)
        {
            _size = size;
            return this;
        }

        /// <summary>
        /// Sets min_score.
        /// </summary>
        /// <param name="minScore">Min score</param>
        /// <returns>Self</returns>
        public Query<T> SetMinScore(double minScore)
        {
            MinScore = minScore;
            return this;
        }

        /// <summary>
        /// Sets from, how much of the result to skip.
        /// </summary>
        /// <param name="from">Size of skip</param>
        /// <returns>Self</returns>
        public Query<T> SetFrom(int from)
        {
            From = from;
            return this;
        }

        /// <summary>
        /// Everything is counted as a match.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> MatchAll()
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Must.Add(QueryHelper.MatchAll());
            return this;
        }

        /// <summary>
        /// Search for a query string. (Normal free text search)
        /// </summary>
        /// <param name="text">The query string to search for.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> For(string text, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            return FreeText(text, boolType);
        }

        /// <summary>
        /// Nested query
        /// </summary>
        /// <param name="fieldExpression">Field that is nested</param>
        /// <param name="qItem">Nested query</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="matchingInnerHits">Return inner hits that match the nested query. <b>Remember to adjust source for the main query!</b></param>
        /// <param name="customInnerHits">InnerHits query object</param>
        /// <param name="scoreMode">score_mode</param>
        /// <returns>Self</returns>
        public Query<T> Nested(Expression<Func<T, object>> fieldExpression, Dictionary<string, IQuery> qItem,
            BooleanQueryType boolType = BooleanQueryType.Must, bool matchingInnerHits = false,
            InnerHits customInnerHits = null, ScoreMode? scoreMode = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            var nested = new NestedQuery
                {Nested = new NestedItem {Path = fieldName, Query = qItem, ScoreMode = scoreMode}};
            if (matchingInnerHits || customInnerHits != null)
                nested.Nested.InnerHits = customInnerHits ?? new InnerHits();
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(nested, boolType);
            return this;
        }

        /// <summary>
        /// Search for a query string. (Normal free text search)
        /// </summary>
        /// <param name="text">The query string to search for.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> FreeText(string text, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new QueryStringQuery(text), boolType);
            return this;
        }

        /// <summary>
        /// Exlude results with null values. (null, [], [null])
        /// </summary>
        /// <param name="fieldExpression">Property check for null values.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Exists(
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new ExistsQuery(fieldName), boolType);
            return this;
        }

        /// <summary>
        /// Exlude results with null values. (null, [], [null])
        /// </summary>
        /// <param name="fieldName">Property check for null values.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Exists(string fieldName, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new ExistsQuery(fieldName), boolType);
            return this;
        }

        /// <summary>
        /// Filter results by ids.
        /// </summary>
        /// <param name="ids">Array of ids.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Ids(string[] ids, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new IdsQuery(ids), boolType);
            return this;
        }

        /// <summary>
        /// Filter results by match in a field with some fuzziness.
        /// </summary>
        /// <param name="text">The text to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Match(
            string text,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            return MatchBase(text, fieldExpression, boolType);
        }

        /// <summary>
        /// Filter results by match in a field with some fuzziness.
        /// </summary>
        /// <param name="value">The value to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Match(
            int value,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            return MatchBase(value, fieldExpression, boolType);
        }

        /// <summary>
        /// Filter results by match in a field with some fuzziness.
        /// </summary>
        /// <param name="value">The value to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Match(
            double value,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            return MatchBase(value, fieldExpression, boolType);
        }

        /// <summary>
        /// Filter results by match in a field with some fuzziness.
        /// </summary>
        /// <param name="date">The date to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Match(
            DateTime date,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            return MatchBase(date, fieldExpression, boolType);
        }

        /// <summary>
        /// Filter results by match in a field with some fuzziness.
        /// </summary>
        /// <param name="enum">The enum to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> Match(
            Enum @enum,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            return MatchBase(@enum, fieldExpression, boolType);
        }

        /// <summary>
        /// Filter results by match in a field with some fuzziness.
        /// </summary>
        /// <param name="value">The value to match for.</param>
        /// <param name="fieldname">NameField to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> Match(object value, string fieldname, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new MatchQuery<T>(fieldname, value), boolType);
            return this;
        }

        /// <summary>
        /// Filter results by wildcard in a field with fuzziness.
        /// </summary>
        /// <param name="text">The text to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> WildcardMatch(
            string text,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return WildcardMatch(text, fieldName, boolType);
        }

        /// <summary>
        /// Filter results by wildcard in a field with fuzziness.
        /// </summary>
        /// <param name="text">The text to match for.</param>
        /// <param name="fieldName">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> WildcardMatch(string text, string fieldName, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new CommonQuery<T>(fieldName, text), boolType);
            return this;
        }

        /// <summary>
        /// Filter results by a range of values in a field.
        /// </summary>
        /// <param name="lesserThan">The highest value that will generate a hit.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="excact">Include the given values as a valid hit.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="boost">Boost</param>
        /// <typeparam name="T2">Type of T2</typeparam>
        /// <returns>The query object it self</returns>
        public Query<T> RangeLesserThan<T2>(
            T2 lesserThan,
            Expression<Func<T, T2>> fieldExpression,
            bool excact = true,
            BooleanQueryType boolType = BooleanQueryType.Must,
            double? boost = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return RangeLesserThan(lesserThan, fieldName, excact, boolType, boost);
        }

        /// <summary>
        /// Filter results by a range of values in a field.
        /// </summary>
        /// <param name="lesserThan">The highest value that will generate a hit.</param>
        /// <param name="fieldName">Field to match on.</param>
        /// <param name="excact">Include the given values as a valid hit.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="boost">Boost</param>
        /// <typeparam name="T2">Type of T2</typeparam>
        /// <returns>The query object it self</returns>
        public Query<T> RangeLesserThan<T2>(
            T2 lesserThan,
            string fieldName,
            bool excact = true,
            BooleanQueryType boolType = BooleanQueryType.Must,
            double? boost = null)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(
                new RangeQuery<T2>(
                    lesserThan,
                    fieldName,
                    excact ? RangeQueryType.LesserThanEqual : RangeQueryType.LesserThan, boost),
                boolType);
            return this;
        }

        /// <summary>
        /// Filter results by a range of values in a field.
        /// </summary>
        /// <param name="greaterThan">The lowest value that will generate a hit.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="excact">Include the given values as a valid hit.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="boost">Boost</param>
        /// <typeparam name="T2">Type of T2</typeparam>
        /// <returns>The query object it self</returns>
        public Query<T> RangeGreaterThan<T2>(
            T2 greaterThan,
            Expression<Func<T, T2>> fieldExpression,
            bool excact = true,
            BooleanQueryType boolType = BooleanQueryType.Must,
            double? boost = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return RangeGreaterThan(greaterThan, fieldName, excact, boolType, boost);
        }

        /// <summary>
        /// Filter results by a range of values in a field.
        /// </summary>
        /// <param name="greaterThan">The lowest value that will generate a hit.</param>
        /// <param name="fieldName">Field to match on.</param>
        /// <param name="excact">Include the given values as a valid hit.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="boost">Boost</param>
        /// <typeparam name="T2">Type of T2</typeparam>
        /// <returns>The query object it self</returns>
        public Query<T> RangeGreaterThan<T2>(
            T2 greaterThan,
            string fieldName,
            bool excact = true,
            BooleanQueryType boolType = BooleanQueryType.Must,
            double? boost = null)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(
                new RangeQuery<T2>(
                    greaterThan,
                    fieldName,
                    excact ? RangeQueryType.GreaterThanEqual : RangeQueryType.GreaterThan,
                    boost),
                boolType);
            return this;
        }

        /// <summary>
        /// Filter results by a range of values in a field.
        /// </summary>
        /// <param name="greaterThan">The lowest value that will generate a hit.</param>
        /// <param name="lesserThan">The highest value that will generate a hit.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="excact">Include the given values as a valid hit.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="boost">Boost</param>
        /// <typeparam name="T2">Type of T2</typeparam>
        /// <returns>The query object it self</returns>
        public Query<T> Range<T2>(
            T2 greaterThan,
            T2 lesserThan,
            Expression<Func<T, T2>> fieldExpression,
            bool excact = true,
            BooleanQueryType boolType = BooleanQueryType.Must,
            double? boost = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(
                new RangeQuery<T2>(
                    greaterThan,
                    lesserThan,
                    fieldName,
                    excact ? RangeQueryType.GreaterLesserThanEqual : RangeQueryType.GreaterLesserThan,
                    boost),
                boolType);

            return this;
        }

        /// <summary>
        /// Filter results by matching on a term in a field.
        /// </summary>
        /// <param name="text">The term to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> Term(
            string text,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new TermQuery<T>(fieldName, text), boolType);
            return this;
        }

        /// <summary>
        /// Filter results by matching on terms in a field.
        /// </summary>
        /// <param name="text">The terms to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> Terms(
            string[] text,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return Terms(text, fieldName, boolType);
        }

        /// <summary>
        /// Filter results by matching on terms in a field.
        /// </summary>
        /// <param name="list">The terms to match for.</param>
        /// <param name="fieldName">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> Terms(string[] list, string fieldName, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new TermsQuery(fieldName, list), boolType);
            return this;
        }

        /// <summary>
        /// Filter results by matching on terms in a field.
        /// </summary>
        /// <param name="list">The terms to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>The query object it self</returns>
        public Query<T> Terms(
            int[] list,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return Terms(list, fieldName, boolType);
        }

        /// <summary>
        /// Filter results by matching on terms in a field.
        /// </summary>
        /// <param name="list">The terms to match for.</param>
        /// <param name="fieldName">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Terms(int[] list, string fieldName, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(
                new TermsQuery(fieldName, list.Select(x => x.ToString()).ToArray()),
                boolType);
            return this;
        }

        /// <summary>
        /// Filter results by matching on terms in a field.
        /// </summary>
        /// <param name="list">The terms to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Terms(double[] list, Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return Terms(list, fieldName, boolType);
        }

        /// <summary>
        /// Filter results by matching on terms in a field.
        /// </summary>
        /// <param name="list">The terms to match for.</param>
        /// <param name="fieldName">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Terms(double[] list, string fieldName, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(
                new TermsQuery(fieldName, list.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()),
                boolType);
            return this;
        }

        /// <summary>
        /// Filter results by matching type.
        /// </summary>
        /// <param name="type">The type to match for.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Type(string type, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new TypeQuery(type), boolType);
            if (TypesList == null)
            {
                TypesList = new List<string>();
            }

            TypesList.Add(TypeHelpers.TypeStringToIndexString(type));
            return this;
        }

        /// <summary>
        /// Filter results by property type.
        /// </summary>
        /// <param name="type">The type to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> TypeMatch(
            string type,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = $"{ExpressionHelper.GetPropertyName(fieldExpression)}.$type";
            return Match(type, fieldName, boolType);
        }

        /// <summary>
        /// Removes repeated queries.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> CleanUp()
        {
            var booleanQuery = BooleanQuery;
            booleanQuery.Should = booleanQuery.Should.DistinctBy(x => x.ToJson()).ToList();
            booleanQuery.Must = booleanQuery.Must.DistinctBy(x => x.ToJson()).ToList();
            booleanQuery.MustNot = booleanQuery.MustNot.DistinctBy(x => x.ToJson()).ToList();
            return this;
        }

        /// <summary>
        /// Filter results by matching type.
        /// </summary>
        /// <param name="type">The type to match for.</param>
        /// <param name="useInheritedTypes">False only creates a query based on the supplied type. True also adds inherited types. </param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> Type(Type type, bool useInheritedTypes = false,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            if (useInheritedTypes)
            {
                ApplyTypeInheritance(type);
            }
            else
            {
                ((BooleanQuery) QueryItem[Strings.Bool]).Add(new TypeQuery(type), boolType);
                if (TypesList == null)
                {
                    TypesList = new List<string>();
                }

                TypesList.Add(TypeHelpers.TypeStringToIndexString(type.FullName));
            }

            return this;
        }

        /// <summary>
        /// Filter results by property type.
        /// </summary>
        /// <param name="type">The type to match for.</param>
        /// <param name="fieldExpression">Field to match on.</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> TypeMatch(
            Type type,
            Expression<Func<T, object>> fieldExpression,
            BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var fieldName = $"{ExpressionHelper.GetPropertyName(fieldExpression)}.$type";
            return Match(type.FullName, fieldName, boolType);
        }

        /// <summary>
        /// Add filter for valid by DateTime.Now.
        /// </summary>
        /// <param name="fromFieldExpression">Valid from field</param>
        /// <param name="toFieldExpression">Valid to field</param>
        /// <returns>Self</returns>
        public Query<T> Validate(
            Expression<Func<T, DateTime?>> fromFieldExpression,
            Expression<Func<T, DateTime?>> toFieldExpression)
        {
            var valueF = DateTime.Now.Date;
            var valueT = DateTime.Now.Date.AddDays(1).AddTicks(-1);
            return Validate(valueF, valueT, fromFieldExpression, toFieldExpression);
        }

        /// <summary>
        /// Add filter for valid by supplied date.
        /// </summary>
        /// <param name="value">Date to validate</param>
        /// <param name="fromFieldExpression">Valid from field</param>
        /// <param name="toFieldExpression">Valid to field</param>
        /// <returns>Self</returns>
        public Query<T> Validate(
            DateTime value,
            Expression<Func<T, DateTime?>> fromFieldExpression = null,
            Expression<Func<T, DateTime?>> toFieldExpression = null)
        {
            var valueF = value;
            var valueT = value.AddDays(1).AddTicks(-1);

            if (fromFieldExpression != null && toFieldExpression != null)
            {
                return Validate(valueF, valueT, fromFieldExpression, toFieldExpression);
            }

            if (fromFieldExpression != null)
            {
                return RangeLesserThan(valueF, fromFieldExpression);
            }

            return toFieldExpression != null ? RangeGreaterThan(valueT, toFieldExpression) : this;
        }

        /// <summary>
        /// Add filter for valid by DateTime.Now.
        /// </summary>
        /// <param name="valueF">Date from</param>
        /// <param name="valueT">Date to</param>
        /// <param name="fromFieldExpression">Valid from field</param>
        /// <param name="toFieldExpression">Valid to field</param>
        /// <returns>Self</returns>
        public Query<T> Validate(
            DateTime valueF,
            DateTime valueT,
            Expression<Func<T, DateTime?>> fromFieldExpression,
            Expression<Func<T, DateTime?>> toFieldExpression)
        {
            return RangeLesserThan(valueF, fromFieldExpression).RangeGreaterThan(valueT, toFieldExpression);
        }

        /// <summary>
        /// Add a list of predetermind queries.
        /// </summary>
        /// <param name="queries">Queries</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> AddQueries(IEnumerable<IQuery> queries, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).AddRange(queries, boolType);
            return this;
        }

        /// <summary>
        /// Add a list of predetermind queries.
        /// </summary>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <param name="queries">Queries</param>
        /// <returns>Self</returns>
        public Query<T> AddQueries(BooleanQueryType boolType = BooleanQueryType.Must, params IQuery[] queries)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).AddRange(queries, boolType);
            return this;
        }

        /// <summary>
        /// Add a predetermind query.
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="boolType">How to use the query. Must, must not or should.</param>
        /// <returns>Self</returns>
        public Query<T> AddQuery(IQuery query, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(query, boolType);
            return this;
        }

        /// <summary>
        /// Add boolean query
        /// </summary>
        /// <param name="query">Query to add</param>
        /// <param name="boolType">Which boolean list to add query to</param>
        /// <returns>Self</returns>
        public Query<T> AddBooleanQuery(BooleanQueryRoot query, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var updatedQuery = new Query<T>();
            var thisQuery = (BooleanQueryRoot) QueryItem;
            var otherQuery = new BooleanQueryRoot {{Strings.Bool, query}};
            updatedQuery.AddQueries(boolType, thisQuery, otherQuery);
            return this;
        }

        /// <summary>
        /// Add boolean query
        /// </summary>
        /// <param name="query">Query to add</param>
        /// <param name="boolType">Which boolean list to add query to</param>
        /// <returns>Self</returns>
        public Query<T> AddBooleanQuery(Query<T> query, BooleanQueryType boolType = BooleanQueryType.Must)
        {
            var updatedQuery = new Query<T>();
            var thisQuery = (BooleanQueryRoot) QueryItem;
            var otherQuery = new BooleanQueryRoot {{Strings.Bool, query.BooleanQuery}};
            updatedQuery.AddQueries(boolType, thisQuery, otherQuery);
            return this;
        }

        /// <summary>
        /// Query will return aggs for nested objects.
        /// </summary>
        /// <param name="path">Property to nested objects.</param>
        /// <param name="aggregations">Nested Aggregation</param>
        /// <returns>Self</returns>
        public Query<T> NestedAggregation(Expression<Func<T, object>> path, params IAggregation[] aggregations)
        {
            var pathName = ExpressionHelper.GetPropertyName(path);
            var incr = 0;

            while (Aggregations.ContainsKey($"nested_{pathName}_{incr}"))
            {
                incr++;
            }

            Aggregations[$"nested_{pathName}_{incr}"] = new NestedAggregation(pathName);

            var aggLvl = Aggregations[$"nested_{pathName}_{incr}"];
            foreach (var agg in aggregations)
            {
                if (aggLvl.Aggregations == null) aggLvl.Aggregations = new Dictionary<string, IAggregation>();
                aggLvl.Aggregations["nested"] = agg;
                aggLvl = aggLvl.Aggregations["nested"];
            }

            return this;
        }

        /// <summary>
        /// Query will return cardinality aggs for a field.
        /// </summary>
        /// <param name="fieldExpression">Property to aggregate on.</param>
        /// <param name="aggregations">Child aggregations</param>
        /// <returns>Self</returns>
        public Query<T> CardinalityAggregation(Expression<Func<T, object>> fieldExpression,
            params IAggregation[] aggregations)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            Aggregations[$"cardinality_{fieldName}"] = new CardinalityAggregation(fieldName);
            foreach (var sa in aggregations)
            {
                if (Aggregations[$"cardinality_{fieldName}"].Aggregations == null)
                    Aggregations[$"cardinality_{fieldName}"].Aggregations = new Dictionary<string, IAggregation>();
                Aggregations[$"cardinality_{fieldName}"].Aggregations[sa.GetType().Name.Replace("Aggregation", "")] =
                    sa;
            }

            return this;
        }

        /// <summary>
        /// Query will return range aggs for a field.
        /// </summary>
        /// <param name="fieldExpression">Property to aggregate on.</param>
        /// <param name="rav">Range values</param>
        /// <param name="aggregations">Child aggregations</param>
        /// <returns>Self</returns>
        public Query<T> RangeAggregation<T2>(Expression<Func<T, T2>> fieldExpression, RangeAggrValue<T2>[] rav,
            params IAggregation[] aggregations)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            Aggregations[$"range_{fieldName}"] = new RangeAggregation<T2>(fieldName, rav);
            foreach (var sa in aggregations)
            {
                if (Aggregations[$"range_{fieldName}"].Aggregations == null)
                    Aggregations[$"range_{fieldName}"].Aggregations = new Dictionary<string, IAggregation>();
                Aggregations[$"range_{fieldName}"].Aggregations[sa.GetType().Name.Replace("Aggregation", "")] = sa;
            }

            return this;
        }

        /// <summary>
        /// Query will return aggs for a field.
        /// </summary>
        /// <param name="fieldExpression">Property to aggregate on.</param>
        /// <param name="size">Top aggregations list size. (Default is 10)</param>
        /// <param name="statAggregation">Aggs like Sam, Avg, Max, etc.</param>
        /// <returns>Self</returns>
        public Query<T> TermsAggregation(Expression<Func<T, object>> fieldExpression, int? size = null, params IAggregation[] statAggregation)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            Aggregations[$"terms_{fieldName}"] = new TermsAggregation(fieldName, size);
            foreach (var sa in statAggregation)
            {
                if (Aggregations[$"terms_{fieldName}"].Aggregations == null)
                    Aggregations[$"terms_{fieldName}"].Aggregations = new Dictionary<string, IAggregation>();

                if (sa.IsSubclassOfRawGeneric(typeof(RangeAggregation<>)) || sa is TermsAggregation)
                {
                    Aggregations[$"terms_{fieldName}"].Aggregations["nested"] = sa;
                }
                else if (sa.IsSubclassOfRawGeneric(typeof(BucketSelectorAggregation)))
                {
                    var key = $"{sa.GetType().Name.Replace("Aggregation", "")}_{Guid.NewGuid()}";
                    Aggregations[$"terms_{fieldName}"].Aggregations[key] = sa;
                }
                else
                {
                    var key = sa.GetType().Name.Replace("Aggregation", "");
                    if (Aggregations[$"terms_{fieldName}"].Aggregations.ContainsKey(key))
                    {
                        key += Guid.NewGuid();
                    }
                    Aggregations[$"terms_{fieldName}"].Aggregations[key] = sa;
                }
            }

            return this;
        }

        /// <summary>
        /// Query will return aggs for a field.
        /// </summary>
        /// <param name="fieldExpression">Property to aggregate on.</param>
        /// <param name="interval">Interval for histogram.</param>
        /// <param name="statAggregation">Aggs like Sam, Avg, Max, etc.</param>
        /// <returns>Self</returns>
        public Query<T> DateHistogramAggregation(Expression<Func<T, object>> fieldExpression, string interval,
            int? minDocCount = null,
            params IAggregation[] statAggregation)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            Aggregations[$"datehistogram_{fieldName}"] = new DateHistogramAggregation(fieldName, interval, minDocCount);
            foreach (var sa in statAggregation)
            {
                if (Aggregations[$"datehistogram_{fieldName}"].Aggregations == null)
                    Aggregations[$"datehistogram_{fieldName}"].Aggregations = new Dictionary<string, IAggregation>();
                if (sa.IsSubclassOfRawGeneric(typeof(RangeAggregation<>)) || sa is TermsAggregation)
                {
                    Aggregations[$"datehistogram_{fieldName}"].Aggregations["nested"] = sa;
                }
                else
                {
                    Aggregations[$"datehistogram_{fieldName}"]
                        .Aggregations[sa.GetType().Name.Replace("Aggregation", "")] = sa;
                }
            }

            return this;
        }

        /// <summary>
        /// Query will filter aggs.
        /// </summary>
        /// <param name="filter">Add filter (BooleanQueryRoot)</param>
        /// <param name="aggs">Aggregations</param>
        /// <returns>Self</returns>
        public Query<T> FilterAggregation(IFilter filter, Dictionary<string, IAggregation> aggs)
        {
            var incr = 0;
            if (Aggregations == null) Aggregations = new Dictionary<string, IAggregation>();
            while (Aggregations.ContainsKey($"agg_filter_{incr}"))
            {
                incr++;
            }
            var filterAgg = new FilterAggregation
            {
                Filter = filter,
                Aggregations = aggs
            };
            Aggregations[$"agg_filter_{incr}"] = filterAgg;
            
            return this;
        }

        /// <summary>
        /// Query will filter aggs.
        /// </summary>
        /// <param name="filter">Add filter (BooleanQueryRoot)</param>
        /// <param name="aggs">Aggregations</param>
        /// <returns>Self</returns>
        public Query<T> FilterAggregation(IFilter filter, params IAggregation[] aggs)
        {
            var incr = 0;
            if (Aggregations == null) Aggregations = new Dictionary<string, IAggregation>();
            while (Aggregations.ContainsKey($"agg_filter_{incr}"))
            {
                incr++;
            }
            var filterAgg = new FilterAggregation
            {
                Filter = filter
            };
            var fAggs = new Dictionary<string, IAggregation>();

            foreach (var sa in aggs)
            {
                if (sa.IsSubclassOfRawGeneric(typeof(RangeAggregation<>)) || sa is TermsAggregation)
                {
                    fAggs["nested"] = sa;
                }
                else
                {
                    fAggs[sa.GetType().Name.Replace("Aggregation", "")] = sa;
                }
            }

            filterAgg.Aggregations = fAggs;
            Aggregations[$"agg_filter_{incr}"] = filterAgg;

            return this;
        }

        /// <summary>
        /// Query will add aggs.
        /// </summary>
        /// <param name="aggs">Aggregations</param>
        /// <returns>Self</returns>
        public Query<T> AddAggregation(IAggregation aggs, string name = null)
        {
            if (Aggregations.IsNull())
            {
                Aggregations = new Dictionary<string, IAggregation>();
            }

            if (name.IsNull())
            {
                name = $"agg_{aggs.GetType().Name.Replace("Aggregation", "")}";
            }

            Aggregations[name] = aggs;

            return this;
        }

        /// <summary>
        /// Query will return aggs for a type.
        /// </summary>
        /// <param name="size">Top aggregations list size. (Default is 10)</param>
        /// <returns>Self</returns>
        public Query<T> TypeAggregation(int? size = null)
        {
            var fieldName = "_type";
            Aggregations[$"terms_{fieldName}"] = new TermsAggregation(fieldName, size);
            return this;
        }

        /// <summary>
        /// Query will return aggs for a field.
        /// </summary>
        /// <param name="fieldName">Name of the source field.</param>
        /// <returns>Self</returns>
        public Query<T> TermsAggregation(string fieldName)
        {
            Aggregations[$"terms_{fieldName}"] = new TermsAggregation(fieldName);
            return this;
        }

        /// <summary>
        /// Query will return no hits. For a specific number of hits, use Size().
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> NoHits()
        {
            _size = 0;
            return this;
        }

        /// <summary>
        /// Change the search type to Count.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> Count()
        {
            _searchType = SearchType.Count;
            return this;
        }

        /// <summary>
        /// Send the query to the Elasticsearch server and get the respons in return.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>As MultipleQuery</returns>
        public MultipleQuery<T> ToMultipleQuery(int size = 10)
        {
            var output = new MultipleQuery<T>();
            var shoulds = ((BooleanQuery) QueryItem[Strings.Bool]).Should;

            output[Strings.All] = SetSize(size);

            foreach (var should in shoulds.Where(query => !string.IsNullOrEmpty(query._Name)))
            {
                var typeQueryParam = should.GetType().GetGenericArguments().FirstOrDefault();
                var query = new Query<T> {QueryItem = {[Strings.Bool] = new BooleanQuery()}};
                if (typeQueryParam != null && typeQueryParam.Name != "Asset")
                {
                    var typeFilter = typeQueryParam.InheritedTypes(typeQueryParam.Assembly)
                        .Select(t => new TypeQuery(t));
                    query = query.AddQueries(typeFilter, BooleanQueryType.Should);
                }

                query = query.AddQuery(should).SetSize(0);
                query.Aggregations = Aggregations;
                output[should._Name] = query;
            }

            return output;
        }

        /// <summary>
        /// Creates a multipleQueryFreeTextSearch
        /// </summary>
        /// <param name="text">Free text string</param>
        /// <param name="size">Size of source list</param>
        /// <returns>Self</returns>
        public MultipleQuery<T> ToMultipleQueryFreeText(string text, int size = 10)
        {
            var output = new MultipleQuery<T> {[Strings.All] = new Query<T>().For(text).SetSize(size)};

            foreach (var should in ((BooleanQuery) QueryItem[Strings.Bool]).Should.Where(
                query => !string.IsNullOrEmpty(query._Name)))
            {
                var typeQueryParam = should.GetType().GetGenericArguments().FirstOrDefault();
                var query = new Query<T> {QueryItem = {[Strings.Bool] = new BooleanQuery()}};
                if (typeQueryParam != null && typeQueryParam.Name != "Asset")
                {
                    var typeFilter = typeQueryParam.InheritedTypes(typeQueryParam.Assembly)
                        .Select(t => new TypeQuery(t));
                    query = query.AddQueries(typeFilter, BooleanQueryType.Should);
                }

                query = query.AddQuery(should).SetSize(0);
                query.Aggregations = Aggregations;
                output[should._Name] = query;
            }

            return output;
        }

        /// <summary>
        /// Apply scrolling/paging to the query.
        /// </summary>
        /// <param name="scrolltime">Amount of time the server will keep the scrolling context</param>
        /// <returns>Self</returns>
        public Query<T> SetScroll(string scrolltime = "1m")
        {
            Scroll = scrolltime;
            return this;
        }

        /// <summary>
        /// Set source.
        /// </summary>
        /// <param name="value">_source string</param>
        /// <returns>Self</returns>
        public Query<T> AdjustSource(string value)
        {
            Source = new Source {Values = new[] {value}};
            return this;
        }

        /// <summary>
        /// Set source.
        /// </summary>
        /// <param name="values">_source strings</param>
        /// <returns>Self</returns>
        public Query<T> AdjustSource(string[] values)
        {
            Source = new Source {Values = values};
            return this;
        }

        /// <summary>
        /// Set which properties to include.
        /// </summary>
        /// <param name="includes">_source include strings</param>
        /// <returns>Self</returns>
        public Query<T> Include(string[] includes)
        {
            var exc = new string[] { };
            if (Source?.Excludes != null)
            {
                exc = Source.Excludes;
            }

            Source = new Source {Includes = includes, Excludes = exc};
            return this;
        }

        /// <summary>
        /// Set which properties to exclude.
        /// </summary>
        /// <param name="excludes">_source exclude strings</param>
        /// <returns>Self</returns>
        public Query<T> Excludes(string[] excludes)
        {
            var inc = new string[] { };
            if (Source?.Includes != null)
            {
                inc = Source.Includes;
            }

            Source = new Source {Includes = inc, Excludes = excludes};
            return this;
        }

        /// <summary>
        /// Set which properties to include.
        /// </summary>
        /// <param name="includes">_source include expressions</param>
        /// <returns>Self</returns>
        public Query<T> Include(params Expression<Func<T, object>>[] includes)
        {
            var exc = new string[] { };
            if (Source?.Excludes != null)
            {
                exc = Source.Excludes;
            }

            var inc = includes.Select(ExpressionHelper.GetPropertyName).ToArray();
            Source = new Source {Includes = inc, Excludes = exc};
            return this;
        }

        /// <summary>
        /// Set which properties to excludes.
        /// </summary>
        /// <param name="excludes">_source excludes expressions</param>
        /// <returns>Self</returns>
        public Query<T> Excludes(params Expression<Func<T, object>>[] excludes)
        {
            var inc = new string[] { };
            if (Source?.Includes != null)
            {
                inc = Source.Includes;
            }

            var exc = excludes.Select(ExpressionHelper.GetPropertyName).ToArray();
            Source = new Source {Includes = inc, Excludes = exc};
            return this;
        }

        /// <summary>
        /// Apply sorting.
        /// </summary>
        /// <param name="field">Name of the filed to sort</param>
        /// <param name="direction">"asc" or "desc"</param>
        /// <returns>Self</returns>
        public Query<T> Sort(string field, string direction)
        {
            if (Sorting != null)
            {
                Sorting.AddSorting(field, direction);
                return this;
            }

            Sorting = new Sort();
            Sorting.AddSorting(field, direction);
            return this;
        }

        /// <summary>
        /// Apply sorting.
        /// </summary>
        /// <param name="fieldExpression">Name of the filed to sort</param>
        /// <param name="direction">"asc" or "desc"</param>
        /// <returns>Self</returns>
        public Query<T> Sort(Expression<Func<T, object>> fieldExpression,
            AggsOrderDirection direction = AggsOrderDirection.Asc)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            var dir = direction.ToString().ToLower();
            if (Sorting != null)
            {
                Sorting.AddSorting(fieldName, dir);
                return this;
            }

            Sorting = new Sort();
            Sorting.AddSorting(fieldName, dir);
            return this;
        }

        /// <summary>
        /// Remove all should.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> ClearShould()
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Should = new List<IQuery>();
            return this;
        }

        /// <summary>
        /// Remove all must.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> ClearMust()
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).Must = new List<IQuery>();
            return this;
        }

        /// <summary>
        /// Remove all must not.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> ClearMustNot()
        {
            ((BooleanQuery) QueryItem[Strings.Bool]).MustNot = new List<IQuery>();
            return this;
        }

        /// <summary>
        /// Remove aggregations.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> ClearAggregations()
        {
            Aggregations.Clear();
            return this;
        }

        /// <summary>
        /// Remove aggregations.
        /// </summary>
        /// <param name="name">Query name</param>
        /// <returns>Self</returns>
        public Query<T> Name(string name = "")
        {
            ((BooleanQuery) QueryItem[Strings.Bool])._Name = name;
            return this;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> Clone()
        {
            return Cloner.Clone(this);
        }

        /// <summary>
        /// Ors the specified query2.
        /// </summary>
        /// <param name="query2">The query2.</param>
        /// <returns>Self</returns>
        public Query<T> Or(Query<T> query2)
        {
            return Merge(query2, BooleanQueryType.Should);
        }

        /// <summary>
        /// Ands the specified query2.
        /// </summary>
        /// <param name="query2">The query2.</param>
        /// <returns>Self</returns>
        public Query<T> And(Query<T> query2)
        {
            return Merge(query2, BooleanQueryType.Must);
        }

        /// <summary>
        /// Adds the specified query2 to the booleanQuery.
        /// </summary>
        /// <param name="query2">The query2.</param>
        /// <param name="booleanQueryType"></param>
        /// <returns>Self</returns>
        public Query<T> AddToBoolean(Query<T> query2, BooleanQueryType booleanQueryType = BooleanQueryType.Must)
        {
            var bqr = new BooleanQueryRoot {[Strings.Bool] = query2.QueryItem[Strings.Bool]};

            switch (booleanQueryType)
            {
                case BooleanQueryType.Must:
                    BooleanQuery.Must.Add(bqr);
                    break;
                case BooleanQueryType.MustNot:
                    BooleanQuery.MustNot.Add(bqr);
                    break;
                case BooleanQueryType.Should:
                    BooleanQuery.Should.Add(bqr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(booleanQueryType), booleanQueryType, null);
            }

            return this;
        }

        /// <summary>
        /// Adds the specified query2 to the post_filter.
        /// </summary>
        /// <param name="query2">The query2.</param>
        /// <returns>Self</returns>
        public Query<T> AddToPostFilter(Query<T> query2)
        {
            PostFilter = new BooleanQueryRoot {[Strings.Bool] = query2.QueryItem[Strings.Bool]};
            return this;
        }
        
        /// <summary>
        /// Cleans this instance.
        /// </summary>
        /// <returns>Self</returns>
        public Query<T> Clean()
        {
            // Not completed
            return this;
        }

        private Query<T> Merge(Query<T> queryToMerge, BooleanQueryType booleanQueryType)
        {
            var query1 = Clone();
            var q1 = new BooleanQueryRoot {{Strings.Bool, (BooleanQuery) query1.QueryItem[Strings.Bool]}};
            var q2 = new BooleanQueryRoot {{Strings.Bool, (BooleanQuery) queryToMerge.QueryItem[Strings.Bool]}};

            var merge = new BooleanQuery();

            var isEmptyQ1 = !((BooleanQuery) q1[Strings.Bool]).Must.Any()
                            && !((BooleanQuery) q1[Strings.Bool]).MustNot.Any()
                            && !((BooleanQuery) q1[Strings.Bool]).Should.Any()
                            && ((BooleanQuery) q1[Strings.Bool]).Filter == null;
            var isEmptyQ2 = !((BooleanQuery) q2[Strings.Bool]).Must.Any()
                            && !((BooleanQuery) q2[Strings.Bool]).MustNot.Any()
                            && !((BooleanQuery) q2[Strings.Bool]).Should.Any()
                            && ((BooleanQuery) q2[Strings.Bool]).Filter == null;
            if (!isEmptyQ1)
            {
                merge.Add(q1, booleanQueryType);
            }

            if (!isEmptyQ2)
            {
                merge.Add(q2, booleanQueryType);
            }

            QueryItem[Strings.Bool] = merge;

            return this;
        }

        private void PrepareBooleanQuery(bool noTypeFilter = false)
        {
            if (Aggregations.IsNull())
            {
                Aggregations = new Dictionary<string, IAggregation>();
            }

            if (QueryItem.IsNull())
            {
                QueryItem = new Dictionary<string, IQuery> {[Strings.Bool] = new BooleanQuery()};
            }

            if (!QueryItem.ContainsKey(Strings.Bool))
            {
                QueryItem[Strings.Bool] = new BooleanQuery();
            }

            if (noTypeFilter)
            {
                return;
            }

            ApplyTypeInheritance(typeof(T));
        }

        private void ApplyTypeInheritance(Type type)
        {
            if (type.IsInterface || type.Name == "Asset" || type == typeof(object)
                || type.Name == "ElasticAssetDocument")
            {
                return;
            }

            var typeFilter = type.InheritedTypes(type.Assembly).Select(t => new TypeQuery(t));
            TypesList = type.InheritedTypes(type.Assembly).Select(TypeHelpers.TypeStringToIndexString).ToList();
            ((BooleanQuery) QueryItem[Strings.Bool]).Should.AddRange(typeFilter);
        }

        private Query<T> MatchBase(object text, Expression<Func<T, object>> fieldExpression, BooleanQueryType boolType)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            ((BooleanQuery) QueryItem[Strings.Bool]).Add(new MatchQuery<T>(fieldName, text), boolType);
            return this;
        }
    }
}