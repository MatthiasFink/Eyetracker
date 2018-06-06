using EyetrackerExperiment.Properties;
using System;
using System.Collections.Generic;
using System.IO;
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
            settings = new Settings();
            DataContext = settings;
        }

        private void bnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender == bnSelectAnswerPath) ? tbAnswerPath : (sender == tbTrackingPathLocal ? tbTrackingPathLocal : tbTrackingPathRemote);
            String newPath = FileDialogs.SelectFolder(tb.Text);
            if (newPath != null && System.IO.Path.IsPathRooted(newPath) && !newPath.StartsWith("\\\\"))
            {
                DriveInfo di = new DriveInfo(newPath.Substring(0, 2));
                if (di.DriveType == DriveType.Network)
                {
                    System.Management.SelectQuery sq = new System.Management.SelectQuery("Win32_LogicalDisk");
                    System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(sq);
                    foreach (System.Management.ManagementObject drive in mos.Get())
                    {
                        String path = drive.Path.ToString();
                        if (path.Contains(newPath.Substring(0,2)))
                        {
                            System.Management.ManagementObject nwd = new System.Management.ManagementObject(drive.Path);
                            UInt32 driveType = Convert.ToUInt32(nwd["DriveType"]);
                            foreach (System.Management.PropertyData prop in nwd.Properties)
                                if (prop.Name == "ProviderName")
                                {
                                    newPath = Convert.ToString(prop.Value) + newPath.Remove(0, 2);
                                    break;
                                }
                        }
                    }
                }

                tb.Text = newPath;
            }
        }

        private void bnOk_Click(object sender, RoutedEventArgs e)
        {
            settings.Save();
            Close();
        }
    }
}
