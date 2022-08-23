using System.Collections.ObjectModel;

namespace Model.DataType;

public class Playlist
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; set; }
    
    public ObservableCollection<Song> Songs { get; set; }

    public TimeSpan TotalDuration
    {
        get
        {
            TimeSpan totalDuration = TimeSpan.Zero;
            foreach (var song in this.Songs)
            {
                totalDuration = totalDuration.Add(song.Duration);
            }
            return totalDuration;
        }
    }
    
    // ==============
    // INITIALIZATION
    // ==============

    public Playlist(string name, ObservableCollection<Song> songs)
    {
        this.Name = name;
        this.Songs = songs;
    }
    
    public Playlist(Song song)
    {
        this.Name = song.Title;
        this.Songs = new ObservableCollection<Song> {song};
    }

    public Playlist(string name)
    {
        this.Name = name;
        this.Songs = new ObservableCollection<Song>();
    }
}