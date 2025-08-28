using System.ComponentModel.DataAnnotations;

namespace RecipePortal.WebApp.Models;

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

    // Navigation property
    public Category? Category { get; set; }

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
    
    // Helper properties for Blazor forms to handle List<RecipeLocalizedText> as multiline strings
    public string IngredientsTextEnglish
    {
        get => string.Join("\n", Ingredients.Select(i => i.English));
        set => UpdateIngredientsEnglish(value);
    }

    public string IngredientsTextVietnamese
    {
        get => string.Join("\n", Ingredients.Select(i => i.Vietnamese));
        set => UpdateIngredientsVietnamese(value);
    }

    public string InstructionsTextEnglish
    {
        get => string.Join("\n", Instructions.Select(i => i.English));
        set => UpdateInstructionsEnglish(value);
    }

    public string InstructionsTextVietnamese
    {
        get => string.Join("\n", Instructions.Select(i => i.Vietnamese));
        set => UpdateInstructionsVietnamese(value);
    }

    private void UpdateIngredientsEnglish(string value)
    {
        var lines = value?.Split('\n', StringSplitOptions.RemoveEmptyEntries) ?? [];
        
        // Ensure we have enough RecipeLocalizedText objects
        while (Ingredients.Count < lines.Length)
        {
            Ingredients.Add(new RecipeLocalizedText());
        }
        
        // Remove excess objects
        while (Ingredients.Count > lines.Length)
        {
            Ingredients.RemoveAt(Ingredients.Count - 1);
        }
        
        // Update English text
        for (int i = 0; i < lines.Length; i++)
        {
            Ingredients[i].English = lines[i].Trim();
        }
    }

    private void UpdateIngredientsVietnamese(string value)
    {
        var lines = value?.Split('\n', StringSplitOptions.RemoveEmptyEntries) ?? [];
        
        // Ensure we have enough RecipeLocalizedText objects
        while (Ingredients.Count < lines.Length)
        {
            Ingredients.Add(new RecipeLocalizedText());
        }
        
        // Update Vietnamese text
        for (int i = 0; i < Math.Min(lines.Length, Ingredients.Count); i++)
        {
            Ingredients[i].Vietnamese = lines[i].Trim();
        }
    }

    private void UpdateInstructionsEnglish(string value)
    {
        var lines = value?.Split('\n', StringSplitOptions.RemoveEmptyEntries) ?? [];
        
        // Ensure we have enough RecipeLocalizedText objects
        while (Instructions.Count < lines.Length)
        {
            Instructions.Add(new RecipeLocalizedText());
        }
        
        // Remove excess objects
        while (Instructions.Count > lines.Length)
        {
            Instructions.RemoveAt(Instructions.Count - 1);
        }
        
        // Update English text
        for (int i = 0; i < lines.Length; i++)
        {
            Instructions[i].English = lines[i].Trim();
        }
    }

    private void UpdateInstructionsVietnamese(string value)
    {
        var lines = value?.Split('\n', StringSplitOptions.RemoveEmptyEntries) ?? [];
        
        // Ensure we have enough RecipeLocalizedText objects
        while (Instructions.Count < lines.Length)
        {
            Instructions.Add(new RecipeLocalizedText());
        }
        
        // Update Vietnamese text
        for (int i = 0; i < Math.Min(lines.Length, Instructions.Count); i++)
        {
            Instructions[i].Vietnamese = lines[i].Trim();
        }
    }
}
