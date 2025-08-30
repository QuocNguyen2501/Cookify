using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.Mobile.Services;

namespace RecipeApp.Mobile.ViewModels;

/// <summary>
/// ViewModel for the language selection popup
/// </summary>
public partial class LanguageSelectionPopupViewModel : BaseViewModel
{
    private readonly ILanguagePreferenceService _languagePreferenceService;
    private readonly LanguageService _languageService;

    [ObservableProperty]
    private string selectedLanguage = "en";

    /// <summary>
    /// Computed property to show English selection state
    /// </summary>
    public bool IsEnglishSelected => SelectedLanguage == "en";

    /// <summary>
    /// Computed property to show Vietnamese selection state
    /// </summary>
    public bool IsVietnameseSelected => SelectedLanguage == "vi";

    public LanguageSelectionPopupViewModel(
        ILanguagePreferenceService languagePreferenceService,
        LanguageService languageService)
    {
        _languagePreferenceService = languagePreferenceService;
        _languageService = languageService;
        
        Title = "Select Language";
        SelectedLanguage = _languageService.CurrentLanguage;
        
        // Set initial selection state
        foreach (var option in LanguageOptions)
        {
            option.IsSelected = option.Code == SelectedLanguage;
        }
        
        // Notify computed properties
        OnPropertyChanged(nameof(IsEnglishSelected));
        OnPropertyChanged(nameof(IsVietnameseSelected));
    }

    /// <summary>
    /// Available language options
    /// </summary>
    public List<LanguageOption> LanguageOptions { get; } = new()
    {
        new LanguageOption { Code = "en", DisplayName = "English", Flag = "🇺🇸" },
        new LanguageOption { Code = "vi", DisplayName = "Tiếng Việt", Flag = "🇻🇳" }
    };

    /// <summary>
    /// Command to select a language option
    /// </summary>
    [RelayCommand]
    private void SelectLanguage(string languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            SelectedLanguage = languageCode;
            
            // Update visual feedback for language options
            foreach (var option in LanguageOptions)
            {
                option.IsSelected = option.Code == languageCode;
            }
            
            // Notify computed properties for visual feedback
            OnPropertyChanged(nameof(IsEnglishSelected));
            OnPropertyChanged(nameof(IsVietnameseSelected));
        }
    }

    /// <summary>
    /// Command to confirm the language selection
    /// </summary>
    [RelayCommand]
    private async Task ConfirmSelection()
    {
        // Update the language service
        _languageService.SetLanguage(SelectedLanguage);
        
        // Save the preference
        _languagePreferenceService.SetLanguagePreference(SelectedLanguage);
        
        // Close the popup with the selected language
        await ClosePopupAsync(SelectedLanguage);
    }

    /// <summary>
    /// Command to cancel the language selection
    /// </summary>
    [RelayCommand]
    private async Task Cancel()
    {
        // Close the popup without making changes
        await ClosePopupAsync(null);
    }

    /// <summary>
    /// Method to close the popup - to be implemented by the popup view
    /// </summary>
    public Func<string?, Task> ClosePopupAsync { get; set; } = _ => Task.CompletedTask;
}

/// <summary>
/// Represents a language option in the selection list
/// </summary>
public class LanguageOption : ObservableObject
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;
    
    public string DisplayText => $"{Flag} {DisplayName}";

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
