using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; 

namespace TestDLL
{
    class Program
    {

        //DLL call
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]

        //DLL functions declaration
        public static extern string pingRobot(int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setCartesian(double x, double y, double z, double q0, double qx, double qy, double qz, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setJoints(double joint1, double joint2, double joint3, double joint4, double joint5, double joint6, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string getCartesian(int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string getJoints(int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setTool(double x, double y, double z, double q0, double qx, double qy, double qz, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setWorkObject(double x, double y, double z, double q0, double qx, double qy, double qz, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setSpeed(double tcp, double ori, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setZone(bool fine = true, double tcp_mm = 5.0, double ori_mm = 5.0, double ori_deg = 1.0, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string specialCommand(int command, double param1, double param2, double param3, double param4, double param5, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setVacuum(int vacuum = 0, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string setDIO(int dio_number = 0, int dio_state = 0, int idCode = 0);
        [DllImport("H:\\RSC920\\GitHub\\RCS920_Project\\KinectTest\\KinectTest\\CommunicationDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string closeConnection(int idCode = 0);
        static void Main(string[] args)
        {
            pingRobot();
        }
    }
}
