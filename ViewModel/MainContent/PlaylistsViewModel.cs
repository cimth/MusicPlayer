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

    private readonly PlaylistImporter _playlistImporter;
    private readonly PlaylistManager _playlistManager;

    private string? _currentDirectoryPath;
    private string? _currentDirectoryNameFromRoot;
    private ObservableCollection<string> _subDirectoryPaths;

    private ObservableCollection<string> _playlistPaths;

    private bool _isPlaylistShown;

    // ==============
    // PROPERTIES 
    // ==============

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

    public ObservableCollection<string> PlaylistPaths
    {
        get => _playlistPaths;
        private set => SetField(ref _playlistPaths, value);
    }
    
    public string? SelectedPlaylistPath { get; set; }

    public bool IsPlaylistShown
    {
        get => _isPlaylistShown;
        set => SetField(ref _isPlaylistShown, value);
    } 
    
    // Commands
    
    public ICommand GoBackCommand { get; }
    
    public ICommand AddSubDirectoryCommand { get; }
    
    public ICommand RemoveSubDirectoryCommand { get; }
    
    public ICommand OpenSubDirectoryCommand { get; }
    
    public ICommand OpenPlaylistCommand { get; }
    
    // ==============
    // INITIALIZATION 
    // ==============

    public PlaylistsViewModel(PlaylistImporter playlistImporter, PlaylistManager playlistManager)
    {
        this._playlistImporter = playlistImporter;
        this._playlistManager = playlistManager;

        this._subDirectoryPaths = new ObservableCollection<string>();
        this._playlistPaths = new ObservableCollection<string>();
        
        // Init commands
        this.GoBackCommand = new DelegateCommand(this.GoBack);
        this.AddSubDirectoryCommand = new DelegateCommand(this.AddSubDirectory);
        this.RemoveSubDirectoryCommand = new DelegateCommand(this.RemoveSubDirectory);
        this.OpenSubDirectoryCommand = new DelegateCommand(this.OpenSubDirectory);
        this.OpenPlaylistCommand = new DelegateCommand(this.OpenPlaylist);
        
        // Set elements that are shown first
        this._isPlaylistShown = false;
        this.LoadContents(null);
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

        // Load playlist files
        string[] playlistFiles = Directory.GetFiles(directoryPath, "*.json");
        this.PlaylistPaths = new ObservableCollection<string>(playlistFiles);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void GoBack()
    {
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
            this._playlistManager.CreatePlaylistDirectory(this.CurrentDirectoryPath, directoryName);
            
            // Update GUI
            this.SubDirectoryPaths.Insert(0, directoryName);
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
        
    }
}