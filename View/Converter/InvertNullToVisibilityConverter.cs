using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace View.Converter;

public class InvertNullToVisibilityConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility result = value != null ? Visibility.Collapsed : Visibility.Visible;
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}