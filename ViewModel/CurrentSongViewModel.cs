using Common;
using Model.DataType;
using Model.Service;
using ViewModel.Command;

namespace ViewModel;

public class CurrentSongViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============
    
    #region FIELDS
    
    private readonly SongImporter _songImporter;
    
    #endregion
    
    // ==============
    // PROPERTIES
    // ==============

    #region PROPERTIES

    private SongPlayer _songPlayer;
    public SongPlayer SongPlayer
    {
        get => _songPlayer;
        set => SetField(ref _songPlayer, value);
    }

    #endregion
    
    // ==============
    // INITIALIZATION
    // ==============

    #region INITIALIZATION

    public CurrentSongViewModel(SongImporter songImporter, SongPlayer songPlayer)
    {
        this._songImporter = songImporter;
        this._songPlayer = songPlayer;
    }

    #endregion
    
    // ==============
    // COMMANDS
    // ==============

    #region COMMANDS
    
    #endregion
}