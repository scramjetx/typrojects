//TO DO
//Make it so the scale doesn't change if its in an ok range so its not constantly clicking between max and mins. gets kind of annoying to see constantly
//should maybe try to push autoscale into the line graph project so don't do it outside by set a flag for realtime scaling enabled.  make function reusable.

//make heart beat LED stay on longer
//Maybe want to create a state machine that counts the heart beats once the pattern is stablized which could be checked in the past readings
    //filtering the past readings could tell you if stable heart beat then sets flag to start counting crossings. 
//then start putting in time stamps for the heart beats that are stored in an array. 
//then count time between stamps in the array and store as differences
//then find average of differences over amount of time sampled to project heartbeat.
//maybe look at time between beats real time and if within 10% for 10 sec then project heart rate from there.
//maybe a counter that counts up consecutive heart beats within 10% and onces hits 10-15 that's the heart rate.
    //then the counter decrements or cuts in half if miss a beat or resets etc..


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


namespace MyAForgeToolbox
{
    public class HeartBeatSensor
    {
        //variables
        public const int NUM_PAST_READINGS = 25;
        public const int NUM_TIMESTAMPS = 7;                //needs 7 timestamps to capture 6 beats because need two to establish first beat time
        public const int NUM_HEARTBEATS = 6;                //always 1 less than the timestamps
        public const double MAX_HEARTBEAT_STDEV = .065;     //acceptable std dev of the heart beats stored
        public Bitmap RawFrame;
        public Bitmap WorkingImage;
        public double ImageNormVar;
        public double ImageStdDev;
        public double ImageVariance;
        public double ImageMean;
        public double ImageMeanSquared;
        
        public double[] PastReadings = new double[NUM_PAST_READINGS];           //holds past image mean readings for scaling graph
        public int CounterPastReading = 0;
        public double MinPastReadings;
        public double MaxPastReadings;
        public double MeanPastReadings;
        public double SumPastReadings;
        public double StdDevPastReadings;
        
        public TimeSpan[] HeartbeatTimeStamps = new TimeSpan[NUM_TIMESTAMPS];   //holds times stamps of sensed heart beat
        public int CounterTimeStamp = 0;

        private int CounterAverageCross = 0;
        public bool HeartBeatFlag = false;

        private bool negCross = false;
        private bool posCross = false;

        private int HeartBeatState = 0;     //tracks state of heartbeat algorithm for derivative method

        private double Derivative = 0;      //derivative of mean graph at that instant
        private double HeartRate = 0;       //calculated heart rate
        //constructor
        public HeartBeatSensor()
        {



        }

        public int getNUM_PAST_READINGS()
        {
            return NUM_PAST_READINGS;
        }
        public void DoHeartBeatSensing(Bitmap b)
        {
            this.RawFrame = b;

            GrayscaleBT709 grayFilter = new GrayscaleBT709();
            
            this.WorkingImage = grayFilter.Apply(this.RawFrame);
            ImageStatistics IStats = new ImageStatistics(this.WorkingImage);

            this.ImageStdDev = IStats.Gray.StdDev;
            this.ImageVariance = this.ImageStdDev * this.ImageStdDev;
            this.ImageMean = IStats.Gray.Mean;
            this.ImageMeanSquared = this.ImageMean * this.ImageMean;

            this.ImageNormVar = this.ImageVariance / this.ImageMeanSquared;
            
            

        }

        public void KeepPastReading(double value)
        {
            if (this.CounterPastReading == NUM_PAST_READINGS)
            {
                this.CounterPastReading = 0;
                CalcStatsPastReading();
            }

            PastReadings[this.CounterPastReading] = value;

            this.CounterPastReading++;
        }

        public void CalcStatsPastReading()
        {
            this.MaxPastReadings = this.PastReadings.Max();
            this.MinPastReadings = this.PastReadings.Min();
            this.MeanPastReadings = ((this.MaxPastReadings - this.MinPastReadings) / 2) + this.MinPastReadings;

            double sumOfSquaresOfDifferences = this.PastReadings.Select(val => (val - this.MeanPastReadings) * (val - this.MeanPastReadings)).Sum();
            this.StdDevPastReadings = Math.Sqrt(sumOfSquaresOfDifferences / this.PastReadings.Length);

            //this.PrintStats();

        }

        public void PrintStats()
        {
            //print stats
            Console.WriteLine("Max: " + this.MaxPastReadings);
            Console.WriteLine("Min: " + this.MinPastReadings);
            Console.WriteLine("Avg: " + this.MeanPastReadings);
            Console.WriteLine("Stddev:" + this.StdDevPastReadings);
            Console.WriteLine();
        }

