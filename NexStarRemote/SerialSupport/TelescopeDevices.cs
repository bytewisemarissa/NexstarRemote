using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexStarRemote.SerialSupport
{
    public class TelescopeDevices
    {
        public static readonly TelescopeDevices AZM_RA_Motor = new TelescopeDevices(0x10, "AZM/RA Motor");
        public static readonly TelescopeDevices ALT_DEC_Motor = new TelescopeDevices(0x11, "ALT/DEC Motor");
        public static readonly TelescopeDevices GPS_Unit = new TelescopeDevices(0xB0, "GPS Unit");
        public static readonly TelescopeDevices RTC = new TelescopeDevices(0xB2, "RTC (CGE Only)");

        public byte DeviceId { get; private set; }
        public string Description { get; private set; }
        private TelescopeDevices(byte deviceId, string descriptor)
        {
            DeviceId = deviceId;
            Description = descriptor;
        }
    }
}
