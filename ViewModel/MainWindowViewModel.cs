using Model.Service;

namespace ViewModel;

public class MainWindowViewModel
{
    // ==============
    // FIELDS
    // ==============
    
    // dependencies for multiple view models
    private readonly SongImporter _songImporter;
    private readonly SongPlayer _songPlayer;
    private readonly AppConfigurator _appConfigurator;
    
    // ==============
    // PROPERTIES
    // ==============
    public DirectoriesViewModel DirectoriesViewModel { get; set; }
    
    public CurrentSongViewModel CurrentSongViewModel { get; set; }

    
    // ==============
    // INITIALIZATION
    // ==============

    public MainWindowViewModel()
    {
        this._songImporter = new SongImporter();
        this._songPlayer = new SongPlayer();
        this._appConfigurator = new AppConfigurator();
        
        this.DirectoriesViewModel = new DirectoriesViewModel(_songImporter, _songPlayer, _appConfigurator);
        this.CurrentSongViewModel = new CurrentSongViewModel(_songPlayer);
    }
}