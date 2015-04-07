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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace KinectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Mode _mode = Mode.Color;
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        Window1 window1 = new Window1();


        public MainWindow()
        {
            InitializeComponent();
            //System.Diagnostics.Process.Start(@"C:\Windows\System32\Kinect\KinectService.exe");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();
            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
                window1.Show();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            //Get a reference to the multi-frame
            var reference = e.FrameReference.AcquireFrame();

            //Open color frame
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Color)
                    {
                        camera.Source = ToBitmap(frame);
                    }
                }
            }

            //Open depth frame
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Depth)
                    {
                        camera.Source = ToBitmap(frame);
                    }
                }
            }

            //Open infrared frame
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Infrared)
                    {
                        camera.Source = ToBitmap(frame);
                    }
                }
            }

            //Output body variables in the console
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                Joint handLeft = joints[JointType.HandLeft];
                                Joint wristLeft = joints[JointType.WristLeft];
                                Joint elbowLeft = joints[JointType.ElbowLeft];
                                Joint shoulderLeft = joints[JointType.ShoulderLeft];
                                Joint handRight = joints[JointType.HandRight];
                                Joint wristRight = joints[JointType.WristRight];
                                Joint elbowRight = joints[JointType.ElbowRight];
                                Joint shoulderRight = joints[JointType.ShoulderRight];
                                Joint head = joints[JointType.Head];
                                Joint spineShoulder = joints[JointType.SpineShoulder];
                                Joint spineBase = joints[JointType.SpineBase];

                                CameraSpacePoint cameraHandLeft = handLeft.Position;
                                CameraSpacePoint cameraWristLeft = wristLeft.Position;
                                CameraSpacePoint cameraElbowLeft = elbowLeft.Position;
                                CameraSpacePoint cameraShoulderLeft = shoulderLeft.Position;
                                CameraSpacePoint cameraHandRight = handRight.Position;
                                CameraSpacePoint cameraWristRight = wristRight.Position;
                                CameraSpacePoint cameraElbowRight = elbowRight.Position;
                                CameraSpacePoint cameraShoulderRight = shoulderRight.Position;
                                CameraSpacePoint cameraHead = head.Position;
                                CameraSpacePoint cameraSpineShoulder = spineShoulder.Position;
                                CameraSpacePoint cameraSpineBase = spineBase.Position;

                                Vector3D shoulderElbowLeft = new Vector3D( Math.Round(cameraElbowLeft.X-cameraShoulderLeft.X, 3),  Math.Round(cameraElbowLeft.Y-cameraShoulderLeft.Y, 3),  Math.Round(cameraElbowLeft.Z-cameraShoulderLeft.Z, 3));
                                Vector3D elbowWristLeft = new Vector3D( Math.Round(cameraWristLeft.X-cameraElbowLeft.X, 3),  Math.Round(cameraWristLeft.Y-cameraElbowLeft.Y, 3),  Math.Round(cameraWristLeft.Z-cameraElbowLeft.Z, 3));
                                Vector3D shoulderElbowRight = new Vector3D(Math.Round(cameraElbowRight.X - cameraShoulderRight.X, 3), Math.Round(cameraElbowRight.Y - cameraShoulderRight.Y, 3), Math.Round(cameraElbowRight.Z - cameraShoulderRight.Z, 3));
                                Vector3D elbowWristRight = new Vector3D(Math.Round(cameraWristRight.X - cameraElbowRight.X, 3), Math.Round(cameraWristRight.Y - cameraElbowRight.Y, 3), Math.Round(cameraWristRight.Z - cameraElbowRight.Z, 3));
                                Vector3D torso = new Vector3D(Math.Round(cameraSpineBase.X - cameraSpineShoulder.X, 3), Math.Round(cameraSpineBase.Y - cameraSpineShoulder.Y, 3), Math.Round(cameraSpineBase.Z - cameraSpineShoulder.Z, 3));

                                var scalarSEEWLeft = Vector3D.DotProduct(shoulderElbowLeft, elbowWristLeft);
                                var scalarEWTLeft = Vector3D.DotProduct(elbowWristLeft, torso);
                                var scalarSEEWRight = Vector3D.DotProduct(shoulderElbowRight, elbowWristRight);
                                var scalarEWTRight = Vector3D.DotProduct(elbowWristRight, torso);

                                Console.WriteLine("Right wrist : X : {0} \" Y : {1} \" : {2}", cameraWristRight.X, cameraWristRight.Y, cameraWristRight.Z);

                                window1.update_pointCoordiantes(String.Format("Left hand : X : {0} Y : {1} Z : {2} \n Left wrsit : X : {3} Y : {4} Z : {5} \n Left elbow : X : {6} Y : {7} Z : {8} \n Left shoulder : X : {9} Y : {10} Z : {11} \n", Math.Round(cameraHandLeft.X, 3), Math.Round(cameraHandLeft.Y, 3), Math.Round(cameraHandLeft.Z, 3), Math.Round(cameraWristLeft.X, 3), Math.Round(cameraWristLeft.Y, 3), Math.Round(cameraWristLeft.Z, 3), Math.Round(cameraElbowLeft.X, 3), Math.Round(cameraElbowLeft.Y, 3), Math.Round(cameraElbowLeft.Z, 3), Math.Round(cameraShoulderLeft.X, 3), Math.Round(cameraShoulderLeft.Y, 3), Math.Round(cameraShoulderLeft.Z, 3)));
                                window1.update_vectors(String.Format("Left shoulder Elbow vector : X {0} Y : {1} Z : {2} \n Left elbow Wrist vector : X : {3} Y : {4} Z : {5} \n Left EWT dot product {6} \n Right EWT dot product {7}", shoulderElbowLeft.X, shoulderElbowLeft.Y, shoulderElbowLeft.Z, elbowWristLeft.X, elbowWristLeft.Y, elbowWristLeft.Z, scalarEWTLeft, scalarEWTRight));
                                

                                if(Math.Abs(scalarEWTRight) < 0.12)
                                {
                                    window1.update_armtracked("Right arm tracked");
                                }
                                else
                                {
                                    if (Math.Abs(scalarEWTLeft) < 0.12)
                                    {
                                        window1.update_armtracked("Left arm tracked");
                                    }
                                    else
                                    {
                                        window1.update_armtracked("No arm tracked");
                                    }
                                }


                                /*foreach (JointType jointType in joints.Keys)
                                {
                                    Console.WriteLine("Start joint");
                                    Console.WriteLine("Joint position X : {0} \" Joint position Y : {1} \" Joint position Z : {2}", joints[jointType].Position.X, joints[jointType].Position.Y, joints[jointType].Position.Z);
                                    Console.WriteLine("End joint");
                                    
                                }*/
                            }
                        }
                    }
                }
            }
        }

        private ImageSource ToBitmap(ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        private ImageSource ToBitmap(DepthFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            ushort[] pixelData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(pixelData);

            int colorIndex = 0;
            for (int depthIndex = 0; depthIndex < pixelData.Length; ++depthIndex)
            {
                ushort depth = pixelData[depthIndex];

                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green
                pixels[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        private ImageSource ToBitmap(InfraredFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort[] frameData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(frameData);

            int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < frameData.Length; infraredIndex++)
            {
                ushort ir = frameData[infraredIndex];

                byte intensity = (byte)(ir >> 7);

                pixels[colorIndex++] = (byte)(intensity / 1); // Blue
                pixels[colorIndex++] = (byte)(intensity / 1); // Green   
                pixels[colorIndex++] = (byte)(intensity / 0.4); // Red

                colorIndex++;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Color;
        }

        private void Depth_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Depth;
        }

        private void Infrared_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Infrared;
        }
    }

    public enum Mode
    {
        Color,
        Depth,
        Infrared
    }
}
