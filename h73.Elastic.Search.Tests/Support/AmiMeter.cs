using System;

namespace eSmart.GroundFault
{
    public class AmiMeter
    {
        public int Id { get; set; }
        public Guid AssetGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object GeoLocation => new
        {
            lat = string.IsNullOrWhiteSpace(Lat) ? null : new double?(double.Parse(Lat)), 
            lon = string.IsNullOrWhiteSpace(Long) ? null : new double?(double.Parse(Long))
        };
        public Guid? SubstationId { get; set; }
        public string SubstationName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int ParticipantId { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int StSrid { get; set; }
    }
}