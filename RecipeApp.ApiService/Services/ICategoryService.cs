using RecipeApp.ApiService.Models;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Interface for category business logic operations
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <returns>Collection of all categories</returns>
    Task<IEnumerable<Category>> GetAllCategoriesAsync();

    /// <summary>
    /// Gets a specific category by ID
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>The category if found, null otherwise</returns>
    Task<Category?> GetCategoryByIdAsync(Guid id);

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="category">The category to create</param>
    /// <returns>The created category</returns>
    Task<Category> CreateCategoryAsync(Category category);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <param name="category">The updated category data</param>
    /// <returns>True if successful, false if category not found</returns>
    Task<bool> UpdateCategoryAsync(Guid id, Category category);

    /// <summary>
    /// Deletes a category if it has no associated recipes
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>True if deleted successfully, false if category not found or has associated recipes</returns>
    Task<bool> DeleteCategoryAsync(Guid id);

    /// <summary>
    /// Checks if a category exists
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>True if category exists, false otherwise</returns>
    Task<bool> CategoryExistsAsync(Guid id);

    /// <summary>
    /// Checks if a category has associated recipes
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>True if category has recipes, false otherwise</returns>
    Task<bool> CategoryHasRecipesAsync(Guid id);
}
