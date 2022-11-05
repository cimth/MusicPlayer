using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Converter;

public class IsNotEmptyStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str && str.Trim().Length > 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}