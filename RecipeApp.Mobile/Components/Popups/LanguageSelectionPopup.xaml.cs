using CommunityToolkit.Maui.Views;
using RecipeApp.Mobile.ViewModels;

namespace RecipeApp.Mobile.Components.Popups;

/// <summary>
/// Language selection popup component that allows users to choose their preferred language
/// </summary>
public partial class LanguageSelectionPopup : Popup
{
    private readonly LanguageSelectionPopupViewModel _viewModel;

    public LanguageSelectionPopup(LanguageSelectionPopupViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Override the close method in the view model
        _viewModel.ClosePopupAsync = async (result) => await CloseAsync(result);
    }
}
