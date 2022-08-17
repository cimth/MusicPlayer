namespace Model.DataType;

public class SimplePlaylist
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; set; }
    
    public List<Song> Songs { get; set; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public SimplePlaylist(string name, List<Song> songs)
    {
        this.Name = name;
        this.Songs = songs;
    }
}