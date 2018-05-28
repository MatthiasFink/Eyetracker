﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace EyeTrackingDemo
{
    /// <summary>
    /// Interaction logic for PresentationWindow.xaml
    /// </summary>
    public partial class PresentationWindow : Window
    {

        String subjName;
        String imageName;
        String path;
        String pathTime;
        String answer;
        String answerTime;

        int count = 0;
        int fileCount;

        BitmapImage actIm;
        Uri actUri;
        Uri actUriTy;

        Uri actCrossUri;
        BitmapImage actCrossImg;

        Uri actCaliUri;
        BitmapImage actCaliImg;
        Boolean cali = false;

        List<Uri> listUri = new List<Uri>();

        Stopwatch overall = new Stopwatch();
        String timeOverall;
        TimeSpan overallTS;

        Stopwatch timer = new Stopwatch();
        String time;
        TimeSpan ts;

        System.Timers.Timer timer1;

        private System.Timers.Timer aTimer;
        private int stimDur = 10000;
        private int fixDur = 1000;

        Byte[] sendBytes;

        // This constructor arbitrarily assigns the local port number.
        UdpClient udpClient = new UdpClient(4444);

        private double crossSize = 50;
        private double crossThickness = 3;

        public PresentationWindow()
        {
            InitializeComponent();
        }

        public PresentationWindow(String _subjName) : this()
        {
            this.subjName = _subjName;
            this.answer = _subjName + Environment.NewLine;
            this.path = @"ans\\" + _subjName + "_Results.txt";

            this.answerTime = _subjName + Environment.NewLine;
            this.pathTime = @"ans\\" + _subjName + "_ResultsTime.txt";

            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.Show();

            actCrossUri = new Uri("Cross\\Cross.png", UriKind.RelativeOrAbsolute);
            actCrossImg = new BitmapImage(actCrossUri);

            actCaliUri = new Uri("Calibration\\Calibration.png", UriKind.RelativeOrAbsolute);
            actCaliImg = new BitmapImage(actCaliUri);

            fileCount = Directory.GetFiles("img\\").Length;
            for (int i = 0; i < fileCount; i++)
            {
                actUri = new Uri("img\\" + i + ".png", UriKind.RelativeOrAbsolute);
                listUri.Add(actUri);
            }

            actIm = new BitmapImage(listUri[count]);

            try
            {
                udpClient.Connect("192.168.1.1", 4444);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Mouse.OverrideCursor = Cursors.None;
            fixationScreen();

            overall.Start();
            stimulusPresentation(actIm);
        }

        private void fixationScreen()
        {

            Line horLine = new Line();
            Line verLine = new Line();

            horLine.Stroke = System.Windows.Media.Brushes.Black;
            horLine.X1 = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - crossSize / 2;
            horLine.X2 = System.Windows.SystemParameters.PrimaryScreenWidth / 2 + crossSize / 2;
            horLine.Y1 = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            horLine.Y2 = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            horLine.StrokeThickness = crossThickness;
            horLine.HorizontalAlignment = HorizontalAlignment.Left;
            horLine.VerticalAlignment = VerticalAlignment.Center;

            verLine.Stroke = Brushes.Black;
            verLine.X1 = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            verLine.X2 = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            verLine.Y1 = System.Windows.SystemParameters.PrimaryScreenHeight / 2 - crossSize / 2;
            verLine.Y2 = System.Windows.SystemParameters.PrimaryScreenHeight / 2 + crossSize / 2;
            verLine.StrokeThickness = crossThickness;

            verLine.HorizontalAlignment = HorizontalAlignment.Left;
            verLine.VerticalAlignment = VerticalAlignment.Center;

            this.FixationPane.Background = new SolidColorBrush(Colors.White);


            this.FixationPane.Children.Add(horLine);
            this.FixationPane.Children.Add(verLine);

            /*          ProcessUITasks();

                      System.Diagnostics.Debug.WriteLine(fileCount);
                      System.Diagnostics.Debug.WriteLine(listUri.Count);

                      System.Threading.Thread.Sleep(fixDur);
                      this.FixationPane.Visibility = Visibility.Hidden;
                      */

            showFixationScreen();
        }

        private void showFixationScreen()
        {
            overall.Stop();
            this.FixationPane.Visibility = Visibility.Visible;
            ProcessUITasks();

            this.StimulusPane.Source = actCrossImg;

          //  System.Diagnostics.Debug.WriteLine(fileCount);
          //  System.Diagnostics.Debug.WriteLine(listUri.Count);

            System.Threading.Thread.Sleep(1000);
            this.FixationPane.Visibility = Visibility.Hidden;
            overall.Start();
        }


        private void stimulusPresentation(BitmapImage _stimulus)
        {
            // Create a timer with a ten second interval.
            //aTimer = new System.Timers.Timer(stimDur);


            // Hook up the Elapsed event for the timer.
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            this.StimulusPane.Source = _stimulus;
            ProcessUITasks();

            timer.Start();

            sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
            udpClient.Send(sendBytes, sendBytes.Length);

            sendBytes = Encoding.ASCII.GetBytes("ET_REC\n");
            udpClient.Send(sendBytes, sendBytes.Length);

            //aTimer.Enabled = true;


        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;

            sendBytes = Encoding.ASCII.GetBytes("ET_STP\n");
            udpClient.Send(sendBytes, sendBytes.Length);

            sendBytes = Encoding.ASCII.GetBytes(String.Format("ET_SAV \"D:\\data\\openDoor\\{0}_{1}.idf\"\n", subjName, imageName));
            udpClient.Send(sendBytes, sendBytes.Length);

            sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
            udpClient.Send(sendBytes, sendBytes.Length);

            udpClient.Close();

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = null;
                this.Close();

            }));

        }

        public static void ProcessUITasks()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (object parameter)
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
                sendBytes = Encoding.ASCII.GetBytes("ET_STP\n");
                udpClient.Send(sendBytes, sendBytes.Length);

                sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
                udpClient.Send(sendBytes, sendBytes.Length);

                udpClient.Close();

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                    this.Close();

                }));
            }
            else if (e.Key == Key.J || e.Key == Key.F)
            {
                if (count == listUri.Count - 1)
                {
                    timer.Stop();
                    //System.Diagnostics.Debug.WriteLine("Time elapsed: {0:hh\\:mm\\:ss\\:ms}", timer.Elapsed);
                    ts = timer.Elapsed;
                    time = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                    overall.Stop();
                    overallTS = overall.Elapsed;
                    timeOverall = String.Format("{0:00}:{1:00}:{2:00}", overallTS.Minutes, overallTS.Seconds, overallTS.Milliseconds / 10);


                    sendBytes = Encoding.ASCII.GetBytes("ET_STP\n");
                    udpClient.Send(sendBytes, sendBytes.Length);

                    sendBytes = Encoding.ASCII.GetBytes(String.Format("ET_SAV \"D:\\data\\openDoor\\{0}_{1}.idf\"\n", subjName, count.ToString()));
                    udpClient.Send(sendBytes, sendBytes.Length);

                    sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
                    udpClient.Send(sendBytes, sendBytes.Length);

                    if (e.Key == Key.J)
                    {
                        answer = answer + "N" + Environment.NewLine;
                        answerTime = answerTime + time + Environment.NewLine;
                        answerTime = answerTime + "Overall: " + timeOverall;
                        udpClient.Close();

                        string appendText = answer + Environment.NewLine;
                        File.AppendAllText(path, appendText);

                        string appendTextTime = answerTime + Environment.NewLine;
                        File.AppendAllText(path, appendTextTime);

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            Mouse.OverrideCursor = null;
                            this.Close();

                        }));
                    }
                    else if (e.Key == Key.F)
                    {
                        answer = answer + "Y" + Environment.NewLine;
                        answerTime = answerTime + time + Environment.NewLine;
                        answerTime = answerTime + "Overall: " + timeOverall;
                        udpClient.Close();

                        string appendText = answer + Environment.NewLine;
                        File.AppendAllText(path, appendText);

                        string appendTextTime = answerTime + Environment.NewLine;
                        File.AppendAllText(path, appendTextTime);

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            Mouse.OverrideCursor = null;
                            this.Close();

                        }));
                    }
                }
               else if ((count + 1) % 8 == 0)
               {
                    if(cali == false)
                    {
                        if(e.Key == Key.F)
                        {
                            answer = answer + "Y" + Environment.NewLine;
                            answerTime = answerTime + time + Environment.NewLine;
                        }
                        else
                        {
                            answer = answer + "N" + Environment.NewLine;
                            answerTime = answerTime + time + Environment.NewLine;
                        }
                        overall.Stop();
                        timer.Stop();

                        sendBytes = Encoding.ASCII.GetBytes("ET_STP\n");
                        udpClient.Send(sendBytes, sendBytes.Length);

                        sendBytes = Encoding.ASCII.GetBytes(String.Format("ET_SAV \"D:\\data\\openDoor\\{0}_{1}.idf\"\n", subjName, count.ToString()));
                        udpClient.Send(sendBytes, sendBytes.Length);

                        sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
                        udpClient.Send(sendBytes, sendBytes.Length);

                        this.StimulusPane.Source = actCaliImg;

                        cali = true;
                    }
                    else
                    {
                        cali = false;

                        showFixationScreen();
                        count++;
                        actIm = new BitmapImage(listUri[count]);
                        this.StimulusPane.Source = actIm;

                        overall.Start();
                        timer.Reset();
                        timer.Start();

                        sendBytes = Encoding.ASCII.GetBytes("ET_REC\n");
                        udpClient.Send(sendBytes, sendBytes.Length);
                    }

                }
                else
                {
                    timer.Stop();
                    //System.Diagnostics.Debug.WriteLine("Time elapsed: {0:hh\\:mm\\:ss\\:ms}", timer.Elapsed);
                    ts = timer.Elapsed;
                    time = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);


                    sendBytes = Encoding.ASCII.GetBytes("ET_STP\n");
                    udpClient.Send(sendBytes, sendBytes.Length);

                    sendBytes = Encoding.ASCII.GetBytes(String.Format("ET_SAV \"D:\\data\\openDoor\\{0}_{1}.idf\"\n", subjName, count.ToString()));
                    udpClient.Send(sendBytes, sendBytes.Length);

                    sendBytes = Encoding.ASCII.GetBytes("ET_CLR\n");
                    udpClient.Send(sendBytes, sendBytes.Length);

                    if (e.Key == Key.J)
                    {
                        answer = answer + "N" + Environment.NewLine;
                        answerTime = answerTime + time + Environment.NewLine;

                        showFixationScreen();
                        count++;
                        actIm = new BitmapImage(listUri[count]);
                        this.StimulusPane.Source = actIm;

                        timer.Reset();
                        timer.Start();

                        sendBytes = Encoding.ASCII.GetBytes("ET_REC\n");
                        udpClient.Send(sendBytes, sendBytes.Length);

                    }
                    else if (e.Key == Key.F)
                    {
                        answer = answer + "Y" + Environment.NewLine;
                        answerTime = answerTime + time + Environment.NewLine;

                        showFixationScreen();
                        count++;
                        actIm = new BitmapImage(listUri[count]);
                        this.StimulusPane.Source = actIm;

                        timer.Reset();
                        timer.Start();

                        sendBytes = Encoding.ASCII.GetBytes("ET_REC\n");
                        udpClient.Send(sendBytes, sendBytes.Length);

                    }

                }




            }

        }

    }
}