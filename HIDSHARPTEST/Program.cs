using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.ComponentModel;

using HidSharp;

namespace HIDSHARPTEST
{
    class Program
    {
        private static HidDevice device;
        protected static bool connectedToDriver = false;
        private static HidStream stream;
        private static int m_receiveBufferLength;

        /// <summary>
        /// Opens the current device.
        /// </summary>
        /// <returns><c>true</c>, if current device was opened, <c>false</c> otherwise.</returns>
        private static bool OpenCurrentDevice()
        {
            connectedToDriver = true;
            device.TryOpen(out stream);

            return true;
        }


        /// <summary>
        /// Opens the device.
        /// </summary>
        /// <returns><c>true</c>, if device was opened, <c>false</c> otherwise.</returns>
        /// <param name="adevice">Pass the parameter of HidDevice to open it directly</param>
        public static bool OpenDevice(HidDevice adevice)
        {
            if (adevice != null)
            {
                device = adevice;

                return OpenCurrentDevice();
            }

            return false;
        }
        public static bool OpenDevice()
        {
            bool result;

            if (device == null)
            {
                HidDeviceLoader loader = new HidDeviceLoader();
                HidDevice adevice = loader.GetDevices(0x10C4, 0xEA90).FirstOrDefault();
                result = OpenDevice(adevice);
            }
            else
            {
                result = OpenCurrentDevice();
            }
            return result;
        }

        private static byte[] Read(HidStream steam, int length, BackgroundWorker worker = null) //TODO length and worker; figure out what this shit does
        {
            var offset = 0;
            var result = new byte[length]; //TODO
            while (offset < length)
            {
                var data = new byte[256]; //TODO
                steam.Read(data);
                var bufferLength = offset + data.Length < length
                    ? data.Length
                    : length - offset;

                Buffer.BlockCopy(data, 1, result, offset, bufferLength - 1);
                offset += bufferLength == data.Length
                    ? bufferLength - 1
                    : bufferLength;

                if (worker != null) worker.ReportProgress((int)(offset * 100f / length));
            }
            if (worker != null) worker.ReportProgress(100);
            return result;
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Open Device? (y/n):  ");
                String open = Console.ReadLine();
                if (open.Equals("y"))
                {
                    bool reponse = OpenDevice();
                    Console.WriteLine(reponse);
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Read Device? (y/n):  ");
                String read = Console.ReadLine();
                if (read.Equals("y"))
                {
                    byte[] reponse = Read(stream, 256, null);
                    Console.WriteLine(reponse);
                    break;
                }
            }
        }

    }
}
