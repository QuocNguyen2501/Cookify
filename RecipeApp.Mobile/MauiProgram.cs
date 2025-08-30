using CommunityToolkit.Maui;
using RecipeApp.Mobile.Services;
using RecipeApp.Mobile.ViewModels;
using RecipeApp.Mobile.Components.Popups;

namespace RecipeApp.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register services
		builder.Services.AddSingleton<ILanguagePreferenceService, LanguagePreferenceService>();
		builder.Services.AddSingleton<LanguageService>();
		builder.Services.AddSingleton<IRecipeDataService, RecipeDataService>();
		builder.Services.AddSingleton<ICategoryDataService, CategoryDataService>();

		// Register ViewModels
		builder.Services.AddSingleton<AppShellViewModel>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<RecipeDetailViewModel>();
		builder.Services.AddTransient<CategoryRecipesViewModel>();
		builder.Services.AddTransient<LanguageSelectionPopupViewModel>();

		// Register Pages
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<RecipeDetailPage>();
		builder.Services.AddTransient<CategoryRecipesPage>();

		// Register Popups
		builder.Services.AddTransient<LanguageSelectionPopup>();

		return builder.Build();
	}
}
