using System.Globalization;
using RecipeApp.Models;

namespace RecipeApp.Mobile.Converters;

public class LanguageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RecipeLocalizedText localizedText)
        {
            // For now, default to English. In a full implementation, 
            // this would check the current language from LanguageService
            return localizedText.English;
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
