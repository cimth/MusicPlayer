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
    private PlaylistSortOrder _sortOrder = PlaylistSortOrder.Alphabetical;
    private string? _relativePath;

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

    public string? RelativePath
    {
        get => _relativePath;
        set => SetField(ref _relativePath, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public Playlist(string name, ObservableCollection<Song> songs, PlaylistSortOrder sortOrder, string relativePath)
    {
        this._name = name;
        this._songs = songs;
        this._sortOrder = sortOrder;
        this._relativePath = relativePath;
        
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

    public Playlist(string name, string relativePath)
    {
        this._name = name;
        this._songs = new ObservableCollection<Song>();
        this._relativePath = relativePath;
        
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
        switch (this.SortOrder)
        {
            case PlaylistSortOrder.Alphabetical:
                SortUtil.SortWithoutNewCollection(this.Songs, song => song.Title);
                break;
            case PlaylistSortOrder.Individual:
                // No further ordering, only display it for the sake of completeness
                break;
            case PlaylistSortOrder.TitleNumber:
                SortUtil.SortWithoutNewCollection(this.Songs, song => song.TrackNumber);
                break;
        }
    }
    
    // ==============
    // REMOVE DUPLICATES
    // ==============

    public void RemoveDuplicates()
    {
        // Remove duplicates.
        // Use ToList() method because the simple IEnumerable will not work anymore after clearing the songs collection.
        IList<Song> noDuplicates = this.Songs.DistinctBy(song => song.FilePath).ToList();
        
        // Clear and re-add the items to avoid assigning a new collection which would result in a loss of all
        // observers of the current collection since they still observe to the first referenced collection.
        this.Songs.Clear();
        foreach (var song in noDuplicates)
        {
            this.Songs.Add(song);
        }
    }
}