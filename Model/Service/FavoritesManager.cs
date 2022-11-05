using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Common;
using Model.DataType;

namespace Model.Service;

public class FavoritesManager : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============
    
    private ObservableCollection<string> _favoriteDirectoryPaths;
    private ObservableCollection<string> _favoritePlaylistRelativePaths;
    
    // ==============
    // PROPERTIES
    // ==============

    public ObservableCollection<string> FavoriteDirectoryPaths
    {
        get => _favoriteDirectoryPaths;
        set => SetField(ref _favoriteDirectoryPaths, value);
    }
    
    public ObservableCollection<string> FavoritePlaylistRelativePaths
    {
        get => _favoritePlaylistRelativePaths;
        set => SetField(ref _favoritePlaylistRelativePaths, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============

    public FavoritesManager()
    {
        if (File.Exists(AppConfig.FavoritesFilePath))
        {
            // Get paths from Favorites file
            FavoritesFileData fileData = this.ReadFavoritesFromFile();
            this._favoriteDirectoryPaths = new ObservableCollection<string>(fileData.DirectoryPaths);
            this._favoritePlaylistRelativePaths = new ObservableCollection<string>(fileData.PlaylistRelativePaths);
        }
        else
        {
            // No Favorites file, thus create empty lists
            this._favoriteDirectoryPaths = new ObservableCollection<string>();
            this._favoritePlaylistRelativePaths = new ObservableCollection<string>();
        }
    }

    private FavoritesFileData ReadFavoritesFromFile()
    {
        Debug.Assert(File.Exists(AppConfig.FavoritesFilePath));
        
        string jsonData = File.ReadAllText(AppConfig.FavoritesFilePath);
        return JsonSerializer.Deserialize<FavoritesFileData>(jsonData) ?? throw new InvalidOperationException();
    }
    
    // ==============
    // SAVE CHANGES
    // ==============

    private void SaveChanges()
    {
        // Create file data object
        List<string> directoryPaths = this.FavoriteDirectoryPaths.ToList();
        List<string> playlistRelativePaths = this.FavoritePlaylistRelativePaths.ToList();
        
        FavoritesFileData fileData = new FavoritesFileData(directoryPaths, playlistRelativePaths);

        // Save
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            // pretty print
            WriteIndented = true
        };
        string dataJson = JsonSerializer.Serialize(fileData, options);
        File.WriteAllText(AppConfig.FavoritesFilePath, dataJson, Encoding.UTF8);
    }
    
    // ==============
    // UPDATE DIRECTORIES
    // ==============

    public void AddDirectoryToFavorites(string directoryPath)
    {
        Debug.Assert(directoryPath != null);
        
        this.FavoriteDirectoryPaths.Add(directoryPath);
        SortUtil.SortWithoutNewCollection(this.FavoriteDirectoryPaths, path => path);
        
        this.SaveChanges();
    }
    
    public void RemoveDirectoryFromFavorites(string directoryPath)
    {
        this.FavoriteDirectoryPaths.Remove(directoryPath);
        this.SaveChanges();
    }
    
    // ==============
    // UPDATE PLAYLISTS
    // ==============
    
    public void AddPlaylistToFavorites(Playlist playlist)
    {
        Debug.Assert(playlist.RelativePath != null);
        
        this.FavoritePlaylistRelativePaths.Add(playlist.RelativePath);
        SortUtil.SortWithoutNewCollection(this.FavoritePlaylistRelativePaths, path => path);
        
        this.SaveChanges();
    }
    
    public void RemovePlaylistFromFavorites(Playlist playlist)
    {
        Debug.Assert(playlist.RelativePath != null);
        
        this.FavoritePlaylistRelativePaths.Remove(playlist.RelativePath);
        this.SaveChanges();
    }
}