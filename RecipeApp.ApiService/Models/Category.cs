using System.ComponentModel.DataAnnotations;

namespace RecipeApp.ApiService.Models;

public class Category
{
    public Guid Id { get; set; }

    [Required]
    public RecipeLocalizedText Name { get; set; } = new();

    // Navigation property for related recipes (for EF Core)
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public Category()
    {
        Id = Guid.NewGuid();
    }

    public Category(string englishName, string vietnameseName = "") : this()
    {
        Name = new RecipeLocalizedText(englishName, vietnameseName);
    }
}
