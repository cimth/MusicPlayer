
using System.Windows.Input;
using Common;
using Model.DataType;
using Model.Service;
using ViewModel.Command;

namespace ViewModel;

public class DirectoriesViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS FOR DATA BINDING IN VIEW
    // ==============

    #region FIELDS

    private string? _currentDirectoryPath;
    public string? CurrentDirectoryPath
    {
        get => _currentDirectoryPath;
        set => SetField(ref _currentDirectoryPath, value);
    }

    private List<string>? _subDirectoryPaths;
    public List<string>? SubDirectoryPaths
    {
        get => _subDirectoryPaths;
        set => SetField(ref _subDirectoryPaths, value);
    }

    private List<string>? _musicFilePaths;
    public List<string>? MusicFilePaths
    {
        get => _musicFilePaths;
        set => SetField(ref _musicFilePaths, value);
    }
    
    public string? SelectedSubDirectoryPath { get; set; }
    
    public string? SelectedMusicFilePath { get; set; }

    private bool _hasSubDirectoriesAndMusicFiles;

    public bool HasSubDirectoriesAndMusicFiles
    {
        get => _hasSubDirectoriesAndMusicFiles;
        set => SetField(ref _hasSubDirectoriesAndMusicFiles, value);
    }

    #endregion
    
    // ==============
    // FIELDS FOR MODEL SERVICES
    // ==============

    private readonly SongImporter _songImporter;
    private readonly SongPlayer _songPlayer;
    
    // ==============
    // INITIALIZATION
    // ==============

    #region INITIALIZATION

    public DirectoriesViewModel(SongImporter songImporter, SongPlayer songPlayer)
    {
        this._songImporter = songImporter;
        this._songPlayer = songPlayer;
        
        // test directory
        _currentDirectoryPath = "";
        this.LoadDirectoryContent(_currentDirectoryPath);

        // init commands
        this.OpenSubDirectoryCommand = new DelegateCommand(OpenSubDirectory);
        this.PlayMusicFileCommand = new DelegateCommand(PlayMusicFile);
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

            // check if both sub directories and music files exist (for conditionally showing a seperator)
            this.HasSubDirectoriesAndMusicFiles = subDirectories.Length > 0 && mp3Files.Length > 0;
        }
    }

    #endregion
    
    // ==============
    // PLAY SELECTED MUSIC FILE
    // ==============

    private void PlayMusic(string filePath)
    {
        Console.WriteLine($"Play '{filePath}'");
        Song song = _songImporter.Import(filePath);
        this._songPlayer.Play(song);
    }
    
    // ==============
    // COMMANDS
    // ==============

    #region COMMANDS
    
    public ICommand OpenSubDirectoryCommand { get; set; }

    private void OpenSubDirectory()
    {
        if (Directory.Exists(SelectedSubDirectoryPath))
        {
            this.LoadDirectoryContent(SelectedSubDirectoryPath);
        }
    }
    
    public ICommand PlayMusicFileCommand { get; set; }

    private void PlayMusicFile()
    {
        if (File.Exists(SelectedMusicFilePath))
        {
            this.PlayMusic(SelectedMusicFilePath);
        }
    }
    
    #endregion
}