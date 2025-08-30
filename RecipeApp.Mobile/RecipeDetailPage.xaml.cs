using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile;

public partial class RecipeDetailPage : ContentPage
{
    private RecipeDetailViewModel _viewModel;

    public RecipeDetailPage(RecipeDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecipe();
    }
}
