namespace Model.DataType;

public class AppConfig
{    
    // ==============
    // PROPERTIES
    // ==============
    
    public List<string> MusicDirectories { get; set; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public AppConfig()
    {
        this.MusicDirectories = new List<string>();
    }
}