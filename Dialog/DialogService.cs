using System.DirectoryServices.ActiveDirectory;
using Common;
using View.Dialog;

namespace Dialog;

public class DialogService
{
    public static bool IsInputDialogOpen { get; private set; }
    
    public bool? ShowInputDialog(object viewModel)
    {
        // Init dialog. Explicitly add resources to make the localized strings accessible.
        InputDialog dialog = new()
        {
            DataContext = viewModel
        };
        dialog.Resources.MergedDictionaries.Add(LanguageUtil.LocalizedResourceDictionary);
        
        // Show the dialog.
        DialogService.IsInputDialogOpen = true;
        bool? result = dialog.ShowDialog();
        DialogService.IsInputDialogOpen = false;
        
        // Return the dialog result.
        return result;
    }

    public void ShowMessageDialog(object viewModel)
    {
        // Init dialog. Explicitly add resources to make the localized strings accessible.
        MessageDialog dialog = new()
        {
            DataContext = viewModel
        };
        dialog.Resources.MergedDictionaries.Add(LanguageUtil.LocalizedResourceDictionary);
        
        // Show the dialog.
        dialog.ShowDialog();
    }
    
    public bool? ShowConfirmDialog(object viewModel)
    {
        // Init dialog. Explicitly add resources to make the localized strings accessible.
        ConfirmDialog dialog = new()
        {
            DataContext = viewModel
        };
        dialog.Resources.MergedDictionaries.Add(LanguageUtil.LocalizedResourceDictionary);
        
        // Show the dialog.
        return dialog.ShowDialog();
    }
}