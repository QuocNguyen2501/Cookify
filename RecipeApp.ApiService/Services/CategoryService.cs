using Microsoft.EntityFrameworkCore;
using RecipeApp.ApiService.Data;
using RecipeApp.Models;
using System.Text.Json;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Service for category business logic operations
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <returns>Collection of all categories</returns>
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    /// <summary>
    /// Gets a specific category by ID
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>The category if found, null otherwise</returns>
    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="category">The category to create</param>
    /// <returns>The created category</returns>
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        // Ensure we have a new ID
        category.Id = Guid.NewGuid();

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category;
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <param name="category">The updated category data</param>
    /// <returns>True if successful, false if category not found</returns>
    public async Task<bool> UpdateCategoryAsync(Guid id, Category category)
    {
        if (id != category.Id)
        {
            return false;
        }

        if (!await CategoryExistsAsync(id))
        {
            return false;
        }

        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CategoryExistsAsync(id))
            {
                return false;
            }
            throw;
        }
    }

    /// <summary>
    /// Deletes a category if it has no associated recipes
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>True if deleted successfully, false if category not found or has associated recipes</returns>
    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        // Check if category has associated recipes
        if (await CategoryHasRecipesAsync(id))
        {
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Checks if a category exists
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>True if category exists, false otherwise</returns>
    public async Task<bool> CategoryExistsAsync(Guid id)
    {
        return await _context.Categories.AnyAsync(e => e.Id == id);
    }

    /// <summary>
    /// Checks if a category has associated recipes
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>True if category has recipes, false otherwise</returns>
    public async Task<bool> CategoryHasRecipesAsync(Guid id)
    {
        return await _context.Recipes.AnyAsync(r => r.CategoryId == id);
    }

    /// <summary>
    /// Exports all categories as JSON for mobile app consumption
    /// </summary>
    /// <returns>JSON string containing all categories</returns>
    public async Task<string> ExportCategoriesAsJsonAsync()
    {
        var categories = await _context.Categories.ToListAsync();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        return JsonSerializer.Serialize(categories, options);
    }
}
