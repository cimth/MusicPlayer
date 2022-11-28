using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Common;
using Model.DataType;

namespace Model.Service;

public class PlaylistManager : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============
    
    private readonly AppConfigurator _appConfigurator;

    private bool _noDuplicates;
    
    // ==============
    // PROPERTIES
    // ==============

    public bool NoDuplicates
    {
        get => _noDuplicates;
        set
        {
            SetField(ref _noDuplicates, value);
            
            // Save in config to restore on restart
            this._appConfigurator.SaveNoDuplicates(this.NoDuplicates);
        } 
    }
    
    // ==============
    // INITIALIZATION
    // ==============

    public PlaylistManager(AppConfigurator appConfigurator)
    {
        // Load app config
        this._appConfigurator = appConfigurator;
        this._noDuplicates = this._appConfigurator.AppConfig.NoDuplicates;
        
        // Create the playlist root directory if it does not exist.
        // The root directory has to exist for the PlaylistViewModel to be correctly initialized.
        if (!Directory.Exists(AppConfig.PlaylistsRootPath))
        {
            Directory.CreateDirectory(AppConfig.PlaylistsRootPath);
        }
    }
    
    // ==============
    // PLAYLIST DIRECTORIES
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
    
    public void RenamePlaylistDirectory(string oldDirectoryName, string newDirectoryName, string? parentDirectoryPath)
    {
        string oldFullPath = this.GetFullPath(parentDirectoryPath, oldDirectoryName);
        string newFullPath = this.GetFullPath(parentDirectoryPath, newDirectoryName);

        Directory.Move(oldFullPath, newFullPath);
    }
    
    // ==============
    // PLAYLIST FILES
    // ==============
    
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

    public void RenamePlaylist(Playlist playlist, string newPlaylistName, string? parentDirectoryPath)
    {
        if (playlist.RelativePath != null)
        {
            // Remove the old file
            this.RemovePlaylistFile(playlist);
            
            // Update data
            string newRelativePath = this.GetRelativePathForNewPlaylist(parentDirectoryPath, newPlaylistName);
            playlist.Name = newPlaylistName;
            playlist.RelativePath = newRelativePath;
            
            // Save the new file
            this.SaveInPlaylistFile(playlist);
        }
    }
    
    // ==============
    // EXPORT
    // ==============

    public void ExportIntoDirectory(string targetDirectoryPath, Playlist selectedPlaylist)
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
    
    public void ExportAsM3U(string targetFilePath, Playlist selectedPlaylist)
    {
        // Get all song paths of the playlist.
        List<string> songPathsAbsolute = new List<string>();
        foreach (var song in selectedPlaylist.Songs)
        {
            songPathsAbsolute.Add(song.FilePath);
        }
        
        // Compute the relative paths from the target file directory.
        string? targetDirectoryPath = Path.GetDirectoryName(targetFilePath);
        List<string> songPathsRelative = new List<string>();
        
        if (targetDirectoryPath != null)
        {
            
            foreach (var songPath in songPathsAbsolute)
            {
                songPathsRelative.Add(Path.GetRelativePath(targetDirectoryPath, songPath));
            }
        }
        
        // Just write the paths into the target file.
        // A M3U file only contains those paths.
        // For more information, see: https://en.wikipedia.org/wiki/M3U
        //
        // Note: Do not encode the file.
        // - If encoding in Unicode, it will work in the 'Groove' app but not in another tested app.
        // - If encoding in UTF-8, it will work in no tested app.
        // - If not encoding, it will work in the 'Groove' and 'Samsung Music' app but not in 'Windows Media Player'.
        //   This seems to be the best way by now.
        File.WriteAllLines(targetFilePath, songPathsRelative);
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