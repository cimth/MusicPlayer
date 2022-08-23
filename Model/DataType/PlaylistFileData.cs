namespace Model.DataType;

public class PlaylistFileData
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; }
    
    public List<string> SongPaths { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public PlaylistFileData(string name, List<string> songPaths)
    {
        this.Name = name;
        this.SongPaths = songPaths;
    }
}