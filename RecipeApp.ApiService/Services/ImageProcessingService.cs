using Tesseract;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Service for handling image processing operations including file conversion and OCR
/// </summary>
public class ImageProcessingService : IImageProcessingService
{
    /// <summary>
    /// Converts an uploaded file to byte array
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <returns>Byte array representation of the file</returns>
    /// <exception cref="ArgumentNullException">Thrown when file is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when file is empty or conversion fails</exception>
    public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file), "File cannot be null");

        if (file.Length == 0)
            throw new InvalidOperationException("File cannot be empty");

        try
        {
            using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert file to byte array: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Converts an uploaded file to base64 string
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <returns>Base64 string representation of the file</returns>
    /// <exception cref="ArgumentNullException">Thrown when file is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when file is empty or conversion fails</exception>
    public async Task<string> ConvertFileToBase64Async(IFormFile file)
    {
        try
        {
            var bytes = await ConvertFileToByteArrayAsync(file);
            return $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            throw new InvalidOperationException($"Failed to convert file to base64: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Extracts text content from image using OCR (Tesseract)
    /// </summary>
    /// <param name="imageBytes">The image as byte array</param>
    /// <returns>Extracted text content from the image</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageBytes is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when OCR processing fails</exception>
    public string ExtractTextFromImage(byte[] imageBytes)
    {
        if (imageBytes == null)
            throw new ArgumentNullException(nameof(imageBytes), "Image bytes cannot be null");

        if (imageBytes.Length == 0)
            throw new ArgumentException("Image bytes cannot be empty", nameof(imageBytes));

        try
        {
            using (var engine = new TesseractEngine(@"./tessdata", "vie+eng", EngineMode.Default))
            using (var img = Pix.LoadFromMemory(imageBytes))
            using (var page = engine.Process(img))
            {
                engine.SetVariable("tessedit_pageseg_mode", "6");
                return page.GetText();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to extract text from image: {ex.Message}", ex);
        }
    }
}