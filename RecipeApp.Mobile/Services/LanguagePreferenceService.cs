namespace RecipeApp.Mobile.Services;

/// <summary>
/// Service for managing language preferences using Microsoft.Maui.Storage.Preferences
/// </summary>
public class LanguagePreferenceService : ILanguagePreferenceService
{
    private const string LanguagePreferenceKey = "user_language_preference";

    /// <summary>
    /// Gets the stored language preference. Returns system locale or "en" if no preference is set.
    /// </summary>
    /// <returns>The language code (e.g., "en", "vi")</returns>
    public string GetLanguagePreference()
    {
        var defaultLanguage = GetSystemDefaultLanguage();
        var savedLanguage = Preferences.Get(LanguagePreferenceKey, defaultLanguage);
        
        return savedLanguage;
    }

    /// <summary>
    /// Gets the system default language, with fallback to "en"
    /// </summary>
    /// <returns>The system language code</returns>
    private string GetSystemDefaultLanguage()
    {
        try
        {
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
            var languageCode = currentCulture.TwoLetterISOLanguageName.ToLower();
            
            // Only return Vietnamese if system is Vietnamese, otherwise default to English
            return languageCode == "vi" ? "vi" : "en";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LanguagePreferenceService: Error getting system language: {ex.Message}");
            return "en"; // Fallback to English if any error
        }
    }

    /// <summary>
    /// Saves the language preference to persistent storage
    /// </summary>
    /// <param name="languageCode">The language code to save (e.g., "en", "vi")</param>
    public void SetLanguagePreference(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            languageCode = "en";
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
