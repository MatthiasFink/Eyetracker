using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using Data;
using System.Linq;

namespace EyetrackerExperiment.EyeTracking
{
    /// <summary>
    /// Interaction logic for PresentationWindow.xaml
    /// </summary>
    public partial class PresentationWindow : Window
    {
        public Test Test { get; set; }
        public EyetrackerEntities Db;
        String subjName;
        String trackingBase;

        BitmapImage actCaliImg;
        bool cali = false;

        int slideNum;
        List<Slide> slides;
        String shortCuts = "";
        DateTime slideStart;

        Stopwatch overall = new Stopwatch();
        Stopwatch timer = new Stopwatch();
        Timer fixationTimer = new Timer(1000);

        EyeTrackingController eyeTracker;


        public PresentationWindow(EyetrackerEntities db, Test test) : base()
        {
            InitializeComponent();

            Db = db;
            Test = test;

            slides = new List<Slide>();
            foreach (Slide s in Test.Test_Definition.Slide.OrderBy(s => s.num))
                slides.Add(s);

            subjName = test.Candidate.personal_code;

            Properties.Settings settings = new Properties.Settings();
            trackingBase = settings.TrackingPathLocal;

            actCaliImg = new BitmapImage(new Uri("..\\Resources\\Calibration.png", UriKind.RelativeOrAbsolute));

            String Ip = settings.EyetrackerIP;
            int Port = settings.EyetrackerPort;
            eyeTracker = new EyeTrackingController(Ip, Port);
            eyeTracker.Start();

            Mouse.OverrideCursor = Cursors.None;
            fixationTimer = new Timer(1000);
            fixationTimer.Elapsed += hideFixationScreen;

            slideNum = -1;
            NextSlide();


            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            Topmost = true;

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens.FirstOrDefault(s => !s.Primary);
            if (screen != null)
            {
                Top = screen.WorkingArea.Top;
                Left = screen.WorkingArea.Left;
                Width = screen.WorkingArea.Width;
                Height = screen.WorkingArea.Height;
            }
            Show();
            WindowState = WindowState.Maximized;

            overall.Start();
        }

        public static String TrackingFilePattern = "{0}\\{1}-{2}-{3}.idf"; // Public static, da auch beim Einlesen verwendbar

        private String localTrackingFile
        {
            get
            {
                return String.Format(TrackingFilePattern, trackingBase, subjName, Test.id, slides[slideNum].num);
            }
        }

        private void hideFixationScreen(Object sender, ElapsedEventArgs e)
        {
            fixationTimer.Stop();
            Dispatcher.Invoke((Action)delegate ()
            {
                FixationPane.Visibility = Visibility.Hidden;
                timer.Start();
                slideStart = DateTime.Now;
            });
        }

        private void showFixationScreen()
        {
            overall.Stop();
            FixationPane.Visibility = Visibility.Visible;
            fixationTimer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            eyeTracker.StopTracking();
            eyeTracker.SaveTracking(localTrackingFile);
            eyeTracker.ClearTracking();
            eyeTracker.Stop();

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = null;
                Close();

            }));
        }

        private void AbortExperiment()
        {
            eyeTracker.StopTracking();
            eyeTracker.ClearTracking();
            eyeTracker.Stop();

            ((App)App.Current).Log(Severity.Warning, String.Format("Eyetracker test for {0} aborted.", Test.Candidate.personal_code));

            Mouse.OverrideCursor = null;
            this.Close();
        }

        private void FinishExperiment()
        {
            eyeTracker.StopTracking();
            eyeTracker.ClearTracking();
            eyeTracker.Stop();

            foreach (Slide_Answer sa in Test.Slide_Answer)
            {
                Db.Slide_Answer.Add(sa);
            }
            Test.LastStep = Test.Test_Definition.EyeTrackerStep;
            if (Test.LastStep == Test.NumSteps)
            {
                Test.status_cd = "TRM";
                Test.end_time = DateTime.Now;
            }
            Db.SaveChanges();

            ((App)App.Current).Log(Severity.Info, String.Format("Eyetracker test for {0} successfully completed.", Test.Candidate.personal_code));

            Mouse.OverrideCursor = null;
            Close();
        }

        private bool NextSlide()
        {
            if (slideNum >= slides.Count - 1)
                return false;

            if ((slideNum + 2) % 8 == 0 && !cali)
            {
                cali = true;
                StimulusPane.Source = actCaliImg;
            } 
            else
            {
                cali = false;
                slideNum++;
                StimulusPane.Source = slides[slideNum].SlideBitmapImage;
                shortCuts = "";
                foreach (String s in slides[slideNum].Slide_Choice.Select(sc => sc.shortcut))
                    shortCuts += s.ToUpper();
                eyeTracker.ClearTracking();
                eyeTracker.StartTracking();
                showFixationScreen();
            }
            return true;
        }

        private void ProcessAnswer(char c)
        {
            eyeTracker.StopTracking();
            eyeTracker.SaveTracking(localTrackingFile);
            eyeTracker.ClearTracking();

            foreach (Slide_Choice sc in slides[slideNum].Slide_Choice)
            {
                if (sc.shortcut.Contains(c))
                {
                    Slide_Answer sa = new Slide_Answer();
                    sa.Slide_Choice = sc;
                    sa.Slide = slides[slideNum];
                    sa.slide_start_time = slideStart;
                    sa.slide_end_time = DateTime.Now;
                    sa.Test = Test;
                    Test.Slide_Answer.Add(sa);
                    break;
                }
            }
            if (!NextSlide())
            {
                FinishExperiment();
            }
        }

        private void Reactivated(object sender, EventArgs e)
        {
            WindowState = WindowState.Maximized;
            Activated -= Reactivated;
            Topmost = true;
            if (!cali)
                eyeTracker.StartTracking();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                AbortExperiment();
                return;
            }

            if (e.Key == Key.F1)
            {
                Topmost = false;
                new TutorialWindow();
                Activated += Reactivated;
                if (!cali)
                    eyeTracker.StopTracking();
            }

            if (cali)
            {
                if (!NextSlide())
                    FinishExperiment();
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                char c = (char)((int)'0' + (e.Key - Key.D0));
                if (shortCuts.Contains(c))
                    ProcessAnswer(c);
            }
            else if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                char c = (char)((int)'A' + (e.Key - Key.A));
                if (shortCuts.Contains(c))
                    ProcessAnswer(c);
            }
        }

    }
}