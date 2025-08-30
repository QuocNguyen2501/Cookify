using RecipeApp.Models;

namespace RecipeApp.Mobile.Services;

/// <summary>
/// Interface for recipe data service operations.
/// This abstraction allows for different implementations (local JSON, API, etc.)
/// </summary>
public interface IRecipeDataService
{
    /// <summary>
    /// Gets all recipes available in the system
    /// </summary>
    /// <returns>A list of all recipes</returns>
    Task<List<Recipe>> GetRecipesAsync();

    /// <summary>
    /// Gets a specific recipe by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the recipe</param>
    /// <returns>The recipe if found, null otherwise</returns>
    Task<Recipe?> GetRecipeByIdAsync(Guid id);

    /// <summary>
    /// Gets all recipes that belong to a specific category
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category</param>
    /// <returns>A list of recipes in the specified category</returns>
    Task<List<Recipe>> GetRecipesByCategoryIdAsync(Guid categoryId);
}
