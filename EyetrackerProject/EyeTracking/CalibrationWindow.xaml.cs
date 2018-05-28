using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;

namespace EyeTrackingDemo
{
	/// <summary>
	/// Interaction logic for CalibrationWindow.xaml
	/// </summary>
    public partial class CalibrationWindow : Window
    {

        // This constructor arbitrarily assigns the local port number.
        UdpClient udpClient = null;
        UdpListener udpListener = null;

        string returnData;
        String[] splitData;

        Byte[] sendBytes;

        private int calPointNum = 9;
        private Int32[,] calPoints;
        private bool calibrated = false;

        private double crossSize = 50;
        private double crossThickness = 3;

        double DpiWidthFactor;
        double DpiHeightFactor;

        Line horLine = new Line();
        Line verLine = new Line();

        public CalibrationWindow()
        {
            InitializeComponent();

            udpClient = new UdpClient(4444);
            udpListener = new UdpListener(udpClient);

            calPoints = new int[calPointNum, 2];

            Window MainWindow = Application.Current.MainWindow;
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(MainWindow);
            Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;

            DpiWidthFactor = m.M11;
            DpiHeightFactor = m.M22;

            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;

            Mouse.OverrideCursor = Cursors.None;
            startCalibration();
        }

        public void startCalibration()
        {
            horLine.Stroke = System.Windows.Media.Brushes.Black;
            horLine.StrokeThickness = crossThickness;
            horLine.HorizontalAlignment = HorizontalAlignment.Left;
            horLine.VerticalAlignment = VerticalAlignment.Center;

            verLine.Stroke = Brushes.Black;
            verLine.StrokeThickness = crossThickness;
            verLine.HorizontalAlignment = HorizontalAlignment.Left;
            verLine.VerticalAlignment = VerticalAlignment.Center;

            this.CalibrationPane.Background = new SolidColorBrush(Colors.White);
            this.CalibrationPane.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.CalibrationPane.Width = System.Windows.SystemParameters.PrimaryScreenWidth;


            this.CalibrationPane.Children.Add(horLine);
            this.CalibrationPane.Children.Add(verLine);

            this.Show();

            udpClient.Connect("192.168.1.1", 4444);

            sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
            udpClient.Send(sendBytes, sendBytes.Length);

            // Sends a message to the host to which you have connected.
            sendBytes = Encoding.ASCII.GetBytes(String.Format("ET_CSZ {0} {1}\n", SystemParameters.PrimaryScreenWidth * DpiWidthFactor, SystemParameters.PrimaryScreenHeight * DpiHeightFactor));
            udpClient.Send(sendBytes, sendBytes.Length);

            Console.WriteLine(String.Format("ET_CSZ {0} {1}\n", SystemParameters.PrimaryScreenWidth * DpiWidthFactor, SystemParameters.PrimaryScreenHeight * DpiHeightFactor));
            sendBytes = Encoding.ASCII.GetBytes("ET_DEF\n");
            udpClient.Send(sendBytes, sendBytes.Length);
            
            sendBytes = Encoding.ASCII.GetBytes("ET_EST\n");
            udpClient.Send(sendBytes, sendBytes.Length);

            sendBytes = Encoding.ASCII.GetBytes("ET_CAL 9\n");
            udpClient.Send(sendBytes, sendBytes.Length);
            
            this.udpListener.NewMessageReceived += ReceiveCallback;
            udpListener.StartListener();
            
        }

