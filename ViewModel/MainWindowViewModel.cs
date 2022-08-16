namespace ViewModel;

public class MainWindowViewModel
{
    public DirectoriesViewModel DirectoriesViewModel { get; set; }
    public CurrentSongViewModel CurrentSongViewModel { get; set; }

    public MainWindowViewModel()
    {
        this.DirectoriesViewModel = new DirectoriesViewModel();
        this.CurrentSongViewModel = new CurrentSongViewModel();
    }
}