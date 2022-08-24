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

    // ==============
    // PROPERTIES
    // ==============

    public string Name
    {
        get => _name; 
        set => SetField(ref _name, value);
    }

    public ObservableCollection<Song> Songs
    {
        get => _songs;
        set
        {
            SetField(ref _songs, value);
            this.UpdateTotalDuration(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public TimeSpan TotalDuration
    {
        get => _totalDuration;
        set => SetField(ref _totalDuration, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============

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
    
    public void Sort(PlaylistSortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case PlaylistSortOrder.Alphabetical:
                this.Songs = new ObservableCollection<Song>(this.Songs.OrderBy(song => song.Title));
                break;
            case PlaylistSortOrder.Individual:
                // No further ordering, only display it for the sake of completeness
                break;
            case PlaylistSortOrder.TitleNumber:
                this.Songs = new ObservableCollection<Song>(this.Songs.OrderBy(song => song.TrackNumber));
                break;
        }
    }
}