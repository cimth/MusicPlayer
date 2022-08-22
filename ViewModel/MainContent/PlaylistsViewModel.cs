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
    // ENUM FOR DIFFERENT SHOWN ELEMENTS
    // ==============

    private enum ShownElements
    {
        ShowPlaylistRootDirectory,
        ShowDirectory,
        ShowPlaylist,
    }
    
    // ==============
    // FIELDS 
    // ==============

    private readonly SongImporter _songImporter;
    private readonly PlaylistManager _playlistManager;

    private string? _currentDirectoryPath;
    private ObservableCollection<string> _subDirectoryPaths;

    private ObservableCollection<string> _playlistPaths;

    private bool _showDirectory;
    private bool _showPlaylist;
    private bool _showPlaylistRootDirectory;

    // ==============
    // PROPERTIES 
    // ==============

    public string? CurrentDirectoryPath
    {
        get => _currentDirectoryPath;
        private set => SetField(ref _currentDirectoryPath, value);
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

    // Current shown elements
    // => Only return the current value. The property changed method is called separate when changing the values to
    //    avoid missing one of the necessary variables (all bool variables have to be updated once because only one
    //    variable is set to true at each time).

    public bool ShowDirectory => _showDirectory;

    public bool ShowPlaylist => _showPlaylist;

    public bool ShowPlaylistRootDirectory => _showPlaylistRootDirectory;
    
    // Commands
    
    public ICommand GoBackCommand { get; }
    
    public ICommand AddSubDirectoryCommand { get; }
    
    public ICommand RemoveSubDirectoryCommand { get; }
    
    public ICommand OpenSubDirectoryCommand { get; }
    
    public ICommand OpenPlaylistCommand { get; }
    
    // ==============
    // INITIALIZATION 
    // ==============

    public PlaylistsViewModel(SongImporter songImporter, PlaylistManager playlistManager)
    {
        this._songImporter = songImporter;
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
        this.UpdateCurrentShownElements(ShownElements.ShowPlaylistRootDirectory);
        this.LoadContents(null);
    }
    
    // ==============
    // UPDATE WHICH ELEMENTS SHOULD BE SHOWN
    // ==============

    private void UpdateCurrentShownElements(ShownElements elementToShow)
    {
        // Hide all elements
        this._showDirectory = false;
        this._showPlaylist = false;
        this._showPlaylistRootDirectory = false;
        
        // Set the correct main content variable to true
        switch (elementToShow)
        {
            case ShownElements.ShowDirectory:
                this._showDirectory = true;
                break;
            case ShownElements.ShowPlaylist:
                this._showPlaylist = true;
                break;
            case ShownElements.ShowPlaylistRootDirectory:
                this._showPlaylistRootDirectory = true;
                break;
        }
        
        // Raise property changed event for all variables
        OnPropertyChanged(nameof(ShowDirectory));
        OnPropertyChanged(nameof(ShowPlaylist));
        OnPropertyChanged(nameof(ShowPlaylistRootDirectory));
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