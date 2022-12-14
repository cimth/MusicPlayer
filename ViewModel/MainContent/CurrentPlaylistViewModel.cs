using System.Windows.Input;
using Common;
using Model.Service;
using ViewModel.Command;

namespace ViewModel.MainContent;

public class CurrentPlaylistViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============

    private int _selectedPlaylistIndex = -1;
    
    // ==============
    // PROPERTIES
    // ==============

    public SongPlayer SongPlayer { get; }

    public int SelectedPlaylistIndex
    {
        get => _selectedPlaylistIndex; 
        set => SetField(ref _selectedPlaylistIndex, value);
    }
    
    // Commands
    
    public ICommand PlaySelectedSongCommand { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public CurrentPlaylistViewModel(SongPlayer songPlayer)
    {
        this.SongPlayer = songPlayer;

        // Init commands
        this.PlaySelectedSongCommand = new DelegateCommand(this.PlaySelectedSong);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void PlaySelectedSong()
    {
        if (this.SelectedPlaylistIndex != -1)
        {
            this.SongPlayer.ChangePlaylistIndex(this.SelectedPlaylistIndex);
        }
    }
}