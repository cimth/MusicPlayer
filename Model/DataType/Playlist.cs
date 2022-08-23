using System.Collections.ObjectModel;

namespace Model.DataType;

public class Playlist
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Name { get; set; }
    
    public ObservableCollection<Song> Songs { get; set; }

    public TimeSpan TotalDuration
    {
        get
        {
            TimeSpan totalDuration = TimeSpan.Zero;
            foreach (var song in this.Songs)
            {
                totalDuration = totalDuration.Add(song.Duration);
            }
            return totalDuration;
        }
    }
    
    // ==============
    // INITIALIZATION
    // ==============

    public Playlist(string name, ObservableCollection<Song> songs)
    {
        this.Name = name;
        this.Songs = songs;
    }
    
    public Playlist(Song song)
    {
        this.Name = song.Title;
        this.Songs = new ObservableCollection<Song> {song};
    }

    public Playlist(string name)
    {
        this.Name = name;
        this.Songs = new ObservableCollection<Song>();
    }
    
    // ==============
    // SORTING
    // ==============
    
    public void SortByTitle()
    {
        // Sort a copy of the collection
        ObservableCollection<Song> sorted = new ObservableCollection<Song>(this.Songs);
        sorted = new ObservableCollection<Song>(sorted.OrderBy(song => song.Title));

        // Clear the current collection and re-add the songs (which are now ordered).
        // Do not simply assign the new sorted collection because then this model class 
        // would have to implement INotifyOnPropertyChanged to notify the GUI on changing
        // the collection which is not desired inside this simple DataType.
        this.Songs.Clear();
        foreach (var song in sorted)
        {
            this.Songs.Add(song);
        }
    }
}