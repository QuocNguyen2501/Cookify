namespace RecipeApp.Mobile.Services;

/// <summary>
/// Service for managing language preferences using Microsoft.Maui.Storage.Preferences
/// </summary>
public class LanguagePreferenceService : ILanguagePreferenceService
{
    private const string LanguagePreferenceKey = "user_language_preference";
    private const string DefaultLanguage = "en";

    /// <summary>
    /// Gets the stored language preference. Returns "en" if no preference is set.
    /// </summary>
    /// <returns>The language code (e.g., "en", "vi")</returns>
    public string GetLanguagePreference()
    {
        return Preferences.Get(LanguagePreferenceKey, DefaultLanguage);
    }

    /// <summary>
    /// Saves the language preference to persistent storage
    /// </summary>
    /// <param name="languageCode">The language code to save (e.g., "en", "vi")</param>
    public void SetLanguagePreference(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            languageCode = DefaultLanguage;
        }

        Preferences.Set(LanguagePreferenceKey, languageCode);
    }

    /// <summary>
    /// Checks if a language preference has been previously set
    /// </summary>
    /// <returns>True if a preference exists, false otherwise</returns>
    public bool HasLanguagePreference()
    {
        return Preferences.ContainsKey(LanguagePreferenceKey);
    }
}
