using System.Globalization;

namespace RecipeApp.Mobile.Converters;

public class ImageFileNameToSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName && !string.IsNullOrEmpty(fileName))
        {
            // Try to load from Images subfolder first, then from root
            var imagePath = $"Images/{fileName}";
            try
            {
                return ImageSource.FromFile(imagePath);
            }
            catch
            {
                return ImageSource.FromFile(fileName);
            }
        }
        
        // Return placeholder image if no image specified
        return ImageSource.FromFile("placeholder.jpg");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
