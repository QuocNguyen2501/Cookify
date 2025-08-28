using System.ComponentModel;

namespace RecipeApp.Mobile.Services;

public class LanguageService : INotifyPropertyChanged
{
    private string _currentLanguage = "en";
    
    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnPropertyChanged();
                LanguageChanged?.Invoke(value);
            }
        }
    }

    public event Action<string>? LanguageChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetLanguage(string languageCode)
    {
        CurrentLanguage = languageCode;
    }

    public static LanguageService Instance { get; } = new();
}
