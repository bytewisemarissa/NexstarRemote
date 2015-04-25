using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexStarRemote.SerialSupport
{
    public static class NexStarCommandHelper
    {
        public static bool RetryEnabled { get; set; }

        private static Dictionary<string, object> _synclockCollection = new Dictionary<string,object>();

        private static readonly string StopCharacter = "#";

        #region MagicStrings
        private static readonly byte VariableSlewPositive = 0x06;
        private static readonly byte VariableSlewNegative = 0x07;
        private static readonly byte FixedSlewPositive = 0x24;
        private static readonly byte FixedSlewNegative = 0x25;
        #endregion

        #region Command Strings
        private static readonly byte[] GetVersionCommand = new byte[] { Convert.ToByte('V') };
        private static readonly byte[] EchoCommand = new byte[] { Convert.ToByte('K') };
        private static readonly byte[] GetModelCommand = new byte[] { Convert.ToByte('m') };
        private static readonly byte[] GetDeviceVersionCommand = new byte[]{Convert.ToByte('P'), 0x01, 0x0, 0xFE, 0x0, 0x0, 0x0, 0x2};
        private static readonly byte[] IsAlignmentCompleteCommand = new byte[] { Convert.ToByte('J') };
        private static readonly byte[] IsGotoCompleteCommand = new byte[] { Convert.ToByte('L') };
        private static readonly byte[] CancelGotoCommand = new byte[] { Convert.ToByte('M') };
        private static readonly byte[] GetTrackingModeCommand = new byte[] { Convert.ToByte('t') };
        private static readonly byte[] SetTrackingModeCommand = new byte[] { Convert.ToByte('T'), 0x0 };
        private static readonly byte[] SetVariableSlewAZMCommand = new byte[] { Convert.ToByte('P'), 0x03, 0x10, 0x0, 0x0, 0x0, 0x0, 0x0 };
        private static readonly byte[] SetVariableSlewDECCommand = new byte[] { Convert.ToByte('P'), 0x03, 0x11, 0x0, 0x0, 0x0, 0x0, 0x0 };
        private static readonly byte[] SetFixedSlewAZMCommand = new byte[] { Convert.ToByte('P'), 0x02, 0x10, 0x0, 0x0, 0x0, 0x0, 0x0 };
        private static readonly byte[] SetFixedSlewDECCommand = new byte[] { Convert.ToByte('P'), 0x02, 0x11, 0x0, 0x0, 0x0, 0x0, 0x0 };
        #endregion

        #region Lookup Dictionaries
        private static readonly Dictionary<int, string> _telescopeModelLookup = new Dictionary<int, string>(){
            {1,"GPS Series"},
            {3,"i-Series"},
            {4,"i-Series SE"},
            {5,"CGE"},
            {6,"Advanced GT"},
            {7,"SLT"},
            {9,"CPC"},
            {10,"GT"},
            {11,"4/5 SE"},
            {12,"6/8 SE"},
            {15,"LCM"}
        };
        #endregion

        #region Tracking Commands API Implementation
        public static TrackingMode GetTrackingMode(SerialPort nexStarPort)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] trackingResponse = SendCommand(nexStarPort, GetTrackingModeCommand);

                Int16 trackingId = Convert.ToInt16(trackingResponse[0]);
                TrackingMode returnValue = TrackingMode.GetTrackingMode(trackingId);
                return returnValue;
            }
        }

        public static void SetTrackingMode(SerialPort nexStarPort, TrackingMode mode)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] formatedCommand = FormatCommand(SetTrackingModeCommand, new byte?[]{ null , mode.Identifier});
                SendCommand(nexStarPort, formatedCommand);
            }
        }
        #endregion

        #region Slewing Commands API Implementation
        public static void SetVariableAZMSlew(SerialPort nexStarPort, int arcSecPerSec)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                if (arcSecPerSec == 0)
                {
                    byte[] formatedStopCommand1 = FormatCommand(SetVariableSlewAZMCommand, new byte?[] { null, null, null, VariableSlewPositive, 0x0, 0x0 });
                    byte[] formatedStopCommand2 = FormatCommand(SetVariableSlewAZMCommand, new byte?[] { null, null, null, VariableSlewNegative, 0x0, 0x0 });

                    SendCommand(nexStarPort, formatedStopCommand1);
                    SendCommand(nexStarPort, formatedStopCommand2);
                }
                else
                {
                    byte signValue = arcSecPerSec > 0 ? VariableSlewPositive : VariableSlewNegative;
                    byte lowOrderByte = (byte)((arcSecPerSec * 4) / 256);
                    byte highOrderByte = (byte)((arcSecPerSec * 4) % 256);

                    byte[] formatedCommand = FormatCommand(SetVariableSlewAZMCommand, new byte?[] { null, null, null, signValue, highOrderByte, lowOrderByte });
                    SendCommand(nexStarPort, formatedCommand);
                }
            }
        }

        public static void SetVariableDECSlew(SerialPort nexStarPort, int arcSecPerSec)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                if (arcSecPerSec == 0)
                {
                    byte[] formatedStopCommand1 = FormatCommand(SetVariableSlewDECCommand, new byte?[] { null, null, null, VariableSlewPositive, 0x0, 0x0 });
                    byte[] formatedStopCommand2 = FormatCommand(SetVariableSlewDECCommand, new byte?[] { null, null, null, VariableSlewNegative, 0x0, 0x0 });

                    SendCommand(nexStarPort, formatedStopCommand1);
                    SendCommand(nexStarPort, formatedStopCommand2);
                }
                else
                {
                    byte signValue = arcSecPerSec > 0 ? VariableSlewPositive : VariableSlewNegative;
                    byte lowOrderByte = (byte)((arcSecPerSec * 4) / 256);
                    byte highOrderByte = (byte)((arcSecPerSec * 4) % 256);

                    byte[] formatedCommand = FormatCommand(SetVariableSlewDECCommand, new byte?[] { null, null, null, signValue, highOrderByte, lowOrderByte });
                    SendCommand(nexStarPort, formatedCommand);
                }
            }
        }

        public static void SetFixedAZMSlew(SerialPort nexStarPort, SlewRate rate, bool slewPositive)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte signValue = slewPositive ? FixedSlewPositive : FixedSlewNegative;

                byte[] formatedCommand = FormatCommand(SetFixedSlewAZMCommand, new byte?[] { null, null, null, signValue, rate.RateValue });
                SendCommand(nexStarPort, formatedCommand);
            }
        }

        public static void SetFixedDECSlew(SerialPort nexStarPort, SlewRate rate, bool slewPositive)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte signValue = slewPositive ? FixedSlewPositive : FixedSlewNegative;

                byte[] formatedCommand = FormatCommand(SetFixedSlewDECCommand, new byte?[] { null, null, null, signValue, rate.RateValue });
                SendCommand(nexStarPort, formatedCommand);
            }
        }
        #endregion

        #region Miscellaneous Commands API Implementation
        public static string GetVersion(SerialPort nexStarPort)
        {
            if(!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] versionResponse = SendCommand(nexStarPort, GetVersionCommand);

                if (versionResponse != null)
                {
                    return string.Format("{0}.{1}", Convert.ToInt16(versionResponse[0]), Convert.ToInt16(versionResponse[1]));
                }
                else
                {
                    return null;
                }
            }
        }

        public static string GetDeviceVersion(SerialPort nexStarPort, TelescopeDevices targetDevice)
        {
            if(!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] formatedCommand = GetDeviceVersionCommand;
                formatedCommand[2] = targetDevice.DeviceId;
                byte[] versionResponse = SendCommand(nexStarPort, formatedCommand);

                if (versionResponse != null)
                {
                    return string.Format("{0}.{1}", Convert.ToInt16(versionResponse[0]), Convert.ToInt16(versionResponse[1]));
                }
                else
                {
                    return null;
                }
            }
        }

        public static string GetModel(SerialPort nexStarPort)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] versionResponse = SendCommand(nexStarPort, GetModelCommand);

                Int16 modelId = Convert.ToInt16(versionResponse[0]);
                return _telescopeModelLookup[modelId];
            }
        }

        public static string Echo(SerialPort nexStarPort, string message)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] modelResponse = SendCommand(nexStarPort, EchoCommand);

                string echodString = Encoding.Default.GetString(modelResponse);
                return echodString;
            }
        }

        public static bool IsAlignmentComplete(SerialPort nexStarPort)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] alignmentResponse = SendCommand(nexStarPort, IsAlignmentCompleteCommand);

                bool returnValue = Convert.ToBoolean(alignmentResponse[0]);
                return returnValue;
            }
        }

        public static bool IsGotoRunning(SerialPort nexStarPort)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                byte[] gotoResponse = SendCommand(nexStarPort, IsGotoCompleteCommand);

                string resultString = Encoding.ASCII.GetString(gotoResponse, 0, 1);
                bool returnValue = resultString == "1" ? true : false;
                return returnValue;
            }
        }

        public static void CancelGoto(SerialPort nexStarPort)
        {
            if (!_synclockCollection.Keys.Contains(nexStarPort.PortName)) _synclockCollection[nexStarPort.PortName] = new object();
            lock (_synclockCollection[nexStarPort.PortName])
            {
                SendCommand(nexStarPort, CancelGotoCommand);
            }
        }
        #endregion

        #region Public Supporting Methods
        public static SerialPort BuildNexStarSerialPort()
        {
            SerialPort returnValue = new SerialPort();
            returnValue.BaudRate = Convert.ToInt32(ConfigurationManager.AppSettings["BaudRate"]);
            returnValue.ReadTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["ReadTimeoutMS"]);
            returnValue.WriteTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WriteTimeoutMS"]);


            switch (ConfigurationManager.AppSettings["Parity"].ToLower())
            {
                case "even":
                    returnValue.Parity = Parity.Even;
                    break;
                case "mark":
                    returnValue.Parity = Parity.Mark;
                    break;
                case "none":
                    returnValue.Parity = Parity.None;
                    break;
                case "Odd":
                    returnValue.Parity = Parity.Odd;
                    break;
                case "Space":
                    returnValue.Parity = Parity.Space;
                    break;
                default:
                    returnValue.Parity = Parity.None;
                    break;
            }

            switch (ConfigurationManager.AppSettings["StopBits"].ToLower())
            {
                case "none":
                    returnValue.StopBits = StopBits.None;
                    break;
                case "one":
                    returnValue.StopBits = StopBits.One;
                    break;
                case "onepointfive":
                    returnValue.StopBits = StopBits.OnePointFive;
                    break;
                case "two":
                    returnValue.StopBits = StopBits.Two;
                    break;
                default:
                    returnValue.StopBits = StopBits.None;
                    break;
            }

            return returnValue;
        }

        public static SerialPort BuildNexStarSerialPort(string portName)
        {
            SerialPort returnValue = BuildNexStarSerialPort();
            returnValue.PortName = portName;
            return returnValue;
        }

        public static string[] FindActiveSerialNexstarDevices()
        {
            Console.WriteLine("Port scaning...");
            List<string> livePorts = new List<string>();
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                SerialPort testPort = NexStarCommandHelper.BuildNexStarSerialPort();
                testPort.PortName = portName;
                string response = NexStarCommandHelper.GetVersion(testPort);
                if (!String.IsNullOrEmpty(response))
                {
                    livePorts.Add(portName);
                    ConsoleManager.WriteLineColor(String.Format("Found NexStar ver {0}!", response), ConsoleColor.Green);
                }
            }
            Console.WriteLine("Port scan finished!");

            return livePorts.ToArray();
        }
        #endregion

        #region Private Supporting Methods
        private static byte[] SendCommand(SerialPort nexStarPort, byte[] commandBytes)
        {
            try
            {
                if (!nexStarPort.IsOpen)
                {
                    Console.Write(String.Format("Opening port {0} -> ", nexStarPort.PortName));
                    nexStarPort.Open();
                }

                Console.Write(String.Format("Sending '{0}' -> ", RenderByteArray(commandBytes)));
                nexStarPort.Write(commandBytes, 0, commandBytes.Count());
                byte[] returnValue = Encoding.UTF8.GetBytes(nexStarPort.ReadTo(StopCharacter));
                Console.Write(String.Format("Recieved '{0}' -> ", RenderByteArray(returnValue)));
                return returnValue;
            }
            catch (TimeoutException)
            {
                if (RetryEnabled)
                {
                    Console.Write("Retry -> ");
                    SendCommand(nexStarPort, commandBytes);
                }

                Console.Write("Timeout -> ");
                return null;
            }
            finally
            {
                if (nexStarPort.IsOpen)
                {
                    nexStarPort.Close();
                    Console.WriteLine("Connection Closed.");
                }
            }
        }

        //private static void SendSlewComand(SerialPort nexStarPort, byte[] commandBytes)
        //{
        //    TrackingMode currentTracking = GetTrackingMode(nexStarPort);
        //    SetTrackingMode(nexStarPort, TrackingMode.Off);
        //    SendCommand(nexStarPort, commandBytes);
        //    SetTrackingMode(nexStarPort, currentTracking);
        //}

        private static string RenderByteArray(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");
            foreach (byte b in bytes)
            {
                builder.Append(b + ", ");
            }
            if (bytes.Count() > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }
            builder.Append(" }");
            return builder.ToString();
        }

        private static byte[] FormatCommand(byte[] command, params byte?[] args)
        {
            byte[] commandToFormat = command;
            for (int i = 0; i < args.Count(); i++)
            {
                if (args[i].HasValue)
                {
                    commandToFormat[i] = args[i].Value;
                }
            }
            return commandToFormat;
        }
        #endregion
    }
}
