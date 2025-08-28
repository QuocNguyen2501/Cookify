namespace RecipeApp.Mobile.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public RecipeLocalizedText Name { get; set; } = new();
    public RecipeLocalizedText Description { get; set; } = new();
    public string PrepTime { get; set; } = string.Empty;
    public string CookTime { get; set; } = string.Empty;
    public string? ImageFileName { get; set; }
    public Guid CategoryId { get; set; }
    
    // For JSON deserialization - the category will be embedded in the JSON
    public Category? Category { get; set; }
    
    public List<RecipeLocalizedText> Ingredients { get; set; } = new();
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
