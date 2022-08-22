using View.Dialog;

namespace Dialog;

public class DialogService
{
    public bool? ShowInputDialog(object viewModel)
    {
        InputDialog dialog = new InputDialog();
        dialog.DataContext = viewModel;
        return dialog.ShowDialog();
    }
}