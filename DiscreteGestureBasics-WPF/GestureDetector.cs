namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using PowerUSB;
    using System;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices;
    using System.Media;

    /// <summary>
    /// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
    /// and updates the associated GestureResultView object with the latest results for the 'Seated' gesture
    /// </summary>
    public class GestureDetector : IDisposable
    {

        /// <summary> Path to the gesture database that was trained with VGB </summary>
        private readonly string gestureDatabase = @"Database\SmartHome.gbd";
        /// <summary> the discrete gesture in the database that we want to track </summary>
        string[,] GestureNames = new string[5,10];
        Boolean[,] GestureDetected = new Boolean[5,10];
        float[,] GestureConfidence = new float[5,10];

       

        public string currentgesture;
        public Boolean currentDetected = false;
        public float currentConfidence = 0;

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureResultView">GestureResultView object to store gesture results of a single body to</param>
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("gestureResultView");
            }
            
            this.GestureResultView = gestureResultView;
            
            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }
            //BASE CASE
            GestureNames[0,0] = "HandShoulder_Right";
            GestureNames[0,1] = "HandUpClosed_Right";
            GestureNames[0,2] = "HandUpOpen_Right";
            GestureNames[0,3] = "HandEar_Right";
            GestureNames[0,4] = "FistsTogether";
            GestureNames[0,5] = "ArmDown_Left";

            //CLIMATE
            GestureNames[1,0] = "ArmDown_Right";
            GestureNames[1,1] = "ArmUp_Right";
            GestureNames[1,2] = "HandFrontOpen_Right";
            GestureNames[1,3] = "HandFrontClosed_Right";
            GestureNames[1,4] = "HandUpOpen_Right";
            GestureNames[1,5] = "HandUpClosed_Right";
            GestureNames[1,6] = "HandOnHead_Right";
            GestureNames[1, 7] = "ArmIn_Right";
            GestureNames[1, 8] = "ArmOut_Right";
            GestureNames[1, 9] = "ArmDown_Left";

            //MUSIC
            GestureNames[2, 0] = "ArmDown_Right";
            GestureNames[2, 1] = "ArmUp_Right";
            GestureNames[2, 2] = "HandFrontOpen_Right";
            GestureNames[2, 3] = "HandFrontClosed_Right";
            GestureNames[2, 4] = "HandUpOpen_Right";
            GestureNames[2, 5] = "HandUpClosed_Right";
            GestureNames[2, 6] = "HandOnHead_Right";
            GestureNames[2, 7] = "ArmIn_Right";
            GestureNames[2, 8] = "ArmOut_Right";
            GestureNames[2, 9] = "ArmDown_Left";

            //TELEVISION
            GestureNames[3, 0] = "ArmDown_Right";
            GestureNames[3, 1] = "ArmUp_Right";
            GestureNames[3, 2] = "HandFrontOpen_Right";
            GestureNames[3, 3] = "HandFrontClosed_Right";
            GestureNames[3, 4] = "HandUpOpen_Right";
            GestureNames[3, 5] = "HandUpClosed_Right";
            GestureNames[3, 6] = "HandOnHead_Right";
            GestureNames[3, 7] = "ArmIn_Right";
            GestureNames[3, 8] = "ArmOut_Right";
            GestureNames[3, 9] = "ArmDown_Left";

            GestureNames[4, 0] = "ArmUp_Left";
            GestureNames[4, 1] = "ArmDown_Left";

            


            // load the gestures from the gesture database. Can also load individual gestures as necessary. However for us, it isn't.
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.gestureDatabase))
            {
                vgbFrameSource.AddGestures(database.AvailableGestures);
            }
        }

        /// <summary> Gets the GestureResultView object which stores the detector results for display in the UI </summary>
        public GestureResultView GestureResultView { get; private set; }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
           
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }

                if (this.vgbFrameSource != null)
                {
                    this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this.vgbFrameSource.Dispose();
                    this.vgbFrameSource = null;
                }
            }
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;

            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
                    if (discreteResults != null)
                    {
                        Array.Clear(GestureConfidence, 0, GestureConfidence.Length);
                        Array.Clear(GestureDetected, 0, GestureDetected.Length);
                        
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {
                            currentConfidence = 0;
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);
                            int i = 0;
                            int index=0;
                            int state = MainWindow.state;
                            
                            for (i = 0; i < 10; i++)
                            {
                                if (gesture.Name.Equals(GestureNames[state, i]) && gesture.GestureType == GestureType.Discrete)
                                {
                                    GestureDetected[state, i] = result.Detected;
                                    GestureConfidence[state, i] = result.Confidence;
                                }
                            }

                            for (i = 0; i < 10;i++)
                            {
                                if(GestureConfidence[state,i] > currentConfidence)
                                {
                                    currentConfidence = GestureConfidence[state, i];
                                    index = i;
                                }
                            }
                            currentDetected = GestureDetected[state, index];
                            currentgesture = GestureNames[state, index];
                            
                            if (result != null)
                            {
                                this.GestureResultView.UpdateGestureResult(true, currentgesture, currentDetected, currentConfidence);
                            }
                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            // update the GestureResultView object to show the 'Not Tracked' image in the UI
            this.GestureResultView.UpdateGestureResult(false,"",false,0);
        }
    }
}
