using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.ApiService.Services;

namespace RecipeApp.ApiService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ImageAIController : ControllerBase
  {
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IRecipeAIAnalysisService _recipeAIAnalysisService;
    
    public ImageAIController(
        IImageProcessingService imageProcessingService,
        IRecipeAIAnalysisService recipeAIAnalysisService)
    {
      _imageProcessingService = imageProcessingService;
      _recipeAIAnalysisService = recipeAIAnalysisService;
    }

    [HttpPost("analyse")]
    [RequestTimeout("AIAnalysisTimeout")]
    public async Task<IActionResult> AnalyseImage(IFormFile file)
    {
      if (file == null || file.Length == 0)
      {
        return BadRequest("No file uploaded.");
      }
      
      try
      {
        // Convert file to byte array using the image processing service
        // var imgBytes = await _imageProcessingService.ConvertFileToByteArrayAsync(file);

        // Extract text from image using the image processing service
        // var imgContent = _imageProcessingService.ExtractTextFromImage(imgBytes);
        
        var imgContent = await _imageProcessingService.ConvertFileToBase64Async(file);
        var analysisResult = await _recipeAIAnalysisService.AnalyzeRecipeFromTextAsync(imgContent);
        // Analyze the extracted text using the AI service
        // var analysisResult = await _recipeAIAnalysisService.AnalyzeRecipeFromTextAsync(imgContent);
        
        return Ok(analysisResult);
      }
      catch (InvalidOperationException ex)
      {
        return BadRequest($"Processing error: {ex.Message}");
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error analyzing image: {ex.Message}");
      }
    }
  }
}
