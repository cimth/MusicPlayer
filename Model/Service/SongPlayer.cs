using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;
using Common;
using Model.DataType;

namespace Model.Service;

public class SongPlayer : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============

    private readonly AppConfigurator _appConfigurator;
    
    private readonly MediaPlayer _mediaPlayer;

    private Playlist? _currentPlaylist;
    private int _currentPlaylistIndex;
    private bool _repeatPlaylist;
    
    private Song? _currentSong;
    private bool _isPlaying;

    // timer
    private double _timerMax;
    private double _timerCurrent;
    private bool _isTimerUpdatedEverySecond;

    // ==============
    // PROPERTIES
    // ==============

    public bool RepeatPlaylist
    {
        get => _repeatPlaylist;
        set
        {
            SetField(ref _repeatPlaylist, value);
            
            // Save in config to restore on restart
            this._appConfigurator.SaveRepeatPlaylist(this.RepeatPlaylist);
        } 
    }

    public Playlist? CurrentPlaylist
    {
        get => _currentPlaylist;
        private set
        {
            SetField(ref _currentPlaylist, value);
            
            // Get notified when the playlist is changed (e.g. by moving or deleting songs)
            if (_currentPlaylist != null)
            {
                _currentPlaylist.Songs.CollectionChanged += this.UpdateAfterPlaylistChange;
            }
        } 
    }
    
    public Song? CurrentSong
    {
        get => _currentSong;
        set => SetField(ref _currentSong, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => SetField(ref _isPlaying, value);
    }

    public ObservableCollection<Playlist> Queue { get; }

    // timer

    public double TimerMax
    {
        get => _timerMax;
        set => SetField(ref _timerMax, value);
    }
    
    public double TimerCurrent
    {
        get => _timerCurrent;
        set => SetField(ref _timerCurrent, value);
    }

    public bool IsTimerUpdatedEverySecond
    {
        get => _isTimerUpdatedEverySecond;
        set => SetField(ref _isTimerUpdatedEverySecond, value);
    }

    // ==============
    // INITIALIZATION
    // ==============

    public SongPlayer(AppConfigurator appConfigurator)
    {
        this._appConfigurator = appConfigurator;
        this._repeatPlaylist = this._appConfigurator.AppConfig.RepeatPlaylist;

        this.Queue = new ObservableCollection<Playlist>();

        this._mediaPlayer = new MediaPlayer();
        this._mediaPlayer.MediaEnded += this.mediaPlayer_Ended;

        this.InitTimer();
    }

    private void InitTimer()
    {
        // init seconds values
        this.TimerCurrent = 0;
        this.TimerMax = 0;
        
        // enable the secondly update of the current time
        this.IsTimerUpdatedEverySecond = true;
        
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += timer_Tick;
        timer.Start();
    }
    
    // ==============
    // TIMER UPDATE
    // ==============
    
    public  void SetToSpecificTimerValue(TimeSpan specificTimerValue)
    {
        if (this._mediaPlayer.Source != null && this._mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            this._mediaPlayer.Position = specificTimerValue;
        }
    }

    private void timer_Tick(object? sender, EventArgs eventArgs)
    {
        // update the current time of the played song if existing
        if (this._mediaPlayer.Source != null 
            && this._mediaPlayer.NaturalDuration.HasTimeSpan
            && this.IsTimerUpdatedEverySecond)
        {
            this.TimerCurrent = this._mediaPlayer.Position.TotalSeconds;
        }
    }
    
    // ==============
    // PLAYLIST WAS UPDATED
    // ==============
    
    private void UpdateAfterPlaylistChange(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (this.CurrentPlaylist != null && this.CurrentSong != null)
        {
            // Re-initialize the index of the played song in the current playlist.
            // The index might have changed because the song itself was moved or because another song was
            // inserted or removed before the current song.
            for (int i = 0; i < this.CurrentPlaylist.Songs.Count; i++)
            {
                Song song = this.CurrentPlaylist.Songs[i];
                if (song == this.CurrentSong)
                {
                    this._currentPlaylistIndex = i;
                    break;
                }
            }
        }
    }
    
    // ==============
    // SONG ENDED
    // ==============

    private void mediaPlayer_Ended(object? sender, EventArgs eventArgs)
    {
        this.PlayNext();
    }
    
    // ==============
    // QUEUE
    // ==============

    public void AddToQueue(Playlist playlist)
    {
        this.Queue.Add(playlist);
    }
    
    public void RemoveFromQueue(int index)
    {
        this.Queue.RemoveAt(index);
    }

    // ==============
    // COMMON AUDIO ACTIONS
    // ==============

    public void Play(Playlist playlist, int indexOfFirstSongToBePlayed)
    {
        Console.WriteLine($"Start Playlist '{playlist.Name}'");
        
        this.CurrentPlaylist = playlist;
        this._currentPlaylistIndex = indexOfFirstSongToBePlayed;

        // Play the first song
        Song songToPlay = playlist.Songs[indexOfFirstSongToBePlayed];
        this.Play(songToPlay);
    }
    
    public void Pause()
    {
        if (this.IsPlaying)
        {
            this.IsPlaying = false;
            _mediaPlayer.Pause();
        }
    }
    
    public void Resume()
    {
        // Play / resume the loaded song
        if (!this.IsPlaying && this.CurrentSong != null)
        {
            this.IsPlaying = true;
            _mediaPlayer.Play();
            return;
        }

        // No song or playlist loaded. Play the first playlist from the Queue if existing.
        if (this.CurrentSong == null && this.CurrentPlaylist == null && this.Queue.Count > 0)
        {
            this.Play(this.Queue[0], 0);
            this.RemoveFromQueue(0);
        }
    }
    
    public void Stop()
    {
        if (this.IsPlaying)
        {
            this.IsPlaying = false;
            _mediaPlayer.Stop();
        }
    }

    public void PlayPrevious()
    {
        // Stop if no playlist is loaded
        if (this.CurrentPlaylist == null)
        {
            return;
        }
        
        // Get the new index
        // => Use modulo division to go to the last song of the playlist if the current song is the first song
        this._currentPlaylistIndex = MathUtil.MathMod(this._currentPlaylistIndex - 1, this.CurrentPlaylist.Songs.Count);

        // Play previous song
        this.Play(this.CurrentPlaylist.Songs[this._currentPlaylistIndex]);
    }

    public void PlayNext()
    {
        // Stop if no playlist is loaded
        if (this.CurrentPlaylist == null)
        {
            return;
        }
        
        // Get helping booleans for easier understanding
        bool isLastSong = this._currentPlaylistIndex == this.CurrentPlaylist.Songs.Count - 1;
        
        if (isLastSong && this.Queue.Count > 0)
        {
            // Start the next playlist from the Queue
            this.Play(this.Queue[0], 0);
            this.Queue.RemoveAt(0);
        } else if ( !isLastSong || (isLastSong && this.RepeatPlaylist) ) {
            // Get the new index for the next song
            // => Use modulo division to go to the first song of the playlist if the current song is the last song
            this._currentPlaylistIndex = MathUtil.MathMod(this._currentPlaylistIndex + 1, this.CurrentPlaylist.Songs.Count);
        
            // Play next song
            this.Play(this.CurrentPlaylist.Songs[this._currentPlaylistIndex]);
        }
    }
    
    public void ChangePlaylistIndex(int selectedPlaylistIndex)
    {
        if (this.CurrentPlaylist != null 
            && selectedPlaylistIndex > 0 
            && selectedPlaylistIndex < this.CurrentPlaylist.Songs.Count)
        {
            this._currentPlaylistIndex = selectedPlaylistIndex;
            this.Play(this.CurrentPlaylist.Songs[selectedPlaylistIndex]);
        }
    }

    // ==============
    // HELPING METHODS
    // ==============
    
    private void Play(Song song)
    {
        // Skip the song if it is missing (i.e. the path does not exist).
        bool skipped = this.SkipSongIfMissing(song);
        if (skipped)
        {
            Console.WriteLine($"Song {song} was skipped because its file does not exist.");
            return;
        }
        
        // Play song
        Console.WriteLine($"Start song '{song.Title}'");
        
        this.CurrentSong = song;
        this.IsPlaying = true;

        this.TimerCurrent = 0;
        this.TimerMax = this.CurrentSong.Duration.TotalSeconds;

        _mediaPlayer.Open(new Uri(song.FilePath));
        _mediaPlayer.Play();
    }

    /// <summary>
    /// Skip the given song if its file is missing (i.e. the path does not exist).
    /// Returns true if the song is skipped.
    /// You do not have to do any further logic when the song is skipped like playing the next song etc.
    /// It is already done by this method.
    /// </summary>
    /// <param name="song">The song which possibly should be skipped.</param>
    /// <returns>True if the song is skipped, else false.</returns>
    private bool SkipSongIfMissing(Song song)
    {
        // Check if the file is already marked as missing.
        bool skipped = song.IsFileMissing;

        // The file is not marked yet, so do it now.
        if (!skipped && !File.Exists(song.FilePath))
        {
            // Substitute the song object with a dummy object so that the user can see the missing file path.
            // Do not use the current playlist index because the collection will be changed what causes the index
            // to be re-computed and the current playlist index will be wrong until resetting it manually after
            // the collection changes.
            int index = _currentPlaylistIndex;
            this.CurrentPlaylist?.Songs.RemoveAt(index);
            this.CurrentPlaylist?.Songs.Insert(index, new Song(song.FilePath));
                
            // Reset playlist index. If another song was played previously, the current index is now set to its
            // index instead to the substituted song because of the collection update.
            // With this manual change, the missing song is now the current index, again.
            this._currentPlaylistIndex = index;

            // Set the result flag.
            skipped = true;
        }

        // If the song should be skipped, play the next song available.
        if (skipped)
        {
            this.PlayNext();
        }

        // True if the song is skipped, else false.
        return skipped;
    }
}