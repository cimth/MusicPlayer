using Model.DataType;

namespace Model.Service;

public class PlaylistImporter
{
    // ==============
    // PUBLIC METHODS
    // => Note: Only return playlists for unified logic
    // ==============
    
    public Playlist Import(string songName, string songPath)
    {
        // create list from single song
        List<Song> songs = new List<Song>();
        Song song = this.Import(songPath);
        songs.Add(song);
        
        return new Playlist(songName, songs);
    }

    public Playlist Import(string playlistName, List<string> songPaths)
    {
        List<Song> songs = new List<Song>();
        
        foreach (string songPath in songPaths)
        {
            songs.Add(this.Import(songPath));
        }
        
        return new Playlist(playlistName, songs);
    }
    
    // ==============
    // HELPING METHOD
    // ==============
    
    private Song Import(string songPath)
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