using System;
using Microsoft.SPOT;

namespace PanTiltServo
{
    /*
 * Servo NETMF Driver
 *&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Coded by Chris Seto August 2010
 *&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <chris@chrisseto.com>
 *
 * Use this code for whatveer you want. Modify it, redistribute it, I don't care.
 * I do ask that you please keep this header intact, however.
 * If you modfy the driver, please include your contribution below:
 *
 * Chris Seto: Inital release (1.0)
 * Chris Seto: Netduino port (1.0 -> Netduino branch)
 * Chris Seto: bool pin state fix (1.1 -> Netduino branch)
 * */

    using System;
    using Microsoft.SPOT.Hardware;
    using SecretLabs.NETMF.Hardware;

    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using SecretLabs.NETMF.Hardware;
    using SecretLabs.NETMF.Hardware.Netduino;

    namespace Servo_API
    {
        public class Servo : IDisposable
        {
            /// <summary>
            /// PWM handle
            /// </summary>
            private PWM servo;

            /// <summary>
            /// Timings range
            /// </summary>
            private int[] range = new int[2];

            /// <summary>
            /// Set servo inversion
            /// </summary>
            public bool inverted = false;

            /// <summary>
            /// Create the PWM pin, set it low and configure timings
            /// </summary>
            /// <param name="pin"></param>
            public Servo(Cpu.PWMChannel pin, double dutycycle, uint duration)
            {
                // Init the PWM pin
                servo = new PWM(pin, 20000, 1500, PWM.ScaleFactor.Microseconds, false);

                servo.DutyCycle = dutycycle;
                servo.Duration = duration;
                servo.Start();

                // Typical settings
                range[0] = 1000;
                range[1] = 2000;
            }

            public void Dispose()
            {
                disengage();
                servo.Dispose();
            }

            /// <summary>
            /// Allow the user to set cutom timings
            /// </summary>
            /// <param name="fullLeft"></param>
            /// <param name="fullRight"></param>
            public void setRange(int fullLeft, int fullRight)
            {
                range[1] = fullLeft;
                range[0] = fullRight;
            }

            /// <summary>
            /// Disengage the servo.
            /// The servo motor will stop trying to maintain an angle
            /// </summary>
            public void disengage()
            {
                // See what the Netduino team say about this...
                servo.DutyCycle = 0;
                
            }

            /// <summary>
            /// Set the servo degree
            /// </summary>
            public double Degree
            {
                set
                {
                    /// Range checks
                    if (value > 160)
                        value = 160;

                    if (value < 0)
                        value = 0;

                    // Are we inverted?
                    if (inverted)
                        value = 160 - value;

                    // Set the pulse
                    servo.Duration = (uint)map((long)value, 0, 160, (long)range[0], (long)range[1]);
                    //Debug.Print("Degree: " + value.ToString() + "-->" + servo.Duration.ToString());
 
                }

                get
                {
                    return Degree;
                }
            }

            /// <summary>
            /// Used internally to map a value of one scale to another
            /// </summary>
            private long map(long x, long in_min, long in_max, long out_min, long out_max)
            {
                return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
            }
        }
    }
}
