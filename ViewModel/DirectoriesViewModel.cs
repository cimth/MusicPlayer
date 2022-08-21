using System.Collections.ObjectModel;
using System.Windows.Input;
using Common;
using Model.DataType;
using Model.Service;
using ViewModel.Command;

namespace ViewModel;

public class DirectoriesViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============

    private readonly SongImporter _songImporter;
    private readonly AppConfigurator _appConfigurator;

    private string? _currentDirectoryPath;
    private List<string> _subDirectoryPaths;
    private string? _selectedSubDirectoryPath;
    
    private Playlist? _playlistFromDirectory;

    private bool _hasSubDirectories;
    private bool _hasMusicFiles;

    // ==============
    // PROPERTIES 
    // ==============
    
    public SongPlayer SongPlayer { get; }

    // Forward the app config data to avoid a to nested access in the View
    public ObservableCollection<string> RootMusicDirectories => this._appConfigurator.AppConfig.MusicDirectories;

    public string? CurrentDirectoryPath
    {
        get => _currentDirectoryPath;
        set => SetField(ref _currentDirectoryPath, value);
    }

    public List<string> SubDirectoryPaths
    {
        get => _subDirectoryPaths;
        set => SetField(ref _subDirectoryPaths, value);
    }

    public Playlist? PlaylistFromDirectory
    {
        get => _playlistFromDirectory;
        set => SetField(ref _playlistFromDirectory, value);
    }

    // Needs to be set up as observable Property because it is used in both the root directory GUI element and in the
    // actual directory content GUI element.
    // When only providing the Property with simple `get` and `set` implementation, entering sub directories after
    // going back to the root directory element does not work any longer.
    public string? SelectedSubDirectoryPath
    {
        get => _selectedSubDirectoryPath;
        set => SetField(ref _selectedSubDirectoryPath, value); 
    }

    public Song? SelectedSong { get; set; }
    
    public int SelectedPlaylistIndex { get; set; }

    public bool HasSubDirectories
    {
        get => _hasSubDirectories;
        set
        {
            SetField(ref _hasSubDirectories, value);
            OnPropertyChanged(nameof(HasSubDirectoriesAndMusicFiles));
        }
    }

    public bool HasMusicFiles
    {
        get => _hasMusicFiles;
        set
        {
            SetField(ref _hasMusicFiles, value);
            OnPropertyChanged(nameof(HasSubDirectoriesAndMusicFiles));
        }
    }

    public bool HasSubDirectoriesAndMusicFiles => this.HasSubDirectories && this.HasMusicFiles;

    // commands

    public ICommand AddMusicDirectoryCommand { get; }
    
    public ICommand RemoveMusicDirectoryCommand { get; }
    
    public ICommand PlayMusicFileCommand { get; set; }
    
    public ICommand OpenSubDirectoryCommand { get; set; }
    
    public ICommand PlayAllSongsInDirectoryStartingWithTheSelectedSongCommand { get; set; }
    
    public ICommand GoBackCommand { get; }

    // ==============
    // INITIALIZATION
    // ==============

    public DirectoriesViewModel(SongImporter songImporter, SongPlayer songPlayer, AppConfigurator appConfigurator)
    {
        this.SongPlayer = songPlayer;
        
        this._songImporter = songImporter;
        this._appConfigurator = appConfigurator;
        this._subDirectoryPaths = new List<string>();
        
        // Set current directory to null to show the root directories first
        this._currentDirectoryPath = null;

        // Init commands
        this.AddMusicDirectoryCommand = new DelegateCommand(AddMusicDirectory);
        this.RemoveMusicDirectoryCommand = new DelegateCommand(RemoveMusicDirectory);
        this.OpenSubDirectoryCommand = new DelegateCommand(OpenSubDirectory);
        this.PlayMusicFileCommand = new DelegateCommand(PlaySelectedSong);
        this.PlayAllSongsInDirectoryStartingWithTheSelectedSongCommand = new DelegateCommand(PlayAllSongsInDirectoryStartingWithTheSelectedSong);
        this.GoBackCommand = new DelegateCommand(GoBack);
    }
    
    // ==============
    // CHANGE DIRECTORY
    // ==============
    
    private void LoadAsCurrentDirectory(string directoryPath)
    {
        // Update current directory
        this.CurrentDirectoryPath = directoryPath;

        // Get only the directory name without the parent directories
        // => You have to use Path.GetFilename() because Path.GetDirectoryName() would return the path to the
        //    parent directory of CurrentDirectoryPath
        string directoryName = Path.GetFileName(directoryPath);

        // Load sub directories
        string[] subDirectories = Directory.GetDirectories(directoryPath);
        this.SubDirectoryPaths = new List<string>(subDirectories);

        // Load music files
        string[] mp3Files = Directory.GetFiles(directoryPath, "*.mp3");

        // Update bool variables for the content (for conditionally showing the ListViews and Separator)
        this.HasSubDirectories = subDirectories.Length > 0;
        this.HasMusicFiles = mp3Files.Length > 0;

        // Create Playlist if songs exist in the new directory, else reset to null
        this.PlaylistFromDirectory = this.HasMusicFiles ? this._songImporter.Import(directoryName, mp3Files.ToList()) : null;
    }

    // ==============
    // COMMAND ACTIONS
    // ==============

    private void AddMusicDirectory()
    {
        // Open dialog to select the directory to add
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        DialogResult dialogResult = dialog.ShowDialog();
        
        if (dialogResult == DialogResult.OK && Directory.Exists(dialog.SelectedPath))
        {
            this._appConfigurator.AddDirectory(dialog.SelectedPath);
        }
    }
    
    private void RemoveMusicDirectory()
    {
        if (this.SelectedSubDirectoryPath != null)
        {
            this._appConfigurator.RemoveDirectory(this.SelectedSubDirectoryPath);
        }
    }

    private void OpenSubDirectory()
    {
        if (Directory.Exists(this.SelectedSubDirectoryPath))
        {
            this.LoadAsCurrentDirectory(this.SelectedSubDirectoryPath);
        }
    }

    private void PlaySelectedSong()
    {
        if (this.SelectedSong != null)
        {
            // convert file to playlist
            // => seems overcomplicated but it makes the logic easier to understand since internally only playlists are
            //    played by SongPlayer and there is basically no different logic for single songs and playlists
            Playlist playlist = new Playlist(this.SelectedSong);
            this.SongPlayer.Play(playlist, 0);
        }
    }

    private void PlayAllSongsInDirectoryStartingWithTheSelectedSong()
    {
        if (Directory.Exists(CurrentDirectoryPath) && this.PlaylistFromDirectory != null)
        {
            // play the directory with the selected song as first song
            Console.WriteLine($"Play all songs in directory '{CurrentDirectoryPath}'");
            this.SongPlayer.Play(this.PlaylistFromDirectory, this.SelectedPlaylistIndex);
        }
    }

    private void GoBack()
    {
        if (Directory.Exists(this.CurrentDirectoryPath))
        {
            string parentPath = Directory.GetParent(this.CurrentDirectoryPath)!.FullName;

            if (this.RootMusicDirectories.Contains(this.CurrentDirectoryPath))
            {
                // The current directory is a root directory, thus set the current directory to null for showing
                // the root directories instead of the current (root) directory's parent
                this.CurrentDirectoryPath = null;
            }
            else
            {
                // The current directory is no root directory, thus load the contents of the parent directory
                this.LoadAsCurrentDirectory(parentPath);
            }
        }
    }
}