using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexStarRemote.SerialSupport
{
    public class SlewRate
    {
        public static readonly SlewRate Stop = new SlewRate(0x0, "Stop");
        public static readonly SlewRate Minimum = new SlewRate(0x1, "Minimum");
        public static readonly SlewRate NoTrackingOverideMax = new SlewRate(0x2, "NoTrackingOverideMax");
        public static readonly SlewRate TrackingOverideMin = new SlewRate(0x3, "TrackingOverideMin");
        public static readonly SlewRate SlowMid = new SlewRate(0x4, "SlowMid");
        public static readonly SlewRate Middle = new SlewRate(0x5, "Middle");
        public static readonly SlewRate FastMiddle = new SlewRate(0x6, "FastMiddle");
        public static readonly SlewRate SlowScan = new SlewRate(0x7, "SlowScan");
        public static readonly SlewRate FastScan = new SlewRate(0x8, "FastScan");
        public static readonly SlewRate Maximum = new SlewRate(0x9, "Maximum");

        public byte RateValue { get; private set; }
        public string Descriptor { get; private set; }

        private SlewRate(byte rate, string descriptor)
        {
            RateValue = rate;
            Descriptor = descriptor;
        }

        public static Dictionary<int, string> GetSlewRateDictionary
        {
            get
            {
                return new Dictionary<int, string>(){
                    {Stop.RateValue, Stop.Descriptor},
                    {Minimum.RateValue, Minimum.Descriptor},
                    {NoTrackingOverideMax.RateValue, NoTrackingOverideMax.Descriptor},
                    {TrackingOverideMin.RateValue, TrackingOverideMin.Descriptor},
                    {SlowMid.RateValue, SlowMid.Descriptor},
                    {Middle.RateValue, Middle.Descriptor},
                    {FastMiddle.RateValue, FastMiddle.Descriptor},
                    {SlowScan.RateValue, SlowScan.Descriptor},
                    {FastScan.RateValue, FastScan.Descriptor},
                    {Maximum.RateValue, Maximum.Descriptor}
                };
            }
        }

        public static SlewRate FindSlewRate(int slewRateId)
        {
            switch (slewRateId)
            {
                case 0:
                    return Stop;
                case 1:
                    return Minimum;
                case 2:
                    return NoTrackingOverideMax;
                case 3:
                    return TrackingOverideMin;
                case 4:
                    return SlowMid;
                case 5:
                    return Middle;
                case 6:
                    return FastMiddle;
                case 7:
                    return SlowScan;
                case 8:
                    return FastScan;
                case 9:
                    return Maximum;
                default:
                    return Stop;
            }
        }
    }
}
