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
    
    // ==============
    // PROPERTIES
    // ==============
    
    public Song? CurrentSong
    {
        get => _currentSong;
        set => SetField(ref _currentSong, value);
    }

    // ==============
    // COMMON AUDIO ACTIONS
    // ==============

    public void Play(Song song)
    {
        this.CurrentSong = song;
        _mediaPlayer.Open(new Uri(song.FilePath));
        _mediaPlayer.Play();
    }
    
    public void Pause()
    {
        _mediaPlayer.Pause();
    }
    
    public void Stop()
    {
        _mediaPlayer.Stop();
    }
}