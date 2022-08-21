using Common;
using Model.Service;

namespace ViewModel;

public class MainWindowViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // ENUM FOR DIFFERENT MAIN CONTENT VIEWS
    // ==============

    public enum MainContent
    {
        CurrentPlaylist,
        Favorites,
        Directories,
        Playlists
    };
    
    // ==============
    // FIELDS
    // ==============

    // Variables to hide/show the correct main content view
    private bool _isCurrentPlaylistViewShown;
    private bool _isFavoritesViewShown;
    private bool _isDirectoriesViewShown;
    private bool _isPlaylistsViewShown;
    
    // ==============
    // PROPERTIES
    // ==============
    
    // View models
    
    public CurrentPlaylistViewModel CurrentPlaylistViewModel { get; }
    
    public FavoritesViewModel FavoritesViewModel { get; }
    
    public DirectoriesViewModel DirectoriesViewModel { get; }
    
    public PlaylistsViewModel PlaylistsViewModel { get; }
    
    public CurrentSongViewModel CurrentSongViewModel { get; }

    public NavigationSidebarViewModel NavigationSidebarViewModel { get; }
    
    // Current shown main content view
    // => Do not use method like ChangeCurrentView() inside view model because then a reference to the View project
    //    would be necessary which is not wanted to keep the View and the ViewModel project separate from each other
    // => Only return the current value. The property changed method is called separate when changing the values to
    //    avoid missing one of the views (all bool variables have to be updated once because only one view is visible
    //    at each time).

    public bool IsCurrentPlaylistViewShown => _isCurrentPlaylistViewShown;

    public bool IsFavoritesViewShown => _isFavoritesViewShown;

    public bool IsDirectoriesViewShown => _isDirectoriesViewShown;

    public bool IsPlaylistsViewShown => _isPlaylistsViewShown;

    // ==============
    // INITIALIZATION
    // ==============

    public MainWindowViewModel()
    {
        // Create model instances that are shared by the view model instances
        SongImporter songImporter = new SongImporter();
        SongPlayer songPlayer = new SongPlayer();
        AppConfigurator appConfigurator = new AppConfigurator();

        // Init view models
        this.CurrentPlaylistViewModel = new CurrentPlaylistViewModel(songPlayer);
        this.FavoritesViewModel = new FavoritesViewModel();
        this.DirectoriesViewModel = new DirectoriesViewModel(songImporter, songPlayer, appConfigurator);
        this.PlaylistsViewModel = new PlaylistsViewModel();
        
        this.CurrentSongViewModel = new CurrentSongViewModel(songPlayer);
        this.NavigationSidebarViewModel = new NavigationSidebarViewModel(this);
        
        // Init first shown main content view
        this.ChangeMainContent(MainContent.Directories);
    }
    
    // ==============
    // CHANGE MAIN VIEW
    // ==============

    public void ChangeMainContent(MainContent mainContent)
    {
        // Hide all main content views
        this._isCurrentPlaylistViewShown = false;
        this._isFavoritesViewShown = false;
        this._isDirectoriesViewShown = false;
        this._isPlaylistsViewShown = false;
        
        // Set the correct main content variable to true
        switch (mainContent)
        {
            case MainContent.CurrentPlaylist:
                this._isCurrentPlaylistViewShown = true;
                break;
            case MainContent.Favorites:
                this._isFavoritesViewShown = true;
                break;
            case MainContent.Directories:
                this._isDirectoriesViewShown = true;
                break;
            case MainContent.Playlists:
                this._isPlaylistsViewShown = true;
                break;
        }
        
        // Raise property changed event for all variables
        OnPropertyChanged(nameof(IsCurrentPlaylistViewShown));
        OnPropertyChanged(nameof(IsFavoritesViewShown));
        OnPropertyChanged(nameof(IsDirectoriesViewShown));
        OnPropertyChanged(nameof(IsPlaylistsViewShown));
    }
}