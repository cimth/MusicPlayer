using System.Collections.ObjectModel;
using System.IO;
using Model.DataType;

namespace Model.Service;

public class PlaylistManager
{
    /// <summary>
    /// Creates the directory with the given name inside the given parent directory.
    /// If no parent directory is given, the directory will be created inside the playlist root directory.
    /// </summary>
    /// <param name="parentDirectoryPath">The parent directory path, might be null</param>
    /// <param name="directoryName">The name of the new playlist directory</param>
    public void CreatePlaylistDirectory(string? parentDirectoryPath, string directoryName)
    {
        // Create in '<parent directory>/<directory path>' or in '<playlists root directory>/<directory path>'
        string fullPath = parentDirectoryPath != null ? Path.Combine(parentDirectoryPath, directoryName) : Path.Combine(AppConfig.PlaylistsRootPath, directoryName);
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
        string fullPath = parentDirectoryPath != null ? Path.Combine(parentDirectoryPath, directoryName) : Path.Combine(AppConfig.PlaylistsRootPath, directoryName);
        Directory.Delete(fullPath, true);
    }
}