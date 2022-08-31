using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Common;
using Dialog;
using Model.Service;
using ViewModel.Command;
using ViewModel.MainContent;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;

namespace ViewModel;

public class MainWindowViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // ENUM FOR DIFFERENT MAIN CONTENT VIEWS
    // ==============

    public enum MainContent
    {
        CurrentPlaylist,
        Queue,
        Favorites,
        Directories,
        Playlists
    };
    
    // ==============
    // FIELDS
    // ==============

    private SongPlayer _songPlayer;

    // Variables to hide/show the correct main content view
    private bool _isCurrentPlaylistViewShown;
    private bool _isQueueViewShown;
    private bool _isFavoritesViewShown;
    private bool _isDirectoriesViewShown;
    private bool _isPlaylistsViewShown;

    // Variable for hooking on keyboard input events even if the application is not focused
    private LowLevelKeyboardHook _hook;
    
    // ==============
    // PROPERTIES
    // ==============
    
    // View models
    
    public CurrentPlaylistViewModel CurrentPlaylistViewModel { get; }
    
    public QueueViewModel QueueViewModel { get; }
    
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

    public bool IsQueueViewShown => _isQueueViewShown;

    public bool IsFavoritesViewShown => _isFavoritesViewShown;

    public bool IsDirectoriesViewShown => _isDirectoriesViewShown;

    public bool IsPlaylistsViewShown => _isPlaylistsViewShown;
    
    // ==============
    // COMMANDS
    // ==============

    public ICommand OnCloseCommand;

    // ==============
    // INITIALIZATION
    // ==============

    public MainWindowViewModel()
    {
        // Create model instances that are shared by the view model instances
        SongImporter songImporter = new SongImporter();
        PlaylistImporter playlistImporter = new PlaylistImporter(songImporter);
        AppConfigurator appConfigurator = new AppConfigurator();
        PlaylistManager playlistManager = new PlaylistManager();
        FavoritesManager favoritesManager = new FavoritesManager();
        DialogService dialogService = new DialogService();
        
        this._songPlayer = new SongPlayer(appConfigurator);

        // Init view models
        this.CurrentPlaylistViewModel = new CurrentPlaylistViewModel(this._songPlayer);
        this.QueueViewModel = new QueueViewModel(this._songPlayer);
        this.FavoritesViewModel = new FavoritesViewModel(appConfigurator, favoritesManager, this);
        this.DirectoriesViewModel = new DirectoriesViewModel(playlistImporter, this._songPlayer, appConfigurator, favoritesManager);
        this.PlaylistsViewModel = new PlaylistsViewModel(songImporter, playlistImporter, playlistManager, this._songPlayer, favoritesManager, dialogService);
        
        this.CurrentSongViewModel = new CurrentSongViewModel(this._songPlayer);
        this.NavigationSidebarViewModel = new NavigationSidebarViewModel(this);
        
        // Register for window-wide key events to pause / resume the current song.
        // Use PreviewKeyDownEvent because only KeyDown is recognized when pressing a button on a bluetooth headset.
        EventManager.RegisterClassHandler(typeof(Window), Keyboard.PreviewKeyDownEvent, new KeyEventHandler(OnWindowWideKeyEvent), true);

        // Register for global key events to react on the media keys pressed.
        // Therefor, the application needs to hook the keyboard events of Windows and unhook when closing
        // the application.
        this.HookKeyboard();
        this.OnCloseCommand = new DelegateCommand(OnClose);
        
        // Init first shown main content view
        this.ChangeMainContent(MainContent.Directories);
    }

    [MemberNotNull(nameof(_hook))]
    private void HookKeyboard()
    {
        // The keys which should be hooked even if the application has no focus
        Key[] registeredKeys = {
            Key.MediaPlayPause,
            Key.MediaNextTrack,
            Key.MediaPreviousTrack
        };
        
        // Initialize the hooking
        this._hook = new LowLevelKeyboardHook(registeredKeys);
        this._hook.OnKeyPressed += OnGlobalKeyDown;
        this._hook.HookKeyboard();
    }

    private void OnClose()
    {
        this._hook.UnHookKeyboard();
    }

    // ==============
    // CHANGE MAIN VIEW
    // ==============

    public void ChangeMainContent(MainContent mainContent, string? directoryPath = null, string? playlistPath = null)
    {
        // Hide all main content views
        this._isCurrentPlaylistViewShown = false;
        this._isQueueViewShown = false;
        this._isFavoritesViewShown = false;
        this._isDirectoriesViewShown = false;
        this._isPlaylistsViewShown = false;
        
        // Set the correct main content variable to true
        switch (mainContent)
        {
            case MainContent.CurrentPlaylist:
                this._isCurrentPlaylistViewShown = true;
                break;
            case MainContent.Queue:
                this._isQueueViewShown = true;
                break;
            case MainContent.Favorites:
                this._isFavoritesViewShown = true;
                break;
            case MainContent.Directories:
                this._isDirectoriesViewShown = true;
                this.DirectoriesViewModel.OpenDirectoryFromExternal(directoryPath);
                break;
            case MainContent.Playlists:
                this._isPlaylistsViewShown = true;
                this.PlaylistsViewModel.OpenPlaylistFromExternal(playlistPath);
                break;
        }
        
        // Raise property changed event for all variables
        OnPropertyChanged(nameof(IsCurrentPlaylistViewShown));
        OnPropertyChanged(nameof(IsQueueViewShown));
        OnPropertyChanged(nameof(IsFavoritesViewShown));
        OnPropertyChanged(nameof(IsDirectoriesViewShown));
        OnPropertyChanged(nameof(IsPlaylistsViewShown));
    }
    
    // ==============
    // WINDOW-GLOBAL KEY FUNCTIONS
    // ==============

    private void OnWindowWideKeyEvent(object sender, KeyEventArgs e)
    {
        this.HandleKeyDown(e.Key);
    }
    
    private void OnGlobalKeyDown(object? sender, Key key)
    {
        this.HandleKeyDown(key);
    }

    private void HandleKeyDown(Key key)
    {
        // Pause / Resume the current song on 'Space' or on 'MediaPlayPause'
        if (key == Key.Space || key == Key.MediaPlayPause)
        {
            if (this._songPlayer.IsPlaying)
            {
                this._songPlayer.Pause();
            }
            else
            {
                this._songPlayer.Resume();
            }
        }
        
        // Next song
        if (key == Key.MediaNextTrack)
        {
            this._songPlayer.PlayNext();
        }
        
        // Previous song
        if (key == Key.MediaPreviousTrack)
        {
            this._songPlayer.PlayPrevious();
        }
    }
}