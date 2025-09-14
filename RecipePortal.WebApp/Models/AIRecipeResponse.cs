
using RecipeApp.Models;

namespace RecipePortal.WebApp.Models;
/// <summary>
/// DTO representing the AI analysis response structure from the ImageAI API
/// </summary>
public class AIRecipeResponse
{
    public RecipeLocalizedText Name { get; set; } = new();
    public RecipeLocalizedText Description { get; set; } = new();
    public string PrepTime { get; set; } = "";
    public string CookTime { get; set; } = "";
    public string ImageFileName { get; set; } = "";
    public List<RecipeLocalizedText> Ingredients { get; set; } = new();
    public List<RecipeLocalizedText> Instructions { get; set; } = new();
}