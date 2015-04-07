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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        public void update_pointCoordiantes(string text)
        {
            pointCoordinates.Text = text;
        }

        public void update_vectors(string text)
        {
            vectors.Text = text;
        }
        public void update_armtracked(string text)
        {
            armtracked.Text = text;
        }
    }
}
