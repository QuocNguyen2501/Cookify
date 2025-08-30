using System.Globalization;

namespace RecipeApp.Mobile.Converters;

public class ImageFileNameToSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName && !string.IsNullOrEmpty(fileName))
        {
            try
            {
                // For MAUI embedded images, try different approaches
                
                // Approach 1: Direct filename (most common for embedded resources)
                var imageSource = ImageSource.FromFile(fileName);
                return imageSource;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image {fileName}: {ex.Message}");
                
                // Fallback: try without extension
                try
                {
                    var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    return ImageSource.FromFile(nameWithoutExtension);
                }
                catch
                {
                    // Final fallback: return a default image
                    return ImageSource.FromFile("dotnet_bot.svg");
                }
            }
        }
        
        // Return default image if no image specified
        return ImageSource.FromFile("dotnet_bot.svg");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
