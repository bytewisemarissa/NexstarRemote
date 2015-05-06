using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexStarRemote.SerialSupport
{
    public class GPSData
    {
        public Int16 DegreesLatitude { get; set; }
        public Int16 MinutesLatitude { get; set; }
        public Int16 SecondsLatitude { get; set; }
        public Boolean IsNorth { get; set; }
        public Int16 DegreesLongitude { get; set; }
        public Int16 MinutesLongitude { get; set; }
        public Int16 SecondsLongitude { get; set; }
        public Boolean IsWest { get; set; }

        public GPSData() { }

        public override string ToString()
        {
            return String.Format("{0}{1}°{2}'{3}\"|{4}{5}°{6}'{7}\"",
                IsNorth ? "N" : "S",
                DegreesLatitude.ToString(),
                MinutesLatitude.ToString(),
                SecondsLatitude.ToString(),
                IsWest ? "W" : "E",
                DegreesLongitude.ToString(),
                MinutesLongitude.ToString(),
                SecondsLongitude.ToString());
        }
    }
}
