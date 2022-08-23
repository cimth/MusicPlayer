using System.IO;
using System.Text.Json;
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
        Song song = this.ImportSong(songPath);
        songs.Add(song);
        
        return new Playlist(songName, songs);
    }

    public Playlist Import(string playlistName, List<string> songPaths)
    {
        List<Song> songs = new List<Song>();
        
        foreach (string songPath in songPaths)
        {
            songs.Add(this.ImportSong(songPath));
        }
        
        return new Playlist(playlistName, songs);
    }

    public Playlist ImportFromPlaylistFile(string filePath)
    {
        // Read file
        string playlistJson = File.ReadAllText(filePath);
        PlaylistFileData fileData = JsonSerializer.Deserialize<PlaylistFileData>(playlistJson) ?? throw new InvalidOperationException();
        
        // Create actual Song objects from the song paths in the file
        List<Song> songs = new List<Song>();
        foreach (var songPath in fileData.SongPaths)
        {
            songs.Add(this.ImportSong(songPath));
        }
        
        // return resolved Playlist
        return new Playlist(fileData.Name, songs);
    }
    
    // ==============
    // HELPING METHOD
    // ==============
    
    private Song ImportSong(string songPath)
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