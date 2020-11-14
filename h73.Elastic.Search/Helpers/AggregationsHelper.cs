using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Search;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;

namespace h73.Elastic.Search.Helpers
{
    public static class AggregationsHelper
    {
        public static IAggregation CardinalityAggregation<T>(this IAggregation agg,
            Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.Aggregations[$"cardinality"] = new CardinalityAggregation(fieldName);
            return agg;
        }

        public static IStatAggregation CardinalityAggregation<T>(Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new CardinalityAggregation(fieldName);
        }

        public static void AddCardinalityAggregation<T>(this IAggregation agg,
            Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.Aggregations[$"cardinality"] = new CardinalityAggregation(fieldName);
        }

        public static IAggregation SumAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Sum"] = new SumAggregation(fieldName);
            return agg;
        }

        public static IStatAggregation SumAggregation<T>(Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new SumAggregation(fieldName);
        }

        public static void AddSumAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Sum"] = new SumAggregation(fieldName);
        }

        public static IAggregation AvgAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Avg"] = new AvgAggregation(fieldName);
            return agg;
        }

        public static IStatAggregation AvgAggregation<T>(Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new AvgAggregation(fieldName);
        }

        public static void AddAvgAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Avg"] = new AvgAggregation(fieldName);
        }

        public static IAggregation MaxAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Max"] = new MaxAggregation(fieldName);
            return agg;
        }

        public static IStatAggregation MaxAggregation<T>(Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new MaxAggregation(fieldName);
        }

        public static void AddMaxAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Max"] = new MaxAggregation(fieldName);
        }

        public static IAggregation MinAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Min"] = new MinAggregation(fieldName);
            return agg;
        }

        public static IStatAggregation MinAggregation<T>(Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new MinAggregation(fieldName);
        }

        public static void AddMinAggregation<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations["Min"] = new MinAggregation(fieldName);
        }

        public static TermsAggregation TermsAggregation<T>(Expression<Func<T, object>> fieldExpression,
            int? size = null, AggsOrder order = null, params IAggregation[] statAggregation) where T : new()
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            var termA = new TermsAggregation(fieldName, size, order);
            foreach (var sa in statAggregation)
            {
                if (termA.Aggregations == null) termA.Aggregations = new Dictionary<string, IAggregation>();
                if (sa.IsSubclassOfRawGeneric(typeof(RangeAggregation<>)) || sa is TermsAggregation)
                {
                    termA.Aggregations["nested"] = sa;
                }
                else
                {
                    termA.Aggregations[sa.GetType().Name.Replace("Aggregation", "")] = sa;
                }
            }

            return termA;
        }

        public static TermsAggregation TermsAggregation<T>(Expression<Func<T, object>> fieldExpression,
            AggsOrderBy orderBy, AggsOrderDirection direction, int? size = null,
            params IAggregation[] statAggregation) where T : new()
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            var termA = new TermsAggregation(fieldName, size, new AggsOrder(orderBy, direction));
            foreach (var sa in statAggregation)
            {
                if (termA.Aggregations == null) termA.Aggregations = new Dictionary<string, IAggregation>();
                if (sa.IsSubclassOfRawGeneric(typeof(RangeAggregation<>)) || sa is TermsAggregation)
                {
                    termA.Aggregations["nested"] = sa;
                }
                else
                {
                    termA.Aggregations[sa.GetType().Name.Replace("Aggregation", "")] = sa;
                }
            }

            return termA;
        }

        public static RangeAggregation<T2> RangeAggregation<T, T2>(Expression<Func<T, T2>> fieldExpression,
            params RangeAggrValue<T2>[] ranges)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new RangeAggregation<T2>(fieldName, ranges);
        }

        public static RangeAggregation<T2> RangeAggregation<T, T2>(Expression<Func<T, object>> fieldExpression,
            params RangeAggrValue<T2>[] ranges)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new RangeAggregation<T2>(fieldName, ranges);
        }

        public static IAggregation TermsAggregation<T>(this IAggregation agg,
            Expression<Func<T, object>> fieldExpression, int? size = null, AggsOrder order = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return agg.TermsAggregation(fieldName, size, order);
        }

        public static IAggregation TermsAggregation(this IAggregation agg, string fieldName, int? size = null,
            AggsOrder order = null)
        {
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.Aggregations["nested"] = new TermsAggregation(fieldName, size, order);
            return agg;
        }

        public static IAggregation DateHistogram<T>(this IAggregation agg, Expression<Func<T, object>> fieldExpression,
            string interval, int? minDocCount = null, AggsOrder order = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.Aggregations["nested"] = new DateHistogramAggregation(fieldName, interval, minDocCount, order);
            return agg;
        }

        public static DateHistogramAggregation DateHistogram<T>(Expression<Func<T, object>> fieldExpression,
            string interval, int? minDocCount = null, AggsOrder order = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new DateHistogramAggregation(fieldName, interval, minDocCount, order);
        }

        public static BucketSelectorAggregation BucketSelector(string pathName, string path, string script)
        {
            return new BucketSelectorAggregation
            {
                BucketSelector = new BucketSelector
                    {BucketsPath = new Dictionary<string, string> {{pathName, path}}, Script = script}
            };
        }

        public static void Add(this IAggregation agg, string key, IAggregation addAgg)
        {
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.Aggregations[key] = addAgg;
        }

        public static IAggregation BucketSort<T>(this IStatAggregation agg, Expression<Func<T, object>> fieldExpression,
            AggsOrderDirection direction, int? size = null, int? from = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            agg.Aggregations[$"bucket_sort_{fieldName}"] = new BucketSortAggregation
            {
                BucketSort = new BucketSort
                {
                    Size = size,
                    From = from,
                    Sorts = new[] {new Sort {[fieldName] = direction.ToString()}}
                }
            };
            return agg;
        }

        public static IStatAggregation BucketSortAggregation(string bucketName, AggsOrderDirection? direction = null,
            int? size = null,
            int? from = null)
        {
            return new BucketSortAggregation
            {
                BucketSort = new BucketSort
                {
                    Size = size,
                    From = from,
                    Sorts = direction != null ? new[] {new Sort {[bucketName] = direction.ToString()}} : null
                }
            };
        }

        public static IStatAggregation BucketSortAggregation(int? size = null, int? from = null, params KeyValuePair<string, AggsOrderDirection>[] sorts)
        {
            var sort = sorts.Select(x => new Sort {[x.Key] = x.Value.ToString()}).ToArray();

            return new BucketSortAggregation
            {
                BucketSort = new BucketSort
                {
                    Size = size,
                    From = from,
                    Sorts = sort.Any() ? sort : null
                }
            };
        }

        public static IStatAggregation TopHitsAggregation(IEnumerable<Sort> sort, Source source,
            int? size = null,
            int? from = null)
        {
            return new TopHitsAggregation
            {
                TopHits = new TopHits
                {
                    Size = size,
                    From = from,
                    Sorts = sort.ToArray(),
                    SourceObject = source
                }
            };
        }

        public static IAggregation TopHitsAggregation(this IAggregation agg, IEnumerable<Sort> sort, Source source,
            int? size = null,
            int? from = null)
        {
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            agg.Aggregations["tophits"] = new TopHitsAggregation
            {
                TopHits = new TopHits
                {
                    Size = size,
                    From = from,
                    Sorts = sort.ToArray(),
                    SourceObject = source
                }
            };

            return agg;
        }

        public static IAggregation HistogramAggregation<T, T2>(Expression<Func<T, object>> fieldExpression, T2 interval,
            int? minDocCount = null)
        {
            var fieldName = ExpressionHelper.GetPropertyName(fieldExpression);
            return new HistogramAggregation<T2>(fieldName, interval, minDocCount);
        }

        public static IAggregation FilterAggregation<T>(this IAggregation agg, IFilter filter,
            params IAggregation[] aggs)
        {
            var incr = 0;
            if (agg.Aggregations == null) agg.Aggregations = new Dictionary<string, IAggregation>();
            while (agg.Aggregations.ContainsKey($"agg_filter_{incr}"))
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
            agg.Aggregations[$"agg_filter_{incr}"] = filterAgg;
            return agg;
        }

        public static IAggregation FilterAggregation<T>(IFilter filter,
            params KeyValuePair<string, IAggregation>[] aggs)
        {
            return new FilterAggregation
            {
                Filter = filter,
                Aggregations = aggs.ToDictionary(pair => pair.Key, pair => pair.Value)
            };
        }

        public static IAggregation FilterAggregation<T>(Query<T> filter,
            params KeyValuePair<string, IAggregation>[] aggs)
        {
            return FilterAggregation<T>(filter.CreateBooleanQueryRoot(), aggs);
        }
    }
}