using System.Windows.Media;
using Common;
using Model.DataType;

namespace Model.Service;

public class SongPlayer : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============
    
    private readonly MediaPlayer _mediaPlayer = new MediaPlayer();
    
    private Song? _currentSong;
    private bool _isPlaying;
    
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

    // ==============
    // COMMON AUDIO ACTIONS
    // ==============

    public void Play(Song song)
    {
        this.CurrentSong = song;
        this.IsPlaying = true;
        
        _mediaPlayer.Open(new Uri(song.FilePath));
        _mediaPlayer.Play();
    }
    
    public void Pause()
    {
        this.IsPlaying = false;
        _mediaPlayer.Pause();
    }
    
    public void Resume()
    {
        this.IsPlaying = true;
        _mediaPlayer.Play();
    }
    
    public void Stop()
    {
        this.IsPlaying = false;
        _mediaPlayer.Stop();
    }
}