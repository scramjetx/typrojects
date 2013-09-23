using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PWMtest
{
    public class Program
    {
        public static void Main()
        {
            // write your code here
            PWM led1 = new PWM(PWMChannels.PWM_ONBOARD_LED, 100, .5, false); // Set later
            //PWM led2 = new PWM(PWMChannels.PWM_PIN_D5, 50, .5, false); //50% brightness
            PWM led2 = new PWM(PWMChannels.PWM_PIN_D5, 20000, 1500, PWM.ScaleFactor.Microseconds, false);

            led1.Start();
            led2.Start();

            Thread.Sleep(1000);

            while (true)
            {
                for (double x = 1000; x < 2000; x = x + 20)
                {
                    led2.Duration = (uint)x;
                    Thread.Sleep(100);
                }

            }


        }//end main()

    }
}
