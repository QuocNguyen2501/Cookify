using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

/// <summary>
/// Represents a recipe category with localized names and associated metadata.
/// Used to organize and categorize recipes in the Cookify application.
/// </summary>
public class Category
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Localized name of the category (English/Vietnamese)
    /// </summary>
    [Required(ErrorMessage = "Category name is required")]
    public RecipeLocalizedText Name { get; set; } = new();

    /// <summary>
    /// Optional image filename for the category icon/thumbnail
    /// </summary>
    [StringLength(255, ErrorMessage = "Image filename cannot exceed 255 characters")]
    public string? ImageFileName { get; set; }

    /// <summary>
    /// Navigation property for related recipes (for Entity Framework Core)
    /// Virtual to enable lazy loading if configured
    /// </summary>
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    /// <summary>
    /// Default constructor - generates new GUID
    /// </summary>
    public Category()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Constructor with localized names
    /// </summary>
    /// <param name="englishName">English name for the category</param>
    /// <param name="vietnameseName">Vietnamese name for the category (optional)</param>
    public Category(string englishName, string vietnameseName = "") : this()
    {
        Name = new RecipeLocalizedText(englishName, vietnameseName);
    }

    /// <summary>
    /// Constructor with localized text object
    /// </summary>
    /// <param name="name">RecipeLocalizedText object containing both languages</param>
    public Category(RecipeLocalizedText name) : this()
    {
        Name = name;
    }

    /// <summary>
    /// Gets the display name in the specified language
    /// </summary>
    /// <param name="languageCode">Language code ("en" or "vi")</param>
    /// <returns>Localized category name</returns>
    public string GetDisplayName(string languageCode = "en")
    {
        return Name.GetLocalizedText(languageCode);
    }

    /// <summary>
    /// Override ToString to return English name by default
    /// </summary>
    public override string ToString()
    {
        return Name.English;
    }

    /// <summary>
    /// Equality comparison based on Id
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is Category other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Hash code generation based on Id
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
