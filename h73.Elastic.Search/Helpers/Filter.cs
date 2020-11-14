using System.Collections.Generic;
using h73.Elastic.Core.Search.Aggregations;
using h73.Elastic.Core.Search.Interfaces;

namespace h73.Elastic.Search.Helpers
{
    public class Filter : AggregationBase
    {
        public Filter(IFilter filter, Dictionary<string, IAggregation> aggregation)
        {
            Filter = filter;
            Aggregations = aggregation;
        }
    }
}