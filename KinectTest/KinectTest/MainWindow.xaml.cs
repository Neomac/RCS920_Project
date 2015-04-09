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
        InformationWindow informationWindow = new InformationWindow();
        ControlCenterWindow controlCenterWindow = new ControlCenterWindow();

        //Control Center variables
        string controlCenterSideMode;
        string controlCenterSide;
        string controlCenterTrackingMode;
        string controlCenterBodyNumberToTrack;

        private static readonly byte[] BodyColor =
        {
            0,
            255,
            150,
            255,
            80,
            200,
            50,
            230,
            190,
            255
        };







        public MainWindow()
        {
            InitializeComponent();
            //System.Diagnostics.Process.Start(@"C:\Windows\System32\Kinect\KinectService.exe");
            controlCenterWindow.SettingUpdated += new EventHandler<ControlCenterEventArgs>(controlCenterWindow_SettingUpdated); 
            informationWindow.Show();
            controlCenterWindow.Show();
        }

        private void controlCenterWindow_SettingUpdated(object sender, ControlCenterEventArgs e)
        {
            if (e != null && e.trackingMode != null)
            {
                controlCenterTrackingMode = e.trackingMode;
                informationWindow.setTrackingMode(e.trackingMode);
            }
            if (e!= null && e.sideMode != null)
            {
                controlCenterSideMode = e.sideMode;
                informationWindow.setSideMode(e.sideMode);
            }
            if (e!= null && e.side != null)
            {
                controlCenterSide = e.side;
                informationWindow.setSide(e.side);
            }
            if (e!= null && e.bodyNumberToTrack != null)
            {
                controlCenterBodyNumberToTrack = e.bodyNumberToTrack;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();
            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

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

            //Output body variables in the console
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                var counterColor = 0;

                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);
                    canvasUser.Children.Clear();
                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                Joint headJoint = body.Joints[JointType.Head];
                                CameraSpacePoint cameraPoint = headJoint.Position;

                                Point point = new Point();

                                if (headJoint.TrackingState == TrackingState.Tracked)
	                            {
                                    if (_mode==Mode.Color)
                                    {
                                        ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(cameraPoint);

                                        point.X = float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X;
                                        point.Y = float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                                    }

                                    TextBlock userNumber = new TextBlock();
                                    userNumber.Text = String.Format("{0}", _bodies.IndexOf(body) + 1);
                                    userNumber.Foreground = new SolidColorBrush(Color.FromArgb(255, BodyColor[counterColor], BodyColor[counterColor + 1], BodyColor[counterColor + 2]));
                                    userNumber.FontSize = 24;
                                    Canvas.SetLeft(userNumber, point.X/2 - 10);
                                    Canvas.SetTop(userNumber, point.Y/2 - 70);
                                    canvasUser.Children.Add(userNumber);
                                    counterColor +=1;
                                    if (counterColor > 9)
	                                {
		                                counterColor = 0;
	                                }
	                            }
                            }
                        }
                    }

                    if (controlCenterBodyNumberToTrack != null)
                    {
                        ArmTracked armTracked = new ArmTracked();

                        //Get or update the value of the tracked arm
                        armTracked.updateValues(_bodies.ElementAt(Convert.ToInt32(controlCenterBodyNumberToTrack) - 1), controlCenterTrackingMode, controlCenterSideMode, controlCenterSide);

                        //Update the information windows
                        informationWindow.updateArmTracked(armTracked);
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

        public string getControlCenterMode()
        {
            return controlCenterSideMode;
        }

        public string getControlCenterSide()
        {
            return controlCenterSide;
        }
    }

    public enum Mode
    {
        Color,
        Depth,
        Infrared
    }
}
