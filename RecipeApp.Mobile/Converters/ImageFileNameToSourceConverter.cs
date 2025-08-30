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
                // Log for debugging
                System.Diagnostics.Debug.WriteLine($"Loading image: {fileName}");
                
                // For MAUI embedded images, try different approaches
                
                // Approach 1: Direct filename (most common for embedded resources)
                var imageSource = ImageSource.FromFile(fileName);
                System.Diagnostics.Debug.WriteLine($"Successfully created ImageSource for: {fileName}");
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
        
        System.Diagnostics.Debug.WriteLine("No image filename provided, returning default");
        // Return default image if no image specified
        return ImageSource.FromFile("dotnet_bot.svg");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
