using System.Windows.Input;
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
    
    // commands
    public ICommand PauseCommand { get; }
    public ICommand ResumeCommand { get; }
    public ICommand StopCommand { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public CurrentSongViewModel(SongPlayer songPlayer)
    {
        this.SongPlayer = songPlayer;
        
        // initialize commands
        this.PauseCommand = new DelegateCommand(this.Pause);
        this.ResumeCommand = new DelegateCommand(this.Resume);
        this.StopCommand = new DelegateCommand(this.Stop);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void Pause()
    {
        this.SongPlayer.Pause();
    }
    
    private void Resume()
    {
        this.SongPlayer.Resume();
    }
    
    private void Stop()
    {
        this.SongPlayer.Stop();
    }
}