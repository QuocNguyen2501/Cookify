using RecipeApp.Mobile.Models;
using RecipeApp.Mobile.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RecipeApp.Mobile.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly RecipeDataService _recipeDataService;
    private readonly LanguageService _languageService;

    public ObservableCollection<Recipe> Recipes { get; }
    public ObservableCollection<Recipe> FilteredRecipes { get; }
    public ObservableCollection<Category> Categories { get; }

    private string _searchText = string.Empty;
    private Category? _selectedCategory;
    private string _currentLanguage = "en";

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterRecipes();
        }
    }

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            FilterRecipes();
        }
    }

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            SetProperty(ref _currentLanguage, value);
            _languageService.SetLanguage(value);
        }
    }

    public ICommand SearchCommand { get; }
    public ICommand GoToRecipeDetailCommand { get; }
    public ICommand ChangeLanguageCommand { get; }

    public MainViewModel()
    {
        _recipeDataService = RecipeDataService.Instance;
        _languageService = LanguageService.Instance;

        Recipes = new ObservableCollection<Recipe>();
        FilteredRecipes = new ObservableCollection<Recipe>();
        Categories = new ObservableCollection<Category>();

        SearchCommand = new Command(() => FilterRecipes());
        GoToRecipeDetailCommand = new Command<Recipe>(async (recipe) => await GoToRecipeDetail(recipe));
        ChangeLanguageCommand = new Command<string>((language) => CurrentLanguage = language);

        Title = "Recipe Book";
        
        // Subscribe to language changes
        _languageService.LanguageChanged += OnLanguageChanged;
        CurrentLanguage = _languageService.CurrentLanguage;

        Task.Run(async () => await LoadData());
    }

    private async Task LoadData()
    {
        try
        {
            IsBusy = true;

            var recipes = await _recipeDataService.GetRecipesAsync();
            var categories = await _recipeDataService.GetCategoriesAsync();

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
                }

                FilterRecipes();
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

    private void FilterRecipes()
    {
        var filtered = Recipes.AsEnumerable();

        // Filter by search text
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(r =>
                r.Name.GetLocalizedText(CurrentLanguage).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Any(i => i.GetLocalizedText(CurrentLanguage).Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        // Filter by category
        if (SelectedCategory != null)
        {
            filtered = filtered.Where(r => r.CategoryId == SelectedCategory.Id);
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

    private async Task GoToRecipeDetail(Recipe recipe)
    {
        if (recipe != null)
        {
            await Shell.Current.GoToAsync($"recipedetail?id={recipe.Id}");
        }
    }

    private void OnLanguageChanged(string newLanguage)
    {
        CurrentLanguage = newLanguage;
        FilterRecipes(); // Re-filter with new language
    }
}
