using RecipeApp.Models;

namespace RecipeApp.Mobile.Services;

/// <summary>
/// Interface for category data service operations.
/// This abstraction allows for different implementations (local JSON, API, etc.)
/// </summary>
public interface ICategoryDataService
{
    /// <summary>
    /// Gets all categories available in the system
    /// </summary>
    /// <returns>A list of all categories</returns>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>
    /// Gets a specific category by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <returns>The category if found, null otherwise</returns>
    Task<Category?> GetCategoryByIdAsync(Guid id);
}
