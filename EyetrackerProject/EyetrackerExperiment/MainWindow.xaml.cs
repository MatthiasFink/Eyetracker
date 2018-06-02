using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyetrackerExperiment
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public EyetrackerEntities db;
 
        public MainWindow()
        {
            InitializeComponent();

            db = EyetrackerEntities.EyeTrackerDB;

            LoadData();
        }

        public void LoadData()
        {
            db.LoadAllCandidatesAndTests();
            DataContext = db;
            gridTests.UpdateLayout();
        }

        private void bnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (DBSettings.ConfigureDB().Value)
            {
                LoadData();
            }
        }
    }
}
