using Facet.Mapping;
using Facet.Extensions;
using RecipeApp.Models;
using RecipePortal.WebApp.Models;

namespace RecipePortal.WebApp.Mappers;

public class PortalRecipeMapper : IFacetMapConfigurationAsync<PortalRecipe, Recipe>
{
    public static Task MapAsync(PortalRecipe source, Recipe target, CancellationToken cancellationToken = default)
    {
        target.Id = source.Id;
        target.Name = source.Name;
        target.Description = source.Description;
        target.CookTime = source.CookTime;
        target.PrepTime = source.PrepTime;
        target.CategoryId = source.CategoryId;
        target.ImageFileName = source.ImageFileName;

        target.Ingredients = ConvertTextToLocalizedList(source.IngredientsTextEnglish, source.IngredientsTextVietnamese);
        target.Instructions = ConvertTextToLocalizedList(source.InstructionsTextEnglish, source.InstructionsTextVietnamese);
        return Task.CompletedTask;
    }

    private static List<RecipeLocalizedText> ConvertTextToLocalizedList(string englishText, string vietnameseText)
    {
        var englishLines = string.IsNullOrWhiteSpace(englishText) ? new string[0] : englishText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var vietnameseLines = string.IsNullOrWhiteSpace(vietnameseText) ? new string[0] : vietnameseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var maxLines = Math.Max(englishLines.Length, vietnameseLines.Length);
        var result = new List<RecipeLocalizedText>();

        for (int i = 0; i < maxLines; i++)
        {
            var english = i < englishLines.Length ? englishLines[i].Trim() : "";
            var vietnamese = i < vietnameseLines.Length ? vietnameseLines[i].Trim() : "";

            // Only add non-empty items (at least English should be provided)
            if (!string.IsNullOrWhiteSpace(english))
            {
                result.Add(new RecipeLocalizedText(english, vietnamese));
            }
        }

        return result;
    }
}