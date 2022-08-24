using System.Collections.ObjectModel;
using System.Windows.Input;
using Common;
using Dialog;
using Model.DataType;
using Model.Service;
using ViewModel.Command;
using ViewModel.Dialog;

namespace ViewModel.MainContent;

public class PlaylistsViewModel : NotifyPropertyChangedImpl
{
    
    // ==============
    // FIELDS 
    // ==============

    private readonly SongImporter _songImporter;
    private readonly PlaylistImporter _playlistImporter;
    private readonly PlaylistManager _playlistManager;
    private readonly SongPlayer _songPlayer;

    private string? _currentDirectoryPath;
    private string? _currentDirectoryNameFromRoot;
    private ObservableCollection<string> _subDirectoryPaths;

    private ObservableCollection<Playlist> _playlistsInDirectory;
    private Playlist? _selectedPlaylist;

    private bool _isPlaylistShown;

    private PlaylistSortOrder _selectedPlaylistSortOrder;

    // ==============
    // PROPERTIES 
    // ==============

    public SongPlayer SongPlayer => _songPlayer;
    
    // Properties for choosing a directory and a playlist

    public string? CurrentDirectoryPath
    {
        get => _currentDirectoryPath;
        private set
        {
            SetField(ref _currentDirectoryPath, value);
            
            // Update the directory name from the playlist root path
            // => The directory '<Full path to root>/<dir1>/<dir2>' will result in '<dir1>/<dir2>'
            this.CurrentDirectoryNameFromRoot = _currentDirectoryPath != null ? Path.GetRelativePath(AppConfig.PlaylistsRootPath, _currentDirectoryPath) : null;
        } 
    }

    public string? CurrentDirectoryNameFromRoot
    {
        get => _currentDirectoryNameFromRoot;
        private set => SetField(ref _currentDirectoryNameFromRoot, value);
    } 

    public ObservableCollection<string> SubDirectoryPaths
    {
        get => _subDirectoryPaths; 
        private set => SetField(ref _subDirectoryPaths, value);
    }
    
    public string? SelectedSubDirectoryPath { get; set; }
    
    public int SelectedSubDirectoryIndex { get; set; }

    public ObservableCollection<Playlist> PlaylistsInDirectory
    {
        get => _playlistsInDirectory;
        private set => SetField(ref _playlistsInDirectory, value);
    }
    
    public int SelectedPlaylistIndex { get; set; }
    
    // Properties for showing the current playlist details

    public Playlist? SelectedPlaylist
    {
        get => _selectedPlaylist; 
        set => SetField(ref _selectedPlaylist, value);
    }

    public int SelectedSongIndex { get; set; }

    public bool IsPlaylistShown
    {
        get => _isPlaylistShown;
        set => SetField(ref _isPlaylistShown, value);
    }
    
    // Sort playlist

    // Creates pairs of <enum value, string representation of enum value>
    // Access inside XAML:
    // * Enum value: `SelectedValue='Key'`
    // * String representation: `DisplayMemberPath='value'`
    // Also use `SelectedValue` instead of `SelectedItem` in XAML for the correct binding below.
    public Dictionary<PlaylistSortOrder, string> PlaylistSortOrders 
        => Enum.GetValues<PlaylistSortOrder>().ToDictionary(sortOrder => sortOrder, PlaylistSortOrderToString.ToString);

    public PlaylistSortOrder SelectedPlaylistSortOrder
    {
        get => _selectedPlaylistSortOrder;
        set => SetField(ref _selectedPlaylistSortOrder, value);
    }

    // Commands
    
    public ICommand GoBackCommand { get; }
    
    public ICommand AddSubDirectoryCommand { get; }
    
    public ICommand RemoveSubDirectoryCommand { get; }
    
    public ICommand OpenSubDirectoryCommand { get; }
    
    public ICommand OpenPlaylistCommand { get; }
    
    public ICommand StartPlaylistBeginningWithTheSelectedSongCommand { get; }
    
    public ICommand AddPlaylistCommand { get; }
    
    public ICommand RemovePlaylistCommand { get; }
    
    public ICommand AddSongToPlaylistCommand { get; }
    
    public ICommand RemoveSongFromPlaylistCommand { get; }
    
    public ICommand SelectedPlaylist_OnSongMovedCommand { get; }
    
    public ICommand ChangePlaylistSortOrderCommand { get; }
    
    // ==============
    // INITIALIZATION 
    // ==============

