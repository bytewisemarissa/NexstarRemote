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
       
        public MainWindow()
        {
            InitializeComponent();

            StatusLabel.Content = "Scaning";
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
                PortComboBox.Dispatcher.Invoke(() =>
                {
                    PortComboBox.IsEnabled = true;
                }, System.Windows.Threading.DispatcherPriority.Normal);
            });
        }

        private void PortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPort = PortComboBox.SelectedItem as string;
            StatusLabel.Content = String.Format("Connected to {0}", _currentPort);

            if (!SlewControler.IsEnabled)
            {
                SlewControler.IsEnabled = true;
                SlewControler.CurrentPort = _currentPort;
            }

            InfoControl.CurrentPort = _currentPort;
            InfoControl.RefreshAllInformation();
        }

    }
}
