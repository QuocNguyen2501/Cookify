using RecipeApp.Models;
using RecipeApp.Mobile.Services;
using RecipeApp.Mobile.Resources.Strings;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RecipeApp.Mobile.ViewModels;

/// <summary>
/// View model for displaying recipes in a specific category using CommunityToolkit.Mvvm
/// </summary>
[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class CategoryRecipesViewModel : BaseViewModel, IDisposable
{
    private readonly IRecipeDataService _recipeDataService;
    private readonly ICategoryDataService _categoryDataService;
    private readonly LanguageService _languageService;

    [ObservableProperty]
    private Category? currentCategory;

    [ObservableProperty]
    private string currentLanguage = "en";

    [ObservableProperty]
    private string searchText = string.Empty;

    public string CategoryId { get; set; } = string.Empty;

    public ObservableCollection<Recipe> CategoryRecipes { get; } = new();
    public ObservableCollection<Recipe> FilteredRecipes { get; } = new();

    /// <summary>
    /// Called when CurrentCategory property changes to update title and load recipes
    /// </summary>
    partial void OnCurrentCategoryChanged(Category? value)
    {
        if (value != null)
        {
            Title = value.Name.GetLocalizedText(CurrentLanguage);
            LoadCategoryRecipes();
        }
    }

    /// <summary>
    /// Called when CurrentLanguage property changes to update content
    /// </summary>
    partial void OnCurrentLanguageChanged(string value)
    {
        if (CurrentCategory != null)
        {
            Title = CurrentCategory.Name.GetLocalizedText(value);
        }
        FilterRecipes();
    }

    /// <summary>
    /// Called when SearchText property changes to trigger filtering
    /// </summary>
    partial void OnSearchTextChanged(string value)
    {
        FilterRecipes();
    }

    public CategoryRecipesViewModel(IRecipeDataService recipeDataService, ICategoryDataService categoryDataService, LanguageService languageService)
    {
        _recipeDataService = recipeDataService;
        _categoryDataService = categoryDataService;
        _languageService = languageService;

        // Subscribe to language changes
        _languageService.LanguageChanged += OnLanguageChanged;
        CurrentLanguage = _languageService.CurrentLanguage;
    }

    /// <summary>
    /// Loads the category and its recipes based on CategoryId
    /// </summary>
    public async Task LoadCategory()
    {
        if (Guid.TryParse(CategoryId, out var categoryId))
        {
            try
            {
                IsBusy = true;
                
                // Load category
                CurrentCategory = await _categoryDataService.GetCategoryByIdAsync(categoryId);
                
                if (CurrentCategory == null)
                {
                    // If category not found, navigate back
                    await Shell.Current.GoToAsync("..");
                    return;
                }
                CurrentCategory.Recipes = await _recipeDataService.GetRecipesByCategoryIdAsync(categoryId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading category: {ex.Message}");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    /// <summary>
    /// Command to navigate to recipe detail page
    /// </summary>
    [RelayCommand]
    private async Task GoToRecipeDetail(Recipe? recipe)
    {
        if (recipe != null)
        {
            await Shell.Current.GoToAsync($"recipedetail?id={recipe.Id}");
        }
    }

    /// <summary>
    /// Command to navigate back to main page
    /// </summary>
    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Loads recipes for the current category
    /// </summary>
    private async void LoadCategoryRecipes()
    {
        if (CurrentCategory == null) return;

        try
        {
            var allRecipes = await _recipeDataService.GetRecipesAsync();
            var categoryRecipes = allRecipes.Where(r => r.CategoryId == CurrentCategory.Id).ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                CategoryRecipes.Clear();
                foreach (var recipe in categoryRecipes)
                {
                    CategoryRecipes.Add(recipe);
                }
                FilterRecipes();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading category recipes: {ex.Message}");
        }
    }

    /// <summary>
    /// Filters recipes based on search text
    /// </summary>
    private void FilterRecipes()
    {
        var filtered = CategoryRecipes.AsEnumerable();

        // Filter by search text
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(r =>
                r.Name.GetLocalizedText(CurrentLanguage).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Any(i => i.GetLocalizedText(CurrentLanguage).Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            FilteredRecipes.Clear();
            foreach (var recipe in filtered)
            {
                FilteredRecipes.Add(recipe);
            }
        });
    }

    /// <summary>
    /// Handles language change events from the language service
    /// </summary>
    private void OnLanguageChanged(string newLanguage)
    {
        CurrentLanguage = newLanguage;
        
        // Force UI refresh by triggering property change notifications
        // This is needed because converters don't automatically re-evaluate when language changes
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Trigger refresh of the FilteredRecipes collection to update the converter bindings
            FilterRecipes();
        });
    }

    /// <summary>
    /// Disposes of resources and unsubscribes from events
    /// </summary>
    public void Dispose()
    {
        _languageService.LanguageChanged -= OnLanguageChanged;
    }
}
