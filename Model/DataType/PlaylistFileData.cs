namespace Model.DataType;

public class PlaylistFileData
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; }
    
    public List<string> SongPaths { get; }

    public string SortOrder { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public PlaylistFileData(string name, List<string> songPaths, string sortOrder)
    {
        this.Name = name;
        this.SongPaths = songPaths;
        this.SortOrder = sortOrder;
    }
}