using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexStarRemote.SerialSupport
{
    public class TrackingMode
    {
        public static TrackingMode Off = new TrackingMode(0x0, "Off");
        public static TrackingMode AltAz = new TrackingMode(0x1, "Alt/Az");
        public static TrackingMode EQNorth = new TrackingMode(0x2, "EQ North");
        public static TrackingMode EQSouth = new TrackingMode(0x3, "EQ South");

        public byte Identifier { get; set; }
        public string Description { get; set; }
        private TrackingMode(byte id, string description)
        {
            Identifier = id;
            Description = description;
        }

        public static Dictionary<int, string> GetTrackingModeDictionary()
        {
            return new Dictionary<int, string>(){
                {Off.Identifier,Off.Description},
                {AltAz.Identifier,AltAz.Description},
                {EQNorth.Identifier,EQNorth.Description},
                {EQSouth.Identifier,EQSouth.Description}
            };
        }

        public static TrackingMode GetTrackingMode(int modeId)
        {
            switch (modeId)
            {
                case 0:
                    return TrackingMode.Off;
                case 1:
                    return TrackingMode.AltAz;
                case 2:
                    return TrackingMode.EQNorth;
                case 3:
                    return TrackingMode.EQSouth;
                default:
                    return TrackingMode.Off;
            }
        }
    }
}