    public PlaylistsViewModel(SongImporter songImporter, PlaylistImporter playlistImporter, PlaylistManager playlistManager, SongPlayer songPlayer)
    {
        this._songImporter = songImporter;
        this._playlistImporter = playlistImporter;
        this._playlistManager = playlistManager;
        this._songPlayer = songPlayer;

        this._subDirectoryPaths = new ObservableCollection<string>();
        this._playlistsInDirectory = new ObservableCollection<Playlist>();

        // Init commands
        this.GoBackCommand = new DelegateCommand(this.GoBack);
        this.AddSubDirectoryCommand = new DelegateCommand(this.AddSubDirectory);
        this.RemoveSubDirectoryCommand = new DelegateCommand(this.RemoveSubDirectory);
        this.OpenSubDirectoryCommand = new DelegateCommand(this.OpenSubDirectory);
        this.OpenPlaylistCommand = new DelegateCommand(this.OpenPlaylist);
        this.StartPlaylistBeginningWithTheSelectedSongCommand = new DelegateCommand(this.StartPlaylistBeginningWithTheSelectedSong);
        this.AddPlaylistCommand = new DelegateCommand(this.AddPlaylist);
        this.RemovePlaylistCommand = new DelegateCommand(this.RemovePlaylist);
        this.AddSongToPlaylistCommand = new DelegateCommand(this.AddSongToPlaylist);
        this.RemoveSongFromPlaylistCommand = new DelegateCommand(this.RemoveSongFromPlaylist);
        this.SelectedPlaylist_OnSongMovedCommand = new DelegateCommand(this.SelectedPlaylist_OnRowMoved);
        this.ChangePlaylistSortOrderCommand = new DelegateCommand(this.ChangePlaylistSortOrder);

        // Set elements that are shown first
        this._isPlaylistShown = false;
        this._selectedPlaylistSortOrder = PlaylistSortOrder.Alphabetical;
        this.LoadContents(null);
    }
    
    // ==============
    // ACTION AFTER DRAGGING ROWS
    // ==============

    private void SelectedPlaylist_OnRowMoved()
    {
        // Save changes
        if (this.SelectedPlaylist != null)
        {
            this._playlistManager.SaveInPlaylistFile(this.CurrentDirectoryPath, this.SelectedPlaylist);
        }
    }

    // ==============
    // CHANGE DIRECTORY
    // ==============
    
