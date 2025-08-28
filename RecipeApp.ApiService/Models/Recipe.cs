using System.ComponentModel.DataAnnotations;

namespace RecipeApp.ApiService.Models;

public class Recipe
{
    public Guid Id { get; set; }

    [Required]
    public RecipeLocalizedText Name { get; set; } = new();

    [Required]
    public RecipeLocalizedText Description { get; set; } = new();

    public string PrepTime { get; set; } = string.Empty;

    public string CookTime { get; set; } = string.Empty;

    public string? ImageFileName { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    // Navigation property for EF Core
    public virtual Category? Category { get; set; }

    [Required]
    public List<RecipeLocalizedText> Ingredients { get; set; } = new();

    [Required]
    public List<RecipeLocalizedText> Instructions { get; set; } = new();

    public Recipe()
    {
        Id = Guid.NewGuid();
    }

    public Recipe(string englishName, string vietnameseName = "") : this()
    {
        Name = new RecipeLocalizedText(englishName, vietnameseName);
    }
}
