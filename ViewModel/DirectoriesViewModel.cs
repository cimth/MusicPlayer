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
    private readonly SongPlayer _songPlayer;

    private string? _currentDirectoryPath;
    private List<string> _subDirectoryPaths;
    private List<string> _musicFilePaths;

    private bool _hasSubDirectories;
    private bool _hasMusicFiles;

    // ==============
    // PROPERTIES 
    // ==============

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

    public List<string> MusicFilePaths
    {
        get => _musicFilePaths;
        set => SetField(ref _musicFilePaths, value);
    }

    public string? SelectedSubDirectoryPath { get; set; }

    public string? SelectedMusicFilePath { get; set; }
    
    public int SelectedMusicFileIndex { get; set; }

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
    
    public ICommand PlayMusicFileCommand { get; set; }
    
    public ICommand OpenSubDirectoryCommand { get; set; }
    
    public ICommand PlayAllSongsInDirectoryStartingWithTheSelectedSongCommand { get; set; }

    // ==============
    // INITIALIZATION
    // ==============

    public DirectoriesViewModel(SongImporter songImporter, SongPlayer songPlayer)
    {
        this._songImporter = songImporter;
        this._songPlayer = songPlayer;
        this._subDirectoryPaths = new List<string>();
        this._musicFilePaths = new List<string>();

        // test directory
        _currentDirectoryPath = "";
        this.LoadDirectoryContent(_currentDirectoryPath);

        // init commands
        this.OpenSubDirectoryCommand = new DelegateCommand(OpenSubDirectory);
        this.PlayMusicFileCommand = new DelegateCommand(PlaySelectedSong);
        this.PlayAllSongsInDirectoryStartingWithTheSelectedSongCommand = new DelegateCommand(PlayAllSongsInDirectoryStartingWithTheSelectedSong);
    }

    private void LoadDirectoryContent(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            // load sub directories
            string[] subDirectories = Directory.GetDirectories(directoryPath);
            this.SubDirectoryPaths = new List<string>(subDirectories);

            // load music files
            string[] mp3Files = Directory.GetFiles(directoryPath, "*.mp3");
            this.MusicFilePaths = new List<string>(mp3Files);

            // update bool variables for the content (for conditionally showing the ListViews and Separator)
            this.HasSubDirectories = subDirectories.Length > 0;
            this.HasMusicFiles = mp3Files.Length > 0;
        }
    }

    // ==============
    // COMMAND ACTIONS
    // ==============

    private void OpenSubDirectory()
    {
        if (Directory.Exists(SelectedSubDirectoryPath))
        {
            // update current directory
            CurrentDirectoryPath = SelectedSubDirectoryPath;
            this.LoadDirectoryContent(SelectedSubDirectoryPath);
        }
    }

    private void PlaySelectedSong()
    {
        if (File.Exists(this.SelectedMusicFilePath))
        {
            // convert file to playlist
            // => seems overcomplicated but it makes the logic easier to understand since internally only playlists are
            //    played by SongPlayer and there is basically no different logic for single songs and playlists
            string songName = Path.GetFileName(this.SelectedMusicFilePath);
            Playlist playlist = _songImporter.Import(songName, this.SelectedMusicFilePath);
            this._songPlayer.Play(playlist, 0);
        }
    }

    private void PlayAllSongsInDirectoryStartingWithTheSelectedSong()
    {
        if (Directory.Exists(CurrentDirectoryPath))
        {
            Console.WriteLine($"Play all songs in directory '{CurrentDirectoryPath}'");
            
            // get only the directory name without the parent directories
            // => you have to use Path.GetFilename() because Path.GetDirectoryName() would return the path to the
            //    parent directory of CurrentDirectoryPath
            string directoryName = Path.GetFileName(CurrentDirectoryPath);
            
            // convert directory to playlist
            Playlist playlist = _songImporter.Import(directoryName, this.MusicFilePaths);
            
            // play the playlist with the selected song as first song
            this._songPlayer.Play(playlist, this.SelectedMusicFileIndex);
        }
    }
}