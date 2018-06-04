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
            if (culture.IetfLanguageTag.StartsWith("de"))
            {
                switch ((String)value)
                {
                    case "NEW": return "Neu";
                    case "PRG": return "In Arbeit";
                    case "TRM": return "Abgeschlossen";
                }
            }
            else if (culture.IetfLanguageTag.StartsWith("en"))
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
            if (culture.IsNeutralCulture || culture.IetfLanguageTag == "DE-de")
            {
                switch ((String)value)
                {
                    case "Neu": return "NEW";
                    case "In Arbeit": return "PRG";
                    case "Abgeschlossen": return "TRM";
                }
            }
            else
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

    public class StringEqualityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String parameterString = (String)parameter;
            if (parameterString == null)
                return System.Windows.DependencyProperty.UnsetValue;

            return parameterString.Equals(value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Boolean)value ? parameter : System.Windows.DependencyProperty.UnsetValue;
        }
    }

    public class IntEqualityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String parameterString = (String)parameter;
            if (parameterString == null)
                return System.Windows.DependencyProperty.UnsetValue;

            return parameterString.Equals(value.ToString());
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Boolean)value ? parameter : System.Windows.DependencyProperty.UnsetValue;
        }
    }
}