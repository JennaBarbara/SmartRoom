//---------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <Description>
// This program tracks up to 6 people simultaneously.
// If a person is tracked, the associated gesture detector will determine if that person is seated or not.
// If any of the 6 positions are not in use, the corresponding gesture detector(s) will be paused
// and the 'Not Tracked' image will be displayed in the UI.
// </Description>
//----------------------------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System.Media;
    using PowerUSB;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interaction logic for the MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary> Active Kinect sensor </summary>
        private KinectSensor kinectSensor = null;
        
        /// <summary> Array for the bodies (Kinect will track up to 6 people simultaneously) </summary>
        private Body[] bodies = null;

        /// <summary> Reader for body frames </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary> Current status text to display </summary>
        private string statusText = null;

        /// <summary> KinectBodyView object which handles drawing the Kinect bodies to a View box in the UI </summary>
        private KinectBodyView kinectBodyView = null;
        
        /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
        private List<GestureDetector> gestureDetectorList = null;

        private float currentConfidence;
        private Boolean currentDetected;
        private string currentName;

        public static int state = 0;
        public int pwrS1 = 0;
        public int pwrS2 = 0;
        public int pwrS3 = 0;
        string[] SongLocations = new string[9];
        string[] VideoLocations = new string[6];
        int Song = 0;
        int SongPlaying = 0;
        int Video = 0;
        int VideoPlaying = 0;
        int Reading = 1;
        private DateTime time = new DateTime();
        private DateTime prevTime = new DateTime();
        MediaWindow window2 = new MediaWindow();
        MediaWindow Music = new MediaWindow();
        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {

            USBControl.init();
            
            SongLocations[0] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/3005.wav";
            SongLocations[1] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/Brainstorm.wav";
            SongLocations[2] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/DoIWannaKnow.wav";
            SongLocations[3] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/GetLucky.wav";
            SongLocations[4] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/High.wav";
            SongLocations[5] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/LonelyBoy.wav";
            SongLocations[6] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/Riptide.wav";
            SongLocations[7] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/SnapOut.wav";
            SongLocations[8] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Music/Suburbs.wav";

            VideoLocations[0] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Videos/BustyBass.mp4";
            VideoLocations[1] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Videos/Habs.mp4";
            VideoLocations[2] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Videos/Kramer.mp4";
            VideoLocations[3] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Videos/StarWars.mp4";
            VideoLocations[4] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Videos/TheOffice.mp4";
            VideoLocations[5] = "C:/Users/Evan/Documents/Github/SmartRoom/DiscreteGestureBasics-WPF/Database/Videos/Seinfeld.mp4";

            // only one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();
            
            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // set the BodyFramedArrived event notifier
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // initialize the BodyViewer object for displaying tracked bodies in the UI
            this.kinectBodyView = new KinectBodyView(this.kinectSensor);

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();

            // initialize the MainWindow
            this.InitializeComponent();
            
            // set our data context objects for display in UI
            this.DataContext = this;
            this.kinectBodyViewbox.DataContext = this.kinectBodyView;
            window2.Show();
            // create a gesture detector for each body (6 bodies => 6 detectors) and create content controls to display results in the UI
            int col0Row = 0;
            int col1Row = 0;
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                GestureResultView result = new GestureResultView("",i, false, false, 0.0f);
                GestureDetector detector = new GestureDetector(this.kinectSensor, result);
                this.gestureDetectorList.Add(detector);
                
                // split gesture results across the first two columns of the content grid
                ContentControl contentControl = new ContentControl();
                contentControl.Content = this.gestureDetectorList[i].GestureResultView;
                
                if (i % 2 == 0)
                {
                    // Gesture results for bodies: 0, 2, 4
                    Grid.SetColumn(contentControl, 0);
                    Grid.SetRow(contentControl, col0Row);
                    ++col0Row;
                }
                else
                {
                    // Gesture results for bodies: 1, 3, 5
                    Grid.SetColumn(contentControl, 1);
                    Grid.SetRow(contentControl, col1Row);
                    ++col1Row;
                }

                this.contentGrid.Children.Add(contentControl);
            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.FrameArrived -= this.Reader_BodyFrameArrived;
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.gestureDetectorList != null)
            {
                // The GestureDetector contains disposable members (VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader)
                foreach (GestureDetector detector in this.gestureDetectorList)
                {
                    detector.Dispose();
                }

                this.gestureDetectorList.Clear();
                this.gestureDetectorList = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.IsAvailableChanged -= this.Sensor_IsAvailableChanged;
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the event when the sensor becomes unavailable (e.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // visualize the new body data
                this.kinectBodyView.UpdateBodyFrame(this.bodies);
                int gestureFound = 0;
                
                // we may have lost/acquired bodies, so update the corresponding gesture detectors
                if (this.bodies != null)
                {
                    // loop through all bodies to see if any of the gesture detectors need to be updated
                    int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        time = DateTime.Now;
                        Body body = this.bodies[i];
                        ulong trackingId = body.TrackingId;

                        // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != this.gestureDetectorList[i].TrackingId)
                        {
                            this.gestureDetectorList[i].TrackingId = trackingId;

                            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            this.gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                        if (gestureFound == 0 && this.gestureDetectorList[i].currentConfidence > 0.3)
                        {
                            currentConfidence = this.gestureDetectorList[i].currentConfidence;
                            currentDetected = this.gestureDetectorList[i].currentDetected;
                            currentName = this.gestureDetectorList[i].currentgesture;
                            gestureFound = 1;
                        }
                        TimeSpan seconds = time - prevTime;
                        double elapsed = seconds.TotalSeconds;
                        CurrentState.Content = state.ToString();
                        if (elapsed > 1)
                            GestureTime.Content = "Ready";
                        else
                            GestureTime.Content = elapsed.ToString();
                        if (currentConfidence != 0 && elapsed > 0.5)
                            {
                                prevTime = DateTime.Now;                               
                                if (state == 0)
                                {
                                    if (currentName == "HandShoulder_Right")
                                    {
                                        state = 1;
                                    }
                                    else if (currentName == "HandEar_Right")
                                    {
                                        state = 2;
                                    }
                                    else if (currentName == "FistsTogether")
                                    {
                                        state = 3;
                                    }
                                    else if (currentName == "HandUpClosed_Right" && pwrS1 == 1)
                                    {
                                        //turn light off
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 0;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                       
                                    }
                                    else if (currentName == "HandUpOpen_Right" && pwrS1 == 0)
                                    {
                                        //turn light on
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 1;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "ArmDown_Left" && Reading == 1)
                                    {
                                        state = 4;
                                        Reading = 0;
                                    }
                                }
                                else if (state == 1)  //Climate 
                                {

                                    if (currentName == "ArmDown_Right")  // Temp Down
                                    { }
                                    else if (currentName == "ArmUp_Right") // Temp Up
                                    {

                                    }
                                    else if (currentName == "HandFrontOpen_Right" && pwrS2 == 0) // Fan On
                                    {
                                        //turn fan on
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS2 = 1;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);

                                    }
                                    else if (currentName == "HandFrontClosed_Right" && pwrS2 == 1) // Fan Off
                                    {
                                        //turn fan off
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS2 = 0;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "HandUpClosed_Right" && pwrS1 == 1)
                                    {
                                        //turn light off
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 0;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);

                                    }
                                    else if (currentName == "HandUpOpen_Right" && pwrS1 == 0)
                                    {
                                        //turn light on
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 1;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "ArmIn_Right" && pwrS3 == 1)
                                    {
                                        //Turn Light 2 (Plug 3) Off
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS3 = 0;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "ArmOut_Right" && pwrS3 == 0)
                                    {
                                        //Turn Light 2 (Plug 3) On
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS3 = 1;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, 1);
                                    }
                                    else if (currentName == "HandOnHead_Right")
                                    {
                                        state = 0;
                                    }
                                    else if (currentName == "ArmDown_Left" && Reading == 1)
                                    {
                                        state = 4;
                                        Reading = 0;
                                    }
                                }
                                else if (state == 2)
                                {

                                    if (currentName == "ArmDown_Right")
                                    {
                                        Music.SetVolume(-0.3);
                                    }
                                    else if (currentName == "ArmUp_Right")
                                    {
                                        Music.SetVolume(0.1);
                                    }
                                    else if (currentName == "HandFrontOpen_Right" && SongPlaying ==0)
                                    {
                                        Music.SetMedia(SongLocations[Song]);
                                        Music.PlayMedia();
                                        SongPlaying = 1;
                                    }
                                    else if (currentName == "HandFrontClosed_Right" && SongPlaying ==1)
                                    {
                                        Music.StopMedia();
                                        SongPlaying = 0;
                                    }
                                    else if (currentName == "HandUpClosed_Right" && pwrS1 == 1)
                                    {
                                        //turn light off
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 0;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);

                                    }
                                    else if (currentName == "HandUpOpen_Right" && pwrS1 == 0)
                                    {
                                        //turn light on
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 1;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "HandOnHead_Right")
                                    {
                                        state = 0;
                                    }
                                    else if (currentName == "ArmIn_Right" && SongPlaying ==1)
                                    {
                                        Song = Song + 1;
                                        if (Song > 8)
                                            Song = 0;
                                        Music.SetMedia(SongLocations[Song]);
                                        Music.PlayMedia();
                                    }
                                    else if (currentName == "ArmOut_Right" && SongPlaying ==1)
                                    {
                                        Song = Song - 1;
                                        if (Song < 0)
                                            Song = 8;
                                        Music.SetMedia(SongLocations[Song]);
                                        Music.PlayMedia();
                                    }
                                    else if (currentName == "ArmDown_Left" && Reading == 1)
                                    {
                                        state = 4;
                                        Reading = 0;
                                    }
                                }
                                else if (state == 3)
                                {
                                    if (currentName == "ArmDown_Right")
                                    {
                                        window2.SetVolume(-0.3);
                                    }
                                    else if (currentName == "ArmUp_Right")
                                    {
                                        window2.SetVolume(0.1);
                                    }
                                    else if (currentName == "HandFrontOpen_Right")
                                    {
                                        window2.SetMedia(VideoLocations[Video]);
                                        window2.PlayMedia();
                                    }
                                    else if (currentName == "HandFrontClosed_Right")
                                    {
                                        window2.StopMedia();   
                                    }
                                    else if (currentName == "HandUpClosed_Right" && pwrS1 == 1)
                                    {
                                        //turn light off
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 0;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "HandUpOpen_Right" && pwrS1 == 0)
                                    {
                                        //turn light on
                                        PwrUSBWrapper.ReadPortStatePowerUSB(out pwrS1, out pwrS2, out pwrS3);
                                        pwrS1 = 1;
                                        PwrUSBWrapper.SetPortPowerUSB(pwrS1, pwrS2, pwrS3);
                                    }
                                    else if (currentName == "HandOnHead_Right")
                                    {
                                        state = 0;
                                    }
                                    else if (currentName == "ArmIn_Right")
                                    {
                                        Video--;
                                        if (Video < 0)
                                            Video = 5;
                                        window2.SetMedia(VideoLocations[Video]);
                                        window2.PlayMedia();
                                    }
                                    else if (currentName == "ArmOut_Right")
                                    {
                                        Video++;
                                        if (Video > 5)
                                            Video = 0;
                                        window2.SetMedia(VideoLocations[Video]);
                                        window2.PlayMedia();
                                    }
                                    else if (currentName == "ArmDown_Left" && Reading == 1)
                                    {
                                        state = 4;
                                        Reading = 0;
                                    }
                                }
                                else if (state == 4)
                                {
                                    if(currentName == "ArmUp_Left" && Reading == 0)
                                    {
                                        state = 0;
                                        Reading = 1;
                                    }
                                }
                            }
                        currentConfidence = 0;
                        currentDetected = false;
                        currentName = "";
                    }
                }
            }
        }
    }
}
