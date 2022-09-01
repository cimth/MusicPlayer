using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
    private readonly FavoritesManager _favoritesManager;
    private readonly DialogService _dialogService;

    private string? _currentDirectoryPath;
    private string? _currentDirectoryNameFromRoot;
    private ObservableCollection<string> _subDirectoryPaths;

    private ObservableCollection<Playlist> _playlistsInDirectory;
    private Playlist? _selectedPlaylist;
    
    private int _selectedSubDirectoryIndex = -1;
    private int _selectedPlaylistIndex = -1;
    private int _selectedSongIndex = -1;

    private bool _isPlaylistShown;

    private bool _isFavorite = false;

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
            if (_currentDirectoryPath != null)
            {
                // Set the relative path seen from the playlist root path.
                // If the current directory is the root directory, set the name to null to avoid showing "." as name
                this.CurrentDirectoryNameFromRoot = !_currentDirectoryPath.Equals(AppConfig.PlaylistsRootPath)
                    ? Path.GetRelativePath(AppConfig.PlaylistsRootPath, _currentDirectoryPath)
                    : null;
            }
        } 
    }

    // Is set when the current directory path changes
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

    public int SelectedSubDirectoryIndex
    {
        get => _selectedSubDirectoryIndex; 
        set => SetField(ref _selectedSubDirectoryIndex, value);
    }

    public ObservableCollection<Playlist> PlaylistsInDirectory
    {
        get => _playlistsInDirectory;
        private set => SetField(ref _playlistsInDirectory, value);
    }

    public int SelectedPlaylistIndex
    {
        get => _selectedPlaylistIndex; 
        set => SetField(ref _selectedPlaylistIndex, value);
    }
    
    // Properties for showing the current playlist details

    public Playlist? SelectedPlaylist
    {
        get => _selectedPlaylist;
        set
        {
            SetField(ref _selectedPlaylist, value);
            
            // Call update method when the collection is changed
            if (_selectedPlaylist != null)
            {
                _selectedPlaylist.Songs.CollectionChanged += this.UpdateAfterPlaylistChange;
            }
        } 
    }

    public int SelectedSongIndex
    {
        get => _selectedSongIndex; 
        set => SetField(ref _selectedSongIndex, value);
    }

    public bool IsPlaylistShown
    {
        get => _isPlaylistShown;
        set => SetField(ref _isPlaylistShown, value);
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetField(ref _isFavorite, value);
    }
    
    // Sort playlist

    // Creates pairs of <enum value, string representation of enum value>
    // Access inside XAML:
    // * Enum value: `SelectedValue='Key'`
    // * String representation: `DisplayMemberPath='Value'`
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
    
    public ICommand GoBackToRootCommand { get; }
    
    public ICommand AddSubDirectoryCommand { get; }
    
    public ICommand RemoveSubDirectoryCommand { get; }
    
    public ICommand OpenSubDirectoryCommand { get; }
    
    public ICommand OpenPlaylistCommand { get; }
    
    public ICommand StartPlaylistBeginningWithTheSelectedSongCommand { get; }
    
    public ICommand AddPlaylistCommand { get; }
    
    public ICommand RemovePlaylistCommand { get; }
    
    public ICommand DuplicatePlaylistCommand { get; }
    
    public ICommand ExportPlaylistCommand { get; }
    
    public ICommand AddToQueueCommand { get; }
    
    public ICommand AddSongToPlaylistCommand { get; }
    
    public ICommand RemoveSongFromPlaylistCommand { get; }
    
    public ICommand ChangePlaylistSortOrderCommand { get; }
    
    public ICommand UpdateOnRowMovedCommand { get; }
    
    public ICommand AddToFavoritesCommand { get; }
    
    public ICommand RemoveFromFavoritesCommand { get; }
    
    // ==============
    // INITIALIZATION 
    // ==============

    public PlaylistsViewModel(SongImporter songImporter, PlaylistImporter playlistImporter, PlaylistManager playlistManager, SongPlayer songPlayer, FavoritesManager favoritesManager, DialogService dialogService)
    {
        this._songImporter = songImporter;
        this._playlistImporter = playlistImporter;
        this._playlistManager = playlistManager;
        this._songPlayer = songPlayer;
        this._favoritesManager = favoritesManager;
        this._dialogService = dialogService;

        this._subDirectoryPaths = new ObservableCollection<string>();
        this._playlistsInDirectory = new ObservableCollection<Playlist>();
        
        this._selectedPlaylistSortOrder = PlaylistSortOrder.Alphabetical;

        // Init commands
        this.GoBackCommand = new DelegateCommand(this.GoBack);
        this.GoBackToRootCommand = new DelegateCommand(this.GoBackToRoot);
        this.AddSubDirectoryCommand = new DelegateCommand(this.AddSubDirectory);
        this.RemoveSubDirectoryCommand = new DelegateCommand(this.RemoveSubDirectory);
        this.OpenSubDirectoryCommand = new DelegateCommand(this.OpenSubDirectory);
        this.OpenPlaylistCommand = new DelegateCommand(this.OpenPlaylist);
        this.StartPlaylistBeginningWithTheSelectedSongCommand = new DelegateCommand(this.StartPlaylistBeginningWithTheSelectedSong);
        this.AddPlaylistCommand = new DelegateCommand(this.AddPlaylist);
        this.RemovePlaylistCommand = new DelegateCommand(this.RemovePlaylist);
        this.DuplicatePlaylistCommand = new DelegateCommand(this.DuplicatePlaylist);
        this.ExportPlaylistCommand = new DelegateCommand(this.ExportPlaylist);
        this.AddToQueueCommand = new DelegateCommand(this.AddToQueue);
        this.AddSongToPlaylistCommand = new DelegateCommand(this.AddSongToPlaylist);
        this.RemoveSongFromPlaylistCommand = new DelegateCommand(this.RemoveSongFromPlaylist);
        this.ChangePlaylistSortOrderCommand = new DelegateCommand(this.ChangePlaylistSortOrder);
        this.UpdateOnRowMovedCommand = new DelegateCommand(this.UpdateOnRowMoved);
        this.AddToFavoritesCommand = new DelegateCommand(this.AddToFavorites);
        this.RemoveFromFavoritesCommand = new DelegateCommand(this.RemoveFromFavorites);

        // Set elements that are shown first
        this.LoadAsCurrentDirectory(null);
    }
    
    // ==============
    // OPEN FROM EXTERN (NEEDED FOR FAVORITES)
    // ==============

    public void OpenPlaylistFromExternal(string? playlistRelativePath)
    {
        // Reset selected values.
        this.SelectedSubDirectoryPath = null;
        this.SelectedSubDirectoryIndex = -1;
        
        this.SelectedPlaylist = null;
        this.SelectedPlaylistIndex = -1;
        
        // Get the full directory path. If it is null, the root directory should be opened.
        string? fullDirectoryPath = null;
        if (playlistRelativePath != null)
        {
            // Get the full path of the directory in which the playlist is located.
            // => This path will be the playlist root directory or a sub directory of it.
            string fullPath = this._playlistManager.GetFullPath(playlistRelativePath);
            int lastDirectorySeparatorIndex = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
            fullDirectoryPath = fullPath.Substring(0, lastDirectorySeparatorIndex);
        }

        // Load the directory. When it is null, the playlist root directory is loaded.
        this.LoadAsCurrentDirectory(fullDirectoryPath);

        // If the given path is not null, a playlist should be opened.
        if (playlistRelativePath != null)
        {
            // Get the correct playlist object.
            // This method needs to be called after loading the current directory because the collection with the
            // playlists in the directory will only be initialized in this case.
            Playlist playlist = this.PlaylistsInDirectory.First(playlist => playlist.RelativePath == playlistRelativePath);

            // Open the playlist as selected playlist.
            this.SelectedPlaylist = playlist;
            this.OpenPlaylist();
        }
    }
    
    // ==============
    // PLAYLIST WAS UPDATED
    // ==============
    
    private void UpdateAfterPlaylistChange(object? sender, NotifyCollectionChangedEventArgs args)
    {
        // Save changes
        if (this.SelectedPlaylist != null)
        {
            this._playlistManager.SaveInPlaylistFile(this.SelectedPlaylist);
        }
    }

    private void UpdateOnRowMoved()
    {
        if (this.SelectedPlaylist != null)
        {
            this.SelectedPlaylist.SortOrder = PlaylistSortOrder.Individual;
            this.SelectedPlaylistSortOrder = PlaylistSortOrder.Individual;
        }
    }

    // ==============
    // CHANGE DIRECTORY
    // ==============
    
    private void LoadAsCurrentDirectory(string? directoryPath)
    {
        // Use playlist root directory if no directory path is specified
        this.CurrentDirectoryPath = directoryPath ?? AppConfig.PlaylistsRootPath;
        
        // Set playlist flag to false because a directory is shown
        this.IsPlaylistShown = false;

        // Load sub directories
        string[] subDirectories = Directory.GetDirectories(this.CurrentDirectoryPath);
        this.SubDirectoryPaths = new ObservableCollection<string>(subDirectories);

        // Load playlists
        string[] playlistFiles = Directory.GetFiles(this.CurrentDirectoryPath, "*.json");
        
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
            this.LoadAsCurrentDirectory(this.CurrentDirectoryPath);
        }
    }
    
    private void GoBackToRoot()
    {
        // Set the current directory to null for showing the root directory
        this.CurrentDirectoryPath = null;
            
        // Reload GUI
        this.LoadAsCurrentDirectory(this.CurrentDirectoryPath);
    }

    private void AddSubDirectory()
    {
        // Open dialog to request the sub directory name
        string request = LanguageUtil.GiveLocalizedString("Str_WhichNameForDirectory");
        InputDialogViewModel dialogViewModel = new InputDialogViewModel(request);
        bool? result = this._dialogService.ShowInputDialog(dialogViewModel);
        
        // Add the directory if the dialog was successful
        if (result != null && result.Value)
        {
            // Show error dialog if the directory name already exists in the current directory.
            string directoryName = dialogViewModel.InputValue;
            string targetPathEnding = Path.DirectorySeparatorChar + directoryName;
            if (this.SubDirectoryPaths.Any(path => path.ToLower().EndsWith(targetPathEnding.ToLower())))
            {
                string dialogTitle = LanguageUtil.GiveLocalizedString("Str_Error");
                string errorMessage = LanguageUtil.GiveLocalizedString("Str_DirectoryAlreadyExists");
                MessageDialogViewModel messageViewModel = new MessageDialogViewModel(dialogTitle, errorMessage);
                this._dialogService.ShowMessageDialog(messageViewModel);
                return;
            }
            
            // Create directory
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
            // Save index to re-assign it after the GUI update
            int origSelectedIndex = this.SelectedSubDirectoryIndex;
            
            // Remove directory
            string directoryName = this.SubDirectoryPaths[this.SelectedSubDirectoryIndex];
            this._playlistManager.RemovePlaylistDirectory(this.CurrentDirectoryPath, directoryName);
            
            // Update GUI
            this.SubDirectoryPaths.RemoveAt(this.SelectedSubDirectoryIndex);
            
            // Re-assign the index to select the next item (or the last one if the removed item was the last one)
            this.SelectedSubDirectoryIndex = origSelectedIndex < this.SubDirectoryPaths.Count ? origSelectedIndex : this.SubDirectoryPaths.Count - 1;
        }
    }

    private void OpenSubDirectory()
    {
        if (Directory.Exists(this.SelectedSubDirectoryPath))
        {
            this.CurrentDirectoryPath = this.SelectedSubDirectoryPath;
            this.LoadAsCurrentDirectory(this.SelectedSubDirectoryPath);
        }
    }

    private void OpenPlaylist()
    {
        if (this.SelectedPlaylist != null && this.SelectedPlaylist.RelativePath != null)
        {
            this.SelectedPlaylistSortOrder = this.SelectedPlaylist.SortOrder;
            this.IsPlaylistShown = true;
            this.IsFavorite = this._favoritesManager.FavoritePlaylistRelativePaths.Contains(this.SelectedPlaylist.RelativePath);
        }
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
        string request = LanguageUtil.GiveLocalizedString("Str_WhichNameForPlaylist");
        InputDialogViewModel dialogViewModel = new InputDialogViewModel(request);
        bool? result = this._dialogService.ShowInputDialog(dialogViewModel);
        
        // Add the playlist if the dialog was successful
        if (result != null && result.Value)
        {
            // Show error dialog if the playlist name already exists in the current directory.
            string playlistName = dialogViewModel.InputValue;
            if (this.PlaylistsInDirectory.Any(playlist => playlist.Name.ToLower().Equals(playlistName.ToLower())))
            {
                MessageDialogViewModel messageViewModel = new MessageDialogViewModel("Str_Error", "Str_PlaylistNameAlreadyUsed");
                this._dialogService.ShowMessageDialog(messageViewModel);
                return;
            }
            
            // Create playlist
            string relativePath = this._playlistManager.GetRelativePathForNewPlaylist(this.CurrentDirectoryPath, playlistName);
            Playlist playlist = new Playlist(playlistName, relativePath);
            this._playlistManager.SaveInPlaylistFile(playlist);
            
            // Update GUI
            this.PlaylistsInDirectory.Add(playlist);
            this.PlaylistsInDirectory = new ObservableCollection<Playlist>(this.PlaylistsInDirectory.OrderBy(p => p.Name));
        }
    }

    private void RemovePlaylist()
    {
        if (this.SelectedPlaylist != null 
            && this.SelectedPlaylistIndex >= 0 
            && this.SelectedPlaylistIndex < this.PlaylistsInDirectory.Count)
        {
            // Save index to re-assign it after the GUI update
            int origSelectedIndex = this.SelectedPlaylistIndex;
            
            // Remove file
            this._playlistManager.RemovePlaylistFile(this.SelectedPlaylist);
            
            // Remove from Favorites
            this._favoritesManager.RemovePlaylistFromFavorites(this.SelectedPlaylist);
            
            // Update GUI
            this.PlaylistsInDirectory.RemoveAt(this.SelectedPlaylistIndex);
            
            // Re-assign the index to select the next item (or the last one if the removed item was the last one)
            this.SelectedPlaylistIndex = origSelectedIndex < this.PlaylistsInDirectory.Count ? origSelectedIndex : this.PlaylistsInDirectory.Count - 1;
        }
    }
    
    private void DuplicatePlaylist()
    {
        // Stop if no playlist is selected
        if (this.SelectedPlaylist == null)
        {
            return;
        }
        
        // Open dialog to request the duplicate playlist name
        string request = LanguageUtil.GiveLocalizedString("Str_WhichNameForPlaylist");
        InputDialogViewModel dialogViewModel = new InputDialogViewModel(request);
        bool? result = this._dialogService.ShowInputDialog(dialogViewModel);
        
        // Add the playlist if the dialog was successful
        if (result != null && result.Value)
        {
            // Show error dialog if the playlist name already exists in the current directory.
            string playlistName = dialogViewModel.InputValue;
            if (this.PlaylistsInDirectory.Any(playlist => playlist.Name.ToLower().Equals(playlistName.ToLower())))
            {
                MessageDialogViewModel messageViewModel = new MessageDialogViewModel("Str_Error", "Str_PlaylistNameAlreadyUsed");
                this._dialogService.ShowMessageDialog(messageViewModel);
                return;
            }
            
            // Copy the Song objects so that the playlist is not referring to the same song objects.
            // When using the same song objects, the current played song will be shown in both the original and the
            // copied playlist.
            ObservableCollection<Song> copiedSongs = new ObservableCollection<Song>();
            foreach (var song in this.SelectedPlaylist.Songs)
            {
                copiedSongs.Add((Song) song.Clone());
            }

            // Create playlist
            string relativePath = this._playlistManager.GetRelativePathForNewPlaylist(this.CurrentDirectoryPath, playlistName);
            Playlist playlist = new Playlist(playlistName, copiedSongs, this.SelectedPlaylist.SortOrder, relativePath);

            this._playlistManager.SaveInPlaylistFile(playlist);
            
            // Update GUI
            this.PlaylistsInDirectory.Add(playlist);
            this.PlaylistsInDirectory = new ObservableCollection<Playlist>(this.PlaylistsInDirectory.OrderBy(p => p.Name));
        }
    }

    private void ExportPlaylist()
    {
        // Stop if no playlist is selected
        if (this.SelectedPlaylist == null)
        {
            return;
        }
        
        // Open dialog to select the target directory for the export
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        DialogResult dialogResult = dialog.ShowDialog();

        if (dialogResult == DialogResult.OK && Directory.Exists(dialog.SelectedPath))
        {
            // Show error dialog if the directory is not empty.
            if (Directory.GetDirectories(dialog.SelectedPath).Length > 0 || Directory.GetFiles(dialog.SelectedPath).Length > 0)
            {
                MessageDialogViewModel errorDialogViewModel = new MessageDialogViewModel("Str_Error", "Str_TargetDirectoryIsNotEmpty");
                this._dialogService.ShowMessageDialog(errorDialogViewModel);
                return;
            }
            
            // Copy the playlist's songs to the target directory.
            this._playlistManager.Export(dialog.SelectedPath, this.SelectedPlaylist);
            
            // Show success dialog
            MessageDialogViewModel successDialogViewModel = new MessageDialogViewModel("Str_Success", "Str_PlaylistSuccessfullyExported");
            this._dialogService.ShowMessageDialog(successDialogViewModel);
        }
    }

    private void AddToQueue()
    {
        if (this.SelectedPlaylist != null)
        {
            this.SongPlayer.AddToQueue(this.SelectedPlaylist);
        }
    }
    
    private void AddSongToPlaylist()
    {
        // Open dialog to select the songs to add
        OpenFileDialog dialog = new OpenFileDialog()
        {
            Multiselect = true,
            Filter = "*.mp3|*.mp3",
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
            this.SelectedPlaylist.Sort();
                
            // Save changes
            this._playlistManager.SaveInPlaylistFile(this.SelectedPlaylist);
        }
    }

    private void RemoveSongFromPlaylist()
    {
        if (this.SelectedPlaylist != null 
            && this.SelectedSongIndex >= 0 
            && this.SelectedSongIndex < this.SelectedPlaylist.Songs.Count)
        {
            // Save index to re-assign it after the GUI update
            int origSelectedIndex = this.SelectedSongIndex;
            
            // If the Song which should be removed is the current song, play the next song because else the
            // song would still continue playing even if it is removed.
            if (this.SelectedPlaylist.Songs[this.SelectedSongIndex] == this._songPlayer.CurrentSong)
            {
                this._songPlayer.PlayNext();
            }
            
            // Update playlist
            this.SelectedPlaylist.Songs.RemoveAt(this.SelectedSongIndex);

            // Save changes
            this._playlistManager.SaveInPlaylistFile(this.SelectedPlaylist);
            
            // Re-assign the index to select the next item (or the last one if the removed item was the last one)
            this.SelectedSongIndex = origSelectedIndex < this.SelectedPlaylist.Songs.Count ? origSelectedIndex : this.SelectedPlaylist.Songs.Count - 1;
        }
    }

    private void ChangePlaylistSortOrder()
    {
        if (this.SelectedPlaylist != null)
        {
            // Sort
            this.SelectedPlaylist.SortOrder = this.SelectedPlaylistSortOrder;
            this.SelectedPlaylist.Sort();

            // Save changes
            this._playlistManager.SaveInPlaylistFile(this.SelectedPlaylist);
        }
    }

    private void AddToFavorites()
    {
        if (this.SelectedPlaylist != null && this.SelectedPlaylist.RelativePath != null) 
        {
            this._favoritesManager.AddPlaylistToFavorites(this.SelectedPlaylist);
            this.IsFavorite = true;
        }
    }
    
    private void RemoveFromFavorites()
    {
        if (this.SelectedPlaylist != null && this.SelectedPlaylist.RelativePath != null)
        {
            this._favoritesManager.RemovePlaylistFromFavorites(this.SelectedPlaylist);
            this.IsFavorite = false;
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
                this.SelectedPlaylist.Sort();

                // Save changes
                this._playlistManager.SaveInPlaylistFile(this.SelectedPlaylist);
            }
        }
    }
}