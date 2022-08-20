using System;
using System.Globalization;
using System.Windows.Data;
using Path = System.IO.Path;

namespace View.Converter;

public class FullPathToLastPathPartConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string path)
        {
            // Returns the file name or the directory name if the path leads to a directory
            // (the method's name is a little irritating as it seems only to work on files)
            return Path.GetFileName(path);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}