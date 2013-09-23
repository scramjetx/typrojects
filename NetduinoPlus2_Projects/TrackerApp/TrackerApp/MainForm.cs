//need the main thread to subscribe to the data recieved event through delegates to process the data.  Pretty critical C# concept to grasp so should spend the time to implement it.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace TrackerApp
{
   

    public partial class MainForm : Form
    {
        SerialPortHelper mySerialPortHelper;

        int mousePosX = 0;  
        int mousePosY = 0;  
       
        public MainForm()
        {
            InitializeComponent();

            buttonConnect.Focus();

        }//end mainform

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (buttonConnect.Text == "Connect")
            {
                buttonConnect.Text = "Disconnect";
                buttonConnect.ForeColor = Color.Red;

                //configure serial port - portname, baud, parity, databits, stopbits
                mySerialPortHelper = new SerialPortHelper(textBoxPortName.Text, int.Parse(textBoxBaudRate.Text), Parity.None, 8, StopBits.One);
                mySerialPortHelper.OpenPort();

                timer1.Interval = 500;
                timer1.Enabled = false;

                //sends servo position data.  20ms is pretty smooth and can move scroll bar around
                timer2.Interval = 20;
                timer2.Enabled = true;

                textBoxPortName.ReadOnly = true;
                textBoxBaudRate.ReadOnly = true;

                //opening the serial port
                //try
                //{
                //    serialPort1.Open();
                //}
                //catch (Exception error)
                //{
                //    MessageBox.Show("Invalid Serial Port");
                //    buttonConnect.PerformClick();
                //}

            }
            else
            {
                buttonConnect.Text = "Connect";
                buttonConnect.ForeColor = Color.Green;

                timer1.Enabled = false;
                timer2.Enabled = false;

                //close port
                mySerialPortHelper.ClosePort();

                textBoxPortName.ReadOnly = false;
                textBoxBaudRate.ReadOnly = false;
            }
        }

        private void buttonSendCMD_Click(object sender, EventArgs e)
        {
           mySerialPortHelper.PrintLine("st" + textBoxCMD.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mySerialPortHelper.isPacketReady())
            {
                string s = mySerialPortHelper.ReadLine();
                Console.WriteLine(s);
            }

            Console.WriteLine("tick");

        }

        private void trackBarPan_Scroll(object sender, EventArgs e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //change postion according to trackbar
            //mySerialPortHelper.PrintLine("sp" + trackBarPan.Value);

            //change position according to picturebox mouse location
            mySerialPortHelper.PrintLine("sp" + mousePosX);
            mySerialPortHelper.PrintLine("st" + mousePosY);

            //consume data from the buffer so it doesn't overflow. currently not a circular buffer
            mySerialPortHelper.ReadLine();
            mySerialPortHelper.ReadLine();

        }

        private void pictureBoxPanTilt_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mousePosX = e.X;
            mousePosY = e.Y;
            labelMouseXYPos.Text = "X: " + mousePosX.ToString() + "    Y: " + mousePosY.ToString();
        }

    }//end class
}//end namespace
