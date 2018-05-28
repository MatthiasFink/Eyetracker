using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace EyeTrackingDemo
{
    class UdpListener
    {
        private int m_portToListen = 4444;
        private volatile UdpClient listener = null;
        private volatile bool listening;
        Thread m_ListeningThread;
        public event EventHandler<MyMessageArgs> NewMessageReceived;

        //constructor
        public UdpListener(UdpClient _udpListener)
        {
            this.listening = false;
            listener = _udpListener;
        }

        public void StartListener()
        {
            if (!this.listening)
            {
                m_ListeningThread = new Thread(ListenForUDPPackages);
                m_ListeningThread.IsBackground = true;
                this.listening = true;
                m_ListeningThread.Start();
            }
        }

        public void StopListener()
        {
            this.listening = false;
            listener.Close();
        }

        public void ListenForUDPPackages()
        {         

            if (listener != null)
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, m_portToListen);

                try
                {
                    while (this.listening)
                    {
                        Console.WriteLine("Waiting for UDP broadcast to port " + m_portToListen);
                        byte[] bytes = listener.Receive(ref groupEP);

                        //raise event                        
                        NewMessageReceived(this, new MyMessageArgs(bytes));
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                }
                finally
                {
                    Console.WriteLine("Done listening for UDP broadcast");
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
}
