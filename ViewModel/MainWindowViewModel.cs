namespace ViewModel;

public class MainWindowViewModel
{
    public CurrentSongViewModel CurrentSongViewModel { get; set; }

    public MainWindowViewModel()
    {
        this.CurrentSongViewModel = new CurrentSongViewModel();
    }
}