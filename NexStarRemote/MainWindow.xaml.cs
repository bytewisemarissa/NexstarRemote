using NexStarRemote.SerialSupport;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

namespace NexStarRemote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentPort;
        private bool _interfaceEnabled;
       
        public MainWindow()
        {
            InitializeComponent();

            StatusLabel.Content = "Scaning";
            StatusProgressBar.IsIndeterminate = true;

            Task.Run(() =>
            {
                string[] portNames = NexStarCommandHelper.FindActiveSerialNexstarDevices();

                StatusLabel.Dispatcher.Invoke(() =>
                {
                    StatusLabel.Content = "Scan Complete - Nothing Found";
                }, System.Windows.Threading.DispatcherPriority.Normal);

                foreach (string name in portNames)
                {
                    PortComboBox.Dispatcher.Invoke(() =>
                    {
                        PortComboBox.Items.Add(name);
                    }, System.Windows.Threading.DispatcherPriority.Normal);
                }
                if (PortComboBox.Items.Count > 0)
                {
                    PortComboBox.Dispatcher.Invoke(() =>
                    {
                        PortComboBox.SelectedIndex = 0;
                    }, System.Windows.Threading.DispatcherPriority.Normal);
                }
                StatusProgressBar.Dispatcher.Invoke(() =>
                {
                    StatusProgressBar.Visibility = System.Windows.Visibility.Hidden;
                }, System.Windows.Threading.DispatcherPriority.Normal);
            });
        }

        private void PortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPort = PortComboBox.SelectedItem as string;
            StatusLabel.Content = String.Format("Connected to {0}", _currentPort);

            SlewControler.CurrentPort = _currentPort;
            TrackingControl.CurrentPort = _currentPort;
            LocationSettings.CurrentPort = _currentPort;

            if (!_interfaceEnabled)
            {
                EnableRemote();
                _interfaceEnabled = true;
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow(_currentPort);
            infoWindow.ShowDialog();
        }


        private void EnableRemote()
        {
            SlewControler.IsEnabled = true;
            PortComboBox.IsEnabled = true;
            InfoButton.IsEnabled = true;
            TrackingControl.IsEnabled = true;
            LocationSettings.IsEnabled = true;
        }

        private void DisableRemote()
        {
            SlewControler.IsEnabled = false;
            PortComboBox.IsEnabled = false;
            InfoButton.IsEnabled = false;
            TrackingControl.IsEnabled = false;
            LocationSettings.IsEnabled = false;
        }
    }
}
