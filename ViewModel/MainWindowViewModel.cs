using Model.Service;

namespace ViewModel;

public class MainWindowViewModel
{
    public DirectoriesViewModel DirectoriesViewModel { get; set; }
    public CurrentSongViewModel CurrentSongViewModel { get; set; }

    // dependencies for multiple view models
    private readonly SongImporter _songImporter;
    private readonly SongPlayer _songPlayer;

    public MainWindowViewModel()
    {
        this._songImporter = new SongImporter();
        this._songPlayer = new SongPlayer();
        
        this.DirectoriesViewModel = new DirectoriesViewModel(_songImporter, _songPlayer);
        this.CurrentSongViewModel = new CurrentSongViewModel(_songImporter, _songPlayer);
    }
}