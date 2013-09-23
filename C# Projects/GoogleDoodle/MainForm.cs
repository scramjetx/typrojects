using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using Utilities;
using System.Runtime.InteropServices;       //import dll
using System.Threading;

namespace Hurdles
{

    public partial class Form1 : Form
    {
        

        const int VK_SPACE = 0x20;
        const uint KEYEVENTF_KEYUP = 0x2;

        globalKeyboardHook Skeypress = new globalKeyboardHook();
        globalKeyboardHook Spacekeypress = new globalKeyboardHook();
        

        int nKeyCount = 1;
        int spacekeyCount = 0;

        bool spacebarState = false;     //true is pressed and holding, false is not pressed
        int countSpacebar = 0;

        [DllImport("user32.dll")]
        public static extern void keybd_event(
        byte bVk,
        byte bScan,
        uint dwFlags,
        uint dwExtraInfo
        );

        double start;
        double span;

        bool startProgramFlag = true;
        bool spaceKeyUpFlag = true;
        bool spaceKeyDownFlag = false;

        //init in text box too
        int SHOTBUFFER1 = 800;
        int TIMING1 = 300;
        int SHOTBUFFER2 = 800;
        int TIMING2 = 475;
        int SHOTBUFFER3 = 800;
        int TIMING3 = 650;
        int SHOTBUFFER4 = 800;
        int TIMING4 = 825;

        int TIMING5 = 925;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Skeypress.HookedKeys.Add(Keys.S);
            Skeypress.KeyDown += new KeyEventHandler(S_KeyDown);
            Skeypress.KeyUp += new KeyEventHandler(S_KeyUp);

            Spacekeypress.HookedKeys.Add(Keys.Space);
            Spacekeypress.KeyDown += new KeyEventHandler(Space_KeyDown);
            Spacekeypress.KeyUp += new KeyEventHandler(Space_KeyUp);

            textBox1.Text = SHOTBUFFER1.ToString();
            textBox2.Text = TIMING1.ToString();
            textBox3.Text = SHOTBUFFER2.ToString();
            textBox4.Text = TIMING2.ToString();
            textBox5.Text = SHOTBUFFER3.ToString();
            textBox6.Text = TIMING3.ToString();
            textBox7.Text = SHOTBUFFER4.ToString();
            textBox8.Text = TIMING4.ToString();

        }

        void S_KeyDown(object sender, KeyEventArgs e)
        {


            //lstLog.Items.Add("Down\t" + e.KeyCode.ToString());
            e.Handled = true;
        }

