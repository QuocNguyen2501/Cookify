using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models;

/// <summary>
/// Represents a complete recipe with localized content, ingredients, instructions, and metadata.
/// This is the primary entity for the Cookify recipe management system.
/// </summary>
public class Recipe
{
    /// <summary>
    /// Unique identifier for the recipe
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Localized name of the recipe (English/Vietnamese)
    /// </summary>
    [Required(ErrorMessage = "Recipe name is required")]
    public RecipeLocalizedText Name { get; set; } = new();

    /// <summary>
    /// Localized description of the recipe (English/Vietnamese)
    /// </summary>
    [Required(ErrorMessage = "Recipe description is required")]
    public RecipeLocalizedText Description { get; set; } = new();

    /// <summary>
    /// Preparation time in human-readable format (e.g., "15 minutes", "1 hour")
    /// </summary>
    [StringLength(50, ErrorMessage = "Prep time cannot exceed 50 characters")]
    public string PrepTime { get; set; } = string.Empty;

    /// <summary>
    /// Cooking time in human-readable format (e.g., "30 minutes", "2 hours")
    /// </summary>
    [StringLength(50, ErrorMessage = "Cook time cannot exceed 50 characters")]
    public string CookTime { get; set; } = string.Empty;

    /// <summary>
    /// Optional filename for the recipe's main image
    /// </summary>
    [StringLength(255, ErrorMessage = "Image filename cannot exceed 255 characters")]
    public string? ImageFileName { get; set; }

    /// <summary>
    /// Foreign key reference to the recipe's category
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Navigation property for the recipe's category (for Entity Framework Core)
    /// Virtual to enable lazy loading if configured
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// List of localized ingredients for the recipe
    /// </summary>
    [Required(ErrorMessage = "At least one ingredient is required")]
    [MinLength(1, ErrorMessage = "At least one ingredient is required")]
    public List<RecipeLocalizedText> Ingredients { get; set; } = new();

    /// <summary>
    /// List of localized cooking instructions for the recipe
    /// </summary>
    [Required(ErrorMessage = "At least one instruction is required")]
    [MinLength(1, ErrorMessage = "At least one instruction is required")]
    public List<RecipeLocalizedText> Instructions { get; set; } = new();

    /// <summary>
    /// Default constructor - generates new GUID
    /// </summary>
    public Recipe()
    {
        Id = Guid.NewGuid();
    }
}
