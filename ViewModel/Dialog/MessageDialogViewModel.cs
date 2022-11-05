using Common;

namespace ViewModel.Dialog;

public class MessageDialogViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // PROPERTIES
    // ==============
    
    public string DialogTitle { get; }
    
    public string DialogMessage { get; }
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public MessageDialogViewModel(string titleResourceName, string messageResourceName)
    {
        this.DialogTitle = LanguageUtil.GiveLocalizedString(titleResourceName);
        this.DialogMessage = LanguageUtil.GiveLocalizedString(messageResourceName);
    }
}