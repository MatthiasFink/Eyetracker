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

namespace EyetrackerExperiment.Configuration
{
    /// <summary>
    /// Interaktionslogik für EyeTrackerConfig.xaml
    /// </summary>
    public partial class EyeTrackerConfig : Window
    {
        Properties.Settings settings;

        public EyeTrackerConfig()
        {
            InitializeComponent();
            settings = Properties.Settings.Default;
            DataContext = settings;
        }

        private void bnOk_Click(object sender, RoutedEventArgs e)
        {
            settings.Save();
            Close();
        }
    }
}
