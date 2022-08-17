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

    private bool _hasSubDirectoriesAndMusicFiles;

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

    public bool HasSubDirectoriesAndMusicFiles
    {
        get => _hasSubDirectoriesAndMusicFiles;
        set => SetField(ref _hasSubDirectoriesAndMusicFiles, value);
    }
    
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
        this.PlayMusicFileCommand = new DelegateCommand(PlayMusicFile);
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

            // check if both sub directories and music files exist (for conditionally showing a separator)
            this.HasSubDirectoriesAndMusicFiles = subDirectories.Length > 0 && mp3Files.Length > 0;
        }
    }

    // ==============
    // PLAY SELECTED MUSIC FILE
    // ==============

    private void PlaySingleSong(string filePath)
    {
        Console.WriteLine($"Play '{filePath}'");
        Song song = _songImporter.Import(filePath);
        this._songPlayer.Play(song);
    }

    private void PlayAllSongsInDirectoryStartingWIthTheSelectedSong(string directoryPath)
    {
        if (Directory.Exists(directoryPath) && File.Exists(this.SelectedMusicFilePath))
        {
            // convert directory to playlist
            Console.WriteLine($"Play all songs in directory '{directoryPath}'");
            string directoryName = Path.GetDirectoryName(directoryPath)!;
            Playlist playlist = _songImporter.Import(directoryName, this.MusicFilePaths);
            
            // play the selected song as first song
            Song firstSong = playlist.Songs[this.SelectedMusicFileIndex];
            Console.WriteLine($"Start with song '{firstSong.Title}'");
            _songPlayer.Play(firstSong);
        }
    }

    // ==============
    // COMMAND ACTIONS
    // ==============

    private void OpenSubDirectory()
    {
        if (Directory.Exists(SelectedSubDirectoryPath))
        {
            this.LoadDirectoryContent(SelectedSubDirectoryPath);
        }
    }

    private void PlayMusicFile()
    {
        if (File.Exists(SelectedMusicFilePath))
        {
            this.PlaySingleSong(SelectedMusicFilePath);
        }
    }

    private void PlayAllSongsInDirectoryStartingWithTheSelectedSong()
    {
        if (Directory.Exists(CurrentDirectoryPath))
        {
            this.PlayAllSongsInDirectoryStartingWIthTheSelectedSong(CurrentDirectoryPath);
        }
    }
}