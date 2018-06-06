using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EyetrackerExperiment;

namespace EyetrackerExperiment.EyeTracking
{
    class UdpListener
    {
        private int port = 4444;
        private volatile UdpClient udpClient = null;
        private volatile bool listening;
        Thread ListeningThread = null;
        public event EventHandler<MyMessageArgs> NewMessageReceived;

        //constructor
        public UdpListener(UdpClient UdpClient, int Port)
        {
            listening = false;
            port = Port;
            udpClient = UdpClient;
        }

        public void StartListener()
        {
            if (!this.listening)
            {
                ListeningThread = new Thread(ListenForUDPPackages);
                ListeningThread.IsBackground = true;
                listening = true;
                ListeningThread.Start();
            }
        }

        public void StopListener()
        {
            this.listening = false;
            udpClient.Close();
        }

        public void ListenForUDPPackages()
        {

            if (udpClient != null)
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);

                try
                {
                    while (this.listening)
                    {
                        Console.WriteLine("Waiting for UDP broadcast to port " + port);
                        byte[] dgram = udpClient.Receive(ref groupEP);

                        //raise event                        
                        NewMessageReceived(this, new MyMessageArgs(dgram));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("EyeTracker: \n" + e.ToString());
                }
                finally
                {
                    Console.WriteLine("EyeTracker: Done listening for UDP broadcast");
                }
            }
        }
    }

    public class MyMessageArgs : EventArgs
    {
        public byte[] data { get; set; }

        public MyMessageArgs(byte[] newData)
        {
            data = newData;
        }
    }

    class EyeTrackingController
    {
        private String ip;
        private int port;
        public UdpClient udpClient;
        public EyeTrackingController(String Ip, int Port)
        {
            ip = Ip;
            port = Port;
            udpClient = new UdpClient(port);
        }

        public bool Send(String command)
        {
            byte[] dgram = Encoding.ASCII.GetBytes(command);
            try
            {
                int count = udpClient.Send(dgram, dgram.Length);
                return (count > 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Eyetracker: " + e.Message);
            }
            return false;
        }

        public bool Start()
        {
            try
            {
                udpClient.Connect(ip, port);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Eyetracker: " + e.Message);
            }
            return false;
        }

        public bool StartTracking()
        {
            return Send("ET_REC\n");
        }

        public bool StopTracking()
        {
            return Send("ET_STP\n");
        }

        public bool SaveTracking(String filePath)
        {
            return Send(String.Format("ET_SAV \"{0}\"\n", filePath));
        }

        public bool ClearTracking()
        {
            return Send("ET_CLR\n");
        }

        public bool Stop()
        {
            try
            {
                udpClient.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Eyetracker: " + e.Message);
            }
            return false;
        }
    }
}
