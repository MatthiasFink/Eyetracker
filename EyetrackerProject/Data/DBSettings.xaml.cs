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

namespace Data
{
    /// <summary>
    /// Interaktionslogik für DBSettings.xaml
    /// </summary>
    public partial class DBSettings : Window
    {
        public DBSettings()
        {
            InitializeComponent();
        }

        private void bnTest_Click(object sender, RoutedEventArgs e)
        {
            Cursor cursorBefore = this.Cursor;
            this.Cursor = Cursors.AppStarting;
            Properties.Settings.Default.Save();
            bool dbOk = false;
            EyetrackerEntities db;
            try
            {
                db = new EyetrackerEntities(null);
                db.Database.Connection.Open();
                dbOk = db.Database.Connection.State == System.Data.ConnectionState.Open;
                statusMsg.Content = "Connection to " + db.Database.Connection.Site + " successful";
                statusBar.Background = Brushes.LightGreen;
                statusToolTipTitle.Content = "Success";
                statusToolTipText.Content = EyetrackerEntities.buildConnString();
                statusMsg.ToolTip = null;
                bnOk.IsEnabled = true;
            }
            catch (Exception ex)
            {
                statusMsg.Content = "Connection to " + Properties.Settings.Default.DBCatalog + "@" + 
                    Properties.Settings.Default.DBServer + " failed";
                statusBar.Background = Brushes.Pink;
                statusToolTipTitle.Content = "Exception occurred";
                statusToolTipText.Content = ex.Message;
                if (ex.InnerException != null) 
                    statusToolTipText.Content += "\n" + ex.InnerException.Message;
                bnOk.IsEnabled = false;
                dbOk = false;
                db = null;
            }
            this.Cursor = cursorBefore;
        }

        private void bnCancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bnCancel.Content = "Quit";
            bnOk.Content = "Continue";
            bnOk.IsEnabled = false;
        }

        private void bnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static bool? ConfigureDB()
        {
            DBSettings dbSettings = new DBSettings();
            return dbSettings.ShowDialog();
        }
    }
}
