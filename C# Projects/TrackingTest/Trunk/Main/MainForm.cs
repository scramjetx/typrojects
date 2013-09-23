//TO DO:
//ideas for image processing to play with
//1. video of a clock or pictures and be able to tell what time it is
//2. extract blobs of letters from license plate and template match the letters to identify what letters they are
//3. make a servo track a postition on the web cam of an object tracked
//4. implement click on an object to track like template matching.  or when you click highlight the template to track.
//5. video of car moving like UAV and see if can do tracking algorithm
//6. hold up hand with number of fingers and it will tell you what number
//7. be able to tell if eyes are open or closed in image/video
//7a. track eyes or face and place funny template over face that moves with the head. Eyes, nose, mouth or face.
//8. ping pong sideview game track and trace the path of the ball and keep score
//9. heartbeat meter with video from phone
//10. play zumas revenge flash game autonomously like jeff did with another game.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using AForge; //gives IntPoints etc...
using AForge.Controls;
using AForge.Fuzzy;
using AForge.Genetic;
using AForge.Imaging;
using AForge.Imaging.ComplexFilters;
using AForge.Imaging.Formats;
using AForge.Imaging.Filters;
using AForge.Imaging.IPPrototyper;
using AForge.MachineLearning;
using AForge.Math;
using AForge.Math.Geometry;
using AForge.Neuro;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Vision;

using MyAForgeToolbox;

using Microsoft.Research.DynamicDataDisplay;

namespace TrackingTest
{
    public partial class MainForm : Form
    {
        Microsoft.Research.DynamicDataDisplay.LineGraph my = new Microsoft.Research.DynamicDataDisplay.LineGraph();

        Bitmap rawFrame;
        Bitmap templateImage = new Bitmap("D:\\C# Projects\\Media\\markercap.jpg");

        static string appRootDir = Directory.GetCurrentDirectory();
        static string logFileDir = "D:\\C# Projects\\TrackingTest\\Trunk\\DataLogs\\";
        static string logFileName = "logfile.csv";
        private StreamWriter DataLog;

        static string videoFileName = "D:\\C# Projects\\Media\\heartbeat2.avi";

        int frameCount = 0;

        MyAForgeToolbox.GreenObjectTracking myGreenObjectTracking = new MyAForgeToolbox.GreenObjectTracking();
        MyAForgeToolbox.MyTemplateMatching myTemplateMatching = new MyAForgeToolbox.MyTemplateMatching();
        MyAForgeToolbox.HeartBeatSensor myHeartBeatSensor = new MyAForgeToolbox.HeartBeatSensor();

        int updateGraphCounter = 0; //used for resetting min and scaler for graphing etc..
        

        public MainForm()
        {
          
            InitializeComponent();
            tabControl1.SelectTab(0);
            this.ActiveControl = buttonPlayVideo;
            //radioButtonVariance.Checked = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer, true);
            

           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
            }
        }

