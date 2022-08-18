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
    
    public ICommand PlayPreviousCommand { get; }
    
    public ICommand PlayNextCommand { get; }
    
    public ICommand RepeatCommand { get; }

    public ICommand SongProgressDragStartedCommand { get; }
    
    public ICommand SongProgressDragCompletedCommand { get; }
    
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
        this.PlayPreviousCommand = new DelegateCommand(this.PlayPrevious);
        this.PlayNextCommand = new DelegateCommand(this.PlayNext);
        this.RepeatCommand = new DelegateCommand(this.Repeat);
        this.SongProgressDragStartedCommand = new DelegateCommand(this.SongProgressDragStarted);
        this.SongProgressDragCompletedCommand = new DelegateCommand(this.SongProgressUpdateCurrentTimeCommand);
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

    private void PlayPrevious()
    {
        this.SongPlayer.PlayPrevious();
    }

    private void PlayNext()
    {
        this.SongPlayer.PlayNext();
    }

    private void Repeat()
    {
        // revert the current status of the repeat variable
        // => if true, set to false (and vice versa)
        this.SongPlayer.RepeatPlaylist = !this.SongPlayer.RepeatPlaylist;
    }

    private void SongProgressDragStarted()
    {
        this.SongPlayer.IsTimerUpdatedEverySecond = false;
    }

    private void SongProgressUpdateCurrentTimeCommand()
    {
        TimeSpan newTimerValue = TimeSpan.FromSeconds(this.SongPlayer.TimerCurrent);
        this.SongPlayer.SetToSpecificTimerValue(newTimerValue);
        this.SongPlayer.IsTimerUpdatedEverySecond = true;
    }
}