using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile;

public partial class RecipeDetailPage : ContentPage
{
    private RecipeDetailViewModel _viewModel;

    public RecipeDetailPage(RecipeDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecipe();
    }
}
