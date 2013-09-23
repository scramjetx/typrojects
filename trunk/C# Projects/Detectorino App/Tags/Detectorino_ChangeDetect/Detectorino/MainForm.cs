//need to sort out logging at some interval like once a minute
    //then the log file will be easy to parse graph and count time present
//clean up display.
    //show graphic of burrito in various states of cooked
    //show message about state of cookedness

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Library;
using System.IO.Ports;
using System.IO;

namespace Detectorino
{
    
    public partial class MainForm : Form
    {
        Gmail myGmail = new Gmail();
        int byteCounter = 0;                        //tracks how many bytes in current packer recieved.
        int detectorinoState = 99;                  //track IO high or low.  If nonZero for preset amount of time possibly not present.
        Boolean flagToggleLED = false;                //flag for timer to log data that the IO state changed 

        TimeSpan timeSinceMovement;                 //how long it's been since sensing a movement    
        DateTime startTime;                         //when started logging
        DateTime moveTime = DateTime.Now;           //time when last movement logged
        TimeSpan logTimeSpan;                       //how long been logging for
        TimeSpan countdownTimer;                         //timer that decrements if no inactivity from X amount of time. If hits zero then noone there.
        double countdownValue;                  //countdown value in minutes
        TimeSpan timePresent;                       //tracks amount of time present based on countdown>0
        int personPresent = 0;                      //bit to track if someone is present or not for logging.  Based on countdownTimer>0

        StreamWriter DataLog;
        string appRootDir;
        //"DataLog_" + DateTime.Now.Hour.ToString() + "hr_" + DateTime.Now.Minute.ToString() + "min.csv"; 
        string filename = "\\DetectorinoLog.csv"; 

        public MainForm()
        {
            InitializeComponent();

            buttonOpenPort.Select();
            timeSinceMovement = TimeSpan.FromSeconds(0);
            logTimeSpan.Equals(0);
            countdownTimer = TimeSpan.FromMinutes(countdownValue);
            
        }

        private void buttonSendEmail_Click(object sender, EventArgs e)
        {
            myGmail.SendEmail("detectorino@gmail.com", "12087570146@vzwpix.com", "moreland85", "testing", "Hi");
           
        }

        private void buttonOpenPort_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            buttonStart.Select();

        }

        private void buttonClosePort_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort spL = (SerialPort)sender;
            byte[] buf = new byte[spL.BytesToRead];
            spL.Read(buf, 0, buf.Length);

            //Console.WriteLine("DATA RECEIVED!");

