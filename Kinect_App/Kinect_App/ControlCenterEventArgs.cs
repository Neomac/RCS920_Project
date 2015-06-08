using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectTest
{
    public class ControlCenterEventArgs : EventArgs
    {
        public string sideMode { get; set; }
        public string side { get; set; }
        public string trackingMode { get; set; }
        public string bodyNumberToTrack { get; set; }
    }
}
