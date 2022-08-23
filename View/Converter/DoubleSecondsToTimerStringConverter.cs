using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace View.Converter;

[ValueConversion(typeof(double), typeof(string))]
public class DoubleSecondsToTimerStringConverter : IValueConverter
{
    /// <summary>
    /// Converts the given value which should be a double representing seconds to a timer string like "mm:ss".
    /// As an example, the double 122 would result in "2:02".
    /// </summary>
    /// <param name="value">the value to convert</param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double timeInSeconds)
        {
            String targetFormat = timeInSeconds >= 3600 ? @"hh\:mm\:ss" : @"mm\:ss";
            return TimeSpan.FromSeconds(timeInSeconds).ToString(targetFormat);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}