using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using RecipeApp.Mobile.Components.Popups;
using RecipeApp.Mobile.Services;

namespace RecipeApp.Mobile.ViewModels;

/// <summary>
/// ViewModel for the AppShell that handles navigation and global commands
/// </summary>
public partial class AppShellViewModel : BaseViewModel
{
    private readonly ILanguagePreferenceService _languagePreferenceService;
    private readonly LanguageService _languageService;
    private readonly IServiceProvider _serviceProvider;

    public AppShellViewModel(
        ILanguagePreferenceService languagePreferenceService,
        LanguageService languageService,
        IServiceProvider serviceProvider)
    {
        _languagePreferenceService = languagePreferenceService;
        _languageService = languageService;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Command to show the language selection popup
    /// </summary>
    [RelayCommand]
    private async Task ShowLanguagePopup()
    {
        try
        {
            IsBusy = true;

            // Get the popup and its view model from DI
            var popup = _serviceProvider.GetRequiredService<LanguageSelectionPopup>();
            
            // Show the popup and await the result
            var result = await Shell.Current.ShowPopupAsync(popup);
            
            // The result will be the selected language code or null if cancelled
            if (result is string selectedLanguage && !string.IsNullOrEmpty(selectedLanguage))
            {
                // Language was already updated in the popup view model
                // We could show a confirmation toast here if needed
            }
        }
        catch (Exception ex)
        {
            // Handle any errors
            await Shell.Current.DisplayAlert("Error", $"Failed to show language selection: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
