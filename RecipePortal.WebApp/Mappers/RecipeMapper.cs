using Facet.Mapping;
using RecipeApp.Models;
using RecipePortal.WebApp.Models;

namespace RecipePortal.WebApp.Mappers;

public class RecipeMapper : IFacetMapConfigurationAsync<Recipe, PortalRecipe>
{ 
    public static Task MapAsync(Recipe source, PortalRecipe target, CancellationToken cancellationToken = default)
    {
        target.Id = source.Id;
        target.Name = source.Name;
        target.Description = source.Description;
        target.CookTime = source.CookTime;
        target.PrepTime = source.PrepTime;
        target.CategoryId = source.CategoryId;
        target.ImageFileName = source.ImageFileName;

        target.IngredientsTextEnglish = string.Join("\n", source.Ingredients.Select(i => i.English));
        target.IngredientsTextVietnamese = string.Join("\n", source.Ingredients.Select(i => i.Vietnamese));
        target.InstructionsTextEnglish = string.Join("\n", source.Instructions.Select(i => i.English));
        target.InstructionsTextVietnamese = string.Join("\n", source.Instructions.Select(i => i.Vietnamese));

        return Task.CompletedTask;
    }
}