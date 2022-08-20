using System.Windows.Input;
using ViewModel.Command;

namespace ViewModel;

public class NavigationSidebarViewModel
{
    // ==============
    // FIELDS
    // ==============

    private MainWindowViewModel _mainWindowViewModel;
    
    // Commands
    
    public ICommand ShowCurrentPlaylistCommand { get; }
    
    public ICommand ShowFavoritesCommand { get; }
    
    public ICommand ShowDirectoriesCommand { get; }
    
    public ICommand ShowPlaylistsCommand { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public NavigationSidebarViewModel(MainWindowViewModel mainWindowViewModel)
    {
        this._mainWindowViewModel = mainWindowViewModel;
        
        // init commands
        this.ShowCurrentPlaylistCommand = new DelegateCommand(ShowCurrentPlaylist);
        this.ShowFavoritesCommand = new DelegateCommand(ShowFavorites);
        this.ShowDirectoriesCommand = new DelegateCommand(ShowDirectories);
        this.ShowPlaylistsCommand = new DelegateCommand(ShowPlaylists);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void ShowCurrentPlaylist()
    {
        this._mainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.CurrentPlaylist);
    }
    
    private void ShowFavorites()
    {
        this._mainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Favorites);
    }
    
    private void ShowDirectories()
    {
        this._mainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Directories);
    }
    
    private void ShowPlaylists()
    {
        this._mainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Playlists);
    }
}