using System;
using System.Windows;

namespace View.Dialog;

public partial class InputDialog : Window
{
    // ==============
    // INITIALIZATION
    // ==============
    
    public InputDialog()
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
    
    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}