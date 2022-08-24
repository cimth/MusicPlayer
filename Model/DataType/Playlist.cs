using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Common;

namespace Model.DataType;

public class Playlist : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============

    private string _name;
    private ObservableCollection<Song> _songs;
    private TimeSpan _totalDuration;
    private PlaylistSortOrder _sortOrder = PlaylistSortOrder.Individual;

    // ==============
    // PROPERTIES
    // ==============

    public string Name
    {
        get => _name; 
        set => SetField(ref _name, value);
    }

    public ObservableCollection<Song> Songs => _songs;

    public TimeSpan TotalDuration
    {
        get => _totalDuration;
        set => SetField(ref _totalDuration, value);
    }

    public PlaylistSortOrder SortOrder
    {
        get => _sortOrder;
        set => SetField(ref _sortOrder, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public Playlist(string name, ObservableCollection<Song> songs, PlaylistSortOrder sortOrder)
    {
        this._name = name;
        this._songs = songs;
        this._sortOrder = sortOrder;
        
        this._songs.CollectionChanged += UpdateTotalDuration;
        this.UpdateTotalDuration(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public Playlist(string name, ObservableCollection<Song> songs)
    {
        this._name = name;
        this._songs = songs;
        
        this._songs.CollectionChanged += UpdateTotalDuration;
        this.UpdateTotalDuration(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
    
    public Playlist(Song song)
    {
        this._name = song.Title;
        this._songs = new ObservableCollection<Song> {song};
        
        this._songs.CollectionChanged += UpdateTotalDuration;
        this.UpdateTotalDuration(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public Playlist(string name)
    {
        this._name = name;
        this._songs = new ObservableCollection<Song>();
        
        this._songs.CollectionChanged += UpdateTotalDuration;
        this.UpdateTotalDuration(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
    
    // ==============
    // UPDATE TOTAL DURATION
    // ==============
    
    private void UpdateTotalDuration(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        TimeSpan totalDuration = TimeSpan.Zero;
        foreach (var song in this.Songs)
        {
            totalDuration = totalDuration.Add(song.Duration);
        }
        this.TotalDuration = totalDuration;
    }
    
    // ==============
    // SORTING
    // ==============
    
    public void Sort()
    {
        List<Song> ordered;
        switch (this.SortOrder)
        {
            case PlaylistSortOrder.Alphabetical:
                ordered = this.Songs.OrderBy(song => song.Title).ToList();
                this.ReAddToSongs(ordered);
                break;
            case PlaylistSortOrder.Individual:
                // No further ordering, only display it for the sake of completeness
                break;
            case PlaylistSortOrder.TitleNumber:
                ordered = this.Songs.OrderBy(song => song.TrackNumber).ToList();
                this.ReAddToSongs(ordered);
                break;
        }
    }

    /// <summary>
    /// Clear the current collection and re-add the given songs (which should already be ordered).
    /// Do not simply assign the new sorted collection because then the registered Observers of the
    /// CollectionChanged event of the current collection would not be notified anymore on changes.
    /// </summary>
    /// <param name="ordered"></param>
    private void ReAddToSongs(List<Song> ordered)
    {
        // Clear the current collection and re-add the songs (which are now ordered).
        // Do not simply assign the new sorted collection because then the registered Observers of the
        // CollectionChanged event of the current collection would not be notified anymore on changes.
        this.Songs.Clear();
        foreach (var song in ordered)
        {
            this.Songs.Add(song);
        }
    }
}