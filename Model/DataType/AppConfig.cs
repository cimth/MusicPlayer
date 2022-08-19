using System.Collections.ObjectModel;

namespace Model.DataType;

public class AppConfig
{    
    // ==============
    // PROPERTIES
    // ==============
    
    public ObservableCollection<string> MusicDirectories { get; set; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public AppConfig()
    {
        this.MusicDirectories = new ObservableCollection<string>();
    }
}