using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using h73.Elastic.Core.Enums;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Search;

namespace h73.Elastic.Search.Helpers
{
    public static class ParameterHelper
    {
        public static IEnumerable<Sort> Sort<T>(
            params KeyValuePair<Expression<Func<T, object>>, AggsOrderDirection>[] sortExpressions)
        {
            return sortExpressions.Select(
                pair =>
                    new Sort {[ExpressionHelper.GetPropertyName(pair.Key)] = pair.Value.ToString()}
            );
        }

        public static Source Include<T>(params Expression<Func<T, object>>[] includes)
        {
            return new Source
            {
                Includes = includes.Select(ExpressionHelper.GetPropertyName).ToArray()
            };
        }
        public static Source Excludes<T>(params Expression<Func<T, object>>[] excludes)
        {
            return new Source
            {
                Excludes = excludes.Select(ExpressionHelper.GetPropertyName).ToArray()
            };
        }
    }
}