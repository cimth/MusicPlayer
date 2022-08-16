using Common;
using Model.DataType;
using Model.Service;
using ViewModel.Command;

namespace ViewModel;

public class CurrentSongViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // PROPERTIES
    // ==============

    public SongPlayer SongPlayer { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public CurrentSongViewModel(SongPlayer songPlayer)
    {
        this.SongPlayer = songPlayer;
    }
}