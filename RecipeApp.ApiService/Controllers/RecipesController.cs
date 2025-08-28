using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApp.ApiService.Data;
using RecipeApp.ApiService.Models;
using System.Text.Json;

namespace RecipeApp.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecipesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all recipes with their associated categories
    /// </summary>
    /// <returns>List of all recipes</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
    {
        return await _context.Recipes
            .Include(r => r.Category)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific recipe by ID with its associated category
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>The recipe if found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetRecipe(Guid id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound();
        }

        return recipe;
    }

    /// <summary>
    /// Creates a new recipe
    /// </summary>
    /// <param name="recipe">The recipe to create</param>
    /// <returns>The created recipe</returns>
    [HttpPost]
    public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
    {
        // Ensure we have a new ID
        recipe.Id = Guid.NewGuid();

        // Validate that the category exists
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == recipe.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("Invalid CategoryId. Category does not exist.");
        }

        _context.Recipes.Add(recipe);
        
        try
        {
            await _context.SaveChangesAsync();
            
            // Load the category for the response
            await _context.Entry(recipe)
                .Reference(r => r.Category)
                .LoadAsync();
        }
        catch (DbUpdateException)
        {
            if (RecipeExists(recipe.Id))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    /// <summary>
    /// Updates an existing recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <param name="recipe">The updated recipe data</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRecipe(Guid id, Recipe recipe)
    {
        if (id != recipe.Id)
        {
            return BadRequest();
        }

        // Validate that the category exists
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == recipe.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("Invalid CategoryId. Category does not exist.");
        }

        _context.Entry(recipe).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RecipeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Exports all recipes with embedded category data as JSON for mobile app
    /// </summary>
    /// <returns>JSON file download containing all recipes</returns>
    [HttpGet("export")]
    public async Task<IActionResult> ExportRecipes()
    {
        try
        {
            // Fetch all recipes with their associated categories
            var recipes = await _context.Recipes
                .Include(r => r.Category)
                .ToListAsync();

            // Serialize to JSON with proper formatting
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonString = JsonSerializer.Serialize(recipes, jsonOptions);

            // Return as downloadable file
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            var fileName = $"recipes_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

            return File(bytes, "application/json", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error exporting recipes: {ex.Message}");
        }
    }

    private bool RecipeExists(Guid id)
    {
        return _context.Recipes.Any(e => e.Id == id);
    }
}
