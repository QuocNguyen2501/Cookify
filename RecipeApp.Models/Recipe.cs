using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    /// <summary>
    /// Constructor with basic recipe information
    /// </summary>
    /// <param name="englishName">English name for the recipe</param>
    /// <param name="vietnameseName">Vietnamese name for the recipe (optional)</param>
    public Recipe(string englishName, string vietnameseName = "") : this()
    {
        Name = new RecipeLocalizedText(englishName, vietnameseName);
    }

    /// <summary>
    /// Constructor with full localized content
    /// </summary>
    /// <param name="name">Localized recipe name</param>
    /// <param name="description">Localized recipe description</param>
    /// <param name="categoryId">Category identifier</param>
    public Recipe(RecipeLocalizedText name, RecipeLocalizedText description, Guid categoryId) : this()
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }

    /// <summary>
    /// Gets the display name in the specified language
    /// </summary>
    /// <param name="languageCode">Language code ("en" or "vi")</param>
    /// <returns>Localized recipe name</returns>
    public string GetDisplayName(string languageCode = "en")
    {
        return Name.GetLocalizedText(languageCode);
    }

    /// <summary>
    /// Gets the display description in the specified language
    /// </summary>
    /// <param name="languageCode">Language code ("en" or "vi")</param>
    /// <returns>Localized recipe description</returns>
    public string GetDisplayDescription(string languageCode = "en")
    {
        return Description.GetLocalizedText(languageCode);
    }

    /// <summary>
    /// Gets the localized ingredients list for the specified language
    /// </summary>
    /// <param name="languageCode">Language code ("en" or "vi")</param>
    /// <returns>List of localized ingredient strings</returns>
    public List<string> GetLocalizedIngredients(string languageCode = "en")
    {
        return Ingredients.Select(ingredient => ingredient.GetLocalizedText(languageCode)).ToList();
    }

    /// <summary>
    /// Gets the localized instructions list for the specified language
    /// </summary>
    /// <param name="languageCode">Language code ("en" or "vi")</param>
    /// <returns>List of localized instruction strings</returns>
    public List<string> GetLocalizedInstructions(string languageCode = "en")
    {
        return Instructions.Select(instruction => instruction.GetLocalizedText(languageCode)).ToList();
    }

    /// <summary>
    /// Gets the total time (prep + cook) in a human-readable format
    /// </summary>
    /// <returns>Combined time string or individual times if format doesn't match</returns>
    public string GetTotalTime()
    {
        // Simple implementation - could be enhanced to parse and calculate actual times
        if (string.IsNullOrWhiteSpace(PrepTime) && string.IsNullOrWhiteSpace(CookTime))
            return "Time not specified";
        
        if (string.IsNullOrWhiteSpace(PrepTime))
            return $"Cook: {CookTime}";
        
        if (string.IsNullOrWhiteSpace(CookTime))
            return $"Prep: {PrepTime}";
        
        return $"{PrepTime} prep, {CookTime} cook";
    }

    /// <summary>
    /// Adds a localized ingredient to the recipe
    /// </summary>
    /// <param name="english">English ingredient text</param>
    /// <param name="vietnamese">Vietnamese ingredient text (optional)</param>
    public void AddIngredient(string english, string vietnamese = "")
    {
        Ingredients.Add(new RecipeLocalizedText(english, vietnamese));
    }

    /// <summary>
    /// Adds a localized instruction to the recipe
    /// </summary>
    /// <param name="english">English instruction text</param>
    /// <param name="vietnamese">Vietnamese instruction text (optional)</param>
    public void AddInstruction(string english, string vietnamese = "")
    {
        Instructions.Add(new RecipeLocalizedText(english, vietnamese));
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
        if (obj is Recipe other)
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

    // Helper properties for web form binding (ingredients and instructions as text)
    
    /// <summary>
    /// Gets or sets ingredients as newline-separated English text for form binding
    /// </summary>
    [NotMapped]
    public string IngredientsTextEnglish
    {
        get => string.Join(Environment.NewLine, Ingredients.Select(i => i.English));
        set
        {
            Ingredients.Clear();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var lines = value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    Ingredients.Add(new RecipeLocalizedText(line.Trim()));
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets ingredients as newline-separated Vietnamese text for form binding
    /// </summary>
    [NotMapped]
    public string IngredientsTextVietnamese
    {
        get => string.Join(Environment.NewLine, Ingredients.Select(i => i.Vietnamese));
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var lines = value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length && i < Ingredients.Count; i++)
                {
                    Ingredients[i].Vietnamese = lines[i].Trim();
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets instructions as newline-separated English text for form binding
    /// </summary>
    [NotMapped]
    public string InstructionsTextEnglish
    {
        get => string.Join(Environment.NewLine, Instructions.Select(i => i.English));
        set
        {
            Instructions.Clear();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var lines = value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    Instructions.Add(new RecipeLocalizedText(line.Trim()));
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets instructions as newline-separated Vietnamese text for form binding
    /// </summary>
    [NotMapped]
    public string InstructionsTextVietnamese
    {
        get => string.Join(Environment.NewLine, Instructions.Select(i => i.Vietnamese));
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var lines = value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length && i < Instructions.Count; i++)
                {
                    Instructions[i].Vietnamese = lines[i].Trim();
                }
            }
        }
    }
}
