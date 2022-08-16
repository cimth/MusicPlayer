using System.Windows.Media;
using Model.DataType;

namespace Model.Service;

public class SongPlayer
{
    private readonly MediaPlayer _mediaPlayer = new MediaPlayer();

    public void Play(Song song)
    {
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