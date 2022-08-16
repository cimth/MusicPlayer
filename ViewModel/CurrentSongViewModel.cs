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
    
    private readonly SongImporter _songImporter;
    private SongPlayer _songPlayer;
    
    // ==============
    // PROPERTIES
    // ==============

    public SongPlayer SongPlayer
    {
        get => _songPlayer;
        set => SetField(ref _songPlayer, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============

    public CurrentSongViewModel(SongImporter songImporter, SongPlayer songPlayer)
    {
        this._songImporter = songImporter;
        this._songPlayer = songPlayer;
    }
}