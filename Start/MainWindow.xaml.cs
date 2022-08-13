using System.Windows;
using ViewModel;

namespace Start
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // create view models and add it as data context for the views
            CurrentSongViewModel currentSongViewModel = new CurrentSongViewModel();
            this.CurrentSongView.DataContext = currentSongViewModel;
        }
    }
}