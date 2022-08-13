namespace Model.DataType;

public class Song 
{
    // ==============
    // FIELDS
    // ==============
    
    #region FIELDS

    public string FilePath { get; set; }
    
    public string Title { get; set; }
    public string Album { get; set; }
    public TimeSpan Length { get; set; }

    #endregion
    
    // ==============
    // INITIALIZATION
    // ==============

    #region INITIALIZATION

    public Song(string filePath, string title, string album, TimeSpan length)
    {
        FilePath = filePath;
        Title = title;
        Album = album;
        Length = length;
    }

    #endregion
}