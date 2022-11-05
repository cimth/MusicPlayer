using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace View.Converter;

public class StringArrayToCommaSeparatedStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string[] array)
        {
            return String.Join(", ", array);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}