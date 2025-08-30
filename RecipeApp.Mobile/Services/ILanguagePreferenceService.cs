namespace RecipeApp.Mobile.Services;

/// <summary>
/// Interface for managing language preference storage and retrieval
/// </summary>
public interface ILanguagePreferenceService
{
    /// <summary>
    /// Gets the stored language preference. Returns "en" if no preference is set.
    /// </summary>
    /// <returns>The language code (e.g., "en", "vi")</returns>
    string GetLanguagePreference();

    /// <summary>
    /// Saves the language preference to persistent storage
    /// </summary>
    /// <param name="languageCode">The language code to save (e.g., "en", "vi")</param>
    void SetLanguagePreference(string languageCode);

    /// <summary>
    /// Checks if a language preference has been previously set
    /// </summary>
    /// <returns>True if a preference exists, false otherwise</returns>
    bool HasLanguagePreference();
}
