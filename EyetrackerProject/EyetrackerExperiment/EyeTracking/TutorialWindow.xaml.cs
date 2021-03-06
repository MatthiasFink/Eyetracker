﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace EyetrackerExperiment.EyeTracking
{
    /// <summary>
    /// Interaction logic for TutorialWindow.xaml
    /// </summary>
    public partial class TutorialWindow : Window
    {
        int imageNum = 0;
        List<BitmapImage> images = new List<BitmapImage>();

        public TutorialWindow() 
        {
            InitializeComponent();


            int i = 0;
            while (true)
            {
                double width = 0;
                BitmapImage image = null;
                try
                {
                    image = new BitmapImage(new Uri("pack://application:,,,/EyetrackerExperiment;component/eyetracking/tut/" + i++ + ".png"));
                    width = image.Width;
                }
                catch (Exception)
                { }

                if (width > 0)
                    images.Add(image);
                else
                    break;
            }

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

            if (images.Count > 0)
                StimulusPane.Source = images[imageNum];
            else
                Close();
            Mouse.OverrideCursor = Cursors.None;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Mouse.OverrideCursor = null;
                    Close();
                    break;
                case Key.Space:
                case Key.J:
                case Key.F:
                    if (imageNum >= images.Count - 1)
                    {
                        Mouse.OverrideCursor = null;
                        Close();
                    }
                    else
                        StimulusPane.Source = images[++imageNum]; 
                    break;
            }
        }
    }
}

