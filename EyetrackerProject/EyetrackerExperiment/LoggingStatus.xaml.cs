using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Shapes;

namespace EyetrackerExperiment
{
    public enum Severity
    {
        Info = 0,
        Warning,
        Error,
        Critical
    }

    public class StatusMessage
    {
        public Severity Severity { get; set; }
        public String Message { get; set; }
        public String Detail { get; set; }
    }

    public class IconVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Severity ps = (Severity)int.Parse(parameter.ToString());

            return ps.Equals(value) ? Visibility.Visible : Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusMessages : ObservableCollection<StatusMessage> { };

    public partial class LoggingStatus : UserControl
    {
        private int keepMax = 100;
        public StatusMessages statusMessages { get; set; }

        public int KeepMax {
            get { return keepMax; }
            set
            {
                while (statusMessages.Count > value)
                    statusMessages.Remove(statusMessages.Last()); keepMax = value;
            }
        }

        public void PostMessage(Severity severity, String message, String detail = null)
        {
            statusMessages.Insert(0, new StatusMessage() { Severity = severity, Message = message, Detail = detail });
            if (statusMessages.Count > keepMax)
                statusMessages.Remove(statusMessages.Last());
            cbStatusList.SelectedIndex = 0;
        }

        public LoggingStatus()
        {
            InitializeComponent();
            statusMessages = new StatusMessages();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            { 
                PostMessage(Severity.Info, "Info Message");
                PostMessage(Severity.Warning, "Warning Message");
                PostMessage(Severity.Error, "Error Message");
                PostMessage(Severity.Critical, "Critical Message");
            }
            cbStatusList.DataContext = statusMessages;
        }
    }
}
