using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Converter;

public class EqualsConverter: IMultiValueConverter
{
    /// <summary>
    /// Compares the first two values for equality if they are not null.
    /// If one of the values is null, return false.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        //Console.WriteLine($"EqualsConverter: {values[0]} | {values[1]}");
        
        // Only continue if both values are given
        if (values[0] == null || values[1] == null)
        {
            return false;
        }

        // Both values not null, thus compare them and return the result
        return values[0]!.Equals(values[1]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}