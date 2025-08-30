using RecipeApp.Mobile.Services;
using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile;

public partial class MainPage : ContentPage
{
    private readonly LanguageService _languageService;

    public MainPage(MainViewModel viewModel, LanguageService languageService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _languageService = languageService;
    }

    private async void OnLanguageClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Select Language", "Cancel", null, "English", "Tiếng Việt");
        
        switch (action)
        {
            case "English":
                _languageService.SetLanguage("en");
                break;
            case "Tiếng Việt":
                _languageService.SetLanguage("vi");
                break;
        }
    }
}

