using CommunityToolkit.Mvvm.ComponentModel;

namespace RecipeApp.Mobile.ViewModels;

/// <summary>
/// Base view model class providing common properties and functionality using CommunityToolkit.Mvvm
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private string title = string.Empty;
}
