namespace RecipeApp.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute("recipe", typeof(RecipeDetailPage));
    }
}
