using System;
using System.Windows;

namespace View.Dialog;

public partial class ErrorDialog : Window
{
    // ==============
    // INITIALIZATION
    // ==============
    
    public ErrorDialog()
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