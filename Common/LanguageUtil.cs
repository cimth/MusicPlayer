using System.Runtime.CompilerServices;
using System.Windows;

namespace Common;

public static class LanguageUtil
{
    // ==============
    // CONSTANTS
    // ==============
    
    private static readonly string UriPathToLanguagesDirectory = "/Start;component/Resources/Languages";

    // ==============
    // FIELDS
    // ==============
    
    private static ResourceDictionary? _localizedResourceDictionary;
    
    // ==============
    // PROPERTIES
    // ==============

    public static ResourceDictionary LocalizedResourceDictionary
    {
        get => _localizedResourceDictionary ?? GiveResourceDictionaryForCurrentCulture();
    }
    
    // ==============
    // LOAD RESOURCE DICTIONARY
    // ==============

    private static ResourceDictionary GiveResourceDictionaryForCurrentCulture()
    {
        // Get current culture string which e.g. is 'en-US' or 'de-DE'
        string currentCulture = Thread.CurrentThread.CurrentCulture.ToString();

        // Load the correct resource file for the current culture
        LanguageUtil._localizedResourceDictionary = new ResourceDictionary();
        if (currentCulture.StartsWith("de"))
        {
            LanguageUtil._localizedResourceDictionary.Source = new Uri($"{UriPathToLanguagesDirectory}/StringResources.de.xaml", UriKind.Relative);
        }
        else
        {
            LanguageUtil._localizedResourceDictionary.Source = new Uri($"{UriPathToLanguagesDirectory}/StringResources.xaml", UriKind.Relative);
        }
        
        return LanguageUtil._localizedResourceDictionary;
    }
    
    // ==============
    // RETURN LOCALIZED STRING
    // ==============

    public static string GiveLocalizedString(string key)
    {
        return LanguageUtil._localizedResourceDictionary?[key].ToString() ?? throw new InvalidOperationException();
    }
}