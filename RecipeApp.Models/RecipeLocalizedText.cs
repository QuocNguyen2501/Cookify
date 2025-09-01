using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

/// <summary>
/// Represents localized text content supporting both English and Vietnamese languages.
/// This is the core model for multi-language support across the Cookify application.
/// </summary>
public class RecipeLocalizedText
{
    [Required(ErrorMessage = "English text is required")]
    [StringLength(1000, ErrorMessage = "English text cannot exceed 500 characters")]
    public string English { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Vietnamese text cannot exceed 500 characters")]
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

    /// <summary>
    /// Default constructor for serialization and model binding
    /// </summary>
    public RecipeLocalizedText()
    {
    }

    /// <summary>
    /// Constructor to initialize with English and optional Vietnamese text
    /// </summary>
    /// <param name="english">English text (required)</param>
    /// <param name="vietnamese">Vietnamese text (optional)</param>
    public RecipeLocalizedText(string english, string vietnamese = "")
    {
        English = english;
        Vietnamese = vietnamese;
    }

    /// <summary>
    /// Implicit conversion from string to RecipeLocalizedText (uses English only)
    /// </summary>
    public static implicit operator RecipeLocalizedText(string english)
    {
        return new RecipeLocalizedText(english);
    }

    /// <summary>
    /// Implicit conversion from RecipeLocalizedText to string (returns English)
    /// </summary>
    public static implicit operator string(RecipeLocalizedText localizedText)
    {
        return localizedText?.English ?? string.Empty;
    }

    /// <summary>
    /// Override ToString to return English text by default
    /// </summary>
    public override string ToString()
    {
        return English;
    }

    /// <summary>
    /// Equality comparison based on both English and Vietnamese content
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is RecipeLocalizedText other)
        {
            return English == other.English && Vietnamese == other.Vietnamese;
        }
        return false;
    }

    /// <summary>
    /// Hash code generation for use in collections
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(English, Vietnamese);
    }
}
