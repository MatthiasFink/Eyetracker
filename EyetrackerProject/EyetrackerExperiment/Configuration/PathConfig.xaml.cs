using EyetrackerExperiment.Properties;
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
    /// Interaktionslogik für PathConfig.xaml
    /// </summary>
    public partial class PathConfig : Window
    {
        internal Settings settings { get; set; }

        public PathConfig()
        {
            InitializeComponent();
            settings = Settings.Default;
            DataContext = settings;
        }

        private void bnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender == bnSelectAnswerPath) ? tbAnswerPath : tbTrackingPath;
            String newPath = FileDialogs.SelectFolder(tb.Text);
            if (newPath != null)
                tb.Text = newPath;
        }

        private void bnOk_Click(object sender, RoutedEventArgs e)
        {
            settings.Save();
            Close();
        }
    }
}
