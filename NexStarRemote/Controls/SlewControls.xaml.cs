using NexStarRemote.SerialSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SlewControls.xaml
    /// </summary>
    public partial class SlewControls : UserControl
    {
        public string CurrentPort { get; set; }

        private DependencyPropertyDescriptor upButtonDescriptor, rightButtonDescriptor, leftButtonDescriptor, downButtonDescriptor;
        private TrackingMode _savedTrackingMode;

        private int SlewSpeedSelected
        {
            get
            {
                return (SlewSpeedComboBox.SelectedItem as int?).HasValue ? (SlewSpeedComboBox.SelectedItem as int?).Value : 0;
            }
        }

        public SlewControls()
        {
            InitializeComponent();

            upButtonDescriptor = DependencyPropertyDescriptor.FromProperty(Button.IsPressedProperty, typeof(Button));
            upButtonDescriptor.AddValueChanged(this.UpButton, new EventHandler(UpButtonPressedHandler));
            rightButtonDescriptor = DependencyPropertyDescriptor.FromProperty(Button.IsPressedProperty, typeof(Button));
            rightButtonDescriptor.AddValueChanged(this.RightButton, new EventHandler(RightButtonPressedHandler));
            leftButtonDescriptor = DependencyPropertyDescriptor.FromProperty(Button.IsPressedProperty, typeof(Button));
            leftButtonDescriptor.AddValueChanged(this.LeftButton, new EventHandler(LeftButtonPressedHandler));
            downButtonDescriptor = DependencyPropertyDescriptor.FromProperty(Button.IsPressedProperty, typeof(Button));
            downButtonDescriptor.AddValueChanged(this.DownButton, new EventHandler(DownButtonPressedHandler));

            this.IsEnabledChanged += EnabledHandler;
        }

        #region IsEnabled Helpers
        private void EnabledHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                EnableControl();
            }
            else
            {
                DisableControl();
            }
        }

        private void EnableControl()
        {
            UpButton.Content = Resources["ConnectedButtonUp"];
            UpButton.IsEnabled = true;

            RightButton.Content = Resources["ConnectedButtonRight"];
            RightButton.IsEnabled = true;

            LeftButton.Content = Resources["ConnectedButtonLeft"];
            LeftButton.IsEnabled = true;

            DownButton.Content = Resources["ConnectedButtonDown"];
            DownButton.IsEnabled = true;

            SlewSpeedComboBox.IsEnabled = true;
        }

        private void DisableControl()
        {
            UpButton.Content = Resources["UnconnectedButtonUp"];
            UpButton.IsEnabled = false;

            RightButton.Content = Resources["UnconnectedButtonRight"];
            RightButton.IsEnabled = false;

            LeftButton.Content = Resources["UnconnectedButtonLeft"];
            LeftButton.IsEnabled = false;

            DownButton.Content = Resources["UnconnectedButtonDown"];
            DownButton.IsEnabled = false;

            SlewSpeedComboBox.IsEnabled = false;
        }
        #endregion

        #region Button Press Handlers
        public void UpButtonPressedHandler(object sender, EventArgs e)
        {
            if (UpButton.IsPressed)
            {
                UpButton.Content = Resources["ActiveButton"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                DisableTracking(slewPort);
                SendFixSlewCommand(slewPort, SlewDirection.Up, SlewRate.FindSlewRate(SlewSpeedSelected));
            }
            else
            {
                UpButton.Content = Resources["ConnectedButtonUp"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                SendFixSlewCommand(slewPort, SlewDirection.Up, SlewRate.Stop);
                EnableTracking(slewPort);
            }
        }

        public void DownButtonPressedHandler(object sender, EventArgs e)
        {
            if (DownButton.IsPressed)
            {
                DownButton.Content = Resources["ActiveButton"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                DisableTracking(slewPort);
                SendFixSlewCommand(slewPort, SlewDirection.Down, SlewRate.FindSlewRate(SlewSpeedSelected));
            }
            else
            {
                DownButton.Content = Resources["ConnectedButtonDown"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                SendFixSlewCommand(slewPort, SlewDirection.Down, SlewRate.Stop);
                EnableTracking(slewPort);
            }
        }

        public void LeftButtonPressedHandler(object sender, EventArgs e)
        {
            if (LeftButton.IsPressed)
            {
                LeftButton.Content = Resources["ActiveButton"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                DisableTracking(slewPort);
                SendFixSlewCommand(slewPort, SlewDirection.Left, SlewRate.FindSlewRate(SlewSpeedSelected));
            }
            else
            {
                LeftButton.Content = Resources["ConnectedButtonLeft"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                SendFixSlewCommand(slewPort, SlewDirection.Left, SlewRate.Stop);
                EnableTracking(slewPort);
            }
        }

        public void RightButtonPressedHandler(object sender, EventArgs e)
        {
            if (RightButton.IsPressed)
            {
                RightButton.Content = Resources["ActiveButton"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                DisableTracking(slewPort);
                SendFixSlewCommand(slewPort, SlewDirection.Right, SlewRate.FindSlewRate(SlewSpeedSelected));
            }
            else
            {
                RightButton.Content = Resources["ConnectedButtonRight"];
                SerialPort slewPort = NexStarCommandHelper.BuildNexStarSerialPort(CurrentPort);
                SendFixSlewCommand(slewPort, SlewDirection.Right, SlewRate.Stop);
                EnableTracking(slewPort);
            }
        }
        #endregion

        private void SendFixSlewCommand(SerialPort slewPort, SlewDirection direction, SlewRate rate)
        {
            if(direction == SlewDirection.Left || direction == SlewDirection.Right)
            {
                NexStarCommandHelper.SetFixedAZMSlew(slewPort, rate, (direction == SlewDirection.Right) ? true : false);
            }
            else
            {
                NexStarCommandHelper.SetFixedDECSlew(slewPort, rate, (direction == SlewDirection.Up) ? true : false);
            }
        }

        private void EnableTracking(SerialPort nexstarPort)
        {
            if (_savedTrackingMode != null)
            {
                NexStarCommandHelper.SetTrackingMode(nexstarPort, _savedTrackingMode);
            }
            else
            {
                NexStarCommandHelper.SetTrackingMode(nexstarPort, TrackingMode.Off);
            }

            _savedTrackingMode = null;
        }

        private void DisableTracking(SerialPort nexstarPort)
        {
            _savedTrackingMode = NexStarCommandHelper.GetTrackingMode(nexstarPort);
            NexStarCommandHelper.SetTrackingMode(nexstarPort, TrackingMode.Off);
        }
    }
}
