using System.Windows.Input;
using Model.DataType;
using Model.Service;
using ViewModel.Base;
using ViewModel.Command;

namespace ViewModel;

public class CurrentSongViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS FOR DATA BINDING IN VIEW
    // ==============

    #region FIELDS

    private Song _song;

    public Song Song
    {
        get => _song;
        set => SetField(ref _song, value);
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

    public CurrentSongViewModel(SongImporter songImporter, SongPlayer songPlayer)
    {
        this._songImporter = songImporter;
        this._songPlayer = songPlayer;
        
        // test song
        _song = songImporter.Import("");
        
        // init commands
        this.ChangeSongCommand = new DelegateCommand(ChangeSong);
    }

    #endregion
    
    // ==============
    // COMMANDS
    // ==============

    #region COMMANDS

    public ICommand ChangeSongCommand { get; set; }

    private void ChangeSong()
    {
        Console.WriteLine("ChangeSong()");
        this.Song = this._songImporter.Import("");
    }
    
    #endregion
}