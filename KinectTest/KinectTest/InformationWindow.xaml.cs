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
using Microsoft.Kinect;

namespace KinectTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        private string mode;
        private string side;

        public InformationWindow()
        {
            InitializeComponent();
        }

        public void updateArmTracked(ArmTracked armTracked)
        {
            sideText.Text = String.Format("The {0} side is tracked.", armTracked.getSide());
            coordinatesText.Text = String.Format("Coordinates of the differentes joint tracked : \n Hand : X={0} Y={1} Z={2} \n Wrist : X={3} Y={4} Z={5} \n Elbow : X={6} Y={7} Z={8} \n Shoulder : X={9} Y={10} Z={11}", armTracked.getHand().Position.X, armTracked.getHand().Position.Y, armTracked.getHand().Position.Z, armTracked.getWrist().Position.X, armTracked.getWrist().Position.Y, armTracked.getWrist().Position.Z, armTracked.getElbow().Position.X, armTracked.getElbow().Position.Y, armTracked.getElbow().Position.Z, armTracked.getShoulder().Position.X, armTracked.getShoulder().Position.Y, armTracked.getShoulder().Position.Z);
            hand_StateText.Text = String.Format("The hand is {0}", armTracked.getHandState().ToString());
        }

        public void setMode(string inputMode)
        {
            mode = inputMode;
        }

        public void setSide(string inputSide)
        {
            side = inputSide;
        }
    }
}
