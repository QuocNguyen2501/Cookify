using Microsoft.AspNetCore.Http;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Interface for image processing operations
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Converts an uploaded file to byte array
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <returns>Byte array representation of the file</returns>
    Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);

    /// <summary>
    /// Converts an uploaded file to base64 string
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <returns>Base64 string representation of the file</returns>
    Task<string> ConvertFileToBase64Async(IFormFile file);

    /// <summary>
    /// Extracts text content from image using OCR (Tesseract)
    /// </summary>
    /// <param name="imageBytes">The image as byte array</param>
    /// <returns>Extracted text content from the image</returns>
    string ExtractTextFromImage(byte[] imageBytes);
}