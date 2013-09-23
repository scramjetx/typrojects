using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace TrackerApp
{

    public class SerialPortHelper
    {
        static SerialPort serialPort;

        const int bufferMax = 4096;
        static byte[] buffer = new Byte[bufferMax];
        static int bufferLength = 0;
        static bool flagPacketRX = false;

        
        public SerialPortHelper(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            serialPort.ReadTimeout = 10; // Set to 10ms. Default is -1?!
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        public void OpenPort()
        {
            serialPort.Open();
        }

        public void ClosePort()
        {
            serialPort.Close();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        { 
            lock (buffer)
            {
                int bytesReceived = serialPort.Read(buffer, bufferLength, bufferMax - bufferLength);
                if (bytesReceived > 0)
                {
                    bufferLength += bytesReceived;
                    if (bufferLength >= bufferMax)
                        throw new ApplicationException("Buffer Overflow.  Send shorter lines, or increase lineBufferMax.");

                    if (buffer[bufferLength - 1] == '\r' || buffer[bufferLength - 1] == '\n')
                    {
                        flagPacketRX = true;
                        
                    }
                }
            }
        }

        public string ReadLine()
        {
            string line = "";

            lock (buffer)
            {
                //-- Look for Return char in buffer --
                for (int i = 0; i < bufferLength; i++)
                {
                    //-- Consider EITHER CR or LF as end of line, so if both were received it would register as an extra blank line. --
                    if (buffer[i] == '\r' || buffer[i] == '\n')
                    {
                        buffer[i] = 0; // Turn NewLine into string terminator
                        line = "" + new string(Encoding.UTF8.GetChars(buffer)); // The "" ensures that if we end up copying zero characters, we'd end up with blank string instead of null string.
                        //Console.WriteLine("LINE: <" + line + ">");
                        bufferLength = bufferLength - i - 1;
                        Array.Copy(buffer, i + 1, buffer, 0, bufferLength); // Shift everything past NewLine to beginning of buffer
                        break;
                    }
                }
            }


            if (bufferLength == 0)
                flagPacketRX = false;

            return line;
        }

        public void Print( string line )
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            byte[] bytesToSend = encoder.GetBytes(line);
            serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public void PrintLine(string line)
        {
            Print(line + "\r");
        }

        public void PrintClear()
        {
            byte[] bytesToSend = new byte[2];
            bytesToSend[0] = 254;
            bytesToSend[1] = 1;
            serialPort.Write(bytesToSend, 0, 2);
            Thread.Sleep(500); // LCD is slow, pause for 500ms before sending more chars
        }

        public bool isPacketReady()
        {
            return flagPacketRX;
        }
    }
}
