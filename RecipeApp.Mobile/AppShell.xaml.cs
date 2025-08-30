using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        // Register routes for navigation
        Routing.RegisterRoute("recipedetail", typeof(RecipeDetailPage));
        Routing.RegisterRoute("categoryrecipes", typeof(CategoryRecipesPage));
    }
}
