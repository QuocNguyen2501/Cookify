namespace RecipeApp.Mobile.Models;

public class Category
{
    public Guid Id { get; set; }
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
