using System.Windows;

namespace View.Dialog;

public partial class MessageDialog : Window
{
    // ==============
    // INITIALIZATION
    // ==============
    
    public MessageDialog()
    {
        InitializeComponent();
    }
    
    // ==============
    // BUTTON ACTIONS
    // ==============

    private void OK_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }
}