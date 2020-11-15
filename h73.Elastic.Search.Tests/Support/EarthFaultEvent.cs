using System;

namespace h73.Elastic.Search.Tests.Support
{
    public class EarthFaultEvent
    {
        public Asset Asset { get; set; }
        public string EventIdGuid { get; set; }
        public WorkOrderState WorkOrderState { get; set; }
        public EventType Type { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public double? Duration =>
            End == null || Start == null ? (double?) null : End.Value.Subtract(Start.Value).TotalMinutes;
    }
}