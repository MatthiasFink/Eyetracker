using System;
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
    /// Interaction logic for TutorialWindow.xaml
    /// </summary>
    public partial class TutorialWindow : Window
    {

        String subjName;
        String imageName;

        int count = 1;


        BitmapImage actIm;
        Uri actUrii;
        Uri actUriTy;

        List<Uri> listUri = new List<Uri>();


        private System.Timers.Timer aTimer;
        private int stimDur = 10000;
        private int fixDur = 1000;

        Byte[] sendBytes;


        private double crossSize = 50;
        private double crossThickness = 3;

        public TutorialWindow()
        {
            InitializeComponent();
        }

        public TutorialWindow(String _subjName) : this()
        {
            this.subjName = _subjName;
      

            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.Show();

            int fileCount = Directory.GetFiles("tut\\").Length;
            for (int i = 0; i < fileCount; i++)
            {
                actUrii = new Uri("tut\\" + i + ".png", UriKind.RelativeOrAbsolute);
                listUri.Add(actUrii);
            }

            actIm = new BitmapImage(listUri[0]);

            actUriTy = new Uri("ty\\thanks.png", UriKind.RelativeOrAbsolute);

            Mouse.OverrideCursor = Cursors.None;

            fixationScreen();

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

            ProcessUITasks();

            System.Threading.Thread.Sleep(fixDur);
            this.FixationPane.Visibility = Visibility.Hidden;
        }

        private void stimulusPresentation(BitmapImage _stimulus)
        {
            // Create a timer with a ten second interval.
            //aTimer = new System.Timers.Timer(stimDur);


            // Hook up the Elapsed event for the timer.
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            this.StimulusPane.Source = _stimulus;
            ProcessUITasks();

            //aTimer.Enabled = true;


        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;

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
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                    this.Close();

                }));
            }
            else if(e.Key == Key.Space || e.Key == Key.J || e.Key == Key.F)
            {

                if (count == listUri.Count)
                {

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Mouse.OverrideCursor = null;
                        this.Close();

                    }));
                    
                }
                else
                {
                    actIm = new BitmapImage(listUri[count]);
                    this.StimulusPane.Source = actIm;
                    count++;
                }

            }

        }
    }
}

