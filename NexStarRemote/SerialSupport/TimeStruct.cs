using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexStarRemote.SerialSupport
{
    public class TimeStruct
    {
        public Int16 Hour { get; set; }
        public Int16 Minutes { get; set; }
        public Int16 Seconds { get; set; }
        public Int16 Month { get; set; }
        public Int16 Day { get; set; }
        public Int16 YearsSince2000 { get; set; }
        public Int16 GMTOffset { get; set; }
        public bool IsDaylightSavings { get; set; }
    }
}
