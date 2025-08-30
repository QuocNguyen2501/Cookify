using RecipeApp.Models;
using System.Text.Json;

namespace RecipeApp.Mobile.Services;

/// <summary>
/// Service responsible for loading and managing category data from local JSON files
/// Follows the Single Responsibility Principle
/// </summary>
public class CategoryDataService : ICategoryDataService
{
    private List<Category>? _categories;

    public async Task<List<Category>> GetCategoriesAsync()
    {
        if (_categories == null)
        {
            await LoadCategoriesAsync();
        }
        return _categories ?? new List<Category>();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        var categories = await GetCategoriesAsync();
        return categories.FirstOrDefault(c => c.Id == id);
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            // Try to load from categories.json first
            using var stream = await FileSystem.OpenAppPackageFileAsync("categories.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            _categories = JsonSerializer.Deserialize<List<Category>>(json, options) ?? new List<Category>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
            _categories = new List<Category>();
        }
    }

    public static CategoryDataService Instance { get; } = new();
}
