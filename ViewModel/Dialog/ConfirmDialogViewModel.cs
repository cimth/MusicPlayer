using Common;

namespace ViewModel.Dialog;

public class ConfirmDialogViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string Request { get; }
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public ConfirmDialogViewModel(string requestResourceName)
    {
        this.Request = LanguageUtil.GiveLocalizedString(requestResourceName);
    }
}