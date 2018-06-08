using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EyetrackerExperiment
{
    /// <summary>
    /// Interaktionslogik für Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        Timer timer = new Timer(3000);

        public Splash()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Deactivated += Splash_Deactivated;
            MouseDown += Splash_Deactivated;
            KeyDown += Splash_Deactivated;

            Timer timer = new Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Deactivated -= Splash_Deactivated;
            MouseDown -= Splash_Deactivated;
            KeyDown -= Splash_Deactivated;
            Dispatcher.Invoke(Close);
        }

        private void Splash_Deactivated(object sender, EventArgs e)
        {
            Deactivated -= Splash_Deactivated;
            MouseDown -= Splash_Deactivated;
            KeyDown -= Splash_Deactivated;

            Close();
        }
    }
}