    private void LoadContents(string? directoryPath)
    {
        // Use playlist root directory if no directory path is specified
        directoryPath ??= AppConfig.PlaylistsRootPath;

        // Load sub directories
        string[] subDirectories = Directory.GetDirectories(directoryPath);
        this.SubDirectoryPaths = new ObservableCollection<string>(subDirectories);

        // Load playlists
        string[] playlistFiles = Directory.GetFiles(directoryPath, "*.json");
        
        ObservableCollection<Playlist> importedPlaylists = new ObservableCollection<Playlist>();
        foreach (var filePath in playlistFiles)
        {
            importedPlaylists.Add(this._playlistImporter.ImportFromPlaylistFile(filePath));
        }

        this.PlaylistsInDirectory = importedPlaylists;
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void GoBack()
    {
        // Just go back to the directory view (the current directory path is still set correctly)
        if (this.IsPlaylistShown)
        {
            this.IsPlaylistShown = false;
            return;
        } 
        
        // Already inside directory view, thus go back to parent directory
        if (Directory.Exists(this.CurrentDirectoryPath))
        {
            string parentPath = Directory.GetParent(this.CurrentDirectoryPath)!.FullName;

            // If the current directory is a root directory, set the current directory to null for showing
            // the root directories instead of the current (root) directory's parent
            this.CurrentDirectoryPath = AppConfig.PlaylistsRootPath.Equals(parentPath) ? null : parentPath;
            
            // Reload GUI
            this.LoadContents(this.CurrentDirectoryPath);
        }
    }

    private void AddSubDirectory()
    {
        // Open dialog to request the sub directory name
        DialogService dialogService = new DialogService();
        InputDialogViewModel dialogViewModel = new InputDialogViewModel("Which name should the directory have?");
        bool? result = dialogService.ShowInputDialog(dialogViewModel);
        
        // Add the directory if the dialog was successful
        if (result != null && result.Value)
        {
            // Create directory
            string directoryName = dialogViewModel.InputValue;
            string fullPath = this._playlistManager.CreatePlaylistDirectory(this.CurrentDirectoryPath, directoryName);
            
            // Update GUI
            this.SubDirectoryPaths.Add(fullPath);
            this.SubDirectoryPaths = new ObservableCollection<string>(this.SubDirectoryPaths.OrderBy(s => s));
        }
    }

    private void RemoveSubDirectory()
    {
        if (this.SelectedSubDirectoryIndex >= 0 && this.SelectedSubDirectoryIndex < this.SubDirectoryPaths.Count)
        {
            // Remove directory
            string directoryName = this.SubDirectoryPaths[this.SelectedSubDirectoryIndex];
            this._playlistManager.RemovePlaylistDirectory(this.CurrentDirectoryPath, directoryName);
            
            // Update GUI
            this.SubDirectoryPaths.RemoveAt(this.SelectedSubDirectoryIndex);
        }
    }

    private void OpenSubDirectory()
    {
        if (Directory.Exists(this.SelectedSubDirectoryPath))
        {
            this.CurrentDirectoryPath = this.SelectedSubDirectoryPath;
            this.LoadContents(this.SelectedSubDirectoryPath);
        }
    }

    private void OpenPlaylist()
    {
        this.IsPlaylistShown = true;
    }
    
    private void StartPlaylistBeginningWithTheSelectedSong()
    {
        if (this.SelectedPlaylist != null 
            && this.SelectedSongIndex >= 0 
            && this.SelectedSongIndex < this.SelectedPlaylist.Songs.Count)
        {
            this.SongPlayer.Play(this.SelectedPlaylist, this.SelectedSongIndex);
        }
    }
    
    private void AddPlaylist()
    {
        // Open dialog to request the sub directory name
        DialogService dialogService = new DialogService();
        InputDialogViewModel dialogViewModel = new InputDialogViewModel("Which name should the playlist have?");
        bool? result = dialogService.ShowInputDialog(dialogViewModel);
        
        // Add the playlist if the dialog was successful
        if (result != null && result.Value)
        {
            // Create playlist
            string playlistName = dialogViewModel.InputValue;
            Playlist playlist = new Playlist(playlistName);
            this._playlistManager.SaveInPlaylistFile(this.CurrentDirectoryPath, playlist);
            
            // Update GUI
            this.PlaylistsInDirectory.Insert(0, playlist);
        }
    }

    private void RemovePlaylist()
    {
        if (this.SelectedPlaylist != null 
            && this.SelectedPlaylistIndex >= 0 
            && this.SelectedPlaylistIndex < this.PlaylistsInDirectory.Count)
        {
            // Remove file
            this._playlistManager.RemovePlaylistFile(this.CurrentDirectoryPath, this.SelectedPlaylist);
            
            // Update GUI
            this.PlaylistsInDirectory.RemoveAt(this.SelectedPlaylistIndex);
        }
    }
    
    private void AddSongToPlaylist()
    {
        // Open dialog to select the songs to add
        OpenFileDialog dialog = new OpenFileDialog()
        {
            Multiselect = true,
            Filter = "Music files (*.mp3)|*.mp3",
        };
        DialogResult dialogResult = dialog.ShowDialog();
        
        // Add songs
        if (dialogResult == DialogResult.OK && this.SelectedPlaylist != null)
        {
            // Update playlist
            foreach (var filePath in dialog.FileNames)
            {
                Song song = this._songImporter.Import(filePath);
                this.SelectedPlaylist.Songs.Add(song);
            }
            
            // Sort playlist
            this.SelectedPlaylist.Sort(this.SelectedPlaylistSortOrder);
                
            // Save changes
            this._playlistManager.SaveInPlaylistFile(this.CurrentDirectoryPath, this.SelectedPlaylist);
        }
    }

    private void RemoveSongFromPlaylist()
    {
        if (this.SelectedPlaylist != null 
            && this.SelectedSongIndex >= 0 
            && this.SelectedSongIndex < this.SelectedPlaylist.Songs.Count)
        {
            // Update playlist
            this.SelectedPlaylist.Songs.RemoveAt(this.SelectedSongIndex);
            
            // Save changes
            this._playlistManager.SaveInPlaylistFile(this.CurrentDirectoryPath, this.SelectedPlaylist);
        }
    }

    private void ChangePlaylistSortOrder()
    {
        if (this.SelectedPlaylist != null)
        {
            // Sort
            this.SelectedPlaylist.Sort(this.SelectedPlaylistSortOrder);

            // Save changes
            this._playlistManager.SaveInPlaylistFile(this.CurrentDirectoryPath, this.SelectedPlaylist);
        }
    }

    // ==============
    // METHOD ACTIONS
    // ==============
    
    // Methods which are called from XAML by CallMethodAction from Microsoft.Xaml.Behaviors
    // => Works with default Event method signature '(object sender, System.Windows.EventArgs args)'
    // => Can only use empty constructor or the '(sender, args)' constructor

    public void DropFileOntoPlaylist(object sender, System.Windows.DragEventArgs args)
    {
        if (args.Data != null && args.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Get the paths of the dropped files
            string[] filePaths = (string[]) args.Data.GetData(DataFormats.FileDrop)!;

            // Add to playlist
            if (this.SelectedPlaylist != null)
            {
                // Update playlist
                foreach (var filePath in filePaths)
                {
                    Song song = this._songImporter.Import(filePath);
                    this.SelectedPlaylist.Songs.Add(song);
                }

                // Sort playlist
                this.SelectedPlaylist.Sort(this.SelectedPlaylistSortOrder);

                // Save changes
                this._playlistManager.SaveInPlaylistFile(this.CurrentDirectoryPath, this.SelectedPlaylist);
            }
        }
    }
}