        private void buttonPlayLiveStream_Click(object sender, EventArgs e)
        {
            timerHeartBeatSensor.Enabled = true;

            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm();

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // create video source
                VideoCaptureDevice videoSource = new VideoCaptureDevice(form.VideoDevice);

                // open it
                OpenVideoSource(videoSource);

                buttonStopStream.Focus();
            }
        }

        private void buttonPlayVideo_Click(object sender, EventArgs e)
        {
            timerHeartBeatSensor.Enabled = true;

            //over writes old pixel count log
            //logValue(pixelCountFFTLogFileName, false, 0);
            //logValue(varianceCountLogFileName, false, 0);

            // create video source
            FileVideoSource fileSource = new FileVideoSource(videoFileName);
            
            // open it
            OpenVideoSource(fileSource);

            //erase previous datalog, double value not actually written to file
            logValue(logFileName, false, 0.999, 0.999);

        }

        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            CloseCurrentVideoSource();

            // stop current video source
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();
            //videoSourcePlayer.Visible = false;

            // reset statistics
            //statIndex = statReady = 0;

            // start timer
            //timer.Start();

            this.Cursor = Cursors.Default;
           
            
        }

        // Close video source if it is running
        private void CloseCurrentVideoSource()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();

                // wait ~ 3 seconds
                for (int i = 0; i < 30; i++)
                {
                    if (!videoSourcePlayer.IsRunning)
                        break;
                    System.Threading.Thread.Sleep(100);
                }

                if (videoSourcePlayer.IsRunning)
                {
                    videoSourcePlayer.Stop();
                }

                videoSourcePlayer.VideoSource = null;
            }
        }

        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {   
            //only every x frame because computationally intensive to do every frame
           // if(frameCount%1 == 0)
            //{
            rawFrame = (Bitmap)image.Clone();

            myHeartBeatSensor.DoHeartBeatSensing(rawFrame);
            //pictureBoxProcessedVideo.Image = myHeartBeatSensor.RawFrame;
            pictureBoxWorkingImage.Image = myHeartBeatSensor.WorkingImage;
            //logValue(logFileName, true, myHeartBeatSensor.ImageNormVar);

            //myGreenObjectTracking.doGreenObjectTracking(rawFrame);
            //pictureBoxProcessedVideo.Image = myGreenObjectTracking.RawFrame;
            //pictureBoxWorkingImage.Image = myGreenObjectTracking.WorkingImage;
            
            
            //if (frameCount % 20 == 0)
            //{
            //    myTemplateMatching.doTemplateMatching(rawFrame, templateImage);
            //    pictureBoxProcessedVideo.Image = myTemplateMatching.RawFrame;
            //    //pictureBoxWorkingImage.Image = templateImage;
            //}


            //Stopwatch sw = Stopwatch.StartNew();

            //sw.Stop();
            //Console.WriteLine("Total time (ms): {0}", (long)sw.ElapsedMilliseconds);
           
            // }

            frameCount++;
        }

        private void timerUpdateUI_Tick(object sender, EventArgs e)
        {
            //don't run event in form designer
            if (this.DesignMode) return;

            //labelStats.Text = myHeartBeatSensor.NormVar.ToString();

            
        }

        private void logValue(string filename, Boolean append, double value, double value2)
        {
            DataLog = new StreamWriter(logFileDir + filename, append);

            //if append is true then just overwrite the file in preparation of new file log
            if (append != false)
                DataLog.WriteLine(value.ToString() + "," + value2.ToString());
            else
                DataLog.Write("");

            DataLog.Close();
        }

        private void buttonOpenResults_Click(object sender, EventArgs e)
        {
            //don't run event in form designer
            if (this.DesignMode) return;

            buttonStopStream.PerformClick();

            //open the log file and the image viewer
            System.Diagnostics.Process.Start(logFileDir);
            //System.Diagnostics.Process.Start(appRootDir + pixelCountFFTLogFileName);
            //System.Diagnostics.Process.Start(appRootDir + varianceCountLogFileName);
            //System.Diagnostics.Process.Start(appRootDir + "\\sharpestFFTImage.jpg");
        }

        private void buttonStopStream_Click(object sender, EventArgs e)
        {

            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
            }

            buttonOpenResults.Focus();
            timerHeartBeatSensor.Enabled = false;
            myLineGraph.Clear();
        }

        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }

        private void timerHeartBeatSensor_Tick(object sender, EventArgs e)
        {
            //don't run event in form designer
            if (this.DesignMode) return;

            float v = (float)myHeartBeatSensor.ImageMeanSquared;
            float v2 = (float)myHeartBeatSensor.MeanPastReadings;
            int time = 1000;
            double min = 0;
            double max = 0;
            double diff = 0;
            myHeartBeatSensor.KeepPastReading(v);


            #region Graph Manipulation
            //scale graph according to max and min of past reading window
            if (updateGraphCounter >= myHeartBeatSensor.getNUM_PAST_READINGS())
            {
                int yMax = (int)(myHeartBeatSensor.MaxPastReadings * 1.03);
                int yMin = (int)(myHeartBeatSensor.MinPastReadings * .97);
                myLineGraph.setMinAndMaxY(yMin, yMax);
                
                //reset counter that triggers scaling update based on stored values
                updateGraphCounter = 0;
            }


            //increase range of graph if current graphed value goes above or below the range.
            if (v > myLineGraph.getMaxY())
            {
                int newmax = (int)(v * 1.03);
                myLineGraph.setMinAndMaxY(myLineGraph.getMinY(), newmax);
                //Console.WriteLine("changed max: " + newmax.ToString() );
            }

            //increase range of graph if current graphed value goes above or below the range.
            if (v < myLineGraph.getMinY())
            {
                int newmin = (int)(v * .97);
                myLineGraph.setMinAndMaxY(newmin, myLineGraph.getMaxY());
                //Console.WriteLine("changed min: " + newmin.ToString());
            }

            #endregion


            #region Heartbeat Count cross Average Method
            ////check if current value crossed average down or up to time heartbeat
            //myHeartBeatSensor.CheckAverageCrossing(v, updateGraphCounter);

            //if (myHeartBeatSensor.HeartBeatFlag == true)
            //{
            //    //light the led
            //    ledHeartBeat.ColorOn = Color.Green;
            //    Console.WriteLine("HEARTBEAT");
            //}
            //else
            //    ledHeartBeat.ColorOn = Color.Red;
            #endregion


            //Derivative method for heartbeat count
            v2 = (float)myHeartBeatSensor.CalcDerivative(v, updateGraphCounter);
            double v2Scaled = v2 + myHeartBeatSensor.MeanPastReadings;
            int curHeartRate = 0;

            //calc derivative and step through heart beat state machine.  If it returns true then get the calculated heart rate
            if (myHeartBeatSensor.DoDerivativeHeartRateMethod())
            {
                curHeartRate = (int)myHeartBeatSensor.GetHeartRate();

                if (curHeartRate > 0)
                {
                    labelHeartRate.Text = curHeartRate.ToString();
                    labelHeartRate.ForeColor = Color.Green;
                }
                else   //paint red to show heart rate isn't current anymore
                    labelHeartRate.ForeColor = Color.Red;
            }

            //toggle LED
            if (myHeartBeatSensor.GetHeartBeatState() == 1)
                ledHeartBeat.ColorOn = Color.Green;
            else
                ledHeartBeat.ColorOn = Color.Red;

            //Console.WriteLine(myHeartBeatSensor.GetHeartBeatState().ToString());

            v2Scaled = 0;
            myLineGraph.Add(v, (float)v2Scaled, time.ToString());
            labelStats.Text = v.ToString();

            logValue(logFileName, true, v, (float)v2);

            //tick
            updateGraphCounter++;
            
        }

      
       

    }
}
