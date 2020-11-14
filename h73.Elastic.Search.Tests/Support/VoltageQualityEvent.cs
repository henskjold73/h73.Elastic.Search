using eSmart.Core;

namespace eSmart.Elastic.Search.Tests.Support
{
    public class VoltageQualityEvent : Event
    {
        public VoltageQualityEvent()
        {

        }
        public VoltageQualityEvent(VoltageAsset asset)
        {
            Asset = asset;
        }

        public new VoltageAsset Asset { get; set; }
        public double Voltage { get; set; }
        public double VoltagePercentage { get; set; }
    }
}