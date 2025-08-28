using RecipeApp.Mobile.Services;
using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile;

public partial class MainPage : ContentPage
{
    private readonly LanguageService _languageService;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _languageService = LanguageService.Instance;
    }

    private void OnSearchClicked(object sender, EventArgs e)
    {
        SearchBar.IsVisible = !SearchBar.IsVisible;
        if (SearchBar.IsVisible)
        {
            SearchBar.Focus();
        }
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

