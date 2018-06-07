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
        int imageNum = 0;
        List<Uri> listUri = new List<Uri>();

        public TutorialWindow()
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            Topmost = true;
            Show();

            foreach (String fileName in Directory.GetFiles("tut\\"))
                listUri.Add(new Uri("tut\\" + fileName, UriKind.RelativeOrAbsolute));

            StimulusPane.Source = new BitmapImage(listUri[0]);
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
                    if (imageNum >= listUri.Count)
                    {
                        Mouse.OverrideCursor = null;
                        Close();
                    }
                    else
                    {
                        StimulusPane.Source = new BitmapImage(listUri[imageNum]);
                        imageNum++;
                    }
                    break;
            }
        }
    }
}

