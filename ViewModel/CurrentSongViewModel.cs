using System.Windows.Input;
using Model.DataType;
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
    // INITIALIZATION
    // ==============

    #region INITIALIZATION

    public CurrentSongViewModel()
    {
        // test song
        _song = new Song("", "Song title", "Song album", TimeSpan.Zero);
        
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
        this.Song = new Song("", "Changed title", "Changed album", TimeSpan.Zero);
    }
    
    #endregion
}