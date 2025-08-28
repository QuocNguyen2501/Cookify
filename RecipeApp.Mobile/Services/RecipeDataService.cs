using RecipeApp.Mobile.Models;
using System.Text.Json;

namespace RecipeApp.Mobile.Services;

public class RecipeDataService
{
    private List<Recipe>? _recipes;
    private List<Category>? _categories;

    public async Task<List<Recipe>> GetRecipesAsync()
    {
        if (_recipes == null)
        {
            await LoadRecipesAsync();
        }
        return _recipes ?? new List<Recipe>();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        if (_categories == null)
        {
            await LoadRecipesAsync();
        }
        return _categories ?? new List<Category>();
    }

    public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
    {
        var recipes = await GetRecipesAsync();
        return recipes.FirstOrDefault(r => r.Id == id);
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
            
            _recipes = JsonSerializer.Deserialize<List<Recipe>>(json, options) ?? new List<Recipe>();
            
            // Extract unique categories from recipes
            _categories = _recipes
                .Where(r => r.Category != null)
                .Select(r => r.Category!)
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recipes: {ex.Message}");
            _recipes = new List<Recipe>();
            _categories = new List<Category>();
        }
    }

    public static RecipeDataService Instance { get; } = new();
}
