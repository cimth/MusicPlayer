using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using Model.DataType;

namespace Model.Service;

public class PlaylistManager
{
    // ==============
    // PUBLIC METHODS 
    // ==============
    
    /// <summary>
    /// Creates the directory with the given name inside the given parent directory.
    /// If no parent directory is given, the directory will be created inside the playlist root directory.
    /// </summary>
    /// <param name="parentDirectoryPath">The parent directory path, might be null</param>
    /// <param name="directoryName">The name of the new playlist directory</param>
    public void CreatePlaylistDirectory(string? parentDirectoryPath, string directoryName)
    {
        // Create in '<parent directory>/<directory path>' or in '<playlists root directory>/<directory path>'
        string fullPath = this.GetFullPath(parentDirectoryPath, directoryName);
        Directory.CreateDirectory(fullPath);
    }
    
    /// <summary>
    /// Removes the directory with the given name from the given parent directory.
    /// If no parent directory is given, the directory will be removed from the playlist root directory.
    /// </summary>
    /// <param name="parentDirectoryPath"></param>
    /// <param name="directoryName"></param>
    public void RemovePlaylistDirectory(string? parentDirectoryPath, string directoryName)
    {
        // Remove from in '<parent directory>/<directory path>' or '<playlists root directory>/<directory path>'
        string fullPath = this.GetFullPath(parentDirectoryPath, directoryName);
        Directory.Delete(fullPath, true);
    }

    /// <summary>
    /// Creates or updates the playlist file for the given playlist inside the given parent directory.
    /// If no parent directory is given, the file will be created inside the playlist root directory.
    /// </summary>
    /// <param name="parentDirectoryPath"></param>
    /// <param name="playlist"></param>
    public void SaveInPlaylistFile(string? parentDirectoryPath, Playlist playlist)
    {
        // Get full path '<parent directory>/<directory path>' or '<playlists root directory>/<directory path>'
        string fileName = playlist.Name + ".json";
        string fullPath = this.GetFullPath(parentDirectoryPath, fileName);
        
        // Convert playlist object to file object for only saving the necessary data
        List<string> songPaths = new List<string>();
        foreach (var song in playlist.Songs)
        {
            songPaths.Add(song.FilePath);
        }
        
        PlaylistFileData fileData = new PlaylistFileData(playlist.Name, songPaths);
        
        // Save playlist as JSON
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            // pretty print
            WriteIndented = true
        };
        string dataJson = JsonSerializer.Serialize(fileData, options);
        File.WriteAllText(fullPath, dataJson, Encoding.UTF8);
    }
    
    /// <summary>
    /// Removes the file corresponding to the given playlist from the given parent directory.
    /// If no parent directory is given, the directory will be removed from the playlist root directory.
    /// </summary>
    /// <param name="parentDirectoryPath"></param>
    /// <param name="playlist"></param>
    public void RemovePlaylistFile(string? parentDirectoryPath, Playlist playlist)
    {
        // Get full path '<parent directory>/<directory path>' or '<playlists root directory>/<directory path>'
        string fileName = playlist.Name + ".json";
        string fullPath = this.GetFullPath(parentDirectoryPath, fileName);
        
        File.Delete(fullPath);
    }
    
    // ==============
    // HELPING METHODS 
    // ==============
    
    private string GetFullPath(string? parentDirectoryPath, string fileOrDirectoryName)
    {
        // Return '<parent directory>/<file or directory name>' if a parent directory is given 
        if (parentDirectoryPath != null)
        {
            return Path.Combine(parentDirectoryPath, fileOrDirectoryName);
        }
        
        // Return '<playlists root directory>/<file or directory name>' if no parent directory is given
        return Path.Combine(AppConfig.PlaylistsRootPath, fileOrDirectoryName);
    }
}