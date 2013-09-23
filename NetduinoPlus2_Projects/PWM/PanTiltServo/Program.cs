//todo
//got the packet parsed now need to figure out how to have event handler store the byte packet and raise the packet flag so it can be parsed in main.
//do a class like the serialporthelper example.  then store the data in the buffer there and can set flags etc...



using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;


namespace PanTiltServo
{
    
    
    public class Program
    {
             
        //SerialPorts.COM1 (uses D0 and D1)
        static SerialPortHelper mySerialPortHelper = new SerialPortHelper(SerialPorts.COM1, 115200, Parity.None, 8, StopBits.One);

        PWM led1;
        PWM led2;
        //PWM panServo;
        //PWM tiltServo;

        Servo_API.Servo pan;

            
        public static void Main()
        {
           
            //PWM led1 = new PWM(PWMChannels.PWM_ONBOARD_LED, 100, .5, false); // Set later
            //PWM led2 = new PWM(PWMChannels.PWM_PIN_D5, 50, .5, false); //50% brightness
            //PWM panServo = new PWM(PWMChannels.PWM_PIN_D5, 20000, 1500, PWM.ScaleFactor.Microseconds, false);
            //PWM tiltServo = new PWM(PWMChannels.PWM_PIN_D6, 20000, 1500, PWM.ScaleFactor.Microseconds, false);

            Servo_API.Servo panServo = new Servo_API.Servo(PWMChannels.PWM_PIN_D5, 2000, 1500);
            Servo_API.Servo tiltServo = new Servo_API.Servo(PWMChannels.PWM_PIN_D6, 2000, 1500);

            //panServo.Start();
            //tiltServo.Start();
            string myPacket = "";

            while (true)
            {


                if (mySerialPortHelper.isPacketReady())
                {
                    myPacket = mySerialPortHelper.ReadLine();
                    Debug.Print(myPacket);

                    //check for servo packet
                    if (myPacket[0] == 's') //'s' is for Servo position packet
                    {
                        //trim off the s in preparation for further parsing
                        myPacket = myPacket.TrimStart('s');

                        //parse the packet
                        //pan or tilt
                        if (myPacket[0] == 'p')    //pan packet with degrees
                        {
                            string s = myPacket.ToString();
                            s = s.TrimStart('p');
                            int deg = int.Parse(s);
                            panServo.Degree = deg;
                            mySerialPortHelper.PrintLine("pan set to --> " + deg.ToString());
                        }
                        else if (myPacket[0] == 't')  //tilt packet with degrees
                        {
                            string s = myPacket.ToString();
                            s = s.TrimStart('t');
                            int deg = int.Parse(s);
                            tiltServo.Degree = deg;
                            mySerialPortHelper.PrintLine("tilt set to --> " + deg.ToString());
                        }
                                                                
                    }//end if servo position packet

                    //check for get servo postion packet
                    if (myPacket[0] == 'g' && myPacket[1] == 'p')
                    {
                        mySerialPortHelper.PrintLine("pp" + panServo.Degree.ToString());
                        mySerialPortHelper.PrintLine("tp" + tiltServo.Degree.ToString());

                    }// end if get servo position

                    //clear the packet
                    myPacket = "";

                }// end if packet ready

                

                //for (int i = 0; i < 160; i++)
                //{
                //    panServo.Degree = i;
                //    tiltServo.Degree = i;
                //    Thread.Sleep(20);
                //}

                //for (int i = 160; i > 0; i--)
                //{
                //    panServo.Degree = i;
                //    tiltServo.Degree = i;
                //    Thread.Sleep(20);
                //}



            }//end while
        }//end main

       
    }//end class Program
}//end namespace
