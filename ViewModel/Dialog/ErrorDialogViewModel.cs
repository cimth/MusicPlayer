using Common;

namespace ViewModel.Dialog;

public class ErrorDialogViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string ErrorMessage { get; }
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public ErrorDialogViewModel(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }
}