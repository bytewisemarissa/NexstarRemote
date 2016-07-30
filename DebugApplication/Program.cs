using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexStarRemote.SerialSupport;

namespace DebugApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort debugPort = new SerialPort("COM5");
            debugPort.ReadTimeout = 4000;

            Console.WriteLine("Get Version:");
            NexStarCommandHelper.GetVersion(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Echo:");
            NexStarCommandHelper.Echo(debugPort, 0x01);
            Console.WriteLine("");

            Console.WriteLine("Get Model:");
            NexStarCommandHelper.GetModel(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Is Alignment Complete:");
            NexStarCommandHelper.IsAlignmentComplete(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Is Goto Running:");
            NexStarCommandHelper.IsGotoRunning(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Cancel Goto:");
            NexStarCommandHelper.CancelGoto(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Get Device Version Alt-Dec Motor:");
            NexStarCommandHelper.GetDeviceVersion(debugPort, TelescopeDevices.ALT_DEC_Motor);
            Console.WriteLine("");

            Console.WriteLine("Get Device Version Azm-Ra Motor:");
            NexStarCommandHelper.GetDeviceVersion(debugPort, TelescopeDevices.AZM_RA_Motor);
            Console.WriteLine("");

            Console.WriteLine("Get Device Version GPS Unit:");
            NexStarCommandHelper.GetDeviceVersion(debugPort, TelescopeDevices.GPS_Unit);
            Console.WriteLine("");

            Console.WriteLine("Get Device Version RTC:");
            NexStarCommandHelper.GetDeviceVersion(debugPort, TelescopeDevices.RTC);
            Console.WriteLine("");

            Console.WriteLine("Get Time:");
            TimeStruct currentTime = NexStarCommandHelper.GetTime(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Time:");
            currentTime.Hour = (Int16)19;
            currentTime.Minutes = (Int16)22;
            currentTime.Seconds = (Int16)5;
            currentTime.Month = (Int16)6;
            currentTime.Day = (Int16)29;
            currentTime.YearsSince2000 = (Int16)14;
            currentTime.GMTOffset = (Int16)62;
            currentTime.IsDaylightSavings = false;
            NexStarCommandHelper.SetTime(debugPort, currentTime);
            Console.WriteLine("");

            Console.WriteLine("Get Time 2:");
            currentTime = NexStarCommandHelper.GetTime(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Get Location:");
            GPSData locationData = NexStarCommandHelper.GetLocation(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Location:");
            locationData.DegreesLatitude = (Int16)38;
            locationData.MinutesLatitude = (Int16)14;
            locationData.SecondsLatitude = (Int16)30;
            locationData.IsNorth = true;
            locationData.DegreesLongitude = (Int16)83;
            locationData.MinutesLongitude = (Int16)42;
            locationData.SecondsLongitude = (Int16)30;
            locationData.IsWest = false;
            NexStarCommandHelper.SetLocation(debugPort, locationData);
            Console.WriteLine("");

            Console.WriteLine("Get Location 2:");
            NexStarCommandHelper.GetLocation(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Get Tracking Mode:");
            NexStarCommandHelper.GetTrackingMode(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Tracking Mode AltAz:");
            NexStarCommandHelper.SetTrackingMode(debugPort, TrackingMode.AltAz);
            Console.WriteLine("");

            Console.WriteLine("Get Tracking Mode 1:");
            NexStarCommandHelper.GetTrackingMode(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Tracking Mode EQNorth:");
            NexStarCommandHelper.SetTrackingMode(debugPort, TrackingMode.EQNorth);
            Console.WriteLine("");

            Console.WriteLine("Get Tracking Mode 2:");
            NexStarCommandHelper.GetTrackingMode(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Tracking Mode EQSouth:");
            NexStarCommandHelper.SetTrackingMode(debugPort, TrackingMode.EQSouth);
            Console.WriteLine("");

            Console.WriteLine("Get Tracking Mode 3:");
            NexStarCommandHelper.GetTrackingMode(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Tracking Mode Off:");
            NexStarCommandHelper.SetTrackingMode(debugPort, TrackingMode.Off);
            Console.WriteLine("");

            Console.WriteLine("Get Tracking Mode 4:");
            NexStarCommandHelper.GetTrackingMode(debugPort);
            Console.WriteLine("");

            Console.WriteLine("Set Variable AZM Slew On:");
            NexStarCommandHelper.SetVariableAZMSlew(debugPort, 5);
            Console.WriteLine("");

            Console.WriteLine("Set Variable AZM Slew Off:");
            NexStarCommandHelper.SetVariableAZMSlew(debugPort, 0);
            Console.WriteLine("");

            Console.WriteLine("Set Variable DEC Slew On:");
            NexStarCommandHelper.SetVariableDECSlew(debugPort, 5);
            Console.WriteLine("");

            Console.WriteLine("Set Variable DEC Slew Off:");
            NexStarCommandHelper.SetVariableDECSlew(debugPort, 0);
            Console.WriteLine("");

            Console.WriteLine("Set Fixed AZM Slew On:");
            NexStarCommandHelper.SetFixedAZMSlew(debugPort, SlewRate.SlowScan, true);
            Console.WriteLine("");

            Console.WriteLine("Set Fixed AZM Slew Off:");
            NexStarCommandHelper.SetFixedAZMSlew(debugPort, SlewRate.Stop, false);
            Console.WriteLine("");

            Console.WriteLine("Set Fixed DEC Slew On:");
            NexStarCommandHelper.SetFixedDECSlew(debugPort, SlewRate.SlowScan, true);
            Console.WriteLine("");

            Console.WriteLine("Set Fixed DEC Slew Off:");
            NexStarCommandHelper.SetFixedDECSlew(debugPort, SlewRate.Stop, false);
            Console.WriteLine("");

            Console.WriteLine("Junk Command 0x19 then GetVersion");
            NexStarCommandHelper.SendCommandPassThough(debugPort, new byte[] {0x19, 0x56});
            Console.WriteLine("");

            Console.WriteLine("Junk Command GetVersion then 0x19");
            NexStarCommandHelper.SendCommandPassThough(debugPort, new byte[] { 0x56, 0x19 });
            Console.WriteLine("");

            Console.WriteLine(@"Debug finsihed....");
            Console.ReadKey();
        }
    }
}
