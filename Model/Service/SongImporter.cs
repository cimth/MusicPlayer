using Model.DataType;

namespace Model.Service;

public class SongImporter
{
    public Song Import(String songPath)
    {
        var songFile = TagLib.File.Create(songPath);

        string title = songFile.Tag.Title;
        string album = songFile.Tag.Album;
        string[] artists = songFile.Tag.Performers;
        uint trackNumber = songFile.Tag.Disc;
        TimeSpan duration = songFile.Properties.Duration;
        
        return new Song(songPath, title, album, artists, trackNumber, duration);
    }
}