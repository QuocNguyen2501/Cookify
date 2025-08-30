using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile;

public partial class CategoryRecipesPage : ContentPage
{
    private readonly CategoryRecipesViewModel _viewModel;

    public CategoryRecipesPage(CategoryRecipesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCategory();
    }
}
