using NexStarRemote.SerialSupport;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NexStarRemote.Controls
{
    /// <summary>
    /// Interaction logic for TelescopeInformationControl.xaml
    /// </summary>
    public partial class TelescopeInformationControl : UserControl
    {
        public string CurrentPort { get; set; }

        public TelescopeInformationControl()
        {
            InitializeComponent();
        }

        public void RefreshAllInformation()
        {
            Task.Run(() =>
            {
                SerialPort infoPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);

                Dictionary<string, string> infoDict = new Dictionary<string, string>();
                infoDict.Add("Model Name", NexStarCommandHelper.GetModel(infoPort));
                infoDict.Add("NexStar Version No.", NexStarCommandHelper.GetVersion(infoPort));
                infoDict.Add("AZM/RA Motor Version No.", NexStarCommandHelper.GetDeviceVersion(infoPort, TelescopeDevices.AZM_RA_Motor));
                infoDict.Add("ALT/DEC Motor Version No.", NexStarCommandHelper.GetDeviceVersion(infoPort, TelescopeDevices.ALT_DEC_Motor));
                infoDict.Add("GPS Version No.", NexStarCommandHelper.GetDeviceVersion(infoPort, TelescopeDevices.GPS_Unit));
                infoDict.Add("RTC (CGE only) Version No.", NexStarCommandHelper.GetDeviceVersion(infoPort, TelescopeDevices.RTC));
                infoDict.Add("Is Align Complete", NexStarCommandHelper.IsAlignmentComplete(infoPort).ToString());
                infoDict.Add("Is GOTO Running", NexStarCommandHelper.IsGotoRunning(infoPort).ToString());
                infoDict.Add("Tracking Mode", NexStarCommandHelper.GetTrackingMode(infoPort).Description);

                InfoListBox.Dispatcher.Invoke(() =>
                {
                    InfoListBox.Items.Clear();
                    foreach (KeyValuePair<string, string> dataPoint in infoDict)
                    {
                        InfoListBox.Items.Add(String.Format("{0} -> {1}", dataPoint.Key, dataPoint.Value));
                    }
                });
            });
        }
    }
}
