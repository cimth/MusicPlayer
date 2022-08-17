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
    
    private readonly MediaPlayer _mediaPlayer;

    private Playlist? _currentPlaylist;
    private int _currentPlaylistIndex;
    
    private Song? _currentSong;
    private bool _isPlaying;

    // timer
    private double _timerMax;
    private double _timerCurrent;
    private bool _isTimerUpdatedEverySecond;

    // ==============
    // PROPERTIES
    // ==============
    
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

    public SongPlayer()
    {
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
    // SONG ENDED
    // ==============

    private void mediaPlayer_Ended(object? sender, EventArgs eventArgs)
    {
        if (this._currentPlaylist != null)
        {
            if (this._currentPlaylistIndex < this._currentPlaylist.Songs.Count - 1)
            {
                // play next song in playlist
                this._currentPlaylistIndex += 1;
                this.Play(this._currentPlaylist.Songs[_currentPlaylistIndex]);
            }
            else
            {
                Console.WriteLine($"End of playlist {_currentPlaylist.Name} reached.");
            }
        }
    }

    // ==============
    // COMMON AUDIO ACTIONS
    // ==============

    public void Play(Song song)
    {
        this.CurrentSong = song;
        this.IsPlaying = true;

        this.TimerCurrent = 0;
        this.TimerMax = this.CurrentSong.Duration.TotalSeconds;

        _mediaPlayer.Open(new Uri(song.FilePath));
        _mediaPlayer.Play();
    }
    
    public void Play(Playlist playlist, int indexOfFirstSongToBePlayed)
    {
        this._currentPlaylist = playlist;
        this._currentPlaylistIndex = indexOfFirstSongToBePlayed;

        // play the first song
        Song songToPlay = playlist.Songs[indexOfFirstSongToBePlayed];
        this.Play(songToPlay);
    }
    
    public void Pause()
    {
        if (this._isPlaying)
        {
            this.IsPlaying = false;
            _mediaPlayer.Pause();
        }
    }
    
    public void Resume()
    {
        if (!this._isPlaying && this.CurrentSong != null)
        {
            this.IsPlaying = true;
            _mediaPlayer.Play();
        }
    }
    
    public void Stop()
    {
        if (this._isPlaying)
        {
            this.IsPlaying = false;
            _mediaPlayer.Stop();
        }
    }
}