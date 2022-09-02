using System;
using System.Windows;
using System.Windows.Controls;

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
    // FOCUS ACTION
    // ==============

    private void TxtInput_OnGotFocus(object sender, RoutedEventArgs e)
    {
        // Select complete input text when focused
        if (sender is TextBox tb)
        {
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }
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