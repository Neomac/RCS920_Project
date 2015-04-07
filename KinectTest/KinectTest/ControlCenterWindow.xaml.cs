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
        string mode;
        string side;

        public ControlCenterWindow()
        {
            InitializeComponent();
        }

        private void radioAuto_Checked(object sender, RoutedEventArgs e)
        {
            mode = "Auto";
            sideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;

            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.mode = mode;
            OnModeUpdate(ccea);
        }

        private void radioManual_Checked(object sender, RoutedEventArgs e)
        {
            mode = "Manual";
            sideSelection.Visibility = System.Windows.Visibility.Visible;
            radioRight.Visibility=  System.Windows.Visibility.Visible;
            radioLeft.Visibility = System.Windows.Visibility.Visible;

            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.mode = mode;
            OnModeUpdate(ccea);
        }

        private void radioRight_Checked(object sender, RoutedEventArgs e)
        {
            side = "Right";
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.mode = "Manual";
            ccea.side = side;
            OnModeUpdate(ccea);
        }

        private void radioLeft_Checked(object sender, RoutedEventArgs e)
        {
            side = "Left";
            ControlCenterEventArgs ccea = new ControlCenterEventArgs();
            ccea.mode = "Manual";
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

        protected virtual void OnSideUpdate(ControlCenterEventArgs e)
        {
            if (SettingUpdated != null)
            {
                SettingUpdated(this, e);
            }
        }

        public string getMode()
        {
            return mode;
        }

        public string getSide()
        {
            return side;
        }
    }
}
