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

        public void Log(Severity severity, String message, String detail = null)
        {
            Dispatcher.Invoke(delegate ()
            {
                if (MainWindow != null && MainWindow.IsLoaded)
                {
                    EyetrackerExperiment.MainWindow mainWindow = (EyetrackerExperiment.MainWindow)MainWindow;

                    mainWindow.Dispatcher.Invoke(new Action(() => mainWindow.LoggingStatus.PostMessage(severity, message, detail)));
                }
            }, System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}