        void S_KeyUp(object sender, KeyEventArgs e)
        {
            for(int i = 0; i<20; i++)
            Console.WriteLine();

            SHOTBUFFER1 = Int16.Parse(textBox1.Text.ToString());
            TIMING1 = Int16.Parse(textBox2.Text.ToString());
            SHOTBUFFER2 = Int16.Parse(textBox3.Text.ToString());
            TIMING2 = Int16.Parse(textBox4.Text.ToString());
            SHOTBUFFER3 = Int16.Parse(textBox5.Text.ToString());
            TIMING3 = Int16.Parse(textBox6.Text.ToString());
            SHOTBUFFER4 = Int16.Parse(textBox7.Text.ToString());
            TIMING4 = Int16.Parse(textBox8.Text.ToString());

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING1);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER1);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING1);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER1);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING1);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER1);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING1);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER1);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING1);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep(700, 50);
            Thread.Sleep(SHOTBUFFER1);
            
            //**line 2
            Console.WriteLine("Line2");
            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING2);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER2);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING2);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER2);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING2);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER2);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING2);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep(700, 50);
            Thread.Sleep(SHOTBUFFER2);

            //**line 3
            Console.WriteLine("Line3");
            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING3);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER3);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING3);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER3);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING3);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER3);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING3);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep(700, 50);
            Thread.Sleep(SHOTBUFFER3);

            //**line4
            Console.WriteLine("Line4");
            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING4);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER4);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING4);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER4);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING4);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER4);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING4);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            //Console.Beep();
            Thread.Sleep(SHOTBUFFER4);

            keybd_event((byte)VK_SPACE, 0, 0, 0);
            Thread.Sleep(TIMING5);
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

            Console.WriteLine("LAST SHOT");

            //lstLog.Items.Add("Up\t" + e.KeyCode.ToString());
            e.Handled = true;
        }

        void Space_KeyDown(object sender, KeyEventArgs e)
        {
            //add flag here cause keydown fires many times while holding
            //if (spaceKeyUpFlag)
            //{
            //    start = DateTime.Now.TimeOfDay.TotalMilliseconds;
            //    spaceKeyUpFlag = false;
            //    spaceKeyDownFlag = true;
            //}

            //if (startProgramFlag)
            //{
            //    timer4.Interval = 400;
            //    timer4.Enabled = true;
            //    startProgramFlag = false;
            //}

            
            
            //Console.WriteLine("keydown");
        }

        void Space_KeyUp(object sender, KeyEventArgs e)
        {
            //if (spaceKeyDownFlag)
            //{
            //    span = DateTime.Now.TimeOfDay.TotalMilliseconds - start;
            //    Console.WriteLine("key press time = " + span.ToString("000.00") + "  COUNT: " + spacekeyCount.ToString());
            //    span = 0;
            //    spaceKeyUpFlag = true;
                

            //    //int temp = spacekeyCount;
            //    //if (temp == 6 || temp == 10 || temp == 15)
            //     //   ChangeSpacebarInterval();
            //    if (spacekeyCount == 3)
            //    {
            //        timer4.Stop();
            //        timer4.Interval = 550;
            //        timer4.Start();
            //        Console.WriteLine("Changed Interval: " + timer4.Interval.ToString());
            //        Console.Beep();
            //    }
            //    else if (spacekeyCount == 13)
            //    {
            //        timer4.Stop();
            //        timer4.Interval = 650;
            //        timer4.Start();
            //        Console.WriteLine("Changed Interval: " + timer4.Interval.ToString());
            //        Console.Beep();
            //    }
            //    else if (spacekeyCount == 22)
            //    {
            //        timer4.Stop();
            //        timer4.Interval = 700;
            //        timer4.Start();
            //        Console.WriteLine("Changed Interval: " + timer4.Interval.ToString());
            //        Console.Beep();
            //    }
            //    else if (spacekeyCount == 28)
            //    {
            //        timer4.Interval = 850;
            //        Console.WriteLine("Changed Interval: " + timer4.Interval.ToString());
            //        Console.Beep();
            //    }

            //    spacekeyCount++;
            //}
        }
       
        private void ChangeSpacebarInterval()
        {
            switch (spacekeyCount)
            {
                case 5:
                    timer4.Interval = 750;
                    break;
                case 10:
                    timer4.Interval = 850;
                    break;
                case 15:
                    timer4.Interval = 1000;
                    break;
            }
            Console.WriteLine("Interval Changed to: " + timer4.Interval.ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{LEFT}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 1;
            timer2.Enabled = true;
            timer2.Interval = 2;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{RIGHT}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();
            nKeyCount = 1;
            spacebarState = false;
            countSpacebar = 0;
            spaceKeyUpFlag = true;
            spacekeyCount = 0;
            startProgramFlag = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer4.Interval = 300;
            timer4.Enabled = true;
            
        
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //SendKeys.Send(" ");

            if (spacebarState == false)
            {
                keybd_event((byte)VK_SPACE, 0, 0, 0);
                spacebarState = true;
            }
            else
            {
                keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)VK_SPACE, 0, 0, 0);
                spacebarState = false;
                countSpacebar++;

                Console.WriteLine("SPACEBAR : " + DateTime.Now.Millisecond.ToString());
                
                if (countSpacebar % 5 == 0)
                {
                    nKeyCount++;
                    ChangeSpacebarInterval();
                }
            }

            
            
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            keybd_event((byte)VK_SPACE, 0, KEYEVENTF_KEYUP, 0);

        }

 
        
    }
}
