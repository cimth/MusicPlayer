using System.Collections.ObjectModel;
using System.Windows.Input;
using Common;

namespace ViewModel.Dialog;

public class InputDialogViewModel : NotifyPropertyChangedImpl
{
    // ==============
    // FIELDS
    // ==============
    
    private string _inputValue;
    
    // ==============
    // PROPERTIES
    // ==============
    
    public string Request { get; }

    public string InputValue 
    { 
        get => _inputValue;
        set => SetField(ref _inputValue, value);
    }
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public InputDialogViewModel(string requestResourceName, string defaultInputValue = "")
    {
        this.Request = LanguageUtil.GiveLocalizedString(requestResourceName);
        this._inputValue = defaultInputValue;
    }
}