using Model.DataType;

namespace Model.Service;

public class SongImporter
{
    public Song Import(string songPath)
    {
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