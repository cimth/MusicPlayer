namespace Model.DataType;

public class Song : ICloneable
{
    // ==============
    // PROPERTIES
    // ==============

    public bool IsFileMissing { get; } = false;

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

    /// <summary>
    /// This constructor should only be used when the song file which should be placed under the given path is not
    /// existing.
    /// The file path is also used as title for the song to display it in a user-friendly way when showing it in a
    /// playlist.
    /// </summary>
    /// <param name="missingFilePath">The missing file path.</param>
    public Song(string missingFilePath)
    {
        this.IsFileMissing = true;
        
        this.FilePath = missingFilePath;
        this.Title = missingFilePath;
        this.Album = "";
        this.Artists = Array.Empty<string>();
        this.TrackNumber = 0;
        this.Duration = TimeSpan.Zero; 
    }
    
    // ==============
    // CLONE
    // ==============

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}