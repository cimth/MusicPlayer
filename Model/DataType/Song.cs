namespace Model.DataType;

public class Song 
{
    // ==============
    // PROPERTIES
    // ==============

    public string FilePath { get; set; }
    
    public string Title { get; set; }
    public string Album { get; set; }
    public string[] Artists { get; set; }
    public uint TrackNumber { get; set; }
    public TimeSpan Duration { get; set; }
    
    // ==============
    // INITIALIZATION
    // ==============

    public Song(string filePath, string title, string album, string[] artists, uint trackNumber, TimeSpan duration)
    {
        this.FilePath = filePath;
        this.Title = title;
        this.Album = album;
        this.Artists = artists;
        this.TrackNumber = trackNumber;
        this.Duration = duration;
    }
}