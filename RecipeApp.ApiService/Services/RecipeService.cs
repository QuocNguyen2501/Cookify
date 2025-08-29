using Microsoft.EntityFrameworkCore;
using RecipeApp.ApiService.Data;
using RecipeApp.ApiService.Models;
using RecipeApp.ApiService.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Service for recipe business logic operations
/// </summary>
public class RecipeService : IRecipeService
{
    private readonly AppDbContext _context;

    public RecipeService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all recipes with their associated categories
    /// </summary>
    /// <returns>Collection of all recipes</returns>
    public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
    {
        return await _context.Recipes
            .Include(r => r.Category)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific recipe by ID with its associated category
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>The recipe if found, null otherwise</returns>
    public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
    {
        return await _context.Recipes
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// Creates a new recipe
    /// </summary>
    /// <param name="recipe">The recipe to create</param>
    /// <returns>The created recipe with category loaded</returns>
    public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
    {
        // Ensure we have a new ID
        recipe.Id = Guid.NewGuid();

        // Validate that the category exists
        if (!await ValidateCategoryExistsAsync(recipe.CategoryId))
        {
            throw new ArgumentException("Invalid CategoryId. Category does not exist.", nameof(recipe));
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        // Load the category for the response
        await _context.Entry(recipe)
            .Reference(r => r.Category)
            .LoadAsync();

        return recipe;
    }

    /// <summary>
    /// Updates an existing recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <param name="recipe">The updated recipe data</param>
    /// <returns>True if successful, false if recipe not found</returns>
    public async Task<bool> UpdateRecipeAsync(Guid id, Recipe recipe)
    {
        if (id != recipe.Id)
        {
            return false;
        }

        // Validate that the category exists
        if (!await ValidateCategoryExistsAsync(recipe.CategoryId))
        {
            throw new ArgumentException("Invalid CategoryId. Category does not exist.", nameof(recipe));
        }

        if (!await RecipeExistsAsync(id))
        {
            return false;
        }

        _context.Entry(recipe).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await RecipeExistsAsync(id))
            {
                return false;
            }
            throw;
        }
    }

    /// <summary>
    /// Deletes a recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>True if deleted successfully, false if recipe not found</returns>
    public async Task<bool> DeleteRecipeAsync(Guid id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return false;
        }

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Exports all recipes with embedded category data as JSON
    /// </summary>
    /// <returns>JSON string containing all recipes</returns>
    public async Task<string> ExportRecipesAsJsonAsync()
    {
        // Fetch all recipes with their associated categories
        var recipes = await _context.Recipes
            .Include(r => r.Category)
            .ToListAsync();

        // Convert to DTOs to avoid circular references
        var recipeDtos = recipes.Select(RecipeDto.FromEntity).ToList();

        // Serialize to JSON with proper formatting
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(recipeDtos, jsonOptions);
    }

    /// <summary>
    /// Checks if a recipe exists
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>True if recipe exists, false otherwise</returns>
    public async Task<bool> RecipeExistsAsync(Guid id)
    {
        return await _context.Recipes.AnyAsync(e => e.Id == id);
    }

    /// <summary>
    /// Validates that a category exists for recipe operations
    /// </summary>
    /// <param name="categoryId">The category ID</param>
    /// <returns>True if category exists, false otherwise</returns>
    public async Task<bool> ValidateCategoryExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }
}
