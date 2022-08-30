using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Model.DataType;

namespace Model.Service;

public class PlaylistManager
{
    // ==============
    // INITIALIZATION
    // ==============

    public PlaylistManager()
    {
        // Create the playlist root directory if it does not exist.
        // The root directory has to exist for the PlaylistViewModel to be correctly initialized.
        if (!Directory.Exists(AppConfig.PlaylistsRootPath))
        {
            Directory.CreateDirectory(AppConfig.PlaylistsRootPath);
        }
    }
    
    // ==============
    // PUBLIC METHODS 
    // ==============
    
    /// <summary>
    /// Creates the directory with the given name inside the given parent directory.
    /// If no parent directory is given, the directory will be created inside the playlist root directory.
    /// </summary>
    /// <param name="parentDirectoryPath">The parent directory path, might be null</param>
    /// <param name="directoryName">The name of the new playlist directory</param>
    /// <returns>The full path of the created directory</returns>
    public string CreatePlaylistDirectory(string? parentDirectoryPath, string directoryName)
    {
        // Create in '<parent directory>/<directory path>' or in '<playlists root directory>/<directory path>'
        string fullPath = this.GetFullPath(parentDirectoryPath, directoryName);
        Directory.CreateDirectory(fullPath);

        return fullPath;
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
    /// Returns the relative path for a playlist file seen from the playlist root path.
    /// </summary>
    /// <param name="parentDirectoryPath">The optional parent directory of the playlist</param>
    /// <param name="playlistName"></param>
    /// <returns></returns>
    public string GetRelativePathForNewPlaylist(string? parentDirectoryPath, string playlistName)
    {
        string fullPath = this.GetFullPath(parentDirectoryPath, $"{playlistName}.json");
        return Path.GetRelativePath(AppConfig.PlaylistsRootPath, fullPath);
    }

    /// <summary>
    /// Creates or updates the playlist file for the given playlist if the file path is set.
    /// </summary>
    /// <param name="playlist"></param>
    public void SaveInPlaylistFile(Playlist playlist)
    {
        // Only continue if the playlist has set a file path
        if (playlist.RelativePath == null)
        {
            Console.WriteLine($"No file path set for playlist '{playlist.Name}'");
            return;
        }
        
        // Convert playlist object to file object for only saving the necessary data
        List<string> songPaths = new List<string>();
        foreach (var song in playlist.Songs)
        {
            songPaths.Add(song.FilePath);
        }
        
        PlaylistFileData fileData = new PlaylistFileData(playlist.Name, songPaths, playlist.SortOrder.ToString());
        
        // Save playlist as JSON
        string fullPath = this.GetFullPath(playlist.RelativePath);
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            // pretty print
            WriteIndented = true
        };
        string dataJson = JsonSerializer.Serialize(fileData, options);
        File.WriteAllText(fullPath, dataJson, Encoding.UTF8);
    }
    
    /// <summary>
    /// Removes the file for the given playlist if existing.
    /// </summary>
    /// <param name="playlist"></param>
    public void RemovePlaylistFile(Playlist playlist)
    {
        if (playlist.RelativePath != null)
        {
            string fullPath = this.GetFullPath(playlist.RelativePath);
            File.Delete(fullPath);
        }
    }
    
    // ==============
    // EXPORT
    // ==============

    public void Export(string targetDirectoryPath, Playlist selectedPlaylist)
    {
        Debug.Assert(Directory.Exists(targetDirectoryPath));

        // Get the formatter for the index string. For example, 'D3' means each number is filled with leading zeros
        // like 001, 002 and so on.
        int digitCount = selectedPlaylist.Songs.Count.ToString().Length;
        String digitFormat = $"D{digitCount}";
        
        int songIndexOneBased = 1;
        foreach (var song in selectedPlaylist.Songs)
        {
            if (File.Exists(song.FilePath))
            {
                // Get the file name including the extension according to the sort order of the playlist.
                // If not considering the sort order, the files in the target directory might be in another order
                // as in the playlist.
                string songFileExtension = Path.GetExtension(song.FilePath);
                
                string targetSongFile;
                switch (selectedPlaylist.SortOrder)
                {
                    case PlaylistSortOrder.Alphabetical:
                        // Just use the title because directories are sorted alphabetical by default
                        targetSongFile = $"{song.Title}{songFileExtension}";
                        break;
                    case PlaylistSortOrder.Individual:
                    case PlaylistSortOrder.TitleNumber:
                        // Set index as prefix
                        string prefix = songIndexOneBased.ToString(digitFormat);
                        targetSongFile = $"{prefix} {song.Title}{songFileExtension}";
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
                
                // Copy the song to the computed target path.
                string targetSongPath = Path.GetFullPath(Path.Combine(targetDirectoryPath, targetSongFile));
                File.Copy(song.FilePath, targetSongPath);
                
                // Increase the index
                songIndexOneBased++;
            }
        }
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
    
    public string GetFullPath(string relativePath)
    {
        return Path.Combine(AppConfig.PlaylistsRootPath, relativePath);
    }
}