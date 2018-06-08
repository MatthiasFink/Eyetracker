using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EyetrackerExperiment
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void LaunchSplash(object sender, EventArgs e)
        {
            Splash splash = new Splash();
            splash.Show();
            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Focus();
        }
    }
}
