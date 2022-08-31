using System.Windows.Input;
using ViewModel.Command;

namespace ViewModel;

public class NavigationSidebarViewModel
{
    // ==============
    // PROPERTIES
    // ==============

    public MainWindowViewModel MainWindowViewModel { get; }
    
    // Commands
    
    public ICommand ShowCurrentPlaylistCommand { get; }
    
    public ICommand ShowQueueCommand { get; }
    
    public ICommand ShowFavoritesCommand { get; }
    
    public ICommand ShowDirectoriesCommand { get; }
    
    public ICommand ShowPlaylistsCommand { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public NavigationSidebarViewModel(MainWindowViewModel mainWindowViewModel)
    {
        this.MainWindowViewModel = mainWindowViewModel;
        
        // init commands
        this.ShowCurrentPlaylistCommand = new DelegateCommand(ShowCurrentPlaylist);
        this.ShowQueueCommand = new DelegateCommand(ShowQueue);
        this.ShowFavoritesCommand = new DelegateCommand(ShowFavorites);
        this.ShowDirectoriesCommand = new DelegateCommand(ShowDirectories);
        this.ShowPlaylistsCommand = new DelegateCommand(ShowPlaylists);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void ShowCurrentPlaylist()
    {
        this.MainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.CurrentPlaylist);
    }
    
    private void ShowQueue()
    {
        this.MainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Queue);
    }
    
    private void ShowFavorites()
    {
        this.MainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Favorites);
    }
    
    private void ShowDirectories()
    {
        this.MainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Directories);
    }
    
    private void ShowPlaylists()
    {
        this.MainWindowViewModel.ChangeMainContent(MainWindowViewModel.MainContent.Playlists);
    }
}