        private void ReceiveCallback(Object sender, MyMessageArgs arg) 
        {

            Byte[] receiveBytes = arg.data;
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            returnData = Encoding.ASCII.GetString(receiveBytes);

            Console.WriteLine(returnData);
            splitData = returnData.Split();
            if (!calibrated)
            {
                if (splitData[0].Contains("ET_PNT"))
                {
                    calPoints[Int32.Parse(splitData[1]) - 1, 0] = Int32.Parse(splitData[2]);
                    calPoints[Int32.Parse(splitData[1]) - 1, 1] = Int32.Parse(splitData[3]);
                }
                else if (splitData[0].Contains("ET_CHG"))
                {

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        horLine.X1 = (calPoints[Int32.Parse(splitData[1]) - 1, 0] - crossSize / 2) / DpiWidthFactor;
                        horLine.X2 = (calPoints[Int32.Parse(splitData[1]) - 1, 0] + crossSize / 2) / DpiWidthFactor;
                        horLine.Y1 = calPoints[Int32.Parse(splitData[1]) - 1, 1] / DpiHeightFactor;
                        horLine.Y2 = calPoints[Int32.Parse(splitData[1]) - 1, 1] / DpiHeightFactor;

                        verLine.X1 = calPoints[Int32.Parse(splitData[1]) - 1, 0] / DpiWidthFactor;
                        verLine.X2 = calPoints[Int32.Parse(splitData[1]) - 1, 0] / DpiWidthFactor;
                        verLine.Y1 = (calPoints[Int32.Parse(splitData[1]) - 1, 1] - crossSize / 2) / DpiHeightFactor;
                        verLine.Y2 = (calPoints[Int32.Parse(splitData[1]) - 1, 1] + crossSize / 2) / DpiHeightFactor;

                        ProcessUITasks();
                    }));
                }
                else if (splitData[0].Contains("ET_CSZ"))
                {
                    Console.WriteLine(returnData);
                }
                else if (splitData[0].Contains("ET_FIN"))
                {
                    sendBytes = Encoding.ASCII.GetBytes("ET_FRM \"%PX %PY\"\n");
                    udpClient.Send(sendBytes, sendBytes.Length);

                    sendBytes = Encoding.ASCII.GetBytes("ET_STR 2\n");
                    udpClient.Send(sendBytes, sendBytes.Length);
                    calibrated = true;         //change
                }
            }
            else
            {
                if (splitData[0].Contains("ET_SPL"))
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        horLine.X1 = (Int32.Parse(splitData[1]) - crossSize / 2) / DpiWidthFactor;
                        horLine.X2 = (Int32.Parse(splitData[1]) + crossSize / 2) / DpiWidthFactor;
                        horLine.Y1 = Int32.Parse(splitData[2]) / DpiHeightFactor;
                        horLine.Y2 = Int32.Parse(splitData[2]) / DpiHeightFactor;

                        verLine.X1 = Int32.Parse(splitData[1]) / DpiWidthFactor;
                        verLine.X2 = Int32.Parse(splitData[1]) / DpiWidthFactor;
                        verLine.Y1 = (Int32.Parse(splitData[2]) - crossSize / 2) / DpiHeightFactor;
                        verLine.Y2 = (Int32.Parse(splitData[2]) + crossSize / 2) / DpiHeightFactor;

                        ProcessUITasks();
                    }));
                }
            }
        }

        public static void ProcessUITasks()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
            {
                sendBytes = Encoding.ASCII.GetBytes("ET_BRK\n");
                udpClient.Send(sendBytes, sendBytes.Length);

                udpListener.StopListener();
                this.udpListener.NewMessageReceived -= ReceiveCallback;
                Mouse.OverrideCursor = null;

                this.Close();
            }
            if (e.Key == Key.Q)
            {
                udpListener.StopListener();
                this.udpListener.NewMessageReceived -= ReceiveCallback;
                Mouse.OverrideCursor = null;

                this.Close();
            }
            else if (e.Key == Key.R)
            {
                sendBytes = Encoding.ASCII.GetBytes("ET_BRK\n");
                udpClient.Send(sendBytes, sendBytes.Length);
            
                sendBytes = Encoding.ASCII.GetBytes("ET_CAL 9\n");
                udpClient.Send(sendBytes, sendBytes.Length);

            }
        }
    }
}


