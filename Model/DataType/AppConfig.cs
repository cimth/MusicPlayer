using System.Collections.ObjectModel;
using System.IO;

namespace Model.DataType;

public class AppConfig
{
    // ==============
    // CONSTANTS
    // ==============
    
    // The app's config file is placed under "<App base directory>/config.json" (if existing)
    public static readonly string AppBaseDirectoryPath = Path.GetFullPath(AppContext.BaseDirectory);
    public static readonly string AppConfigFilePath = Path.GetFullPath(Path.Combine(AppBaseDirectoryPath, "config.json"));
    
    public static readonly string PlaylistsRootPath = Path.GetFullPath(Path.Combine(AppBaseDirectoryPath, "Playlists"));
    
    // ==============
    // PROPERTIES
    // ==============

    public ObservableCollection<string> MusicDirectories { get; set; }
    
    public bool RepeatPlaylist { get; set; }

    // ==============
    // INITIALIZATION
    // ==============

    public AppConfig()
    {
        this.MusicDirectories = new ObservableCollection<string>();
    }
}