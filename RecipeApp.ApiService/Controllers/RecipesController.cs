using Microsoft.AspNetCore.Mvc;
using RecipeApp.ApiService.Models;
using RecipeApp.ApiService.Services;

namespace RecipeApp.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Gets all recipes with their associated categories
    /// </summary>
    /// <returns>List of all recipes</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
    {
        var recipes = await _recipeService.GetAllRecipesAsync();
        return Ok(recipes);
    }

    /// <summary>
    /// Gets a specific recipe by ID with its associated category
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>The recipe if found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetRecipe(Guid id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        return Ok(recipe);
    }

    /// <summary>
    /// Creates a new recipe
    /// </summary>
    /// <param name="recipe">The recipe to create</param>
    /// <returns>The created recipe</returns>
    [HttpPost]
    public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
    {
        try
        {
            var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = createdRecipe.Id }, createdRecipe);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating recipe: {ex.Message}");
        }
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
            return BadRequest("Recipe ID mismatch.");
        }

        try
        {
            var success = await _recipeService.UpdateRecipeAsync(id, recipe);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating recipe: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a recipe
    /// </summary>
    /// <param name="id">The recipe ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var success = await _recipeService.DeleteRecipeAsync(id);
        
        if (!success)
        {
            return NotFound();
        }

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
            var jsonString = await _recipeService.ExportRecipesAsJsonAsync();

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
}
