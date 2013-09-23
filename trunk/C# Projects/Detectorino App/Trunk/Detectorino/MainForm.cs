//clean up display.
    //show graphic of burrito in various states of cooked
    //show message about state of cookedness
//when shorting pin max can get on ADC = 2.03V, now need to add resistor to drop that to 1.2V
    //BJT typically drop 0.7V base to emitter but this PN2222 or 2N2222 BJT seems to be 1.3V 
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
        Boolean flagToggleLED = false;              //flag for timer to log data that the IO state changed 

        TimeSpan timeSinceMovement;                 //how long it's been since sensing a movement    
        DateTime startTime;                         //when started logging
        DateTime moveTime = DateTime.Now;           //time when last movement logged
        TimeSpan logTimeSpan;                       //how long been logging for
        TimeSpan countdownTimer;                    //timer that decrements if no inactivity from X amount of time. If hits zero then noone there.
        double countdownValue;                      //countdown value in minutes
        TimeSpan timePresent;                       //tracks amount of time present based on countdown>0
        int personPresent = 0;                      //bit to track if someone is present or not for logging.  Based on countdownTimer>0

        string newHexReading;                       //add the two hex bytes together and store here
        bool validPacket = false;                   //tracks packet parsing window. start of correct packet and end.
        int newADCReading = 0;                      //the 2 hex bytes added together and converted to decimal
        float newVoltReading = 9.99f;                   //the ADC reading converted to a actual voltage. scaled by 1023 max or 0x03 + 0xFF

        StreamWriter DataLog;
        string appRootDir;
        //"DataLog_" + DateTime.Now.Hour.ToString() + "hr_" + DateTime.Now.Minute.ToString() + "min.csv"; 
        string filename = "\\DetectorinoLog.csv";
        string header = "CurTime,logDuration,capVoltage,timesinceMovement,timePresent,personPresent";
        string lineOfData = "";                     //data line logged to file
        bool logDataFlag = false;                   //sync when data is logged. according to getting valid voltage reading


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
            //text message to my phone: 12087570146@vzwpix.com
            myGmail.SendEmail("detectorino@gmail.com", "scramjetx@gmail.com", "moreland85", "testing", lineOfData);
           
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
                //page 68 of manual explains data. 
                //3rd and 2nd to last bytes make up the analog sample we're interested in. 03 FF (1023 dec) is max reading
                //to get analog reading the equation is 1023/3.3 = NewADCReading/(NewVoltReading) => NewVoltReading = NewADCReading * 3.3V / 1023

                if (b == 0x7E)  //or 126 decimal which is start of packet header byte for Xbee 
                {
                    Console.WriteLine();
                    byteCounter = 0;
                    validPacket = true;
                }

                //converts dec value to hex
                string hexValue = b.ToString("X2");
               
                //convert hex to into
                //val = Convert.ToInt32(hexValue, 16);

                //these are the 2 bytes we're interested in
                if (byteCounter == 19 || byteCounter == 20 
                    && validPacket == true)
                {
                    //Console.Write(hexValue + " ");

                    //add byte 20 and 21, take byte 20 and is FF but should be FF00 if going to add it to byte 21 which is AA but should be 00AA
                    newHexReading = newHexReading + hexValue;
                    
                    //need to add up the 2 bytes here.  Then check what the value is.  If it's below where it should be in 5min
                    //means no one around in last that amount of time.  Time constant set by resistor and cap on sensor.
                    //everytime sensor is tripped it recharges the cap.
                    //then we say toggle the LED otherwise don't
                    if (byteCounter == 20)
                    {
                        newADCReading = Convert.ToInt32(newHexReading, 16);
                        newVoltReading = (float)newADCReading * 1.2f / 1023f;     //1.2f because 1.2V is max ADC pin voltage xbee reads as 03FF (ie 1023)
                        
                        //Console.WriteLine();
                        //Console.WriteLine(newHexReading);
                        Console.WriteLine(newVoltReading.ToString());
                        
                        //if below certain value after 5min then we know no activity around the sensor
                        //after 5min the cap voltage will drain to 0.3V. below that point nothing around in the last 5 min so start counter
                        //5 min is the reporting rate of the remote Xbee with sensor.  Good combo of update rate and battery life.
                        if(newVoltReading >= 0.3f)
                            flagToggleLED = true;

                        newADCReading = 0;
                        newHexReading = null;

                        logDataFlag = true;
                        validPacket = false;  //end of useful info so don't step into this statement again.
                    }
                }
                //increment count
                byteCounter++;
            } //end foreach


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

            if(logDataFlag == true)
            {
                logDataLine();
                logDataFlag= false;

            }

            flagToggleLED = false;

        }

        private void timerLogData_Tick(object sender, EventArgs e)
        {

           

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
            Console.WriteLine(header);
            logValue(appRootDir + filename, false, header);

            
        }

        private void timerCountDown_Tick(object sender, EventArgs e)
        {
            //every 1 sec tick subtract that from the count down time span
            TimeSpan interval = TimeSpan.FromSeconds(timerCountDown.Interval/1000);

            ////subtract from timer and if it runs out set the led red so know there is no activity since within that interval
            //if (countdownTimer.TotalSeconds > 0)
            //    countdownTimer = countdownTimer.Subtract(interval);
            //else if(countdownTimer.TotalSeconds == 0)
            //    ledStatus.ColorOn = Color.Red;

            //labelCountDownTimer.Text = countdownTimer.ToString();
            ////Console.WriteLine(countdown.ToString());

            ////calc time something is present which is when countdown>0
            //if (countdownTimer.TotalSeconds > 0)
            //    timePresent = timePresent.Add(interval);

            //labelTimePresent.Text = timePresent.ToString();

            //this replaces above commented code in favor of using the capacitor as the timer so xbee can sleep for 5min between samples
            //calc time something is present which is when voltage reading >=0.3V
            if (newVoltReading >= 0.3f)
            {
                timePresent = timePresent.Add(interval);
                personPresent = 1;
            }
            else
            {
                ledStatus.ColorOn = Color.Red;
                personPresent = 0;
            }
            labelCountDownTimer.Text = newVoltReading.ToString("0.##") + "V";
            labelTimePresent.Text = timePresent.ToString();
           

            //calc time since last movement
            timeSinceMovement = DateTime.Now - moveTime;
            string time = timeSinceMovement.Hours.ToString() + ":" + timeSinceMovement.Minutes.ToString() + ":" + timeSinceMovement.Seconds.ToString();
            labelTimeSinceMovement.Text = time;


            if (newVoltReading > 30f)
            {
                int thisstuff;
            }

        }

        private void logValue(string filename, Boolean append, string line)
        {
            DataLog = new StreamWriter(filename, append);

            DataLog.WriteLine(line);

            DataLog.Close();
        }

        private void logDataLine()
        {
            //log time stamp, log duration from start in min, current countdown timer min and sec, sensor state as 1 or 0
            string curTime = DateTime.Now.ToString("HH:mm:ss");

            logTimeSpan = DateTime.Now - startTime;
            string logDuration = logTimeSpan.Hours.ToString() + ":" + logTimeSpan.Minutes.ToString() + ":" + logTimeSpan.Seconds.ToString();

            //Track if someone is present based on countdown timer for graphing
            //if (countdownTimer.TotalSeconds > 0)
            //    personPresent = 1;
            //else if (countdownTimer.TotalSeconds == 0)
            //    personPresent = 0;


            //write a line of data
            lineOfData = curTime +
                "," + logDuration +
                "," + newVoltReading.ToString("#0.0#") +
                "," + timeSinceMovement.TotalMinutes.ToString("###0.0#") +
                "," + timePresent.TotalMinutes.ToString("###0.0#") +
                "," + personPresent.ToString();

            Console.WriteLine(lineOfData);
            logValue(appRootDir + filename, true, lineOfData);
        }

       

    } //end main class
}
