using System;
using eSmart.Elastic.Core.Data;
using eSmart.GroundFault;
using MoreLinq;

namespace eSmart.Core
{
    public class Event : AmiMeter
    {
        public Event() {}
        public Event(AmiMeter asset)
        {
            typeof(AmiMeter).GetProperties().ForEach(pi =>
            {
                if (pi.SetMethod != null)
                {
                    pi.SetValue(this, pi.GetValue(asset));
                }
            });
        }

        public string EventIdGuid { get; set; }
        public string Type { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public double? Duration => 
            End == null || Start == null ? 
                (double?) null : 
                End.Value.Subtract(Start.Value).TotalMinutes;
    }
}