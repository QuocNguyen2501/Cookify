namespace RecipeApp.ApiService.Models;

public class RecipeLocalizedText
{
    public string English { get; set; } = string.Empty;
    public string Vietnamese { get; set; } = string.Empty;

    /// <summary>
    /// Gets the localized text based on the provided language code.
    /// Defaults to English if the language code is unknown or if the specified language text is null/empty.
    /// </summary>
    /// <param name="languageCode">The language code (e.g., "en" for English, "vi" for Vietnamese)</param>
    /// <returns>The localized text in the specified language</returns>
    public string GetLocalizedText(string languageCode)
    {
        return languageCode?.ToLower() switch
        {
            "vi" => !string.IsNullOrWhiteSpace(Vietnamese) ? Vietnamese : English,
            "en" or _ => English
        };
    }

    public RecipeLocalizedText()
    {
    }

    public RecipeLocalizedText(string english, string vietnamese = "")
    {
        English = english;
        Vietnamese = vietnamese;
    }
}
