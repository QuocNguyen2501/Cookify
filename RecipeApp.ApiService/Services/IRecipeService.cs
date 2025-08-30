using RecipeApp.Models;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Interface for recipe business logic operations
/// </summary>
public interface IRecipeService
{
    /// <summary>
    /// Gets all recipes with their associated categories
    /// </summary>
    /// <returns>Collection of all recipes</returns>
    Task<IEnumerable<Recipe>> GetAllRecipesAsync();

    /// <summary>
    /// Gets a specific recipe by ID with its associated category
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>The recipe if found, null otherwise</returns>
    Task<Recipe?> GetRecipeByIdAsync(Guid id);

    /// <summary>
    /// Creates a new recipe
    /// </summary>
    /// <param name="recipe">The recipe to create</param>
    /// <returns>The created recipe with category loaded</returns>
    Task<Recipe> CreateRecipeAsync(Recipe recipe);

    /// <summary>
    /// Updates an existing recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <param name="recipe">The updated recipe data</param>
    /// <returns>True if successful, false if recipe not found</returns>
    Task<bool> UpdateRecipeAsync(Guid id, Recipe recipe);

    /// <summary>
    /// Deletes a recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>True if deleted successfully, false if recipe not found</returns>
    Task<bool> DeleteRecipeAsync(Guid id);

    /// <summary>
    /// Exports all recipes with embedded category data as JSON
    /// </summary>
    /// <returns>JSON string containing all recipes</returns>
    Task<string> ExportRecipesAsJsonAsync();

    /// <summary>
    /// Checks if a recipe exists
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>True if recipe exists, false otherwise</returns>
    Task<bool> RecipeExistsAsync(Guid id);

    /// <summary>
    /// Validates that a category exists for recipe operations
    /// </summary>
    /// <param name="categoryId">The category ID</param>
    /// <returns>True if category exists, false otherwise</returns>
    Task<bool> ValidateCategoryExistsAsync(Guid categoryId);
}
