using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace KinectTest
{
    /// <summary>
    /// Interaction logic for ControlCenterWindow.xaml
    /// </summary>
    public partial class ControlCenterWindow : Window
    {
        public event EventHandler<ControlCenterEventArgs> SettingUpdated;
        string sideMode;
        string side;
        string trackingMode;

        public ControlCenterWindow()
        {
            InitializeComponent();
        }

        private void RadioMimicking_Checked(object sender, RoutedEventArgs e)
        {
            trackingMode = "Mimicking";
            sideMode = "";
            side = "";
            textSideModeSelection.Visibility = System.Windows.Visibility.Visible;
            radioAuto.Visibility = System.Windows.Visibility.Visible;
            radioManual.Visibility = System.Windows.Visibility.Visible;
            textSideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.trackingMode = trackingMode;
            OnModeUpdate(ccea);
        }

        private void RadioMovingObject_Checked(object sender, RoutedEventArgs e)
        {
            trackingMode = "MovingObject";
            sideMode = "";
            side = "";
            textSideModeSelection.Visibility = System.Windows.Visibility.Visible;
            radioAuto.Visibility = System.Windows.Visibility.Visible;
            radioManual.Visibility = System.Windows.Visibility.Hidden;
            textSideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.trackingMode = trackingMode;
            OnModeUpdate(ccea);
        }

        private void radioAuto_Checked(object sender, RoutedEventArgs e)
        {
            sideMode = "Auto";
            side = "";
            textSideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.sideMode = sideMode;
            ccea.side = side;
            OnModeUpdate(ccea);
        }

        private void radioManual_Checked(object sender, RoutedEventArgs e)
        {
            sideMode = "Manual";
            side = "";
            textSideSelection.Visibility = System.Windows.Visibility.Visible;
            radioRight.Visibility=  System.Windows.Visibility.Visible;
            radioLeft.Visibility = System.Windows.Visibility.Visible;

            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.sideMode = sideMode;
            ccea.side = side;
            OnModeUpdate(ccea);
        }

        private void radioRight_Checked(object sender, RoutedEventArgs e)
        {
            side = "Right";
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.sideMode = "Manual";
            ccea.side = side;
            OnModeUpdate(ccea);
        }

        private void radioLeft_Checked(object sender, RoutedEventArgs e)
        {
            side = "Left";
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.sideMode = "Manual";
            ccea.side = side;
            OnModeUpdate(ccea);
        }

        protected virtual void OnModeUpdate(ControlCenterEventArgs e)
        {
            if (SettingUpdated != null)
            {
                SettingUpdated(this, e);
            }
        }

        protected virtual void OnSideModeUpdate(ControlCenterEventArgs e)
        {
            if (SettingUpdated != null)
            {
                SettingUpdated(this, e);
            }
        }

        protected virtual void OnTrackingModeUpdate(ControlCenterEventArgs e)
        {
            if (SettingUpdated != null)
            {
                SettingUpdated(this, e);
            }
        }

        public string getSideMode()
        {
            return sideMode;
        }

        public string getSide()
        {
            return side;
        }

        public string getTrackingMode()
        {
            return trackingMode;
        }
    }
}
