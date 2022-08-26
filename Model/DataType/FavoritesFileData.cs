namespace Model.DataType;

public class FavoritesFileData
{
    // ==============
    // PROPERTIES
    // ==============
    
    public List<string> DirectoryPaths { get; }
    
    public List<string> PlaylistRelativePaths { get; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public FavoritesFileData(List<string> directoryPaths, List<string> playlistRelativePaths)
    {
        this.DirectoryPaths = directoryPaths;
        this.PlaylistRelativePaths = playlistRelativePaths;
    }
}