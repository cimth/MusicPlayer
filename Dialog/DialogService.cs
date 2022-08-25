using Common;
using View.Dialog;

namespace Dialog;

public class DialogService
{
    public bool? ShowInputDialog(object viewModel)
    {
        // Init dialog. Explicitly add resources to make the localized strings accessible.
        InputDialog dialog = new()
        {
            DataContext = viewModel
        };
        dialog.Resources.MergedDictionaries.Add(LanguageUtil.LocalizedResourceDictionary);
        
        // Show the dialog
        return dialog.ShowDialog();
    }
}