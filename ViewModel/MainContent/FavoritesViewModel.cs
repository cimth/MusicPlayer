using System.Collections.ObjectModel;
using System.Windows.Input;
using Model.Service;
using ViewModel.Command;

namespace ViewModel.MainContent;

public class FavoritesViewModel
{
    // ==============
    // FIELDS
    // ==============

    private readonly FavoritesManager _favoritesManager;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    // ==============
    // PROPERTIES
    // ==============

    public ObservableCollection<string> DirectoryPaths => _favoritesManager.FavoriteDirectoryPaths;

    public ObservableCollection<string> PlaylistPaths => _favoritesManager.FavoritePlaylistRelativePaths;
    
    public string? SelectedDirectoryPath { get; set; }
    
    public string? SelectedPlaylistPath { get; set; }
    
    // Commands
    
    public ICommand OpenDirectoryCommand { get; }
    
    public ICommand OpenPlaylistCommand { get; }

    // ==============
    // INITIALIZATION
    // ==============

    public FavoritesViewModel(MainWindowViewModel mainWindowViewModel, FavoritesManager favoritesManager)
    {
        this._mainWindowViewModel = mainWindowViewModel;
        this._favoritesManager = favoritesManager;
        
        // Init commands
        this.OpenDirectoryCommand = new DelegateCommand(this.OpenDirectory);
        this.OpenPlaylistCommand = new DelegateCommand(this.OpenPlaylist);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void OpenDirectory()
    {
        if (this.SelectedDirectoryPath != null)
        {
            this._mainWindowViewModel.OpenDirectory(this.SelectedDirectoryPath);
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