        public void CheckAverageCrossing(float value, int index)
        {
            //wraps around the subtraction by amount negative with modulus operator
            int i = (index + NUM_PAST_READINGS - 8) % NUM_PAST_READINGS;
            //HeartBeatFlag = false;

            

            //check if this value has crossed some threshold below or above the average
            //neg slope crossing
            if (value < MeanPastReadings && PastReadings[i] > MeanPastReadings && negCross != true)
            {
                Console.WriteLine("neg cross");
                negCross = true;

                //check for negative or positive slope
                if (CounterAverageCross == 2)
                {
                    //HeartBeatFlag = true;
                    CounterAverageCross = 0;
                    posCross = false;
                }
                else
                {
                    //start of heartbeat
                    posCross = false;
                }

                CounterAverageCross++;
            }
            //pos slope crossing
            else if (value > MeanPastReadings && PastReadings[i] < MeanPastReadings && posCross != true)
            {
                Console.WriteLine("pos cross");

                negCross = false;
                posCross = true;

                //need one of these before next negative slope to count as heartbeat
                CounterAverageCross++;
            }
            
        }

        public double CalcDerivative(float value, int index)
        {
            float dt = 3; //8 was too big.  didn't turn negative slope soon enough.  lagged chart too much
            double dv = 0;
            double vOld = 0;

            //wraps around the subtraction by amount negative with modulus operator
            int i = (index + NUM_PAST_READINGS - (int)dt) % NUM_PAST_READINGS;
            vOld = PastReadings[i];

            dv = (PastReadings[index] - PastReadings[i]);

            this.Derivative = dv / dt;

            return this.Derivative;
        }

        public bool DoDerivativeHeartRateMethod()
        {
            bool heartRateReadyToCalc = false;          //flag set to do heart rate calculation
            
            //heartbeat state machine
            //1: derivative <-75, take a timestamp
            //2: derivative > 0 
            //once 1 condition becomes true again that's one heart beat.  take the diff between current time and prev time
            //light LED during once it's in state1 then off once its in state 2

            if (this.Derivative < -75 && (HeartBeatState == 0 || HeartBeatState == 2))
            {
                HeartBeatState = 1;
                
                //take timestamp
                int j = (CounterTimeStamp + NUM_TIMESTAMPS) % NUM_TIMESTAMPS;
                HeartbeatTimeStamps[j] = DateTime.Now.TimeOfDay;
                Console.WriteLine(HeartbeatTimeStamps[j].ToString());
                CounterTimeStamp++;
                //Console.WriteLine(DateTime.Now.ToString("HH:MM ss.ffff "));

                //timestamp buffer full...set doProcessing flag
                if (j == 6)
                {
                    Console.WriteLine("buffer full...processing");
                    heartRateReadyToCalc = true;
                }
                    
            }

            //State 2
            if (this.Derivative > 0 && HeartBeatState == 1)
                HeartBeatState = 2;


            
            return heartRateReadyToCalc;
        }

        public double GetHeartRate()
        {
            
            

            //have 7 time stamps which makes 6 heart beats
            double[] diff = new double[NUM_HEARTBEATS];

            //calc difference between the beats and store
            for (int z = 0; z < NUM_HEARTBEATS; z++)
            {
                diff[z] = (float)(HeartbeatTimeStamps[z + 1].TotalSeconds - HeartbeatTimeStamps[z].TotalSeconds);

            }


            //calc std dev of the stored differences for reliable heart rate
            double stdev = getStandardDeviation(diff);
            Console.WriteLine(stdev.ToString());

            //if std dev acceptable calc the heart rate
            if (stdev < MAX_HEARTBEAT_STDEV)
            {
                double sumOfDiff = diff.Sum();

                //calc heart rate
                //total sample time / average heartbeat duration * scaler to beats per min
                this.HeartRate = sumOfDiff / (sumOfDiff / NUM_HEARTBEATS) * (60 / sumOfDiff);
                Console.WriteLine(this.HeartRate.ToString());
            }
            else
            {
                //return 0 to signal the std dev was too big to calc reliable heart rate
                return 0;
            }
                

            return this.HeartRate;
            

        }

        public int GetHeartBeatState()
        {
            return this.HeartBeatState;
        }

        public static double getStandardDeviation(double [] valueList)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double value in valueList)
            {
                double tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return Math.Sqrt(S / (k - 1));
        }


    }
}
