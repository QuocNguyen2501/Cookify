using RecipeApp.Models;
using System.Text.Json;

namespace RecipeApp.Mobile.Services;

/// <summary>
/// Service responsible for loading and managing recipe data from local JSON files
/// Follows the Single Responsibility Principle
/// </summary>
public class RecipeDataService : IRecipeDataService
{
    private List<Recipe>? _recipes;

    // Simple DTO to match the JSON structure exactly
    private class RecipeDto
    {
        public Guid Id { get; set; }
        public RecipeLocalizedText Name { get; set; } = new();
        public RecipeLocalizedText Description { get; set; } = new();
        public string PrepTime { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string? ImageFileName { get; set; }
        public List<RecipeLocalizedText> Ingredients { get; set; } = new();
        public List<RecipeLocalizedText> Instructions { get; set; } = new();
        public Category Category { get; set; } = new(); // This matches the JSON "category" property
    }

    public async Task<List<Recipe>> GetRecipesAsync()
    {
        if (_recipes == null)
        {
            await LoadRecipesAsync();
        }
        return _recipes ?? new List<Recipe>();
    }

    public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
    {
        var recipes = await GetRecipesAsync();
        return recipes.FirstOrDefault(r => r.Id == id);
    }

    public async Task<List<Recipe>> GetRecipesByCategoryIdAsync(Guid categoryId)
    {
        var recipes = await GetRecipesAsync();
        return recipes.Where(r => r.CategoryId == categoryId).ToList();
    }

    private async Task LoadRecipesAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("recipes.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            // Deserialize to DTO that matches JSON structure
            var recipeDtos = JsonSerializer.Deserialize<List<RecipeDto>>(json, options) ?? new List<RecipeDto>();
            
            // Map DTOs to Recipe models
            _recipes = recipeDtos.Select(dto => new Recipe
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                PrepTime = dto.PrepTime,
                CookTime = dto.CookTime,
                ImageFileName = dto.ImageFileName,
                Ingredients = dto.Ingredients,
                Instructions = dto.Instructions,
                Category = dto.Category,
                CategoryId = dto.Category.Id
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recipes: {ex.Message}");
            _recipes = new List<Recipe>();
        }
    }

    public static RecipeDataService Instance { get; } = new();
}
