using System.Windows;

namespace View.Dialog;

public partial class ConfirmDialog : Window
{
    // ==============
    // INITIALIZATION
    // ==============
    
    public ConfirmDialog()
    {
        InitializeComponent();
    }
    
    // ==============
    // BUTTON ACTIONS
    // ==============

    private void Yes_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }
    
    private void No_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}