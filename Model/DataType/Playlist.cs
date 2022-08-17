namespace Model.DataType;

public class Playlist
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; set; }
    
    public List<Song> Songs { get; set; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public Playlist(string name, List<Song> songs)
    {
        this.Name = name;
        this.Songs = songs;
    }
}