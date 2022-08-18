using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Converter;

public class DynamicExpanderToRowHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            return isExpanded ? "*" : "Auto";
        }
        return "*";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}