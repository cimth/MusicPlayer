using System.Windows.Input;
using Common;
using Model.Service;
using ViewModel.Command;

namespace ViewModel.MainContent;

public class QueueViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // PROPERTIES
    // ==============

    public SongPlayer SongPlayer { get; }

    public int SelectedQueueIndex { get; set; } = -1;
    
    // Commands
    
    public ICommand RemoveFromQueueCommand { get; }

    // ==============
    // INITIALIZATION
    // ==============

    public QueueViewModel(SongPlayer songPlayer)
    {
        this.SongPlayer = songPlayer;

        // Init commands
        this.RemoveFromQueueCommand = new DelegateCommand(this.RemoveFromQueue);
    }
    
    // ==============
    // COMMAND ACTIONS
    // ==============

    private void RemoveFromQueue()
    {
        if (this.SelectedQueueIndex != -1)
        {
            this.SongPlayer.RemoveFromQueue(this.SelectedQueueIndex);
        }
    }
}