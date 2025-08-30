using RecipeApp.Models;
using RecipeApp.Mobile.Services;
using RecipeApp.Mobile.Resources.Strings;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RecipeApp.Mobile.ViewModels;

/// <summary>
/// Main view model for the recipe list page using CommunityToolkit.Mvvm
/// </summary>
public partial class MainViewModel : BaseViewModel, IDisposable
{
    private readonly RecipeDataService _recipeDataService;
    private readonly CategoryDataService _categoryDataService;
    private readonly LanguageService _languageService;

    public ObservableCollection<Recipe> Recipes { get; }
    public ObservableCollection<Recipe> FilteredRecipes { get; }
    public ObservableCollection<Category> Categories { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private Category? selectedCategory;

    [ObservableProperty]
    private string currentLanguage = "en";

    [ObservableProperty]
    private bool isLanguagePopupVisible = false;

    /// <summary>
    /// Called when CurrentLanguage property changes to update language service
    /// </summary>
    partial void OnCurrentLanguageChanged(string value)
    {
        _languageService.SetLanguage(value);
    }

    public MainViewModel(RecipeDataService recipeDataService, CategoryDataService categoryDataService, LanguageService languageService)
    {
        _recipeDataService = recipeDataService;
        _categoryDataService = categoryDataService;
        _languageService = languageService;

        Recipes = new ObservableCollection<Recipe>();
        FilteredRecipes = new ObservableCollection<Recipe>();
        Categories = new ObservableCollection<Category>();

        UpdateLocalizedTitle();
        
        // Subscribe to language changes
        _languageService.LanguageChanged += OnLanguageChanged;
        CurrentLanguage = _languageService.CurrentLanguage;

        Task.Run(async () => await LoadData());
    }

    /// <summary>
    /// Updates the localized title based on current language
    /// </summary>
    private void UpdateLocalizedTitle()
    {
        try
        {
            var resourceManager = AppResources.ResourceManager;
            var cultureInfo = new System.Globalization.CultureInfo(CurrentLanguage);
            Title = resourceManager.GetString("SelectCategory", cultureInfo) ?? "Select category";
        }
        catch
        {
            Title = "Select category"; // Fallback
        }
    }

    /// <summary>
    /// Command to change the current language
    /// </summary>
    [RelayCommand]
    private void ChangeLanguage(string? language)
    {
        if (!string.IsNullOrEmpty(language))
        {
            CurrentLanguage = language;
        }
    }

    /// <summary>
    /// Command to navigate to category recipes page
    /// </summary>
    [RelayCommand]
    private async Task GoToCategory(Category? category)
    {
        System.Diagnostics.Debug.WriteLine($"GoToCategory command called with category: {category?.Name?.English}");
        
        if (category != null)
        {
            System.Diagnostics.Debug.WriteLine($"Navigating to categoryrecipes?categoryId={category.Id}");
            await Shell.Current.GoToAsync($"categoryrecipes?categoryId={category.Id}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Category is null!");
        }
    }

    /// <summary>
    /// Command to show the language selection popup
    /// </summary>
    [RelayCommand]
    private void ShowLanguagePopup()
    {
        System.Diagnostics.Debug.WriteLine("ShowLanguagePopup command called");
        IsLanguagePopupVisible = true;
    }

    /// <summary>
    /// Command to close the language selection popup
    /// </summary>
    [RelayCommand]
    private void CloseLanguagePopup()
    {
        IsLanguagePopupVisible = false;
    }

    /// <summary>
    /// Command to select a language and close the popup
    /// </summary>
    [RelayCommand]
    private void SelectLanguage(string? language)
    {
        if (!string.IsNullOrEmpty(language))
        {
            CurrentLanguage = language;
            IsLanguagePopupVisible = false;
        }
    }

    /// <summary>
    /// Loads recipes and categories data asynchronously
    /// </summary>
    private async Task LoadData()
    {
        try
        {
            IsBusy = true;

            var recipes = await _recipeDataService.GetRecipesAsync();
            var categories = await _categoryDataService.GetCategoriesAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Recipes.Clear();
                Categories.Clear();

                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }

                foreach (var category in categories)
                {
                    Categories.Add(category);
                    // Debug logging
                    System.Diagnostics.Debug.WriteLine($"Loaded category: {category.Name.English}, ImageFileName: {category.ImageFileName}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Handles language change events from the language service
    /// </summary>
    private void OnLanguageChanged(string newLanguage)
    {
        CurrentLanguage = newLanguage;
        
        // Update the localized title
        UpdateLocalizedTitle();
        
        // Force UI refresh by triggering property change notifications
        // This is needed because converters don't automatically re-evaluate when language changes
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Trigger refresh of the Categories collection to update the converter bindings
            var tempCategories = Categories.ToList();
            Categories.Clear();
            foreach (var category in tempCategories)
            {
                Categories.Add(category);
            }
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
