using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
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
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;


namespace KinectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Mode for the Kinect camera, only the color one is used
        /// </summary>
        Mode _mode = Mode.Color;

        /// <summary>
        /// Instabce of the sensor
        /// </summary>
        KinectSensor _sensor;

        /// <summary>
        /// Frames of the sensor
        /// </summary>
        MultiSourceFrameReader _reader;

        /// <summary>
        /// Stream for 32b-16b conversion.
        /// </summary>
        KinectAudioStream convertStream = null;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine = null;

        /// <summary>
        /// List of all UI span elements used to select recognized text.
        /// </summary>
        private List<Span> recognitionSpans;

        /// <summary>
        /// List of bodies
        /// </summary>
        IList<Body> _bodies;

        //Control Center variables
        /// <summary>
        /// mode of the detection side (Auot or Manual)
        /// </summary>
        string controlCenterSideMode;

        /// <summary>
        /// Side to track (Right or Left)
        /// </summary>
        string controlCenterSide;

        /// <summary>
        /// Tracking mode (Mimicking or MovingObject)
        /// </summary>
        string controlCenterTrackingMode;

        /// <summary>
        /// Number of the user to track
        /// </summary>
        string controlCenterBodyNumberToTrack;

        string controlCenterVoiceOrder;

        /// <summary>
        /// Array of values for the color of the number of the user
        /// </summary>
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

        int[] bodyIDTracked = new int[7];

        private bool keywordOK = false;






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


                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = this._sensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);

                RecognizerInfo ri = TryGetKinectRecognizer();

                if (null != ri)
                {
                    this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                     var keyword = new Choices();
                     keyword.Add(new SemanticResultValue("robot", "ROBOT"));

                     var order = new Choices();
                     order.Add(new SemanticResultKey("go home", "HOME"));
                     order.Add(new SemanticResultKey("rest", "HOME"));
                     order.Add(new SemanticResultKey("home", "HOME"));
                
                     var gb = new GrammarBuilder { Culture = ri.Culture };
                     gb.Append("robot");
                     gb.Append(order);
                     gb.Append("please");
                
                     var g = new Grammar(gb);

                     speechEngine.LoadGrammar(g);

                    // Create a grammar from grammar definition XML file.
                    /*using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                    {
                        var g = new Grammar(memoryStream);
                        this.speechEngine.LoadGrammar(g);
                    }*/

                    this.speechEngine.SpeechRecognized += this.SpeechRecognized;
                    this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

                    // let the convertStream know speech is going active
                    this.convertStream.SpeechActive = true;

                    // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                    // This will prevent recognition accuracy from degrading over time.
                    speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                    this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                    this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);

                }
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

            //OperationCanceledExceptions on bodies
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
                                    //Dysplay the number of the body detected on the image
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

                    var counterBodyID = 0;

                    foreach (var body in _bodies)
                    {
                        if (body.IsTracked == true)
                        {   
                            bodyIDTracked[counterBodyID] = _bodies.IndexOf(body)+1;
                            counterBodyID++;
                            bodyIDTracked[6] = 1;
                        }
                    }

                    //Launch the computation about the arm if the user as specified a user to track
                    if (bodyIDTracked[6] == 1)
                    {
                        sideText.Text = String.Format("Select between the user numbers ");
                        var counterBodyIDLoop = 0;
                        while (bodyIDTracked[counterBodyIDLoop] != 0)
	                    {
                            sideText.Text += String.Format("{0} ", bodyIDTracked[counterBodyIDLoop]);
                            counterBodyIDLoop++;
	                    }
                    }

                    if (controlCenterBodyNumberToTrack != null && controlCenterBodyNumberToTrack != "")
                    {
                        if (Convert.ToInt32(controlCenterBodyNumberToTrack) <= _bodies.Count)
                        {
                            if (_bodies.ElementAt(Convert.ToInt32(controlCenterBodyNumberToTrack) - 1).IsTracked != false)
                            {
                                ArmTracked armTracked = new ArmTracked();

                                //Get or update the value of the tracked arm
                                armTracked.updateValues(_bodies.ElementAt(Convert.ToInt32(controlCenterBodyNumberToTrack) - 1), controlCenterTrackingMode, controlCenterSideMode, controlCenterSide);

                                //Update the information windows
                                updateArmTracked(armTracked);

                                //Update voice command textblock
                                if (controlCenterVoiceOrder != "")
                                {
                                    voiceCommandText.Text = String.Format("Voice command detected, order is : {0}", controlCenterVoiceOrder);
                                }
                            }
                            else
                            {
                                sideText.Text = String.Format("The number doesn't correspond to anyone");
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

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.5;


            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Words[1].Text)
                {
                    case "HOME":
		                controlCenterVoiceOrder = "Home";
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
        }







        //Control center interface functions
        /// <summary>
        /// Manage the radio mimicking button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioMimicking_Checked(object sender, RoutedEventArgs e)
        {
            controlCenterTrackingMode = "Mimicking";
            controlCenterSideMode = "";
            controlCenterSide = "";
            textSideModeSelection.Visibility = System.Windows.Visibility.Visible;
            radioAuto.Visibility = System.Windows.Visibility.Visible;
            radioAuto.IsChecked = false;
            radioManual.Visibility = System.Windows.Visibility.Visible;
            radioManual.IsChecked = false;
            textSideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Manage the radio moving object button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioMovingObject_Checked(object sender, RoutedEventArgs e)
        {
            controlCenterTrackingMode = "MovingObject";
            controlCenterSideMode = "Auto";
            controlCenterSide = "";
            textSideModeSelection.Visibility = System.Windows.Visibility.Visible;
            radioAuto.Visibility = System.Windows.Visibility.Visible;
            radioAuto.IsChecked = true;
            radioManual.Visibility = System.Windows.Visibility.Hidden;
            radioManual.IsChecked = false;
            textSideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Manage the radio auto button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioAuto_Checked(object sender, RoutedEventArgs e)
        {
            controlCenterSideMode = "Auto";
            controlCenterSide = "";
            textSideSelection.Visibility = System.Windows.Visibility.Hidden;
            radioRight.Visibility = System.Windows.Visibility.Hidden;
            radioRight.IsChecked = false;
            radioLeft.Visibility = System.Windows.Visibility.Hidden;
            radioLeft.IsChecked = false;
        }

        /// <summary>
        /// Manage the radio manual button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioManual_Checked(object sender, RoutedEventArgs e)
        {
            controlCenterSideMode = "Manual";
            controlCenterSide = "";
            textSideSelection.Visibility = System.Windows.Visibility.Visible;
            radioRight.Visibility = System.Windows.Visibility.Visible;
            radioRight.IsChecked = false;
            radioLeft.Visibility = System.Windows.Visibility.Visible;
            radioLeft.IsChecked = false;
        }

        /// <summary>
        /// Manage the radio right button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioRight_Checked(object sender, RoutedEventArgs e)
        {
            controlCenterSide = "Right";
        }

        /// <summary>
        /// Manage the radio left button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioLeft_Checked(object sender, RoutedEventArgs e)
        {
            controlCenterSide = "Left";
        }

        /// <summary>
        /// Manage the number input textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            controlCenterBodyNumberToTrack = bodyNumberToTrack.Text;
        }


        //Information panel function
        /// <summary>
        /// Update the textblock zith the informations of the tracked user
        /// </summary>
        /// <param name="armTracked"></param>
        public void updateArmTracked(ArmTracked armTracked)
        {
            if (controlCenterTrackingMode == "Mimicking")
            {
                if (controlCenterSideMode == "Auto")
                {
                    sideText.Text = String.Format("The {0} side is tracked.", armTracked.getSide());
                    coordinatesText.Text = String.Format("Coordinates of the differentes joint tracked : \nHand : X={0} Y={1} Z={2} \nWrist : X={3} Y={4} Z={5} \nElbow : X={6} Y={7} Z={8} \nShoulder : X={9} Y={10} Z={11} \nAngle Torso/Arm : {12} \nAngle Arm/Front Arm : {13}", armTracked.getHand().Position.X, armTracked.getHand().Position.Y, armTracked.getHand().Position.Z, armTracked.getWrist().Position.X, armTracked.getWrist().Position.Y, armTracked.getWrist().Position.Z, armTracked.getElbow().Position.X, armTracked.getElbow().Position.Y, armTracked.getElbow().Position.Z, armTracked.getShoulder().Position.X, armTracked.getShoulder().Position.Y, armTracked.getShoulder().Position.Z, armTracked.getAngleTSE(), armTracked.getAngleSEEW());
                    hand_StateText.Text = String.Format("The hand is {0}", armTracked.getHandState().ToString());
                }
                else if (controlCenterSideMode == "Manual")
                {
                    if (controlCenterSide == "")
                    {
                        sideText.Text = "Select a side";
                        coordinatesText.Text = "";
                        hand_StateText.Text = "";
                    }
                    if (controlCenterSide == "Right" || controlCenterSide == "Left")
                    {
                        sideText.Text = String.Format("The {0} side is tracked.", armTracked.getSide());
                        coordinatesText.Text = String.Format("Coordinates of the differentes joint tracked : \nHand : X={0} Y={1} Z={2} \nWrist : X={3} Y={4} Z={5} \nElbow : X={6} Y={7} Z={8} \nShoulder : X={9} Y={10} Z={11} \nAngle Torso/Arm : {12} \nAngle Arm/Front Arm : {13}", armTracked.getHand().Position.X, armTracked.getHand().Position.Y, armTracked.getHand().Position.Z, armTracked.getWrist().Position.X, armTracked.getWrist().Position.Y, armTracked.getWrist().Position.Z, armTracked.getElbow().Position.X, armTracked.getElbow().Position.Y, armTracked.getElbow().Position.Z, armTracked.getShoulder().Position.X, armTracked.getShoulder().Position.Y, armTracked.getShoulder().Position.Z, armTracked.getAngleTSE(), armTracked.getAngleSEEW());
                        hand_StateText.Text = String.Format("The hand is {0}", armTracked.getHandState().ToString());
                    }
                }
                else
                {
                    sideText.Text = "Select a mode";
                    coordinatesText.Text = "";
                    hand_StateText.Text = "";
                }
            }
            else if (controlCenterTrackingMode == "MovingObject")
            {
                if (controlCenterSideMode == "Auto")
                {
                    sideText.Text = String.Format("The {0} side is tracked.", armTracked.getSide());
                    coordinatesText.Text = String.Format("Coordinates of the differentes joint tracked : \n Hand : X={0} Y={1} Z={2}", armTracked.getHand().Position.X, armTracked.getHand().Position.Y, armTracked.getHand().Position.Z);
                    hand_StateText.Text = String.Format("The hand is {0}", armTracked.getHandState().ToString());
                }
                else
                {
                    sideText.Text = "Select a mode";
                    coordinatesText.Text = "";
                    hand_StateText.Text = "";
                }
            }
            else
            {
                sideText.Text = "Select a tracking mode";
                coordinatesText.Text = "";
                hand_StateText.Text = "";
            }
        }
    }

    public enum Mode
    {
        Color
    }
}
