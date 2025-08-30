using Microsoft.AspNetCore.Mvc;
using RecipeApp.Models;
using RecipeApp.ApiService.Services;

namespace RecipeApp.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <returns>List of all categories</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets a specific category by ID
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>The category if found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="category">The category to create</param>
    /// <returns>The created category</returns>
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category category)
    {
        try
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating category: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <param name="category">The updated category data</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(Guid id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest("Category ID mismatch.");
        }

        try
        {
            var success = await _categoryService.UpdateCategoryAsync(id, category);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating category: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var success = await _categoryService.DeleteCategoryAsync(id);
        
        if (!success)
        {
            // Check if category exists to provide appropriate error message
            var categoryExists = await _categoryService.CategoryExistsAsync(id);
            if (!categoryExists)
            {
                return NotFound();
            }
            
            // Category exists but has associated recipes
            return BadRequest("Cannot delete category that has associated recipes.");
        }

        return NoContent();
    }

    /// <summary>
    /// Exports all categories as JSON for mobile app consumption
    /// </summary>
    /// <returns>JSON file download containing all categories</returns>
    [HttpGet("export")]
    public async Task<IActionResult> ExportCategories()
    {
        try
        {
            var jsonString = await _categoryService.ExportCategoriesAsJsonAsync();

            // Return as downloadable file
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            var fileName = $"categories_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

            return File(bytes, "application/json", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error exporting categories: {ex.Message}");
        }
    }
}
