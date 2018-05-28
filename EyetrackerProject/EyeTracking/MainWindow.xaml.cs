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
using System.Windows.Navigation;
using System.IO;

namespace EyeTrackingDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

        }

		private void runExp_Click(object sender, RoutedEventArgs e)
		{
            PresentationWindow stimWin = new PresentationWindow(this.subjectName.Text);	
		}

        private void runExp_ClickDE(object sender, RoutedEventArgs e)
        {
            PresentationWindowDe stimWin = new PresentationWindowDe(this.subjectName.Text);
        }

        private void calibrateButton_Click(object sender, RoutedEventArgs e)
		{
			CalibrationWindow calWin = new CalibrationWindow();
		}

        private void tut_Click(object sender, RoutedEventArgs e)
        {
            TutorialWindow tutWin = new TutorialWindow(this.subjectName.Text);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

    }
}
