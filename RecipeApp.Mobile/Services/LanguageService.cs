namespace RecipeApp.Mobile.Services;

/// <summary>
/// Service for managing the current language state and persistence
/// </summary>
public class LanguageService
{
    private readonly ILanguagePreferenceService _languagePreferenceService;
    private string _currentLanguage;
    
    public LanguageService(ILanguagePreferenceService languagePreferenceService)
    {
        _languagePreferenceService = languagePreferenceService;
        
        // Initialize with saved preference or default to English
        _currentLanguage = _languagePreferenceService.GetLanguagePreference();
    }
    
    public string CurrentLanguage
    {
        get => _currentLanguage;
        private set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                LanguageChanged?.Invoke(value);
            }
        }
    }

    public event Action<string>? LanguageChanged;

    /// <summary>
    /// Sets the current language and persists the preference
    /// </summary>
    /// <param name="languageCode">The language code to set (e.g., "en", "vi")</param>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            languageCode = "en";
        }

        CurrentLanguage = languageCode;
        _languagePreferenceService.SetLanguagePreference(languageCode);
    }

    /// <summary>
    /// Checks if this is the first time the user is using the app (no language preference set)
    /// </summary>
    /// <returns>True if no language preference exists, false otherwise</returns>
    public bool IsFirstTimeUser()
    {
        return !_languagePreferenceService.HasLanguagePreference();
    }
}
