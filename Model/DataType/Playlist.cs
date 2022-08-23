namespace Model.DataType;

public class Playlist
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; set; }
    
    public List<Song> Songs { get; set; }

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

    public Playlist(string name, List<Song> songs)
    {
        this.Name = name;
        this.Songs = songs;
    }
    
    public Playlist(Song song)
    {
        this.Name = song.Title;
        this.Songs = new List<Song> {song};
    }

    public Playlist(string name)
    {
        this.Name = name;
        this.Songs = new List<Song>();
    }
}