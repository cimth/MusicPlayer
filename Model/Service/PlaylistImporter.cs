using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Model.DataType;

namespace Model.Service;

public class PlaylistImporter
{
    // ==============
    // FIELDS
    // ==============

    private SongImporter _songImporter;
    
    // ==============
    // INITIALIZATION
    // ==============

    public PlaylistImporter(SongImporter songImporter)
    {
        this._songImporter = songImporter;
    }
    
    // ==============
    // METHODS
    // ==============
    
    public Playlist Import(string songName, string songPath)
    {
        // create list from single song
        ObservableCollection<Song> songs = new();
        Song song = this._songImporter.Import(songPath);
        songs.Add(song);
        
        return new Playlist(songName, songs);
    }

    public Playlist Import(string playlistName, List<string> songPaths)
    {
        ObservableCollection<Song> songs = new();
        
        foreach (string songPath in songPaths)
        {
            songs.Add(this._songImporter.Import(songPath));
        }
        
        return new Playlist(playlistName, songs);
    }

    public Playlist ImportFromPlaylistFile(string filePath)
    {
        // Read file
        string playlistJson = File.ReadAllText(filePath);
        PlaylistFileData fileData = JsonSerializer.Deserialize<PlaylistFileData>(playlistJson) ?? throw new InvalidOperationException();
        
        // Create actual Song objects from the song paths in the file
        ObservableCollection<Song> songs = new();
        foreach (var songPath in fileData.SongPaths)
        {
            songs.Add(this._songImporter.Import(songPath));
        }
        
        // return resolved Playlist
        return new Playlist(fileData.Name, songs);
    }
}