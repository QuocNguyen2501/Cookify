using System.ComponentModel.DataAnnotations;

namespace RecipePortal.WebApp.Models;

public class Category
{
    public Guid Id { get; set; }

    [Required]
    public RecipeLocalizedText Name { get; set; } = new();

    public Category()
    {
        Id = Guid.NewGuid();
    }

    public Category(string englishName, string vietnameseName = "") : this()
    {
        Name = new RecipeLocalizedText(englishName, vietnameseName);
    }
}
