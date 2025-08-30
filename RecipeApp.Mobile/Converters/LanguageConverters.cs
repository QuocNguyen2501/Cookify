using System.Globalization;
using System.Resources;
using RecipeApp.Models;
using RecipeApp.Mobile.Services;

namespace RecipeApp.Mobile.Converters;

public class LanguageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RecipeLocalizedText localizedText)
        {
            // Get the current language from the service locator
            var languageService = Application.Current?.Handler?.MauiContext?.Services?.GetService<LanguageService>();
            var currentLanguage = languageService?.CurrentLanguage ?? "en";
            return localizedText.GetLocalizedText(currentLanguage);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StaticResourceConverter : IValueConverter
{
    private static readonly ResourceManager ResourceManager = new("RecipeApp.Mobile.Resources.Strings.AppResources", typeof(StaticResourceConverter).Assembly);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string resourceKey && !string.IsNullOrWhiteSpace(resourceKey))
        {
            // Get the current language from the service locator
            var languageService = Application.Current?.Handler?.MauiContext?.Services?.GetService<LanguageService>();
            var currentLanguage = languageService?.CurrentLanguage ?? "en";
            
            try
            {
                var cultureInfo = new CultureInfo(currentLanguage);
                var localizedValue = ResourceManager.GetString(resourceKey, cultureInfo);
                return localizedValue ?? resourceKey; // Fallback to key if translation not found
            }
            catch (Exception)
            {
                // Fallback to key if any error occurs
                return resourceKey;
            }
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class LanguageToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currentLanguage && parameter is string targetLanguage)
        {
            return currentLanguage == targetLanguage ? Color.FromArgb("#4F46E5") : Color.FromArgb("#F3F4F6");
        }
        return Color.FromArgb("#F3F4F6");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class LanguageToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currentLanguage && parameter is string targetLanguage)
        {
            return currentLanguage == targetLanguage;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
