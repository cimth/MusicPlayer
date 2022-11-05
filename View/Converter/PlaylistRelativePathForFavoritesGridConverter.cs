using System;
using System.Globalization;
using System.Windows.Data;
using Path = System.IO.Path;

namespace View.Converter;

public class PlaylistRelativePathForFavoritesGridConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string relativePath)
        {
            string pathWithoutFileExtension = relativePath.Replace(".json", "");
            
            // Init result variables for the case that there is only the playlist name without a directory
            string directories = "";
            string separator = "";
            string playlistName = pathWithoutFileExtension;
            
            // Separate the playlist directory from the playlist name when the relative path contains a directory
            int lastDirectorySeparatorIndex = pathWithoutFileExtension.LastIndexOf(Path.DirectorySeparatorChar);
            if (lastDirectorySeparatorIndex != -1)
            {
                directories = pathWithoutFileExtension.Substring(0, lastDirectorySeparatorIndex);
                separator = " | ";
                playlistName = pathWithoutFileExtension.Substring(lastDirectorySeparatorIndex + 1);
            }

            // Example: "Mixes/Quiet Mix.json" will result in "Mixes | Quiet Mix"
            return $"{directories}{separator}{playlistName}";
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}