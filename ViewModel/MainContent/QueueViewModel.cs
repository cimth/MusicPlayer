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
            // Save index to re-assign it after the GUI update
            int origSelectedIndex = this.SelectedQueueIndex;
            
            // Remove the selected playlist
            this.SongPlayer.RemoveFromQueue(this.SelectedQueueIndex);
            
            // Re-assign the index to select the next item (or the last one if the removed item was the last one)
            this.SelectedQueueIndex = origSelectedIndex < this.SongPlayer.Queue.Count ? origSelectedIndex : this.SongPlayer.Queue.Count - 1;
        }
    }
}