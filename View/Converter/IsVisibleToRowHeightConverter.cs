using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Converter;

public class IsVisibleToRowHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isVisible)
        {
            return isVisible ? "*" : "Auto";
        }
        return "Auto";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}