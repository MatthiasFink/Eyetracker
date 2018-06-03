using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Data
{
    public class TestStatusConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || (String)parameter == "de")
            {
                switch ((String)value)
                {
                    case "NEW": return "Neu";
                    case "PRG": return "In Arbeit";
                    case "TRM": return "Abgeschlossen";
                }
            }
            else if ((String)parameter == "en")
            {
                switch ((String)value)
                {
                    case "NEW": return "New";
                    case "PRG": return "In Progress";
                    case "TRM": return "Finished";
                }
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || (String)parameter == "de")
            {
                switch ((String)value)
                {
                    case "Neu": return "NEW";
                    case "In Arbeit": return "PRG";
                    case "Abgeschlossen": return "TRM";
                }
            }
            else if ((String)parameter == "en")
            {
                switch ((String)value)
                {
                    case "New": return "NEW";
                    case "In Progress": return "PRG";
                    case "Finished": return "TRM";
                }
            }
            return ((String)value).Substring(0, 3);
        }
    }
}
