using RecipeApp.Models;
using RecipeApp.Mobile.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RecipeApp.Mobile.ViewModels;

/// <summary>
/// View model for recipe detail page using CommunityToolkit.Mvvm
/// </summary>
[QueryProperty(nameof(RecipeId), "id")]
public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly RecipeDataService _recipeDataService;
    private readonly LanguageService _languageService;

    [ObservableProperty]
    private Recipe? currentRecipe;

    [ObservableProperty]
    private string currentLanguage = "en";

    public string RecipeId { get; set; } = string.Empty;

    // Properties for localized display
    public string DisplayName => CurrentRecipe?.Name?.GetLocalizedText(CurrentLanguage) ?? "";
    public string DisplayDescription => CurrentRecipe?.Description?.GetLocalizedText(CurrentLanguage) ?? "";
    public string DisplayCategoryName => CurrentRecipe?.Category?.Name?.GetLocalizedText(CurrentLanguage) ?? "";
    
    public ObservableCollection<string> DisplayIngredients { get; } = new();
    public ObservableCollection<string> DisplayInstructions { get; } = new();

    /// <summary>
    /// Called when CurrentRecipe property changes to update localized content
    /// </summary>
    partial void OnCurrentRecipeChanged(Recipe? value)
    {
        if (value != null)
        {
            Title = value.Name.GetLocalizedText(CurrentLanguage);
            UpdateLocalizedContent();
        }
    }

    /// <summary>
    /// Called when CurrentLanguage property changes to update content
    /// </summary>
    partial void OnCurrentLanguageChanged(string value)
    {
        if (CurrentRecipe != null)
        {
            Title = CurrentRecipe.Name.GetLocalizedText(value);
            UpdateLocalizedContent();
        }
    }

    public RecipeDetailViewModel(RecipeDataService recipeDataService, LanguageService languageService)
    {
        _recipeDataService = recipeDataService;
        _languageService = languageService;

        // Subscribe to language changes
        _languageService.LanguageChanged += OnLanguageChanged;
        CurrentLanguage = _languageService.CurrentLanguage;
    }

    /// <summary>
    /// Loads the recipe data based on RecipeId
    /// </summary>
    public async Task LoadRecipe()
    {
        if (Guid.TryParse(RecipeId, out var id))
        {
            try
            {
                IsBusy = true;
                CurrentRecipe = await _recipeDataService.GetRecipeByIdAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recipe: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    /// <summary>
    /// Updates the localized content for ingredients and instructions
    /// </summary>
    private void UpdateLocalizedContent()
    {
        if (CurrentRecipe == null) return;

        // Update ingredients
        DisplayIngredients.Clear();
        foreach (var ingredient in CurrentRecipe.Ingredients)
        {
            DisplayIngredients.Add(ingredient.GetLocalizedText(CurrentLanguage));
        }

        // Update instructions with numbering
        DisplayInstructions.Clear();
        for (int i = 0; i < CurrentRecipe.Instructions.Count; i++)
        {
            var instruction = CurrentRecipe.Instructions[i].GetLocalizedText(CurrentLanguage);
            DisplayInstructions.Add($"{i + 1}. {instruction}");
        }

        // Notify UI of property changes for computed properties
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(DisplayDescription));
        OnPropertyChanged(nameof(DisplayCategoryName));
    }

    /// <summary>
    /// Handles language change events from the language service
    /// </summary>
    private void OnLanguageChanged(string newLanguage)
    {
        CurrentLanguage = newLanguage;
    }
}
