using System.IO;
using Model.DataType;

namespace Model.Service;

public class SongImporter
{
    /// <summary>
    /// Returns a full Song object for the song under the given path.
    /// <para/>
    /// If the path does not exist, a dummy object will be returned with the value of <see cref="Song.IsFileMissing"/>
    /// set to true and with the file path as title so that the error can be shown to the user directly inside a
    /// playlist.
    /// </summary>
    /// <param name="songPath">The song path.</param>
    /// <returns>The full Song object or a dummy Song object if the file path does not exist.</returns>
    public Song Import(string songPath)
    {
        // When there is no file under the given path, return a dummy object that indicates that the file is missing.
        // This dummy object is used because it can still be shown in the View next to the existing songs. So, missing
        // files will be visible for the user.
        if (!File.Exists(songPath))
        {
            return new Song(songPath);
        }
        
        // Read song file
        var songFile = TagLib.File.Create(songPath);

        // Import straight forward attributes
        string title = songFile.Tag.Title;
        string album = songFile.Tag.Album;
        string[] artists = songFile.Tag.Performers;
        uint trackNumber = songFile.Tag.Track;
        TimeSpan duration = songFile.Properties.Duration;

        // Sometimes the ";" separator does not get recognized correctly by TagLib, thus split manually
        List<string> allArtists = new();
        foreach (var artist in artists)
        {
            string[] splitArtists = artist.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            allArtists.AddRange(splitArtists);
        }

        // Return an internal Song object
        return new Song(songPath, title, album, allArtists.ToArray(), trackNumber, duration);
    }
}