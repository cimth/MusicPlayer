using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Input;
using Common;
using Model.Service;
using ViewModel.Command;

namespace ViewModel.MainContent;

public class FavoritesViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============

    private readonly AppConfigurator _appConfigurator;
    private readonly FavoritesManager _favoritesManager;
    private readonly MainWindowViewModel _mainWindowViewModel;

    private ListCollectionView _groupedDirectoryPairs;

    // ==============
    // PROPERTIES
    // ==============

    // Use pairs of <root path, sub path> to group the directories by their root path
    public ListCollectionView GroupedDirectoryPairs
    {
        get => _groupedDirectoryPairs; 
        set => SetField(ref _groupedDirectoryPairs, value);
    }

    public ObservableCollection<string> PlaylistPaths => _favoritesManager.FavoritePlaylistRelativePaths;

    // A pair <root path, sub path> of the selected directory. The paths have to be combined to get a full path.
    public KeyValuePair<string, string>? SelectedDirectoryPair { get; set; }

    public string? SelectedPlaylistPath { get; set; }
    
    // Commands
    
    public ICommand OpenDirectoryCommand { get; }
    
    public ICommand OpenPlaylistCommand { get; }

    // ==============
    // INITIALIZATION
    // ==============

    public FavoritesViewModel(AppConfigurator appConfigurator, FavoritesManager favoritesManager, MainWindowViewModel mainWindowViewModel)
    {
        this._appConfigurator = appConfigurator;
        this._favoritesManager = favoritesManager;
        this._mainWindowViewModel = mainWindowViewModel;
        
        // Init commands
        this.OpenDirectoryCommand = new DelegateCommand(this.OpenDirectory);
        this.OpenPlaylistCommand = new DelegateCommand(this.OpenPlaylist);
        
        // Init the Dictionary with <full path, relative path> entries where the relative paths are seen from the
        // music root directories
        this._groupedDirectoryPairs = this.InitGroupedDirectoryPairs();
        
        // Update the relative paths when the music directories or the favorite directories are changed
        this._appConfigurator.AppConfig.MusicDirectories.CollectionChanged += UpdateRelativePaths;
        this._favoritesManager.FavoriteDirectoryPaths.CollectionChanged += UpdateRelativePaths;
    }
    
    private ListCollectionView InitGroupedDirectoryPairs()
    {
        // Create pairs <root path, sub path> for the full paths which are set as favorite directory paths
        List<KeyValuePair<string, string>> pathGroups = new();
        
        ObservableCollection<string> rootDirectories = this._appConfigurator.AppConfig.MusicDirectories;
        foreach (var directory in rootDirectories)
        {
            foreach (var fullPath in this._favoritesManager.FavoriteDirectoryPaths)
            {
                // Add pair <full path, relative path>
                if (fullPath.StartsWith(directory))
                {
                    KeyValuePair<string, string> pair = new(directory, Path.GetRelativePath(directory, fullPath));
                    pathGroups.Add(pair);
                }
            }
        }
        
        // Sort by root paths
        SortUtil.SortWithoutNewCollection(pathGroups, pair => pair.Key);
        
        // Group the directory pairs by their key, i.e. group them by the root directory.
        // See: https://www.wpftutorial.net/datagrid.html#grouping
        ListCollectionView groupedDirectoryPairs = new ListCollectionView(pathGroups);
        PropertyGroupDescription groupDescription = new PropertyGroupDescription("Key");
        groupedDirectoryPairs.GroupDescriptions?.Add(groupDescription);

        return groupedDirectoryPairs;
    }
    
    // ==============
    // UPDATE DIRECTORY PATHS
    // ==============

    private void UpdateRelativePaths(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.GroupedDirectoryPairs = this.InitGroupedDirectoryPairs();
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void OpenDirectory()
    {
        if (this.SelectedDirectoryPair != null)
        {
            // Get the full path from the <root path, sub path> pair
            KeyValuePair<string, string> pair = this.SelectedDirectoryPair.Value;
            string fullPath = Path.GetFullPath(Path.Combine(pair.Key, pair.Value));
            
            // Open the directory
            this._mainWindowViewModel.OpenDirectory(fullPath);
        }
    }

    private void OpenPlaylist()
    {
        if (this.SelectedPlaylistPath != null)
        {
            this._mainWindowViewModel.OpenPlaylist(this.SelectedPlaylistPath);
        }
    }
}