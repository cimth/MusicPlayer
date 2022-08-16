
using System.Windows.Input;
using ViewModel.Base;
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

    private List<string>? _currentDirectoryContentPaths;
    public List<string>? CurrentDirectoryContentPaths
    {
        get => _currentDirectoryContentPaths;
        set => SetField(ref _currentDirectoryContentPaths, value);
    }
    
    public string? SelectedPath { get; set; }

    #endregion
    
    // ==============
    // INITIALIZATION
    // ==============

    #region INITIALIZATION

    public DirectoriesViewModel()
    {
        // test directory
        _currentDirectoryPath = "";
        this.LoadDirectoryContent(_currentDirectoryPath);

        // init commands
        this.DoubleClickOnPathCommand = new DelegateCommand(DoubleClickOnPath);
    }

    private void LoadDirectoryContent(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            string[] subDirectories = Directory.GetDirectories(directoryPath);
            string[] mp3Files = Directory.GetFiles(directoryPath, "*.mp3");

            this.CurrentDirectoryContentPaths = new List<string>();
            this.CurrentDirectoryContentPaths.AddRange(subDirectories);
            this.CurrentDirectoryContentPaths.AddRange(mp3Files);
        }
    }

    #endregion
    
    // ==============
    // PLAY SELECTED MUSIC FILE
    // ==============

    private void PlayMusic(string filePath)
    {
        Console.WriteLine($"Play '{filePath}'");
    }
    
    // ==============
    // COMMANDS
    // ==============

    #region COMMANDS
    
    public ICommand DoubleClickOnPathCommand { get; set; }

    private void DoubleClickOnPath()
    {
        Console.WriteLine($"DoubleClickOnPath() with '{SelectedPath}'");
        
        if (Directory.Exists(SelectedPath))
        {
            this.LoadDirectoryContent(SelectedPath);
        } else if (File.Exists(SelectedPath))
        {
            this.PlayMusic(SelectedPath);
        }
    }
    
    #endregion
}