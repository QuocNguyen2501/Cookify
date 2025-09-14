using RecipeApp.Models;
using RecipePortal.WebApp.Models;

namespace RecipePortal.WebApp.Services;

/// <summary>
/// Service interface for recipe form operations including CRUD operations and AI analysis
/// </summary>
public interface IRecipeFormService
{
    /// <summary>
    /// Loads all available categories
    /// </summary>
    /// <returns>List of categories</returns>
    /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when operation fails</exception>
    Task<List<Category>> LoadCategoriesAsync();
    
    /// <summary>
    /// Loads a specific recipe by ID and converts it to PortalRecipe for form binding
    /// </summary>
    /// <param name="id">Recipe ID</param>
    /// <returns>The PortalRecipe for form binding</returns>
    /// <exception cref="ArgumentException">Thrown when recipe is not found</exception>
    /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when operation fails</exception>
    Task<PortalRecipe> LoadRecipeAsync(Guid id);
    
    /// <summary>
    /// Saves a recipe (creates new or updates existing)
    /// </summary>
    /// <param name="portalRecipe">The recipe data from the form</param>
    /// <param name="id">Recipe ID for updates, or empty Guid for new recipes</param>
    /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when save operation fails</exception>
    Task SaveRecipeAsync(PortalRecipe portalRecipe, Guid id);
    
    /// <summary>
    /// Analyzes an uploaded image using AI and returns structured recipe data
    /// </summary>
    /// <param name="fileBytes">Image file bytes</param>
    /// <param name="contentType">MIME type of the image</param>
    /// <param name="fileName">Original filename</param>
    /// <returns>AI analysis response with recipe data</returns>
    /// <exception cref="ArgumentException">Thrown when file is too large or invalid</exception>
    /// <exception cref="TimeoutException">Thrown when AI analysis times out</exception>
    /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when AI analysis fails</exception>
    Task<AIRecipeResponse> AnalyzeImageWithAIAsync(byte[] fileBytes, string contentType, string fileName);
}