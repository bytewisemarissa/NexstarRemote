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
    /// Interaction logic for TrackingControl.xaml
    /// </summary>
    public partial class TrackingControl : UserControl
    {
        private SerialPort _nexstarConnection;

        private string _nexstarPortName;
        public string CurrentPort
        {
            get
            {
                return _nexstarPortName;
            }
            set
            {
                _nexstarConnection = NexStarCommandHelper.BuildNexStarSerialPort(value);

                TrackingMode currentMode = CurrentTrackingMode;
                TrackingComboBox.SelectedValue = currentMode.Identifier;

                if (_nexstarPortName == null)
                {
                    TrackingComboBox.SelectionChanged += TrackingComboBox_SelectionChanged;
                }
                _nexstarPortName = value;
            }
        }
        public TrackingMode CurrentTrackingMode
        {
            get
            {
                return NexStarCommandHelper.GetTrackingMode(_nexstarConnection);
            }
            set
            {
                NexStarCommandHelper.SetTrackingMode(_nexstarConnection, value);
                TrackingMode test = NexStarCommandHelper.GetTrackingMode(_nexstarConnection);

                if (test != value)
                {
                    MessageBox.Show("The tracking mode could be set. Probably align issue.");
                    TrackingComboBox.SelectionChanged -= TrackingComboBox_SelectionChanged;
                    TrackingComboBox.SelectedValue = test.Identifier;
                    TrackingComboBox.SelectionChanged += TrackingComboBox_SelectionChanged;
                    return;
                }

                if ((byte)TrackingComboBox.SelectedValue != value.Identifier)
                {
                    TrackingComboBox.SelectionChanged -= TrackingComboBox_SelectionChanged;
                    TrackingComboBox.SelectedValue = value.Identifier;
                    TrackingComboBox.SelectionChanged += TrackingComboBox_SelectionChanged;
                }
            }
        }

        public TrackingControl()
        {
            InitializeComponent();

            TrackingComboBox.DisplayMemberPath = "Description";
            TrackingComboBox.SelectedValuePath = "Identifier";

            foreach(TrackingMode mode in TrackingMode.GetTrackingModes())
                TrackingComboBox.Items.Add(mode);

            TrackingComboBox.SelectedIndex = 0;
        }

        private void TrackingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentTrackingMode = (TrackingMode)TrackingComboBox.SelectedItem;
        }
    }
}
