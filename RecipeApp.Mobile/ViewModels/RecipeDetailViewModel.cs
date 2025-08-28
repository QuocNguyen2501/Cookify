using RecipeApp.Mobile.Models;
using RecipeApp.Mobile.Services;

namespace RecipeApp.Mobile.ViewModels;

[QueryProperty(nameof(RecipeId), "id")]
public class RecipeDetailViewModel : BaseViewModel
{
    private readonly RecipeDataService _recipeDataService;
    private readonly LanguageService _languageService;

    private Recipe? _currentRecipe;
    private string _currentLanguage = "en";

    public Recipe? CurrentRecipe
    {
        get => _currentRecipe;
        set => SetProperty(ref _currentRecipe, value);
    }

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set => SetProperty(ref _currentLanguage, value);
    }

    public string RecipeId { get; set; } = string.Empty;

    public RecipeDetailViewModel()
    {
        _recipeDataService = RecipeDataService.Instance;
        _languageService = LanguageService.Instance;

        // Subscribe to language changes
        _languageService.LanguageChanged += OnLanguageChanged;
        CurrentLanguage = _languageService.CurrentLanguage;
    }

    public async Task LoadRecipe()
    {
        if (Guid.TryParse(RecipeId, out var id))
        {
            try
            {
                IsBusy = true;
                CurrentRecipe = await _recipeDataService.GetRecipeByIdAsync(id);
                
                if (CurrentRecipe != null)
                {
                    Title = CurrentRecipe.Name.GetLocalizedText(CurrentLanguage);
                }
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

    private void OnLanguageChanged(string newLanguage)
    {
        CurrentLanguage = newLanguage;
        if (CurrentRecipe != null)
        {
            Title = CurrentRecipe.Name.GetLocalizedText(CurrentLanguage);
        }
    }
}
