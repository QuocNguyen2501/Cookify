using Facet;
using RecipeApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RecipePortal.WebApp.Models;

[Facet(typeof(Recipe),exclude:[nameof(Recipe.Ingredients),nameof(Recipe.Instructions)], GenerateConstructor = true)]
public partial class PortalRecipe
{
    public PortalRecipe()
    {
        Name = new RecipeLocalizedText();
        Description = new RecipeLocalizedText();
    }

    // Additional properties for form binding
    [Required(ErrorMessage = "English ingredients are required")]
    public string IngredientsTextEnglish { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vietnamese ingredients are required")]
    public string IngredientsTextVietnamese { get; set; } = string.Empty;

    [Required(ErrorMessage = "English instructions are required")]
    public string InstructionsTextEnglish { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vietnamese instructions are required")]
    public string InstructionsTextVietnamese { get; set; } = string.Empty;

}