            foreach (Byte b in buf)
            { 
                //Frame looks like:
                //(In Decimal) 126 0 18 146 0 19 162 0 64 137 32 189 61 62 1 1 0 2 0 0 0 147
                //(In Hex)      7E 0 12  92 0 13  A2 0 40  89 20  BD 3D 3E 1 1 0 2 0 0 2  91
                //Header (1), length(2), Frame data (4-n), Checksum (1+n)
                //byte index 3 means IO sample frame (0x92)
                //page 67 of manual explains data. 
                //2nd to last byte shows that DIO1 is toggling.  changes from 0 if low to 2 if high.  binary 00000010 and 000000000 
                //all we really care about is if we get a new packet which tells me the value changed and that's good enough.
                //then we can refresh the countdown timer

                if (b == 0x7E)  //or 126 decimal which is start of packet header byte for Xbee 
                {
                    //Console.WriteLine();
                    byteCounter = 0;
                }

                //converts dec value to hex
                string hexValue = b.ToString("X");

                //converts hex to decimal
                //int decValue = Convert.ToInt32(hexValue, 16);

                //this is the value interested in.  if 0 the DIO1 is low if =2 DIO1 is high
                if (byteCounter == 20)
                {
                    detectorinoState = b;
                    //Console.Write(hexValue + " ");
                    flagToggleLED = true;
                
                }
                

                //increment count
                byteCounter++;
            }


        }

        private void timerToggleLED_Tick(object sender, EventArgs e)
        {
            if (flagToggleLED)
            {
                //reset the countdown timer cause there was movement
                countdownTimer = TimeSpan.FromMinutes(countdownValue);

                //toggle the LED
                if (ledStatus.ColorOn != Color.Green)
                    ledStatus.ColorOn = Color.Green;
                else
                    ledStatus.ColorOn = Color.ForestGreen;

                //anytime the LED flickers reset time since last movement to now
                moveTime = DateTime.Now;

            }

            flagToggleLED = false;

        }

        private void timerLogData_Tick(object sender, EventArgs e)
        {

            //log time stamp, log duration from start in min, current countdown timer min and sec, sensor state as 1 or 0
            string curTime = DateTime.Now.ToString("HH:mm:ss");

            logTimeSpan = DateTime.Now - startTime;
            string logDuration = logTimeSpan.Hours.ToString() + ":" + logTimeSpan.Minutes.ToString() + ":" + logTimeSpan.Seconds.ToString();

            //Track if someone is present based on countdown timer for graphing
            if (countdownTimer.TotalSeconds > 0)
                personPresent = 1;
            else if (countdownTimer.TotalSeconds == 0)
                personPresent = 0;


            //write a line of data
            string lineOfData = curTime + 
                "," + logDuration + 
                "," + countdownTimer.ToString() +
                "," + timeSinceMovement.TotalMinutes.ToString() +
                "," + timePresent.TotalMinutes.ToString() +
                "," + personPresent.ToString();
            
            Console.WriteLine(lineOfData);
            logValue(appRootDir + filename, true, lineOfData);

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;

            //start countdown timer
            timerCountDown.Interval = 1000; // 1 second intervals
            timerCountDown.Enabled = true;
            timePresent = TimeSpan.FromSeconds(0);

            timerToggleLED.Enabled = true;

            timerLogData.Interval = 60000;  //1 minute logging interval = 60,000 milliseconds
            timerLogData.Enabled = true;

            countdownValue = 5.0;             //countdownvalue in minutes

            //get directory then back off a few folders to get out of the /bin/debug
            appRootDir = Directory.GetCurrentDirectory();
            appRootDir = Directory.GetParent(appRootDir).ToString();
            appRootDir = Directory.GetParent(appRootDir).ToString();
            appRootDir = Directory.GetParent(appRootDir).ToString();

            //write header to datafile
            string header = "CurTime,logDuration,countdownTimer,timesinceMovement,timePresent,personPresent";
            Console.WriteLine(header);
            logValue(appRootDir + filename, false, header);

            
        }

        private void timerCountDown_Tick(object sender, EventArgs e)
        {
            //every 1 sec tick subtract that from the count down time span
            TimeSpan interval = TimeSpan.FromSeconds(timerCountDown.Interval/1000);

            //subtract from timer and if it runs out set the led red so know there is no activity since within that interval
            if (countdownTimer.TotalSeconds > 0)
                countdownTimer = countdownTimer.Subtract(interval);
            else if(countdownTimer.TotalSeconds == 0)
                ledStatus.ColorOn = Color.Red;

            labelCountDownTimer.Text = countdownTimer.ToString();
            //Console.WriteLine(countdown.ToString());

            //calc time something is present which is when countdown>0
            if (countdownTimer.TotalSeconds > 0)
                timePresent = timePresent.Add(interval);
            labelTimePresent.Text = timePresent.ToString();
            
            //calc time since last movement
            timeSinceMovement = DateTime.Now.TimeOfDay - moveTime.TimeOfDay;
            string time = timeSinceMovement.Hours.ToString() + ":" + timeSinceMovement.Minutes.ToString() + ":" + timeSinceMovement.Seconds.ToString();
            labelTimeSinceMovement.Text = time;
            
            
        }

        private void logValue(string filename, Boolean append, string line)
        {
            DataLog = new StreamWriter(filename, append);

            DataLog.WriteLine(line);

            DataLog.Close();
        }
       


       

    } //end main class
}
