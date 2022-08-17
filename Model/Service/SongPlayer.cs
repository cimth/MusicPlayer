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

    private Song? _currentSong;
    private bool _isPlaying;

    // timer
    private double? _timerMax;
    private double? _timerCurrent;

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

    public double? TimerMax
    {
        get => _timerMax;
        set => SetField(ref _timerMax, value);
    }
    
    public double? TimerCurrent
    {
        get => _timerCurrent;
        set => SetField(ref _timerCurrent, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============

    public SongPlayer()
    {
        this._mediaPlayer = new MediaPlayer();
        this.InitTimerUpdate();
    }

    private void InitTimerUpdate()
    {
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += timer_Tick;
        timer.Start();
    }
    
    // ==============
    // TIMER UPDATE
    // ==============

    private void timer_Tick(object? sender, EventArgs eventArgs)
    {
        // update the current time of the played song if existing
        if (this._mediaPlayer.Source != null && this._mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            this.TimerCurrent = this._mediaPlayer.Position.TotalSeconds;
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