using NexStarRemote.SerialSupport;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for LocationControl.xaml
    /// </summary>
    public partial class LocationControl : UserControl
    {
        private Regex _filterRegex = new Regex(@"\D", RegexOptions.Compiled);

        private GPSData _currentGPSLocation;
        private SerialPort _currentConnection;
        private string _currentPort;

        public string CurrentPort
        {
            get
            {
                return _currentPort;
            }
            set
            {
                _currentPort = value;
                _currentConnection = NexStarCommandHelper.BuildNexStarSerialPort(value);
                GetLocationFromAPI();
                UpdateLocationLabel();
            }
        }
 
        public LocationControl()
        {
            InitializeComponent();
            _currentGPSLocation = new GPSData();
        }

        #region Event Handlers
        private void EditLocationButton_Click(object sender, RoutedEventArgs e)
        {
            EnableEditControls();

            NSComboBox.SelectedIndex = _currentGPSLocation.IsNorth ? 0 : 1; 
            DegLatTextbox.Text = _currentGPSLocation.DegreesLatitude.ToString();
            MinLatTextbox.Text = _currentGPSLocation.MinutesLatitude.ToString();
            SecLatTextbox.Text = _currentGPSLocation.SecondsLatitude.ToString();

            WEComboBox.SelectedIndex = _currentGPSLocation.IsWest ? 0 : 1;
            DegLonTextbox.Text = _currentGPSLocation.DegreesLongitude.ToString();
            MinLonTextbox.Text = _currentGPSLocation.MinutesLongitude.ToString();
            SecLonTextbox.Text = _currentGPSLocation.SecondsLongitude.ToString();
        }

        private void CancelLocationButton_Click(object sender, RoutedEventArgs e)
        {
            DisableEditControls();
        }
        
        private void NumericTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox senderTextbox = (TextBox)sender;
            int carotPosition = senderTextbox.CaretIndex;

            senderTextbox.TextChanged -= NumericTextbox_TextChanged;
            senderTextbox.Text = _filterRegex.Replace(senderTextbox.Text, "");
            senderTextbox.TextChanged += NumericTextbox_TextChanged;

            try
            {
                senderTextbox.CaretIndex = carotPosition;
            }
            catch
            {
                senderTextbox.CaretIndex = senderTextbox.Text.Length - 1;
            }
        }

        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            _currentGPSLocation = new GPSData()
            {
                IsNorth = (NSComboBox.SelectedIndex == 0) ? true : false,
                DegreesLatitude = Convert.ToInt16(DegLatTextbox.Text),
                MinutesLatitude = Convert.ToInt16(MinLatTextbox.Text),
                SecondsLatitude = Convert.ToInt16(SecLatTextbox.Text),
                IsWest = (WEComboBox.SelectedIndex == 0) ? true : false,
                DegreesLongitude = Convert.ToInt16(DegLonTextbox.Text),
                MinutesLongitude = Convert.ToInt16(MinLonTextbox.Text),
                SecondsLongitude = Convert.ToInt16(SecLatTextbox.Text)
            };

            NexStarCommandHelper.SetLocation(_currentConnection, _currentGPSLocation);
            GetLocationFromAPI();
            UpdateLocationLabel();
            DisableEditControls();
        }
        #endregion

        #region Helper Methods
        private void GetLocationFromAPI()
        {
            GPSData currentLocation = NexStarCommandHelper.GetLocation(_currentConnection);
            _currentGPSLocation = currentLocation;
        }

        private void UpdateLocationLabel()
        {
            GPSLocationLabel.Content = _currentGPSLocation.ToString();
        }

        private void EnableEditControls()
        {
            EditLocationButton.Visibility = Visibility.Collapsed;
            SaveLocationButton.Visibility = Visibility.Visible;
            CancelLocationButton.Visibility = Visibility.Visible;
            EditToolbar.Visibility = Visibility.Visible;
        }

        private void DisableEditControls()
        {
            EditLocationButton.Visibility = Visibility.Visible;
            SaveLocationButton.Visibility = Visibility.Collapsed;
            CancelLocationButton.Visibility = Visibility.Collapsed;
            EditToolbar.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
